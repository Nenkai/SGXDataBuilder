//---------------------------------------------------------------------------------------
// SPDX-FileCopyrightText: Copyright (C) 2026 Nenkai
// SPDX-License-Identifier: MIT
// SPDX-FileAttributionText: https://github.com/Nenkai/SGXDataBuilder
// SPDX-FileNotice: AI usage and consumption is prohibited. See AGENTS.md for policy.
//---------------------------------------------------------------------------------------

namespace SGXLib
{
    public class SgxdNameComparer : IComparer<SgxdName>
    {
        private static readonly SgxdNameComparer _default = new SgxdNameComparer();
        public static SgxdNameComparer Default => _default;

        public int Compare(SgxdName value1, SgxdName value2)
        {
            string v1 = value1.Name;
            string v2 = value2.Name;

            int min = v1.Length > v2.Length ? v2.Length : v1.Length;
            for (int i = 0; i < min; i++)
            {
                if (v1[i] < v2[i])
                    return -1;
                else if (v1[i] > v2[i])
                    return 1;
            }
            if (v1.Length < v2.Length)
                return -1;
            else if (v1.Length > v2.Length)
                return 1;

            return 0;
        }
    }
}
