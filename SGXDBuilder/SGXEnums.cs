using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGXDataBuilder
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
