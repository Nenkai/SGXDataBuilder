using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Text.Json;

using SGXLib.Utils;
using SGXLib.Shared.Project;
using SGXLib.AudioFormats;

using Syroot.BinaryData;

namespace SGXLib
{
    public class Sgxd
    {
        public SgxdNameHeader NameHeader { get; set; } = new SgxdNameHeader();
        public SgxdWaveHeader WaveHeader { get; set; } = new SgxdWaveHeader();
        private SgxdName _thisSgxdName { get; set; }

        public List<IAudioFormat> _files { get; set; } = new();
        private int _currentBodySize;
        public bool SplitBody { get; set; }
        public string Label { get; set; }

        public SgxdWave AddNewFile(string path, string waveName, bool convertLEWaveToBE = false)
        {
            InputAudioFormat format;
            using (var fs = new FileStream(path, FileMode.Open))
                format = DetectFormat(Path.GetExtension(path), fs);

            IAudioFormat audioFormat;
            var wave = new SgxdWave();

            switch (format)
            {
                case InputAudioFormat.AC3:
                    audioFormat = AC3.Read(path);
                    wave.Format = SgxDataFormat.AC3;
                    wave.BitRate_Par0 = audioFormat.GetBitRate_ForPar0();
                    wave.BitRate_Par1 = audioFormat.GetFrameSize_ForPar1();
                    break;

                case InputAudioFormat.Atrac3Plus:
                    audioFormat = Atrac3Plus.Read(path);
                    wave.Format = SgxDataFormat.ATRAC3plus;
                    wave.BitRate_Par0 = audioFormat.GetBitRate_ForPar0();
                    wave.BitRate_Par1 = audioFormat.GetFrameSize_ForPar1();
                    break;

                case InputAudioFormat.VAG:
                    audioFormat = VAG.Read(path);
                    wave.Format = SgxDataFormat.PSADPCM;
                    break;

                case InputAudioFormat.WAV:
                    audioFormat = Waveform.Read(path);
                    if ((audioFormat as Waveform).BigEndian)
                        wave.Format = SgxDataFormat.LinearPCM_BE;
                    else
                    {
                        wave.Format = SgxDataFormat.LinearPCM_LE;
                        if (convertLEWaveToBE)
                        {
                            wave.Format = SgxDataFormat.LinearPCM_BE;
                            wave.ConvertLeWaveToBe = true;
                        }
                    }
                    break;

                default:
                    throw new InvalidDataException($"Unsupported file format for '{path}'.");
                    
            }

            Debug.Assert(WaveHeader.Waves.Count < ushort.MaxValue, $"Too many sound files (> {ushort.MaxValue}.");

            string fileName = Path.GetFileNameWithoutExtension(path);
            Console.WriteLine($"Added file: {fileName} ({format})");

            wave.FullFileSize = audioFormat.GetSizeWithHeaderIfNeededForSGX();
            wave.BodyOffset = audioFormat.GetBodyOffset();
            wave.BodySize = audioFormat.GetBodySize();
            wave.FullPath = path;
            wave.Frequence = audioFormat.GetSampleRate_Frequence();
            wave.Channels = audioFormat.GetChannelCount();
            wave.BodySize = audioFormat.GetBodySize();
            wave.Name = NameHeader.AddNew(waveName, SGXRequest.gSgxSndWaveSet, (ushort)WaveHeader.Waves.Count, 0);
            wave.WEnd = audioFormat.GetTotalSampleCount();

            Console.WriteLine($"- Frequence: {wave.Frequence}Hz");
            Console.WriteLine($"- Channels: {wave.Channels}");
            Console.WriteLine($"- Sample Count: {wave.WEnd}");

            wave.aBody[0] = _currentBodySize;
            wave.aBody[1] = _currentBodySize + wave.FullFileSize;
            _currentBodySize += wave.FullFileSize;

            WaveHeader.Waves.Add(wave);

            Console.WriteLine();

            return wave;
        }

        public void RemoveWave(SgxdWave wave)
        {
            WaveHeader.Waves.Remove(wave);
            NameHeader.Names.Remove(wave.Name);
        }

        public void Build(string path)
        {
            path = Path.GetFullPath(path);
            string pathWithoutExtension = path.Substring(0, path.LastIndexOf(".")); // Not using GetPathWithoutExtension as D:\\something.txt gets converted to just "something"

            // Add the WaveStrSet based on name
            if (_thisSgxdName is null)
            {
                _thisSgxdName = NameHeader.AddNew(!string.IsNullOrEmpty(Label) ? Label : Path.GetFileNameWithoutExtension(pathWithoutExtension),
                    SGXRequest.gSgxSndWaveStrSet, // Important
                    0, 0);
                _thisSgxdName.WaveNamePointerOffset = 0x04; // It's just after the magic
            }

            Console.WriteLine("Building Header..");
            WriteFileHeader(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(pathWithoutExtension));
            Console.WriteLine("SGX Audio Bank creation complete.");
        }

        private void WriteFileHeader(string outputDir, string fileNameWithoutExtension)
        {
            string outputPath = Path.Combine(outputDir, fileNameWithoutExtension);
            using var ms = new FileStream(outputPath + (SplitBody ? ".sgh" : ".sgd"), FileMode.Create);
            using var bs = new BinaryStream(ms);

            // Skip header and write ToC for now
            bs.Position += 0x10;

            WaveHeader.Build(this, bs);
            NameHeader.Build(this, bs);

            int bodyOffset = (int)bs.Position;
            int headerSize = bodyOffset;

            // Done, write header
            bs.Position = 0;
            bs.WriteUInt32(0x44584753); // SGXD
            bs.Position += 4; // SGXD Name - Already rewritten
            bs.WriteUInt32((uint)headerSize);
            int bodySizeBits = _currentBodySize;
            if (!SplitBody)
            {
                Console.WriteLine("Not splitting SGX File (.sgd)..");
                bodySizeBits |= (1 << 31);
            }
            else
            {
                Console.WriteLine("Splitting body and header (.sgb/.sgh)..");
            }

            bs.WriteInt32(bodySizeBits);

            // Now write contents
            if (SplitBody)
            {
                using var bodyFile = new FileStream(outputPath + ".sgb", FileMode.Create);

                foreach (var wave in WaveHeader.Waves)
                    CopyAudio(wave, bodyFile, wave.BodyOffset, wave.FullFileSize);
            }
            else
            {
                bs.Position = bodyOffset;
                foreach (var wave in WaveHeader.Waves)
                    CopyAudio(wave, bs, wave.BodyOffset, wave.FullFileSize);
            }
        }

        private void CopyAudio(SgxdWave wave, Stream output, int beginOffset, int size)
        {
            using var audioFs = File.OpenRead(wave.FullPath);
            audioFs.Position = beginOffset;

            byte[] buffer = new byte[32768];
            int read;
            while (size > 0)
            {
                read = audioFs.Read(buffer, 0, Math.Min(buffer.Length, size));

                if (wave.Format == SgxDataFormat.LinearPCM_LE && wave.ConvertLeWaveToBe)
                {
                    // WAV le to be test
                    for (int i = 0; i < read; i += 2)
                    {
                        byte tmp = buffer[i];
                        buffer[i] = buffer[i + 1];
                        buffer[i + 1] = tmp;
                    }
                }

                output.Write(buffer, 0, read);
                size -= read;
            }
        }

        private InputAudioFormat DetectFormat(string extension, Stream stream)
        {
            byte[] magic = new byte[4];
            stream.Read(magic);

            int magicVal = BinaryPrimitives.ReadInt32LittleEndian(magic);

            InputAudioFormat format = InputAudioFormat.Unknown;
            if ((magicVal & 0x0000FFFF) == 0x770B)
            {
                format = InputAudioFormat.AC3;
            }
            else if (magicVal == 0x70474156)
            {
                format = InputAudioFormat.VAG;
            }
            else if (magicVal == 0x46464952) // RIFF container
            {
                stream.Position = 0;

                if (extension.Equals(".at3", StringComparison.OrdinalIgnoreCase))
                    format = InputAudioFormat.Atrac3Plus;
                else if (extension.Equals(".wav", StringComparison.OrdinalIgnoreCase))
                    format = InputAudioFormat.WAV;
                else
                    return InputAudioFormat.Unknown;
            }
                

            return format;
        }

        public void Reset()
        {
            _thisSgxdName = null;
            _currentBodySize = 0;
            SplitBody = false;
            _files.Clear();

            NameHeader.Clear();
            WaveHeader.Clear();
        }

        public void ImportFromProject(string fileName)
        {
            Reset();

            if (!File.Exists(fileName))
                throw new FileNotFoundException($"Project file {fileName} does not exist.");

            string json = File.ReadAllText(fileName);
            SgxdProject project = JsonSerializer.Deserialize<SgxdProject>(json);

            if (project is null)
                return;

            SplitBody = project.SplitBody;

            foreach (SgxdProjectSoundEntry entry in project.SgxdProjectSoundEntries)
            {
                if (!File.Exists(entry.Path))
                    throw new FileNotFoundException($"File '{entry.Path}' does not exist in project file.");

                AddNewFile(entry.Path, entry.Name);
            }

            Label = project.Label;
        }

        public void ExportAsProject(string fileName)
        {
            var project = new SgxdProject();

            project.Label = Label;
            project.SplitBody = SplitBody;

            foreach (var entry in WaveHeader.Waves)
            {
                project.SgxdProjectSoundEntries.Add(new SgxdProjectSoundEntry()
                {
                    Name = entry.Name.Name,
                    Path = entry.FullPath,
                    SampleStart = entry.LBeg,
                    SampleEnd = entry.LEnd
                });
            }

            string json = JsonSerializer.Serialize(project, new JsonSerializerOptions()
            {
                WriteIndented = true,
            });
            File.WriteAllText(fileName, json);
        }

        enum InputAudioFormat
        {
            Unknown,
            AC3,
            WAV,
            VAG,
            Atrac3Plus
        }
    }
}
