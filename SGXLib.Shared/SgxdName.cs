using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
