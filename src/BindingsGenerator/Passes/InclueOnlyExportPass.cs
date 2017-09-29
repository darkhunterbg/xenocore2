using CppSharp.AST;
using CppSharp.Passes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BindingsGenerator.Passes
{
    class IncludeOnlyExportsPass : TranslationUnitPass
    {
        public String ExportMacroName { get; set; }

        public IncludeOnlyExportsPass(String macroName)
        {
            ExportMacroName = macroName;
        }



        public override bool VisitClassDecl(Class @class)
        {
            if (@class.GenerationKind == GenerationKind.None)
                return false;


            if (!@class.Methods.Any(p => p.GenerationKind == GenerationKind.Generate))
            {
                @class.GenerationKind = GenerationKind.None;
                return false;
            }

            return base.VisitClassDecl(@class);
        }

        public override bool VisitMethodDecl(Method method)
        {
            if (method.DebugText.StartsWith(ExportMacroName))
                return base.VisitMethodDecl(method);

            method.GenerationKind = GenerationKind.None;

            var @class = ASTContext.FindCompleteClass(method.QualifiedLogicalName.Split(new String[] { "::" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault());
            if (@class == null)
                return false;

            VisitClassDecl(@class);

            return false;
        }

    }
}
