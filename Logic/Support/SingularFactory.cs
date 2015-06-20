using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Communications;
using Swarmops.Basic.Types.Financial;
using Swarmops.Basic.Types.Governance;
using Swarmops.Basic.Types.Swarm;
using Swarmops.Common.Interfaces;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Governance;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Support
{
    /// <summary>
    ///     This class supports the PluralBase foundation.
    /// </summary>
    public class SingularFactory
    {
        public static void RegisterAssembly (Type type)
        {
            string assemblyName = type.Assembly.FullName;

            if (!_assemblyLookup.ContainsKey (assemblyName))
            {
                _assemblyLookup[assemblyName] = type.Assembly;
            }
        }

        internal static object FromBasic (IHasIdentity basic)
        {
            MethodInfo basicConverterMethod = null;
            Type basicType = basic.GetType();

            if (_converterLookup.ContainsKey (basicType))
            {
                basicConverterMethod = _converterLookup[basicType];
            }
            else
            {

                Assembly logicAssembly = typeof (SingularFactory).Assembly;
                Assembly basicAssembly = typeof (BasicPerson).Assembly;

                System.Type[] logicTypes =
                    logicAssembly.GetTypes().Where (type => type.IsSubclassOf (basicType)).ToArray();

                if (logicTypes.Length > 1)
                {
                    throw new InvalidOperationException ("More than one type in Swarmops.Logic derives from " +
                                                         basicType.ToString());
                }
                if (logicTypes.Length == 0)
                {
                    // There are no matching types in Swarmops.Logic; look through registered assemblies

                    foreach (Assembly foreignAssembly in _assemblyLookup.Values)
                    {
                        logicTypes = foreignAssembly.GetTypes().Where(type => type.IsSubclassOf(basicType)).ToArray();
                        if (logicTypes.Length == 1)
                        {
                            break;
                        }
                    }

                    if (logicTypes.Length == 0)
                    {
                        throw new InvalidOperationException (
                            "Unable to find higher-order class for base type " + basicType.ToString() + "; if it's in a plugin, was the higher-order assembly registered with SingularFactory?");
                    }
                }

                Type logicType = logicTypes[0];

                basicConverterMethod = logicType.GetMethod ("FromBasic", BindingFlags.Static | BindingFlags.Public);

                if (basicConverterMethod == null)
                {
                    throw new InvalidOperationException(
                        "Unable to find a public static method named \"" + logicType.ToString() + ".FromBasic (" +basicType.ToString()+ ")\" in a loaded assembly");
                }

                _converterLookup[basicType] = basicConverterMethod;
            }

            object result = basicConverterMethod.Invoke(null, new object[] {basic});

            return result;

        }

        static private Dictionary<Type, MethodInfo> _converterLookup = new Dictionary<Type, MethodInfo>();

        static private Dictionary<string, Assembly> _assemblyLookup = new Dictionary<string, Assembly>(); 
    }
}