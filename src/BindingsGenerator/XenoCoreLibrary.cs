using CppSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CppSharp.AST;
using CppSharp.Generators;
using System.IO;

namespace BindingsGenerator
{
    class XenoCoreLibrary : ILibrary
    {
        const String ProjectName = "XenoCore";

        public readonly String PlatformName ;
        public readonly String LibraryName;
        public readonly String LibraryFileName;

        public String SourceDir { get; set; }
        public String LibDir { get; set; }


        public XenoCoreLibrary( String platformName)
        {
            this.PlatformName = platformName;
            LibraryName = $"{ProjectName}.{PlatformName}";

            LibraryFileName = $"{LibraryName}.lib";
        }

        public void Postprocess(Driver driver, ASTContext ctx)
        {
        }

        public void Preprocess(Driver driver, ASTContext ctx)
        {
        }

        public void Setup(Driver driver)
        {
            var options = driver.Options;
            options.GeneratorKind = GeneratorKind.CLI;
            var module = options.AddModule(LibraryName);

            module.IncludeDirs.Add(Path.Combine(SourceDir, ProjectName));
            module.IncludeDirs.Add(Path.Combine(SourceDir, module.LibraryName));

            foreach (var includeDir in module.IncludeDirs)
            {

                foreach (var file in Directory.GetFiles(includeDir, "*.h"))
                {
                    module.Headers.Add(file.Substring(includeDir.Length + 1));
                }
            }
            module.LibraryDirs.Add(LibDir);
            module.Libraries.Add($"{module.LibraryName}.lib");

        }

        public void SetupPasses(Driver driver)
        {
        }
    }
}
