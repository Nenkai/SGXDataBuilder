using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SGXDataBuilder.Utils;

namespace SGXDataBuilder.AudioFormats
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
