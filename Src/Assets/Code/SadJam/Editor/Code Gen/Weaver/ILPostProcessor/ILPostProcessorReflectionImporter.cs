using Mono.CecilX;
using System.Linq;
using System.Reflection;

namespace SadJamEditor.Weaver
{
    public class ILPostProcessorReflectionImporter : DefaultReflectionImporter
    {
        const string SystemPrivateCoreLib = "System.Private.CoreLib";
        readonly AssemblyNameReference fixedCoreLib;

        public ILPostProcessorReflectionImporter(ModuleDefinition module) : base(module)
        {
            // find the correct library for "System.Private.CoreLib".
            // either mscorlib or netstandard.
            // defaults to System.Private.CoreLib if not found.
            fixedCoreLib = module.AssemblyReferences.FirstOrDefault(a => a.Name == "mscorlib" || a.Name == "netstandard" || a.Name == SystemPrivateCoreLib);
        }

        public override AssemblyNameReference ImportReference(AssemblyName name)
        {
            // System.Private.CoreLib?
            if (name.Name == SystemPrivateCoreLib && fixedCoreLib != null)
                return fixedCoreLib;

            // otherwise import as usual
            return base.ImportReference(name);
        }
    }
}
