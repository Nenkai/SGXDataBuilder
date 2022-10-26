using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Buffers.Binary;
using System.Diagnostics;

using SGXDataBuilder.Utils;
using SGXDataBuilder.AudioFormats;

using Syroot.BinaryData;

namespace SGXDataBuilder
{
    public class Sgxd
    {
        public SgxdNameHeader NameHeader { get; set; } = new SgxdNameHeader();
        public SgxdWaveHeader WaveHeader { get; set; } = new SgxdWaveHeader();

        public List<IAudioFormat> Files { get; set; } = new();

        private SgxdName SgxdName;
        public string RawPath { get; set; }

        private int _currentBodySize;

        public bool SplitBody { get; set; }

        public static Sgxd Create(string sgxdFileName, bool splitBody = false)
        {
            var sgxd = new Sgxd();
            sgxd.SplitBody = splitBody;
            sgxd.SgxdName = sgxd.NameHeader.AddNew(Path.GetFileNameWithoutExtension(sgxdFileName));
            sgxd.RawPath = sgxdFileName;

            Console.WriteLine($"SGXD Name: {sgxd.SgxdName.Name}");

            return sgxd;
        }

        public void AddNewFile(string path, bool convertLEWaveToBE = false)
        {
            InputAudioFormat format;
            using (var fs = new FileStream(path, FileMode.Open))
                format = DetectFormat(fs);

            IAudioFormat audioFormat;
            var wave = new SgxdWave();

            switch (format)
            {
                case InputAudioFormat.AC3:
                    audioFormat = AC3.Read(path);
                    wave.Format = WaveFormat.AC3;
                    wave.BitRate_Par0 = audioFormat.GetBitRate();
                    wave.BitRate_Par1 = audioFormat.GetFrameSize();
                    break;

                case InputAudioFormat.WAV:
                    audioFormat = RIFFWav.Read(path);
                    if ((audioFormat as RIFFWav).BigEndian)
                        wave.Format = WaveFormat.LinearPCM_BE;
                    else
                    {
                        wave.Format = WaveFormat.LinearPCM_LE;
                        if (convertLEWaveToBE)
                        {
                            wave.Format = WaveFormat.LinearPCM_BE;
                            wave.ConvertLeWaveToBe = true;
                        }
                    }
                    break;

                default:
                    throw new InvalidDataException($"Unsupported file format for '{path}'.");
                    
            }

            string fileName = Path.GetFileNameWithoutExtension(path);

            Console.WriteLine($"Added file: {fileName} ({format})");

            Debug.Assert(WaveHeader.Waves.Count < ushort.MaxValue, $"Too many sound files (> {ushort.MaxValue}.");

            uint src = 0;
            src |= (uint)((WaveHeader.Waves.Count & 0xFFFF) << 24); // Wave Index
            src |= (uint)(0 << 16); // Seq Index
            src |= ((3 & 0b1111) << 4); // Req type, SgxSndWaveSet
            

            src = BinaryPrimitives.ReverseEndianness(src);

            wave.BodyOffset = audioFormat.GetBodyOffset();
            wave.FullPath = path;

            wave.Frequence = audioFormat.GetSampleRate_Frequence();
            Console.WriteLine($"- Frequence: {wave.Frequence}Hz");

            wave.Channels = audioFormat.GetChannelCount();
            Console.WriteLine($"- Channels: {wave.Channels}");

            wave.BodySize = audioFormat.GetBodySize();
            wave.Name = NameHeader.AddNew(Path.GetFileNameWithoutExtension(path), src);

            wave.WEnd = audioFormat.GetTotalSampleCount();
            Console.WriteLine($"- Sample Count: {wave.WEnd}");

            wave.aBody[0] = _currentBodySize;
            wave.aBody[1] = _currentBodySize + wave.BodySize;
            _currentBodySize += wave.BodySize;

            WaveHeader.Waves.Add(wave);

            Console.WriteLine();
        }

        public void Build(bool bigEndianWave = false)
        {
            Console.WriteLine("Building Header..");

            string fullPath = Path.GetFullPath(this.RawPath);
            string dir = Path.GetDirectoryName(fullPath);

            using var ms = new FileStream(Path.Combine(dir, SgxdName.Name) + (SplitBody ? ".sgh" : ".sgd"), FileMode.Create);
            using var bs = new BinaryStream(ms);

            bs.WriteUInt32(0x44584753); // SGXD
            SgxdName.NamePointerOffset = (int)bs.Position;

            bs.WriteUInt32(0); // SGXD Name - write later
            bs.WriteUInt32(0); // Header Size - write later
            bs.WriteUInt32(0); // Body size - write later

            WaveHeader.Build(this, bs);
            NameHeader.Build(this, bs);

            int bodyOffset = (int)bs.Position;
            int headerSize = bodyOffset;

            bs.Position = 0x08;
            bs.WriteInt32(headerSize);

            int bodySizeBits = _currentBodySize;
            if (!SplitBody)
            {
                Console.WriteLine("Splitting body and header (.sgb/.sgh)..");
                bodySizeBits |= (1 << 31);
            }

            bs.WriteInt32(bodySizeBits);

            if (SplitBody)
            {
                using var bodyFile = new FileStream(Path.Combine(dir, SgxdName.Name) + ".sgb", FileMode.Create);

                foreach (var wave in WaveHeader.Waves)
                    CopyAudio(wave, bodyFile, wave.BodyOffset, wave.BodySize);
            }
            else
            {
                bs.Position = bodyOffset;
                foreach (var wave in WaveHeader.Waves)
                    CopyAudio(wave, bs, wave.BodyOffset, wave.BodySize);
            }

            Console.WriteLine("SGX Audio Bank creation complete.");
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

                if (wave.Format == WaveFormat.LinearPCM_BE && wave.ConvertLeWaveToBe)
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

        private InputAudioFormat DetectFormat(Stream stream)
        {
            byte[] magic = new byte[4];
            stream.Read(magic);

            int magicVal = BinaryPrimitives.ReadInt32LittleEndian(magic);

            InputAudioFormat format = InputAudioFormat.Unknown;
            if ((magicVal & 0x0000FFFF) == 0x770B)
                format = InputAudioFormat.AC3;
            else if (magicVal == 0x46464952)
                format = InputAudioFormat.WAV;

            return format;
        }

        enum InputAudioFormat
        {
            Unknown,
            AC3,
            WAV,
        }
    }
}
