using Driven.Metrics.Interfaces;
using Mono.Cecil;

using System.Collections.Generic;

using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.Extensibility.Licensing;

namespace Driven.Metrics
{
    public class AssemblyLoader : IAssemlyLoader
    {
        private string _assemblyName;

        public static ModuleDeclaration module;
        public static List<TypeDefDeclaration> all_valid_types = new List<TypeDefDeclaration>();
        
        public AssemblyLoader(string assemblyName)
        {
            _assemblyName = assemblyName;
        }

        public AssemblyDefinition Load()
        {
            var assemblyDef = AssemblyFactory.GetAssembly(_assemblyName);
			assemblyDef.MainModule.LoadSymbols();

            return assemblyDef;
        }

        public static void LoadAssemblyPostSharp(string assemblyLocation)
        {
            all_valid_types.Clear();
            Domain domain = new Domain();
            module = domain.LoadAssembly(assemblyLocation, true).ManifestModule;
            foreach (var type in module.Types)
            {
                if (!type.Name.Contains("Settings") && !type.Name.Contains("Resources") && !type.Name.Contains("DataSet")
                    && !type.Name.Contains("<Module>") && !type.Name.Contains("<PrivateImplementationDetails>")
                    && !type.Name.Contains("__"))

                    all_valid_types.Add(type);
            }
        }


    }
}