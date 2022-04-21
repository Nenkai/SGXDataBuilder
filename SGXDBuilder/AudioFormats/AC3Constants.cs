using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGXDataBuilder.AudioFormats
{
    public class AC3Constants
    {
        public const int AUDIO_SAMPLES_PER_AUDIO_BLOCK = 256;
        public const int AC3_SYNCFRAME_AUDIO_SAMPLE_COUNT = 6 * AUDIO_SAMPLES_PER_AUDIO_BLOCK;
        public static byte[] CHANNEL_COUNT_BY_ACMOD = new byte[] { 2, 1, 2, 3, 3, 4, 4, 5 };

        public const int AC3_NUM_FRAME_SIZE_TABLE_ENTRIES = 38;
        public const int AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES = 3;
        public static int[] kAC3SampleRateTable = new[] { 48000, 44100, 32000 };

        public static int[] BITRATE_BY_HALF_FRMSIZECOD = new int[] {
            32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, 384, 448, 512, 576, 640
        };

        public static int[][] AC3FrameSizeTable = new int[AC3_NUM_FRAME_SIZE_TABLE_ENTRIES][]
        {
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 64, 69, 96 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 64, 70, 96 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 80, 87, 120 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 80, 88, 120 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 96, 104, 144 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 96, 105, 144 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 112, 121, 168 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 112, 122, 168 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 128, 139, 192 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 128, 140, 192 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 160, 174, 240 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 160, 175, 240 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 192, 208, 288 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 192, 209, 288 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 224, 243, 336 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 224, 244, 336 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 256, 278, 384 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 256, 279, 384 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 320, 348, 480 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 320, 349, 480 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 384, 417, 576 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 384, 418, 576 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 448, 487, 672 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 448, 488, 672 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 512, 557, 768 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 512, 558, 768 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 640, 696, 960 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 640, 697, 960 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 768, 835, 1152 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 768, 836, 1152 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 896, 975, 1344 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 896, 976, 1344 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 1024, 1114, 1536 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 1024, 1115, 1536 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 1152, 1253, 1728 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 1152, 1254, 1728 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 1280, 1393, 1920 },
           new int[AC3_NUM_SAMPLE_RATE_TABLE_ENTRIES]{ 1280, 1394, 1920 }
        };
    }
}
