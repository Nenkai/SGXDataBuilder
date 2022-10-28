using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace SGXLib
{
    public class SgxdNameHeader
    {
        public int Flags { get; set; }

        // Sorted for BSearch (required)
        public SortedSet<SgxdName> Names { get; set; } = new SortedSet<SgxdName>(SgxdNameComparer.Default);

        public void Build(Sgxd parent, BinaryStream bs)
        {
            // Order for bsearch (important, player wants them ordered)
            int chunkStartOffset = (int)bs.Position;
            bs.WriteUInt32(0x454D414E); // NAME
            bs.WriteUInt32(0); // Chunk Length - Write later

            bs.WriteInt32(Flags);
            bs.WriteInt32(Names.Count);

            int tocStartOffset = (int)bs.Position;
            int lastNameOffset = tocStartOffset + (Names.Count * 8);

            int i = 0;
            foreach (var name in Names)
            {
                bs.Position = tocStartOffset + (i * 8);
                bs.WriteUInt32(name.GetSourceBits());
                bs.WriteInt32(lastNameOffset);

                bs.Position = lastNameOffset;
                int strOffset = (int)bs.Position;
                bs.WriteString(name.Name, StringCoding.ZeroTerminated);
                lastNameOffset = (int)bs.Position;

                // Update reference pointer in WAVE if possible
                bs.Position = name.WaveNamePointerOffset;
                bs.WriteInt32(strOffset);

                i++;
            }

            bs.Position = lastNameOffset;
            bs.Align(0x10, grow: true);

            int chunkEndOffset = (int)bs.Position;
            bs.Position = chunkStartOffset + 4;
            bs.WriteInt32(chunkEndOffset - (chunkStartOffset + 8));
            bs.Position = chunkEndOffset;
        }

        public void Clear()
        {
            Flags = 0;
            Names.Clear();
        }

        public SgxdName AddNew(string name, SGXRequest reqType, ushort waveRequestIndex, byte seqRequestIndex)
        {
            var sgxName = new SgxdName(name);
            sgxName.RequestType = reqType;
            sgxName.WaveIndex = waveRequestIndex;
            sgxName.SeqIndex = seqRequestIndex;
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
