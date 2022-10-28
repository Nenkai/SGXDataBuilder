using System;
using System.Collections.Generic;

using SGXDataBuilder.AudioFormats;
using CommandLine;


namespace SGXDataBuilder
{
    public class Program
    {
        public static string Version = "2.0.0";

        public static void Main(string[] args)
        {
            Console.WriteLine($"SGXDataBuilder by Nenkai#9075 - Ver.{Version}");
            Console.WriteLine();

            Parser.Default.ParseArguments<MakeVerbs>(args)
                .WithParsed(Make)
                .WithNotParsed(HandleNotParsedArgs);
        }

        public static void Make(MakeVerbs make)
        {
            Console.WriteLine("SGXD Creation started...");
            Sgxd sgxd = new Sgxd();
            sgxd.ImportFromProject(make.InputFile);

            Console.WriteLine("Building SGX file..");
            sgxd.Build(make.OutputPath);
        }

        public static void HandleNotParsedArgs(IEnumerable<Error> errors)
        {

        }
    }

    [Verb("build", HelpText = "Builds a Sony SGX SGD/SGH/SGB audio file.")]
    public class MakeVerbs
    {
        [Option('i', "input", Required = true, HelpText = "Input .sgxdproj file (created by the GUI program)")]
        public string InputFile { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output file")]
        public string OutputPath { get; set; }
    }
}