using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Elders.Reflection
{
    public interface IDynamicFactory
    {
        object CreateInstance(object[] parmeters);
    }
    public class Dynamics
    {
        string AssemblyName = "Elders.Reflection.Runtime";
        AssemblyBuilder assemblyBuilder;
        ModuleBuilder moduleBuilder;
        FactoryBuilder factoryBuilder;
        public Dynamics(bool produceAssembly = false)
        {
            AssemblyName asmName = new AssemblyName(AssemblyName);

            if (produceAssembly)
            {
                assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave);
                moduleBuilder = assemblyBuilder.DefineDynamicModule(AssemblyName + "Module", AssemblyName + ".dll", true);
            }
            else
            {
                assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
                moduleBuilder = assemblyBuilder.DefineDynamicModule(AssemblyName + "Module");
            }
            factoryBuilder = new FactoryBuilder(moduleBuilder);


        }
        public void Warm(params Type[] types)
        {
            foreach (var item in types)
            {
                factories.GetOrAdd(item, x => factoryBuilder.CreateFactory(x));
            }
        }
        ConcurrentDictionary<Type, IDynamicFactory> factories = new ConcurrentDictionary<Type, IDynamicFactory>();

        public void SaveAssembly()
        {
            assemblyBuilder.Save(AssemblyName + ".dll");
        }

        public object CreateInstance(Type type, object[] parmeters)
        {
            if (parmeters == null)
                parmeters = new object[] { };
            var concreteFactory = factories.GetOrAdd(type, x => factoryBuilder.CreateFactory(x));
            return concreteFactory.CreateInstance(parmeters);
        }

        public IDynamicFactory GetFactory(Type type)
        {
            return factories.GetOrAdd(type, x => factoryBuilder.CreateFactory(x));
        }


    }
}
