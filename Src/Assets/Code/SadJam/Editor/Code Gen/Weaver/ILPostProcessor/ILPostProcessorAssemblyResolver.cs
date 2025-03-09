using Mono.CecilX;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using Unity.CompilationPipeline.Common.ILPostProcessing;

namespace SadJamEditor.Weaver
{
    public class ILPostProcessorAssemblyResolver : IAssemblyResolver
    {
        private readonly string[] _assemblyReferences;

        private readonly ConcurrentDictionary<string, AssemblyDefinition> _assemblyCache = new();

        private readonly ConcurrentDictionary<string, string> _fileNameCache = new();

        private readonly ICompiledAssembly _compiledAssembly;
        AssemblyDefinition _selfAssembly;

        private readonly Logger _logger;

        public ILPostProcessorAssemblyResolver(ICompiledAssembly compiledAssembly, Logger logger)
        {
            this._compiledAssembly = compiledAssembly;
            _assemblyReferences = compiledAssembly.References;
            this._logger = logger;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Cleanup
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name) =>
            Resolve(name, new ReaderParameters(ReadingMode.Deferred));

        public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            if (name.Name == _compiledAssembly.Name) return _selfAssembly;

            if (!_fileNameCache.TryGetValue(name.Name, out string fileName))
            {
                fileName = FindFile(name.Name);
                _fileNameCache.TryAdd(name.Name, fileName);
            }

            if (fileName == null)
            {
                _logger.Warning($"ILPostProcessorAssemblyResolver.Resolve: Failed to find file for {name}");
                return null;
            }

            DateTime lastWriteTime = File.GetLastWriteTime(fileName);
            string cacheKey = fileName + lastWriteTime;
            if (_assemblyCache.TryGetValue(cacheKey, out AssemblyDefinition result)) return result;

            parameters.AssemblyResolver = this;
            MemoryStream ms = MemoryStreamFor(fileName);

            string pdb = fileName + ".pdb";
            if (File.Exists(pdb))
                parameters.SymbolStream = MemoryStreamFor(pdb);

            AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(ms, parameters);
            _assemblyCache.TryAdd(cacheKey, assemblyDefinition);
            return assemblyDefinition;
        }

        private string FindFile(string name)
        {
            foreach (string r in _assemblyReferences)
            {
                if (Path.GetFileNameWithoutExtension(r) == name)
                    return r;
            }

            string dllName = name + ".dll";

            foreach (string parentDir in _assemblyReferences.Select(Path.GetDirectoryName).Distinct())
            {
                string candidate = Path.Combine(parentDir, dllName);
                if (File.Exists(candidate))
                    return candidate;
            }

            return null;
        }

        private static MemoryStream MemoryStreamFor(string fileName)
        {
            return Retry(10, TimeSpan.FromSeconds(1), () =>
            {
                byte[] byteArray;
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    byteArray = new byte[fs.Length];
                    int readLength = fs.Read(byteArray, 0, (int)fs.Length);
                    if (readLength != fs.Length)
                        throw new InvalidOperationException("File read length is not full length of file.");
                }

                return new MemoryStream(byteArray);
            });
        }

        private static MemoryStream Retry(int retryCount, TimeSpan waitTime, Func<MemoryStream> func)
        {
            try
            {
                return func();
            }
            catch (IOException)
            {
                if (retryCount == 0)
                    throw;
                Console.WriteLine($"Caught IO Exception, trying {retryCount} more times");
                Thread.Sleep(waitTime);
                return Retry(retryCount - 1, waitTime, func);
            }
        }

        public void SetAssemblyDefinitionForCompiledAssembly(AssemblyDefinition assemblyDefinition)
        {
            _selfAssembly = assemblyDefinition;
        }
    }
}
