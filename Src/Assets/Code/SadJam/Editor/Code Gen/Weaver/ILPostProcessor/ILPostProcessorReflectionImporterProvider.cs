using Mono.CecilX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadJamEditor.Weaver
{
    public class ILPostProcessorReflectionImporterProvider : IReflectionImporterProvider
    {
        public IReflectionImporter GetReflectionImporter(ModuleDefinition module) => new ILPostProcessorReflectionImporter(module);
    }
}
