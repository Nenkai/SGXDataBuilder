using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace SGXLib.AudioFormats
{
    public class RIFFFile
    {
        public ushort FormatTag { get; set; }
        public ushort ChannelCount { get; set; }
        public ushort BitsPerSample { get; set; }
        public int SampleCount { get; set; }
        public uint dwSamplesPerSec { get; set; }
        public uint dwAvgBytesPerSec { get; set; }
        public ushort wBlockAlign { get; set; }
        public int BodyOffset { get; set; }
        public int DataChunkSize { get; set; }
        public bool BigEndian { get; set; } = false;

        /// <summary>
        /// Size of the riff contents, without padding based on the riff chunk size
        /// </summary>
        public int RIFFSize { get; set; }

        /// <summary>
        /// Size of just the riff chunk
        /// </summary>
        public int RIFFChunkSize { get; set; }

        /// <summary>
        /// Size of the file on disk
        /// </summary>
        public int RIFFFullFileSize { get; set; }

        public static RIFFFile Read(string fileName)
        {
            using FileStream fs = new FileStream(fileName, FileMode.Open);
            return FromStream(fs);
        }

        public static RIFFFile FromStream(Stream stream)
        {
            BinaryStream bs = new BinaryStream(stream);

            string magic = bs.ReadString(4);
            if (magic != "RIFF" && magic != "RIFX" && magic != "FFIR")
                throw new InvalidDataException("Unexpected magic while loading RIFF file.");

            RIFFFile riff = new RIFFFile();

            if (magic == "RIFX" || magic == "FFIR")
            {
                bs.ByteConverter = ByteConverter.Big;
                riff.BigEndian = true;
            }

            riff.RIFFChunkSize = bs.ReadInt32();
            riff.RIFFSize = riff.RIFFChunkSize + 0x08;
            riff.RIFFFullFileSize = (int)bs.Length;

            string riffType = bs.ReadString(4);

            if (riffType != "WAVE")
                throw new NotSupportedException($"Did not find WAVE chunk in RIFF file");

            if (!TryFindChunk(bs, "fmt ", out int fmtChunkSize))
                throw new NotSupportedException($"Did not find fmt chunk in RIFF file");

            long fmtBasePos = bs.Position;
            riff.FormatTag = bs.ReadUInt16();
            riff.ChannelCount = bs.ReadUInt16();
            riff.dwSamplesPerSec = bs.ReadUInt32();
            riff.dwAvgBytesPerSec = bs.ReadUInt32();
            riff.wBlockAlign = bs.ReadUInt16(); // The size of one "frame" from one channel
            riff.BitsPerSample = bs.ReadUInt16();

            bs.Position = fmtBasePos + fmtChunkSize;

            // Find data chunk, nothing specifies it has to be after the main header
            if (!TryFindChunk(bs, "data", out int dataChunkSize))
                throw new InvalidDataException($"Did not find data chunk in RIFF file");

            if (dataChunkSize == 0)
                throw new InvalidDataException($"Invalid data chunk size {dataChunkSize} in RIFF file");

            int framesTotal = dataChunkSize / riff.wBlockAlign;

            riff.DataChunkSize = dataChunkSize;
            riff.BodyOffset = (int)bs.Position;
            riff.SampleCount = framesTotal;

            return riff;
        }

        public static bool TryFindChunk(BinaryStream bs, string name, out int chunkSize)
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
    }
}
