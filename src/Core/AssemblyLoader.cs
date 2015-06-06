using Driven.Metrics.Interfaces;
using Mono.Cecil;

using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using PostSharp.Sdk.Binary;
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
            Domain domain = new Domain(ReflectionLoadOptions.All, true);
            //AssemblyDefinition definition = AssemblyFactory.GetAssembly(assemblyLocation);
            //Assembly assembly = AssemblyFactory.CreateReflectionAssembly(definition);

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            module = domain.LoadAssembly(assemblyLocation, LoadAssemblyOptions.None).ManifestModule;
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