﻿using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Helpers
{
    public static class DataFilterHelper
    {
        /// <summary>
        /// This method accepts List<T> and userQuery. Data that is filtered is refered as "data" on userQuery.
        /// It compiles the user query as a in memory assembly by filling it in a simple class. Once compiled,
        /// source data is passed in to the assembly instance and excution result is collected.
        /// 
        /// Usage:
        /// ======================================================
        /// List<object> sourceData = new List<object>()
        /// ............. load sourceData
        /// string query = "data.Where(x => x.Region == \"East\" && x.Units > 50).Select(x => new {x.Item})";
        /// string query = "from d in data where d.Item == \"Pencil\" && d.Units > 50 select new {d.Region, d.Units, d.Item}";
        /// string query = @"data.GroupBy(x => x.Region).Select(x => new {Region = x.Key, Units = x.Sum(y => y.Units)})";
        ///
        /// var filteredData = DataFilterHelper.GetFilteredData(sourceData, query);
        /// ==========================================================
        ///
        /// 
        /// </summary>
        public static List<dynamic> GetFilteredData<T>(List<T> sourceData, string userQuery)
        {
            //cast objects to dynamic so that it can be passed on to another assembly.
            var data = sourceData.Cast<dynamic>().ToList();

            #region template Code

            //add required namespaces
            var defaultNamespaces = new[]
                {
                    "System", " System.Dynamic", "System.Collections.Generic", "System.Linq", "System.Text",
                    "System.Windows.Forms"
                };

            //complete class as string which will be compiled to an in memory assembly
            string executeCode =
                defaultNamespaces.Aggregate("",
                                            (current, defaultNamespace) =>
                                            current + string.Format("using {0};\n", defaultNamespace)) +
                @"namespace MyNamespace {
                    public class MyClass {
                        public List<dynamic> FilterData(List<dynamic> data,  string userQuery) {
                            try{
                                    var result = ((IEnumerable<dynamic>)(" + userQuery + @")).ToList();
                                    return result ;
                               }catch(Exception ex)
                               {
                                    return new List<dynamic>{ex.Message + ex.StackTrace};
                               }
                        }   
                     }    
                }";

            #endregion template Code

            //add required assembly references
            var defaultAssemblies = new[]
                {
                    "System.dll", "System.Core.dll", "Microsoft.CSharp.dll", "System.Data.dll", "System.Xml.dll",
                    "System.Xml.Linq.dll", "System.Windows.Forms.dll"
                };

            var compilerParams = new CompilerParameters
            {
                GenerateInMemory = true,
                TreatWarningsAsErrors = false,
                GenerateExecutable = false,
                CompilerOptions = "/optimize",
            };
            compilerParams.ReferencedAssemblies.AddRange(defaultAssemblies);

            //compile assembly
            var compiledAssembly = new CSharpCodeProvider().CompileAssemblyFromSource(compilerParams, executeCode);

            if (compiledAssembly.Errors.HasErrors)
            {
                var exceptionMessage = compiledAssembly.Errors.Cast<CompilerError>()
                                           .Aggregate("Compilation error on the query:\n",
                                                      (x, y) => x + ("rn" + y.ToString()));

                MessageBox.Show(exceptionMessage);
            }

            // create instance of the assembly
            dynamic instance =
                Activator.CreateInstance(compiledAssembly.CompiledAssembly.GetType("MyNamespace.MyClass"));

            //execute the method and collect result. since "instance" is of type dynamic, FiltereData() method will be resolved at run time
            dynamic result = instance.FilterData(data, userQuery);

            return result;
        }
    }
}