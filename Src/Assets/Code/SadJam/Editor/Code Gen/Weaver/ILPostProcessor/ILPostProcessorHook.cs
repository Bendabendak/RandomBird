using Mono.CecilX;
using Mono.CecilX.Cil;
using System.IO;
using System.Linq;
using Unity.CompilationPipeline.Common.ILPostProcessing;

namespace SadJamEditor.Weaver
{
    public class ILPostProcessorHook : ILPostProcessor
    {
        public const string SADJAM_ASSEMBLY_NAME = "SadJam";
        public const string IGNORE_DEFINE = "ILPP_IGNORE";

        public override ILPostProcessor GetInstance() => this;

        public override ILPostProcessResult Process(ICompiledAssembly compiledAssembly)
        {
            ILPostProcessorLogger logger = new();

            byte[] peData = compiledAssembly.InMemoryAssembly.PeData;
            using (MemoryStream stream = new(peData))
            using (ILPostProcessorAssemblyResolver asmResolver = new(compiledAssembly, logger))
            {
                using (MemoryStream symbols = new(compiledAssembly.InMemoryAssembly.PdbData))
                {
                    ReaderParameters readerParameters = new()
                    {
                        SymbolStream = symbols,
                        ReadWrite = true,
                        ReadSymbols = true,
                        AssemblyResolver = asmResolver,
                        ReflectionImporterProvider = new ILPostProcessorReflectionImporterProvider()
                    };

                    using (AssemblyDefinition asmDef = AssemblyDefinition.ReadAssembly(stream, readerParameters))
                    {
                        asmResolver.SetAssemblyDefinitionForCompiledAssembly(asmDef);

                        Weaver weaver = new(logger);

                        if (weaver.Weave(asmDef, out bool modified) && modified)
                        {
                            if (asmDef.MainModule.AssemblyReferences.Any(r => r.Name == asmDef.Name.Name))
                            {
                                asmDef.MainModule.AssemblyReferences.Remove(asmDef.MainModule.AssemblyReferences.First(r => r.Name == asmDef.Name.Name));
                            }

                            MemoryStream peOut = new();
                            MemoryStream pdbOut = new();
                            WriterParameters writerParameters = new()
                            {
                                SymbolWriterProvider = new PortablePdbWriterProvider(),
                                SymbolStream = pdbOut,
                                WriteSymbols = true
                            };

                            asmDef.Write(peOut, writerParameters);

                            InMemoryAssembly inMemory = new(peOut.ToArray(), pdbOut.ToArray());
                            return new ILPostProcessResult(inMemory, logger.Logs);
                        }
                    }
                }
            }

            return new ILPostProcessResult(compiledAssembly.InMemoryAssembly, logger.Logs);
        }

        public override bool WillProcess(ICompiledAssembly compiledAssembly)
        {
            bool relevant = compiledAssembly.Name == SADJAM_ASSEMBLY_NAME || compiledAssembly.References.Any(filePath => Path.GetFileNameWithoutExtension(filePath) == SADJAM_ASSEMBLY_NAME);
            bool ignore = HasDefine(compiledAssembly, IGNORE_DEFINE);

            return relevant && !ignore;
        }

        private static bool HasDefine(ICompiledAssembly assembly, string define) => assembly.Defines != null && assembly.Defines.Contains(define);
    }
}
