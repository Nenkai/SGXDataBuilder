using System;
using System.Collections.Generic;

using SGXDataBuilder.AudioFormats;
using CommandLine;


namespace SGXDataBuilder
{
    public class Program
    {
        public static string Version = "1.0.0";

        public static void Main(string[] args)
        {
            Console.WriteLine($"SGXDataBuilder by Nenkai#9075 - Ver.{Version}");
            Console.WriteLine();

            Parser.Default.ParseArguments<MakeVerbs>(args)
                .WithParsed<MakeVerbs>(Make)
                .WithNotParsed(HandleNotParsedArgs);
        }

        public static void Make(MakeVerbs make)
        {
            Console.WriteLine("SGXD Creation started...");
            Sgxd sgxd = Sgxd.Create(make.OutputPath, make.SplitBody);

            foreach (string file in make.InputFiles)
                sgxd.AddNewFile(file, make.BigEndianWave);

            Console.WriteLine("Building SGX file..");
            sgxd.Build();
        }

        public static void HandleNotParsedArgs(IEnumerable<Error> errors)
        {

        }
    }

    [Verb("make", HelpText = "Builds a Sony SGX SGD/SGH/SGB audio file.")]
    public class MakeVerbs
    {
        [Option('i', "input", Required = true, HelpText = "Input audio files (currently supported: .wav (PCM16 LE/BE) or .ac3)")]
        public IEnumerable<string> InputFiles { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output file(s)")]
        public string OutputPath { get; set; }

        [Option("split-body", HelpText = "Whether to split body and header into .sgh and .sgb files.")]
        public bool SplitBody { get; set; }

        [Option("wave-big-endian", HelpText = "Reverse endianess of wav frames, so that they are readable in vgmstream (vgmstream does not support enum 0 (PCM16LE) [https://github.com/vgmstream/vgmstream/blob/master/src/meta/sgxd.c]")]
        public bool BigEndianWave { get; set; }
    }
}