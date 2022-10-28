using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGXDataBuilder
{
    public class SgxdWave
    {
        public string FullPath { get; set; }
        public int FullFileSize { get; set; }

        public int Flags { get; set; }
        public SgxdName Name { get; set; }
        public SgxDataFormat Format { get; set; }
        public byte Channels { get; set; }
        public byte NumberOfLoops { get; set; }
        //resv
        public int Frequence { get; set; }
        public int BitRate_Par0 { get; set; }
        public int BitRate_Par1 { get; set; }
        public short VolumeLeft { get; set; } = 0x1000;
        public short VolumeRight { get; set; } = 0x1000;
        public int WBeg { get; set; }
        public int WEnd { get; set; }
        public int LBeg { get; set; } = -1;
        public int LEnd { get; set; } = -1;
        public int BodySize { get; set; }
        public int[] aBody { get; set; } = new int[2];

        public int BodyOffset { get; set; }
        public bool ConvertLeWaveToBe { get; set; } = false;

        public TimeSpan GetLength()
        {
            return TimeSpan.FromSeconds(WEnd / Frequence);
        }
    }

    public enum SgxDataFormat
    {
        LinearPCM_LE = 0, // WAV Implemented
        LinearPCM_BE = 1, // WAV BE Implemented
        OGG_VORBIS = 2, // Or is it?
        PSADPCM = 3, // Unimplemented (Used by GTPSP, GT5/6 sfx)
        ATRAC3plus = 4,
        ShortAPCDM = 5, // Unimplemented, used by LocoRoco Cocoreccho
        AC3 = 6, // AC3 Implemented
        Unk10 = 10,
        Unk11 = 11,
    }
}
