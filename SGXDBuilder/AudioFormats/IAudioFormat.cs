using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGXDataBuilder.AudioFormats
{
    public interface IAudioFormat
    {
        public byte GetChannelCount();

        public int GetTotalSampleCount();

        public int GetFrameSize();

        public int GetSampleRate_Frequence();

        public int GetBodySize();

        public int GetBitRate();

        public int GetBodyOffset();
    }
}
