using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SGXLib.Utils;

using static SGXLib.AudioFormats.AC3Constants;

namespace SGXLib.AudioFormats
{
    public class AC3 : IAudioFormat
    {
        public AC3SynchronizationInformation syncinfo = new AC3SynchronizationInformation();
        public AC3BinaryStreamInformation bsi = new AC3BinaryStreamInformation();

        public int FileSize;

        public static AC3 Read(string fileName)
        {
            AC3 ac3 = new AC3();

            byte[] header = new byte[0x100];
            using FileStream fs = new FileStream(fileName, FileMode.Open);
            fs.Read(header);
            ac3.FileSize = (int)fs.Length;

            BitStream bs = new BitStream(BitStreamMode.Read, header);
            ac3.syncinfo.Read(ref bs);
            ac3.bsi.Read(ref bs);

            return ac3;
        }

        public int GetBodyOffset()
        {
            return 0;
        }

        public int GetBodySize()
        {
            return FileSize;
        }

        public int GetSizeWithHeaderIfNeededForSGX()
        {
            return FileSize;
        }

        public byte GetChannelCount()
        {
            return CHANNEL_COUNT_BY_ACMOD[bsi.acmod];
        }

        public int GetSyncFrameCount()
        {
            return FileSize / GetFrameSize_ForPar1();
        }

        public int GetTotalSampleCount()
        {
            return GetSyncFrameCount() * AC3_SYNCFRAME_AUDIO_SAMPLE_COUNT;
        }

        public int GetFrameSize_ForPar1()
        {
            return sizeof(ushort) // Number of 16-bit words
                * AC3FrameSizeTable[syncinfo.frmsizecod][syncinfo.fscod];
        }

        public int GetBitRate_ForPar0()
        {
            return BITRATE_BY_HALF_FRMSIZECOD[syncinfo.frmsizecod / 2];
        }

        public int GetSampleRate_Frequence()
        {
            return kAC3SampleRateTable[syncinfo.fscod];
        }


    }
}
