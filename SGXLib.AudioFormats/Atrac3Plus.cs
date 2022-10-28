using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;
using SGXLib.Utils;

namespace SGXLib.AudioFormats
{
    public class Atrac3Plus : IAudioFormat
    {
        public ushort ChannelCount { get; set; }
        public ushort BitsPerSample { get; set; }
        public int SampleCount { get; set; }
        public uint dwSamplesPerSec { get; set; }
        public uint dwAvgBytesPerSec { get; set; }
        public int BodyOffset { get; set; }
        public int DataSize { get; set; }
        public int FileSize { get; set; }
        public bool BigEndian { get; set; } = false;

        public static Atrac3Plus Read(string fileName)
        {
            using FileStream fs = new FileStream(fileName, FileMode.Open);
            RIFFFile riff = RIFFFile.FromStream(fs);

            var atrac3Plus = new Atrac3Plus();
            atrac3Plus.ChannelCount = riff.ChannelCount;
            atrac3Plus.BitsPerSample = riff.BitsPerSample;
            atrac3Plus.dwSamplesPerSec = riff.dwSamplesPerSec;
            atrac3Plus.dwAvgBytesPerSec = riff.dwAvgBytesPerSec;
            atrac3Plus.BodyOffset = 0;
            atrac3Plus.BigEndian = riff.BigEndian;
            atrac3Plus.DataSize = riff.RIFFSize;
            atrac3Plus.FileSize = riff.RIFFFullFileSize;

            fs.Position = 0x0C;
            using var bs = new BinaryStream(fs);
            if (!RIFFFile.TryFindChunk(bs, "fact", out int chunkSize))
                throw new InvalidDataException("Atrac3 file is missing 'fact' chunk in header file");

            atrac3Plus.SampleCount = bs.ReadInt32();

            return atrac3Plus;
        }

        public int GetBodyOffset()
        {
            return BodyOffset;
        }

        public int GetBodySize()
        {
            return DataSize;
        }

        public int GetFullFileSize()
        {
            return FileSize;
        }

        public byte GetChannelCount()
        {
            return (byte)ChannelCount;
        }

        public int GetTotalSampleCount()
        {
            return SampleCount;
        }

        public int GetFrameSize_ForPar1()
        {
            return DataSize;
        }

        public int GetBitRate_ForPar0()
        {
            return (int)(dwAvgBytesPerSec / 125);
        }

        public int GetSampleRate_Frequence()
        {
            return (int)dwSamplesPerSec;
        }
    }
}
