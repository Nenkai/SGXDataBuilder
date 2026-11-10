//---------------------------------------------------------------------------------------
// SPDX-FileCopyrightText: Copyright (C) 2026 Nenkai
// SPDX-License-Identifier: MIT
// SPDX-FileAttributionText: https://github.com/Nenkai/SGXDataBuilder
// SPDX-FileNotice: AI usage and consumption is prohibited. See AGENTS.md for policy.
//---------------------------------------------------------------------------------------

using SGXLib.Utils;

namespace SGXLib.AudioFormats
{
    public class AC3SynchronizationInformation
    {
        public short syncword;
        public ushort crc1;
        public int fscod;
        public int frmsizecod;

        public void Read(ref BitStream bs)
        {
            syncword = (short)bs.ReadBits(16);
            if (syncword != 0xB77)
                throw new InvalidDataException("Expected syncword 0xB77. Invalid file format?");

            crc1 = (ushort)bs.ReadBits(16); // crc1
            fscod = (int)bs.ReadBits(2);
            frmsizecod = (int)bs.ReadBits(6);
        }
    }
}
