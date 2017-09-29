using CppSharp;
using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml;
using System.Linq;

namespace BindingsGenerator
{
    class Program
    {
        const String ExtraHeaderFileName = "CppSharp.h";
        const String GenFileName = "gen";

        const String LibPlatform = "Win32";

        static void Main(string[] args)
        {
            int workingDir = args.ToList().IndexOf("--dir");
            String currentDir = null;
            if (workingDir > -1)
            {
                currentDir = args[workingDir + 1];
            }
            else
            {
                currentDir = Directory.GetCurrentDirectory();
            }

            XenoCoreLibrary library = CreateLibrary(currentDir);

            var outputDir = Path.Combine(currentDir, "bindings", library.LibraryName);

            if (!File.Exists(Path.Combine(currentDir, library.LibraryFileName)))
            {
                Console.WriteLine($"{library.LibraryFileName} is missing. Skipping bindings generation...");
                BuildProjectFile(outputDir, library);
                return;
            }

            String genFile = Path.Combine(outputDir, GenFileName);
            var genModified = File.GetLastWriteTime(genFile);
            var toolModified = File.GetLastWriteTime(Path.Combine(currentDir, "BindingsGenerator.exe"));

            bool forceRebuild = toolModified > genModified;

            if (args.Contains("--rebuild"))
                forceRebuild = true;



            //if (!forceRebuild)
            //{
            //    if (File.Exists(genFile))
            //    {
            //        var libModified = File.GetLastWriteTime(Path.Combine(currentDir, library.LibraryFileName));

            //        if (libModified < genModified)
            //        {
            //            Console.WriteLine("Library was not modified. Skipping binding generation...");
            //            return;
            //        }
            //    }
            //}

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
                PostBuildCleanup(outputDir);
            }

            BuildProjectFile(outputDir, library);

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

        static void BuildProjectFile(String outputDir, XenoCoreLibrary lib)
        {
            String projectName = $"{lib.LibraryName}.NET";

            String projectFile = Path.GetFullPath(Path.Combine($@"..\..\..\src\{projectName}\{projectName}.vcxproj"));

            XNamespace msbuild = "http://schemas.microsoft.com/developer/msbuild/2003";
            XName ClCompile = msbuild + "ClCompile";
            XName ClInclude = msbuild + "ClInclude";
            XName ItemGroup = msbuild + "ItemGroup";

            XDocument xml = XDocument.Load(projectFile);

            var rootSource = (xml.FirstNode as XElement).Elements(ItemGroup).FirstOrDefault(p => p.Element(ClCompile) != null);
            var rootHeader = (xml.FirstNode as XElement).Elements(ItemGroup).FirstOrDefault(p => p.Element(ClInclude) != null);

            if (rootHeader == null)
            {
                rootHeader = new XElement(ItemGroup);
                (xml.FirstNode as XElement).Add(rootHeader);
            }
            if (rootSource == null)
            {
                rootSource = new XElement(ItemGroup);
                (xml.FirstNode as XElement).Add(rootSource);
            }

            var sourceFiles = rootSource.Elements()
                .Select(p => p.FirstAttribute.Value)
                .ToList();
            var headerFiles = rootHeader.Elements()
                .Select(p => p.FirstAttribute.Value)
                .ToList();

            var root = xml.FirstNode as XElement;

            int modifications = 0;


            var generatedFiles = Directory.GetFiles(outputDir).Where(p => p.EndsWith(".cpp") || p.EndsWith(".h"));

            foreach (var file in generatedFiles)
            {
                if (file.EndsWith(".cpp"))
                {
                    if (!sourceFiles.Contains(file))
                    {
                        var e = new XElement(ClCompile);
                        e.SetAttributeValue("Include", file);
                        rootSource.Add(e);
                        ++modifications;
                    }
                }
                if (file.EndsWith(".h"))
                {
                    if (!headerFiles.Contains(file))
                    {
                        var e = new XElement(ClInclude);
                        e.SetAttributeValue("Include", file);
                        rootHeader.Add(e);
                        ++modifications;
                    }
                }
            }

            var usedFiles = rootHeader.Elements().ToList();
            usedFiles.AddRange(rootSource.Elements());

            foreach (var file in usedFiles)
            {
                if (!generatedFiles.Contains(file.FirstAttribute.Value))
                {
                    ++modifications;
                    file.Remove();
                }
            }

            if (modifications > 0)
                xml.Save(projectFile);
        }

        static void PostBuildCleanup(String outputDir)
        {
            var generatedFiles = Directory.GetFiles(outputDir).Where(p => p.EndsWith(".h"));

            foreach (var file in generatedFiles)
            {
                bool del = true;

                using (StreamReader reader = new StreamReader(file))
                {
                    while (!reader.EndOfStream)
                    {
                        String text = reader.ReadLine();
                        if (text.Contains("class") || text.Contains("enum"))
                        {
                            del = false;
                            break;
                        }
                    }
                }

                if (del)
                {
                    String outHeader = file;
                    if (File.Exists(outHeader))
                        File.Delete(outHeader);

                    String outSource = file.Substring(0, file.Length - 1) + "cpp";
                    if (File.Exists(outSource))
                        File.Delete(outSource);
                }
            }
        }
    }
}
