using Mono.CecilX;
using Mono.CecilX.Rocks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SadJamEditor.Weaver
{
    public class Weaver
    {
        public Logger Logger;

        private bool _weavingFailed;
        private AssemblyDefinition _currentAssembly;
        private WeaverTypes _weaverTypes;

        public Weaver(Logger logger)
        {
            Logger = logger;
        }

        private bool WeaveModule(ModuleDefinition moduleDefinition)
        {
            bool modified = false;

            List<TypeDefinition> allTypes = moduleDefinition.GetAllTypes().ToList();
            Dictionary<TypeDefinition, List<GameConfigPropertyDefinition>> gameConfigProps = new();

            foreach (TypeDefinition td in allTypes)
            {
                if (td.IsClass && td.BaseType.CanBeResolved())
                {
                    modified |= GameConfigSerializePropertyProcessor.Process(td, _weaverTypes, Logger, ref _weavingFailed, ref gameConfigProps);
                }
            }

            foreach (TypeDefinition td in allTypes)
            {
                if (td.IsClass && td.BaseType.CanBeResolved())
                {
                    modified |= OnGameConfigChangedProcessor.Process(td, gameConfigProps, _weaverTypes, Logger, ref _weavingFailed);
                    modified |= OnNewGameConfigSetProcessor.Process(td, gameConfigProps, _weaverTypes, Logger, ref _weavingFailed);
                }
            }

            return modified;
        }

        public bool Weave(AssemblyDefinition assembly, out bool modified)
        {
            _weavingFailed = false;
            modified = false;

            try
            {
                _currentAssembly = assembly;

                _weaverTypes = new WeaverTypes(_currentAssembly, Logger, ref _weavingFailed);

                ModuleDefinition moduleDefinition = _currentAssembly.MainModule;
                modified |= WeaveModule(moduleDefinition);

                if (_weavingFailed)
                {
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Logger.Error($"Exception :{e}");
                _weavingFailed = true;
                return false;
            }
        }
    }
}
