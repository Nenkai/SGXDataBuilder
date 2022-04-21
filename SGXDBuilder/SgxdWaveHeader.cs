using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace SGXDataBuilder
{
    public class SgxdWaveHeader
    {
        public uint Flags { get; set; }
        public List<SgxdWave> Waves { get; set; } = new List<SgxdWave>();

        public void Build(Sgxd parent, BinaryStream bs)
        {
            int chunkStartOffset = (int)bs.Position;
            bs.WriteUInt32(0x45564157);
            bs.WriteUInt32(0);

            bs.WriteInt32(0);
            bs.WriteInt32(Waves.Count);

            foreach (var wave in Waves)
            {
                bs.WriteInt32(wave.Flags);
                wave.Name.NamePointerOffset = (int)bs.Position; bs.Position += 4;
                bs.WriteByte((byte)wave.Format);
                bs.WriteByte(wave.Channels);
                bs.WriteByte(wave.NumberOfLoops);
                bs.WriteByte(0);
                bs.WriteInt32(wave.Frequence);
                bs.WriteInt32(wave.BitRate_Par0);
                bs.WriteInt32(wave.BitRate_Par1);
                bs.WriteInt16(wave.VolumeLeft);
                bs.WriteInt16(wave.VolumeRight);
                bs.WriteInt32(wave.WBeg);
                bs.WriteInt32(wave.WEnd);
                bs.WriteInt32(wave.LBeg);
                bs.WriteInt32(wave.LEnd);
                bs.WriteInt32(wave.BodySize);
                bs.WriteInt32(wave.aBody[0]);
                bs.WriteInt32(wave.aBody[1]);
            }

            bs.Align(0x10, grow: true);

            int chunkEndOffset = (int)bs.Position;
            bs.Position = chunkStartOffset + 4;
            bs.WriteInt32(chunkEndOffset - (chunkStartOffset + 8));
            bs.Position = chunkEndOffset;
        }
    }
}
