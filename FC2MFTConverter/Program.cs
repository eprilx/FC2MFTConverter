/*
MIT License

Copyright (c) 2021 eprilx

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using Mono.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;

namespace FC2MFTConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            string ToolVersion;
            try
            {
                ToolVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                ToolVersion = ToolVersion.Remove(ToolVersion.Length - 2);
            }
            catch
            {
                ToolVersion = "1.0.0";
            }
            string originalMFT = null;
            string fntBMF = null;
            string output = null;
            string command = null;

            // Change current culture
            CultureInfo culture;
            culture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            var p = new OptionSet()
            {
                {"fnt2mft", "Convert FNT to MFT",
                v => {command = "fnt2mft"; } },
                {"mft2fnt", "Convert MFT to FNT",
                v=> {command = "mft2fnt"; } }
            };
            p.Parse(args);

            switch (command)
            {
                case "fnt2mft":
                    p = new OptionSet() {
                { "f|originalMFT=", "(required) Original MFT file (*.MFT|*.Fire_Font_Descriptor)",
                    v => originalMFT = v },
                { "b|charDesc=", "(required) Character description file (*.fnt)",
                    v => fntBMF = v },
                { "o|NewMFT=",
                   "(optional) Output new MFT file",
                    v => output = v },
                };
                    break;
                case "mft2fnt":
                    p = new OptionSet() {
                { "f|originalMFT=", "(required) Original MFT file (*.MFT|*.Fire_Font_Descriptor)",
                    v => originalMFT = v },
                { "o|NewFNT=",
                   "(optional) Output FNT file",
                    v => output = v },
                };
                    break;
            }
            p.Parse(args);


            if (args.Length == 0 || originalMFT == null || (fntBMF == null && command == "fnt2mft"))
            {
                ShowHelp(p);
                return;
            }

            if (!originalMFT.EndsWith(".mft", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Unknown MFT file.");
                ShowHelp(p);
                return;
            }

            if (command == "fnt2mft")
            {
                if (!fntBMF.EndsWith(".fnt", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Unknown character description file.");
                    ShowHelp(p);
                    return;
                }
            }

            // CreateMFT
            try
            {
                switch (command)
                {
                    case "fnt2mft":
                        if (output == null)
                            output = originalMFT + ".new";
                        MFTFunction.CreateMFTfromFNT(originalMFT, fntBMF, output);
                        break;
                    case "mft2fnt":
                        if (output == null)
                            output = originalMFT + ".FNT";
                        MFTFunction.ConvertMFTtoFNT(originalMFT, output);
                        break;
                }
                Done();
            }
            finally
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(0);
            }

            void ShowHelp(OptionSet p)
            {
                switch (command)
                {
                    case "fnt2mft":
                        Console.WriteLine("\nUsage: FC2MFTConverter --fnt2mft [OPTIONS]");
                        break;
                    case "mft2fnt":
                        Console.WriteLine("\nUsage: FC2MFTConverter --mft2fnt [OPTIONS]");
                        break;
                    default:
                        PrintCredit();
                        Console.WriteLine("\nUsage: FC2MFTConverter [OPTIONS]");
                        break;
                }

                Console.WriteLine("Options:");
                p.WriteOptionDescriptions(Console.Out);

                if (command == null)
                {
                    Console.WriteLine("\nExample:");
                    Console.WriteLine("FC2MFTConverter --fnt2mft -f farcry2_25.mft -b yahei.fnt -o farcry2_25.mft.new");
                    Console.WriteLine("FC2MFTConverter --mft2fnt -f farcry2_25.mft -o farcry2_25.mft.fnt");
                    Console.WriteLine("\nMore usage: https://github.com/eprilx/FC2MFTConverter#usage");
                    Console.Write("More update: ");
                    Console.WriteLine("https://github.com/eprilx/FC2MFTConverter/releases");
                }
            }

            void PrintCredit()
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\nFC2MFTConverter v" + ToolVersion);
                Console.WriteLine(" by eprilx");
                Console.Write("Special thanks to: ");
                Console.WriteLine("abodora, Rick Gibbed");
                Console.ResetColor();
            }
            void Done()
            {
                Console.Write("\n********************************************");
                PrintCredit();
                Console.WriteLine("********************************************");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\n" + output + " has been created!");
                Console.ResetColor();
            }
        }
    }
}
