using System.IO;

using CommandLine.Text;
using CommandLine;

namespace VagConvSharp
{
    public class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("VagConvSharp by Nenkai#9075 - Ported from VAG Packer by bITmASTER");
            Console.WriteLine();

            Parser.Default.ParseArguments<ConvVerbs>(args)
                .WithParsed(Convert)
                .WithNotParsed(HandleNotParsedArgs);
        }

        public static void Convert(ConvVerbs options)
        {
            if (!File.Exists(options.InputWavFile))
            {
                Console.WriteLine("Input file does not exist");
                return;
            }

            var v = new WavToVagConverter();
            string output;
            if (!string.IsNullOrEmpty(options.OutputVagFile))
            {
                string outputName = options.OutputVagFile;
                string fullPath = Path.GetFullPath(outputName);
                output = Path.ChangeExtension(fullPath, ".vag");
            }
            else
            {
                output = Path.ChangeExtension(options.InputWavFile, ".vag");
            }

            if (options.Label.Length > 15)
            {
                Console.WriteLine("Vag label must not be >15 characters");
                return;
            }

            try
            {
                v.Convert(options.InputWavFile, output,
                    !string.IsNullOrEmpty(options.Label) ? options.Label : "VagConvSharp",
                    options.Loop);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to convert to vag: {e.Message}");
                return;
            }

            Console.WriteLine($"{options.InputWavFile} -> {output}");
        }

        public static void HandleNotParsedArgs(IEnumerable<Error> errors)
        {

        }

        [Verb("conv", HelpText = "Builds Sony Vag file from WAV files.")]
        public class ConvVerbs
        {
            [Option('i', "input", Required = true, HelpText = "Input .wav file")]
            public string InputWavFile { get; set; }

            [Option('o', "output", HelpText = "Output .vag file")]
            public string OutputVagFile { get; set; }

            [Option('n', "name", HelpText = "Optional: Vag header label/name (15 chars max)")]
            public string Label { get; set; }

            [Option('l', "loop", HelpText = "Enable looping")]
            public bool Loop { get; set; }
        }
    }
}