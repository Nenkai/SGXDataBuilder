using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Syroot.BinaryData;

using SGXLib.Utils;
using SGXLib.AudioFormats;

namespace VagConvSharp
{
    public class WavToVagConverter
    {
        private bool _enableLooping;

        public WavToVagConverter()
        {

        }

        public void Convert(string wavFile, string outputVagFile, string vagLabel, bool enableLooping = false)
        {
            RIFFFile riff = RIFFFile.Read(wavFile);

            if (riff.ChannelCount != 1)
                throw new InvalidDataException("WAV File must be mono");

            // Write header
            using var vagFs = File.Create(outputVagFile);
            using var vagStreamWriter = new BinaryStream(vagFs, ByteConverter.Big);

            vagStreamWriter.WriteString("VAGp", StringCoding.Raw);
            vagStreamWriter.WriteUInt32(0x20); // Version
            vagStreamWriter.WriteInt32(0);

            int data_size = riff.DataChunkSize;
            int sample_size = riff.BitsPerSample;

            if (riff.BitsPerSample != 8 && riff.BitsPerSample != 16)
                throw new InvalidDataException("WAV File must be 8 or 16 Bits PCM");

            if (sample_size == 16)
                data_size /= 2;

            int size_in_samples = data_size / 28;
            if (data_size % 28 != 0)
                size_in_samples++;

            vagStreamWriter.WriteInt32(0x10 * (size_in_samples + 2));
            vagStreamWriter.WriteUInt32(riff.dwSamplesPerSec);
            vagStreamWriter.Position += 0x0C;

            vagStreamWriter.WriteString(vagLabel, StringCoding.Raw);
            vagStreamWriter.Align(0x10, grow: true);

            using var waveFs = File.Open(wavFile, FileMode.Open);
            using var waveStreamReader = new BinaryStream(waveFs);
            waveStreamReader.Position = riff.BodyOffset;

            const int BufferSize = 512 * 28;
            short[] buffer = new short[BufferSize];

            double[] d_samples = new double[28];
            int predict_nr = 0;
            int shift_factor = 0;
            int flags = 0;
            short[] four_bit = new short[28];

            if (enableLooping)
                flags = 6;

            while (data_size > 0)
            {
                int i;
                int current_size = Math.Min(data_size, BufferSize);
                if (riff.BitsPerSample == 8)
                {
                    for (i = 0; i < current_size; i++)
                        buffer[i] = (short)((waveStreamReader.ReadByte() ^ 0x80) << 8);
                }
                else
                {
                    for (i = 0; i < current_size; i++)
                        buffer[i] = waveStreamReader.ReadInt16();
                }

                i = current_size / 28;
                if (current_size % 28 != 0)
                {
                    for (var j = current_size % 28; j < 28; j++)
                        buffer[28 * i + j] = 0;
                    i++;
                }

                for (var j = 0; j < i; j++)
                {
                    Span<short> ptr = buffer.AsSpan(j * 28);
                    FindPredict(ptr, d_samples, ref predict_nr, ref shift_factor);
                    Pack(d_samples, four_bit, predict_nr, shift_factor);

                    byte d = (byte)((predict_nr << 4) | shift_factor);
                    vagStreamWriter.WriteByte(d);
                    vagStreamWriter.WriteByte((byte)flags);

                    for (var k = 0; k < 28; k += 2)
                    {
                        d = (byte)( (byte)((four_bit[k + 1] >> 8) & 0xF0) | (byte)((four_bit[k] >> 12) & 0x0F) );
                        vagStreamWriter.WriteByte(d);
                    }

                    data_size -= 28;
                    if (data_size < 28 && !_enableLooping)
                        flags = 1;

                    if (_enableLooping)
                        flags = 2;
                }
            }

            vagStreamWriter.WriteByte((byte)((predict_nr << 4) | shift_factor));

            if (_enableLooping)
                vagStreamWriter.WriteByte(3);
            else
                vagStreamWriter.WriteByte(7);

            for (var i = 0; i < 14; i++)
                vagStreamWriter.WriteByte(0);

            vagStreamWriter.BaseStream.Dispose();
            waveFs.Dispose();
        }

        static readonly double[][] f = new double[5][/*2*/]
        {
            new[] { 0.0, 0.0 },
            new[] {  -60.0 / 64.0, 0.0 },
            new[] { -115.0 / 64.0, 52.0 / 64.0 },
            new[] {  -98.0 / 64.0, 55.0 / 64.0 },
            new[] { -122.0 / 64.0, 60.0 / 64.0 } 
        };

        static double _s_1 = 0.0;
        static double _s_2 = 0.0;

        private static void FindPredict(Span<short> samples, Span<double> d_samples, ref int predict_nr, ref int shift_factor)
        {
            double[] max = new double[5];
            double min = 1e10;

            double[][] buffer = new double[28][/*5*/];
            for (var i = 0; i < 28; i++)
                buffer[i] = new double[5];

            double s_1 = 0.0, s_2 = 0.0;

            for (int i = 0; i < 5; i++)
            {
                max[i] = 0.0;
                s_1 = _s_1;
                s_2 = _s_2;

                for (var j = 0; j < 28; j++)
                {
                    double s_0 = Math.Clamp((double)samples[j], -30720.0, 30719.0);
                    var ds = s_0 + s_1 * f[i][0] + s_2 * f[i][1];
                    buffer[j][i] = ds;
                    if (Math.Abs(ds) > max[i])
                        max[i] = Math.Abs(ds);

                    s_2 = s_1;
                    s_1 = s_0;
                }

                if (max[i] < min)
                {
                    min = max[i];
                    predict_nr = i;
                }

                if (min <= 7)
                {
                    predict_nr = 0;
                    break;
                }
            }

            _s_1 = s_1;
            _s_2 = s_2;

            for (var i = 0; i < 28; i++)
                d_samples[i] = buffer[i][predict_nr];

            int min2 = (int)min;
            int shift_mask = 0x4000;
            shift_factor = 0;

            while (shift_factor < 12)
            {
                if ((shift_mask & (min2 + (shift_mask >> 3))) != 0)
                    break;

                shift_factor++;
                shift_mask >>= 1;
            }
        }

        private static void Pack(Span<double> d_samples, Span<short> four_bit, int predict_nr, int shift_factor)
        {
            double s_1 = 0.0, s_2 = 0.0;
            for (var i = 0; i < 28; i++)
            {
                double s_0 = d_samples[i] + s_1 * f[predict_nr][0] + s_2 * f[predict_nr][1];
                double ds = s_0 * (double)(1 << shift_factor);
                int di = (int)(((int)ds + 0x800) & 0xFFFFF000);
                di = Math.Clamp(di, -32768, 32767);

                four_bit[i] = (short)di;

                di >>= shift_factor;
                s_2 = s_1;
                s_1 = (double)di - s_0;
            }
        }
    }
}
