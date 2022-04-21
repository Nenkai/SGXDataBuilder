using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace SGXDataBuilder
{
    public class SgxdNameHeader
    {
        public int Flags { get; set; }

        // Sorted for BSearch (required)
        public List<SgxdName> Names { get; set; } = new List<SgxdName>();

        public void Build(Sgxd parent, BinaryStream bs)
        {
            // Order for bsearch (important, player wants them ordered)
            Names.Sort(SgxdNameComparer.Default);

            int chunkStartOffset = (int)bs.Position;
            bs.WriteUInt32(0x454D414E); // NAME
            bs.WriteUInt32(0); // Chunk Length - Write later

            bs.WriteInt32(Flags);
            bs.WriteInt32(Names.Count);

            int tocStartOffset = (int)bs.Position;
            int lastNameOffset = tocStartOffset + (Names.Count * 8);

            for (int i = 0; i < Names.Count; i++)
            {
                bs.Position = tocStartOffset + (i * 8);
                bs.WriteUInt32(Names[i].Source);
                bs.WriteInt32(lastNameOffset);

                bs.Position = lastNameOffset;
                int strOffset = (int)bs.Position;
                bs.WriteString(Names[i].Name, StringCoding.ZeroTerminated);
                lastNameOffset = (int)bs.Position;

                // Update reference pointer
                bs.Position = Names[i].NamePointerOffset;
                bs.WriteInt32(strOffset);

                bs.Position = lastNameOffset;
            }

            bs.Align(0x10, grow: true);

            int chunkEndOffset = (int)bs.Position;
            bs.Position = chunkStartOffset + 4;
            bs.WriteInt32(chunkEndOffset - (chunkStartOffset + 8));
            bs.Position = chunkEndOffset;
        }

        public SgxdName AddNew(string name, uint srcFlags = 0)
        {

            var sgxName = new SgxdName(name);
            sgxName.Source = srcFlags;
            Names.Add(sgxName);

            return sgxName;
        }

        public bool Exists(string name)
        {
            foreach (var n in Names)
            {
                if (n.Name == name)
                    return true;
            }

            return false;
        }
    }

}
