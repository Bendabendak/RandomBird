using System;
using System.IO;
using Unity.CompilationPipeline.Common.Diagnostics;
using Unity.CompilationPipeline.Common.ILPostProcessing;

namespace SadJamEditor.Weaver
{
    public static class ILPostProcessorFromFile
    {
        public static void ILPostProcessFile(string assemblyPath, string[] references, Action<string> OnWarning, Action<string> OnError)
        {
            CompiledAssemblyFromFile assembly = new CompiledAssemblyFromFile(assemblyPath);
            assembly.References = references;

            ILPostProcessorHook ilpp = new ILPostProcessorHook();
            if (ilpp.WillProcess(assembly))
            {
                ILPostProcessResult result = ilpp.Process(assembly);

                foreach (DiagnosticMessage message in result.Diagnostics)
                {
                    if (message.DiagnosticType == DiagnosticType.Warning)
                    {
                        OnWarning(message.MessageData);
                    }
                    else if (message.DiagnosticType == DiagnosticType.Error)
                    {
                        OnError(message.MessageData);
                    }
                }

                File.WriteAllBytes(assemblyPath, result.InMemoryAssembly.PeData);
            }
        }
    }
}
