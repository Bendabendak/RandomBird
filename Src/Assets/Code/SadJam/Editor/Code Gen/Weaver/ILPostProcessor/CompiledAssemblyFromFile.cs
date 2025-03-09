using System.IO;
using Unity.CompilationPipeline.Common.ILPostProcessing;

namespace SadJamEditor.Weaver
{
    public class CompiledAssemblyFromFile : ICompiledAssembly
    {
        readonly string assemblyPath;

        public string Name => Path.GetFileNameWithoutExtension(assemblyPath);
        public string[] References { get; set; }
        public string[] Defines { get; set; }
        public InMemoryAssembly InMemoryAssembly { get; }

        public CompiledAssemblyFromFile(string assemblyPath)
        {
            this.assemblyPath = assemblyPath;
            byte[] peData = File.ReadAllBytes(assemblyPath);
            string pdbFileName = Path.GetFileNameWithoutExtension(assemblyPath) + ".pdb";
            byte[] pdbData = File.ReadAllBytes(Path.Combine(Path.GetDirectoryName(assemblyPath), pdbFileName));
            InMemoryAssembly = new InMemoryAssembly(peData, pdbData);
        }
    }
}
