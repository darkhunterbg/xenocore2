using CppSharp;
using Microsoft.Build.Execution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Logging;
using System.Diagnostics;

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

            XenoCoreLibrary library = CreateLibrary(currentDir);
            String genFile = Path.Combine(outputDir, GenFileName);
            var genModified = File.GetLastWriteTime(genFile);
            var toolModified = File.GetLastWriteTime(Path.Combine(currentDir, "BindingsGenerator.exe"));

            bool forceRebuild = true;// toolModified > genModified;

            if (!forceRebuild)
            {
                if (File.Exists(genFile))
                {
                    var libModified = File.GetLastWriteTime(Path.Combine(currentDir, library.LibraryFileName));

                    if (libModified < genModified)
                    {
                        Console.WriteLine("Library was not modified. Skipping binding generation...");
                        return;
                    }
                }
            }

            int modifiedFiles = 0;
            foreach (var source in library.Sources)
            {
                var dir = new DirectoryInfo(source.IncludeDir);

                foreach (var file in dir.GetFiles("*.h"))
                {
                    var bindingModified = File.GetLastWriteTime(Path.Combine(outputDir, file.Name));

                    if (forceRebuild || file.LastWriteTime > bindingModified)
                    {
                        ++modifiedFiles;
                        source.Headers.Add(file.Name);
                    }
                }
            }


            Console.WriteLine($"{modifiedFiles} files were modified and need new bindings.");

            if (modifiedFiles > 0)
            {
                GenerateBindings(currentDir, outputDir, library);
                BuildProjectFile(outputDir);
            }
            File.WriteAllText(genFile, DateTime.Now.ToString());
        }

        static XenoCoreLibrary CreateLibrary(String currentDir)
        {
            String sourceDir = Path.GetFullPath(Path.Combine(currentDir, $@"..\..\..\src"));
            String libDir = currentDir;
            XenoCoreLibrary library = new XenoCoreLibrary(LibPlatform)
            {
                LibDir = libDir
            };
            library.Sources.Add(new XenoCoreSource()
            {
                IncludeDir = Path.Combine(sourceDir, library.ProjectName)
            });
            library.Sources.Add(new XenoCoreSource()
            {
                IncludeDir = Path.Combine(sourceDir, library.LibraryName)
            });
            return library;
        }

        static void GenerateBindings(String currentDir, String outputDir, ILibrary library)
        {
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            Directory.SetCurrentDirectory(outputDir);

            //Generate code
            ConsoleDriver.Run(library);

            String outExtraFile = Path.Combine(outputDir, ExtraHeaderFileName);

            if (!File.Exists(outExtraFile))
                File.Copy(Path.Combine(currentDir, ExtraHeaderFileName), outExtraFile);


            Directory.SetCurrentDirectory(currentDir);
        }

        static void BuildProjectFile(String outputDir)
        {
            String projectFile = Path.GetFullPath(Path.Combine(@"..\..\..\src\XenoCore.Win32.NET\XenoCore.Win32.NET.vcxproj"));


            Project project = new Project(projectFile);
            foreach (var file in Directory.GetFiles(outputDir))
            {
                if (file.EndsWith(".cpp"))
                {
                    project.AddItem("ClCompile", file);
                }
                if (file.EndsWith(".h"))
                {
                    project.AddItem("ClInclude", file);
                }
            }

            //Will reuse existing project
            project.Save(Path.GetFullPath(Path.Combine(@"..\..\..\src\XenoCore.Win32.NET\tmp.XenoCore.Win32.NET.vcxproj")));
            // project.Build(new String[] { "Build" }, new Microsoft.Build.Framework.ILogger[] { new ConsoleLogger() });

        
        }
    }
}
