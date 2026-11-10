namespace SGXLib.AudioFormats
{
    public interface IAudioFormat
    {
        public byte GetChannelCount();

        public int GetTotalSampleCount();

        public int GetFrameSize_ForPar1();

        public int GetSampleRate_Frequence();

        public int GetBodySize();

        public int GetSizeWithHeaderIfNeededForSGX();

        public int GetBitRate_ForPar0();

        public int GetBodyOffset();

    }
}
