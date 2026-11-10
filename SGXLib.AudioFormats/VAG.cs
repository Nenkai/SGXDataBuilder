//---------------------------------------------------------------------------------------
// SPDX-FileCopyrightText: Copyright (C) 2026 Nenkai
// SPDX-License-Identifier: MIT
// SPDX-FileAttributionText: https://github.com/Nenkai/SGXDataBuilder
// SPDX-FileNotice: AI usage and consumption is prohibited. See AGENTS.md for policy.
//---------------------------------------------------------------------------------------

using Syroot.BinaryData;

namespace SGXLib.AudioFormats
{
    public class VAG : IAudioFormat
    {
        public int DataSize { get; set; }
        public int Frequence { get; set; }
        public string Name { get; set; }
        public int BodyOffset { get; set; }

        public static VAG Read(string fileName)
        {
            VAG vag = new VAG();

            using FileStream fs = new FileStream(fileName, FileMode.Open);
            using BinaryStream bs = new BinaryStream(fs, ByteConverter.Big); // always big

            if (bs.ReadString(4) != "VAGp")
                throw new InvalidDataException("Not a VAG file.");

            uint version = bs.ReadUInt32();
            bs.ReadInt32();

            vag.DataSize = bs.ReadInt32();
            vag.Frequence = bs.ReadInt32();
            bs.Position += 0x0C;
            vag.Name = bs.ReadString(0x10).TrimEnd('\0');

            vag.BodyOffset = (int)bs.Position;

            return vag;
        }

        public int GetBodyOffset()
        {
            return BodyOffset;
        }

        public int GetBodySize()
        {
            return DataSize;
        }

        public int GetSizeWithHeaderIfNeededForSGX()
        {
            // In this case, we do not return it, SGX does not store the header
            return DataSize;
        }

        public byte GetChannelCount()
        {
            return 1;
        }

        public int GetTotalSampleCount()
        {
            return DataSize / GetChannelCount() / 0x10 * 28;
        }

        public int GetFrameSize_ForPar1()
        {
            return 0;
        }

        public int GetBitRate_ForPar0()
        {
            return 0;
        }

        public int GetSampleRate_Frequence()
        {
            return Frequence;
        }
    }
}
