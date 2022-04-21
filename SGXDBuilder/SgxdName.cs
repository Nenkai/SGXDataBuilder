using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGXDataBuilder
{
    public class SgxdName
    {
        public uint Source { get; set; }
        public string Name { get; set; }

        public int NamePointerOffset { get; set; }

        public SgxdName(string name)
        {
            Name = name;
        }
    }
}
