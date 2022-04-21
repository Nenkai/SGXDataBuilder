using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SGXDataBuilder.Utils;

namespace SGXDataBuilder.AudioFormats
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
