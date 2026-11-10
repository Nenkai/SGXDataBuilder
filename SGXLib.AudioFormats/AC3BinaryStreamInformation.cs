//---------------------------------------------------------------------------------------
// SPDX-FileCopyrightText: Copyright (C) 2026 Nenkai
// SPDX-License-Identifier: MIT
// SPDX-FileAttributionText: https://github.com/Nenkai/SGXDataBuilder
// SPDX-FileNotice: AI usage and consumption is prohibited. See AGENTS.md for policy.
//---------------------------------------------------------------------------------------

using SGXLib.Utils;

namespace SGXLib.AudioFormats
{
    public class AC3BinaryStreamInformation
    {
        public byte bsid;
        public byte bsmod;
        public byte acmod;

        public void Read(ref BitStream bs)
        {
            bsid = (byte)bs.ReadBits(5);
            bsmod = (byte)bs.ReadBits(3);
            acmod = (byte)bs.ReadBits(3);
        }
    }
}
