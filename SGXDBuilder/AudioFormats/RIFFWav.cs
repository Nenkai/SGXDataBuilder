using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SGXDataBuilder.Utils;

namespace SGXDataBuilder.AudioFormats
{
    internal class RIFFWav : IAudioFormat
    {
        public int FileLength;

        public ushort ChannelCount { get; set; }
        public ushort BitsPerSample { get; set; }
        public int SampleCount { get; set; }
        public uint dwSamplesPerSec { get; set; }

        public int BodyOffset { get; set; }

        public static RIFFWav Read(string fileName)
        {
            RIFFWav riff = new RIFFWav();

            byte[] header = new byte[0x30];
            using FileStream fs = new FileStream(fileName, FileMode.Open);
            fs.Read(header);
            riff.FileLength = (int)fs.Length;

            BitStream bs = new BitStream(BitStreamMode.Read, header, BitStreamSignificantBitOrder.MSB);
            if (bs.ReadStringRaw(4) != "RIFF")
                throw new InvalidDataException("Unexpected magic");

            int size = bs.ReadInt32();
            string riffType = bs.ReadStringRaw(4);

            string chunkId = bs.ReadStringRaw(4);
            int chunkSize = bs.ReadInt32();
            short formatTag = bs.ReadInt16();
            riff.ChannelCount = bs.ReadUInt16();
            riff.dwSamplesPerSec = bs.ReadUInt32();
            uint dwAvgBytesPerSec = bs.ReadUInt32();
            ushort wBlockAlign = bs.ReadUInt16();
            riff.BitsPerSample = bs.ReadUInt16();

            string dataChunkId = bs.ReadStringRaw(4);
            int dataChunkSize = bs.ReadInt32();

            riff.BodyOffset = bs.Position;
            riff.SampleCount = dataChunkSize / (riff.BitsPerSample / 8);

            return riff;
        }

        public int GetBodyOffset()
        {
            return BodyOffset;
        }

        public int GetBodySize()
        {
            return ((BitsPerSample / 8) * ChannelCount) * SampleCount;
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
