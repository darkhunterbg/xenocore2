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
    class XenoCoreSource
    {
        public String IncludeDir { get; set; }
        public List<String> Headers { get; private set; } = new List<string>();
    }

    class XenoCoreLibrary : ILibrary
    {
        public readonly String ProjectName = "XenoCore";

        public readonly String PlatformName ;
        public readonly String LibraryName;
        public readonly String LibraryFileName;

        public String LibDir { get; set; }

        public List<XenoCoreSource> Sources { get; private set; } = new List<XenoCoreSource>();

        public XenoCoreLibrary( String platformName)
        {
            this.PlatformName = platformName;
            LibraryName = $"{ProjectName}.{PlatformName}";

            LibraryFileName = $"{LibraryName}.lib";
        }


        public void Setup(Driver driver)
        {
            var options = driver.Options;
            options.GeneratorKind = GeneratorKind.CLI;
            var module = options.AddModule(LibraryName);


            //module.IncludeDirs.Add(Path.Combine(SourceDir, ProjectName));
            //module.IncludeDirs.Add(Path.Combine(SourceDir, module.LibraryName));
            module.OutputNamespace = ProjectName;

            module.IncludeDirs.AddRange(Sources.Select(p => p.IncludeDir));
            module.Headers.AddRange(Sources.SelectMany(p => p.Headers));

            module.LibraryDirs.Add(LibDir);
            module.Libraries.Add($"{module.LibraryName}.lib");

        }

        public void SetupPasses(Driver driver)
        {
        }

        public void Preprocess(Driver driver, ASTContext ctx)
        {
        }

        public void Postprocess(Driver driver, ASTContext ctx)
        {
        }
    }
}
