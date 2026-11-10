//---------------------------------------------------------------------------------------
// SPDX-FileCopyrightText: Copyright (C) 2026 Nenkai
// SPDX-License-Identifier: MIT
// SPDX-FileAttributionText: https://github.com/Nenkai/SGXDataBuilder
// SPDX-FileNotice: AI usage and consumption is prohibited. See AGENTS.md for policy.
//---------------------------------------------------------------------------------------

namespace SGXLib
{
    public class SgxdName
    {
        public SGXRequest RequestType { get; set; }
        public byte SeqIndex { get; set; }
        public ushort WaveIndex;

        public string Name { get; set; }

        public int WaveNamePointerOffset { get; set; }

        public uint GetSourceBits()
        {
            return (uint)RequestType << 28 | (uint)SeqIndex << 16 | WaveIndex;
        }

        public SgxdName(string name)
        {
            Name = name;
        }
   
    }
}
