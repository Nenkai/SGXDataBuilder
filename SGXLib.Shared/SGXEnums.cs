//---------------------------------------------------------------------------------------
// SPDX-FileCopyrightText: Copyright (C) 2026 Nenkai
// SPDX-License-Identifier: MIT
// SPDX-FileAttributionText: https://github.com/Nenkai/SGXDataBuilder
// SPDX-FileNotice: AI usage and consumption is prohibited. See AGENTS.md for policy.
//---------------------------------------------------------------------------------------

namespace SGXLib
{
    public enum SGXRequest : byte
    {
        gSgxSndWaveStrSet = 0, // 3
        gSgxSndNoteSet = 1, // 1
        gSgxSndSeqSet = 2, // 2
        gSgxSndWaveSet = 3, // 0 - Direct call to sound

        SgxBusCalcSetupReqBuf = 4, // 16
        SgxBusCallEffectParam = 5, // 17
        SgxConfCall = 6,
    }
}
