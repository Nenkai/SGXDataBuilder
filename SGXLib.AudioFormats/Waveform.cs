using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;
using SGXLib.Utils;

namespace SGXLib.AudioFormats
{
    public class Waveform : IAudioFormat
    {
        public ushort ChannelCount { get; set; }
        public ushort BitsPerSample { get; set; }
        public int SampleCount { get; set; }
        public uint dwSamplesPerSec { get; set; }

        public int BodyOffset { get; set; }
        public bool BigEndian { get; set; } = false;

        public static Waveform Read(string fileName)
        {
            RIFFFile riff = RIFFFile.Read(fileName);

            if (riff.BitsPerSample != 16)
                throw new NotSupportedException($"WAVE must be PCM 16 bits (got: {riff.BitsPerSample}). File: {fileName}");

            var wav = new Waveform();
            wav.ChannelCount = riff.ChannelCount;
            wav.BitsPerSample = riff.BitsPerSample;
            wav.SampleCount = riff.SampleCount;
            wav.dwSamplesPerSec = riff.dwSamplesPerSec;
            wav.BodyOffset = riff.BodyOffset;
            wav.BigEndian = riff.BigEndian;

            return wav;
        }

        public int GetBodyOffset()
        {
            return BodyOffset;
        }

        public int GetBodySize()
        {
            return SampleCount * BitsPerSample / 8 * ChannelCount;
        }

        public int GetSizeWithHeaderIfNeededForSGX()
        {
            return GetBodySize();
        }

        public byte GetChannelCount()
        {
            return (byte)ChannelCount;
        }

        public int GetTotalSampleCount()
        {
            return SampleCount;
        }

        public int GetFrameSize_ForPar1()
        {
            return 0;
        }

        public int GetBitRate_ForPar0()
        {
            return 0;
        }

        public int GetSampleRate_Frequence()
        {
            return (int)dwSamplesPerSec;
        }
    }
}
