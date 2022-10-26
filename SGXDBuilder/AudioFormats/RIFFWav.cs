using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;
using SGXDataBuilder.Utils;

namespace SGXDataBuilder.AudioFormats
{
    internal class RIFFWav : IAudioFormat
    {
        public ushort ChannelCount { get; set; }
        public ushort BitsPerSample { get; set; }
        public int SampleCount { get; set; }
        public uint dwSamplesPerSec { get; set; }

        public int BodyOffset { get; set; }
        public bool BigEndian { get; set; } = false;

        public static RIFFWav Read(string fileName)
        {
            using FileStream fs = new FileStream(fileName, FileMode.Open);
            using BinaryStream bs = new BinaryStream(fs);

            string magic = bs.ReadString(4);
            if (magic != "RIFF" && magic != "RIFX")
                throw new InvalidDataException("Unexpected magic while loading RIFF file.");

            RIFFWav riff = new RIFFWav();

            if (magic == "RIFX" || magic == "FFIR")
            {
                bs.ByteConverter = ByteConverter.Big;
                riff.BigEndian = true;
            }

            int size = bs.ReadInt32();
            string riffType = bs.ReadString(4);

            if (!TryFindChunk(bs, "fmt ", out int chunkSize))
                throw new NotSupportedException($"Did not find fmt chunk in WAV file: {fileName}");

            short formatTag = bs.ReadInt16();
            riff.ChannelCount = bs.ReadUInt16();
            riff.dwSamplesPerSec = bs.ReadUInt32();
            uint dwAvgBytesPerSec = bs.ReadUInt32();
            ushort wBlockAlign = bs.ReadUInt16(); // The size of one "frame" from one channel
            riff.BitsPerSample = bs.ReadUInt16();

            if (riff.BitsPerSample != 16)
                throw new NotSupportedException($"WAVE must be PCM 16 bits (got: {riff.BitsPerSample}). File: {fileName}");

            // Find data chunk, nothing specifies it has to be after the main header
            if (!TryFindChunk(bs, "data", out int dataChunkSize))
                throw new InvalidDataException($"Did not find data chunk in WAV file: {fileName}");

            if (dataChunkSize == 0)
                throw new InvalidDataException($"Invalid data chunk size {dataChunkSize} in WAV file: {fileName}");

            int framesTotal = dataChunkSize / wBlockAlign; 

            riff.BodyOffset = (int)bs.Position;
            riff.SampleCount = framesTotal;

            return riff;
        }

        private static bool TryFindChunk(BinaryStream bs, string name, out int chunkSize)
        {
            chunkSize = 0;

            while (bs.Position < bs.Length)
            {
                string dataChunkId = bs.ReadString(4);
                chunkSize = bs.ReadInt32();
                if (dataChunkId == name)
                    return true;
                
                if (bs.Position + chunkSize >= bs.Length)
                    return false;

                bs.Position += chunkSize;
            }

            return false;
        }

        public int GetBodyOffset()
        {
            return BodyOffset;
        }

        public int GetBodySize()
        {
            return SampleCount * ((BitsPerSample / 8) * ChannelCount);
        }

        public byte GetChannelCount()
        {
            return (byte)ChannelCount;
        }

        public int GetTotalSampleCount()
        {
            return SampleCount;
        }

        public int GetFrameSize()
        {
            return 0;
        }

        public int GetBitRate()
        {
            return 0;
        }

        public int GetSampleRate_Frequence()
        {
            return (int)dwSamplesPerSec;
        }
    }
}
