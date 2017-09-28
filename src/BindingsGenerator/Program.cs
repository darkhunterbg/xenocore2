using CppSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BindingsGenerator
{
    class Program
    {
        const String ExtraHeaderFileName = "CppSharp.h";
        const String GenFileName = "gen";

#if WIN32
        const String LibPlatform = "Win32";
#endif

        static void Main(string[] args)
        {
            var currentDir = Directory.GetCurrentDirectory();
            var outputDir = Path.Combine(currentDir, "bindings");

            String sourceDir = Path.GetFullPath(Path.Combine(currentDir, $@"..\..\..\src"));
            String libDir = currentDir;
            XenoCoreLibrary library = new XenoCoreLibrary(LibPlatform)
            {
                SourceDir = sourceDir,
                LibDir = libDir
            };

            String genFile = Path.Combine(outputDir, GenFileName);

            if (File.Exists(genFile))
            {
                var genModified = File.GetLastWriteTime(genFile);

                var libModified = File.GetLastWriteTime(Path.Combine(currentDir, library.LibraryName));

                if (libModified < genModified)
                {
                    Console.WriteLine("Library was not modified. Skipping binding generation...");
                    return;
                }
            }

            GenerateBindings(currentDir, outputDir, library);

            File.WriteAllText(genFile, DateTime.Now.ToString());
        }

        static void GenerateBindings(String currentDir, String outputDir, ILibrary library)
        {


            if (Directory.Exists(outputDir))
                Directory.Delete(outputDir, true);

            Directory.CreateDirectory(outputDir);

            Directory.SetCurrentDirectory(outputDir);

            //Generate code
            ConsoleDriver.Run(library);

            File.Copy(Path.Combine(currentDir, ExtraHeaderFileName), Path.Combine(outputDir, ExtraHeaderFileName));


            Directory.SetCurrentDirectory(currentDir);
        }
    }
}
