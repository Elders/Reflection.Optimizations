using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Elders.Reflection
{
    public class DynamicsFactoryBuilder : IFactoryBuilder
    {
        private class DynamicMethodFactory : IDynamicFactory
        {
            Func<object[], object> factoryMethod;
            private readonly Func<object, string, object[], object> invokeMethod;
            public DynamicMethodFactory(Func<object[], object> factoryMethod, Func<object, string, object[], object> invokeMethod)
            {
                this.invokeMethod = invokeMethod;
                this.factoryMethod = factoryMethod;
            }

            public object CreateInstance(object[] parmeters)
            {
                return factoryMethod(parmeters);
            }


            public object Invoke(object instance, string name, object[] parmeters)
            {
                return invokeMethod(instance, name, parmeters);
            }
        }
        public IDynamicFactory CreateFactory(Type forType)
        {

            DynamicMethod factoryMethod = new DynamicMethod(
            "CI" + forType.FullName.Replace(".", "_"),
            typeof(object), new Type[1] { typeof(object[]) },
              forType.Module, true);
            EmitFactoryCode(forType, factoryMethod.GetILGenerator());

            DynamicMethod anyMethod = new DynamicMethod(
           "MI" + forType.FullName.Replace(".", "_"),
           typeof(object), new Type[3] { typeof(object), typeof(string), typeof(object[]) },
             forType.Module, true);
            EmitMethodCode(forType, anyMethod.GetILGenerator());
            return new DynamicMethodFactory(factoryMethod.CreateDelegate(typeof(Func<object[], object>)) as Func<object[], object>,
                anyMethod.CreateDelegate(typeof(Func<object, string, object[], object>)) as Func<object, string, object[], object>
                );
        }

        private void EmitFactoryCode(Type forType, ILGenerator emitter)
        {
            emitter.DeclareLocal(typeof(int));//0 parameters array size
            emitter.DeclareLocal(typeof(object));//1 return value
            emitter.DeclareLocal(typeof(bool));//2 storage for the "if"s

            emitter.Emit(OpCodes.Ldarg_0);
            emitter.Emit(OpCodes.Ldlen);
            emitter.Emit(OpCodes.Conv_I4);
            emitter.Emit(OpCodes.Stloc_0);


            var exit = emitter.DefineLabel();
            Dictionary<ConstructorInfo, Label> labels = new Dictionary<ConstructorInfo, Label>();
            var constructors = forType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).OrderByDescending(x => x, new OverloadComparer());
            foreach (var item in constructors)
            {
                var continueLabel = emitter.DefineLabel();
                var parametersCount = item.GetParameters().Count();
                emitter.Emit(OpCodes.Ldloc_0);
                emitter.Emit(OpCodes.Ldc_I4, parametersCount);
                emitter.Emit(OpCodes.Ceq);
                emitter.Emit(OpCodes.Stloc_2);
                emitter.Emit(OpCodes.Ldloc_2);
                emitter.Emit(OpCodes.Brfalse_S, continueLabel);
                for (int i = 0; i < parametersCount; i++)
                {
                    emitter.Emit(OpCodes.Ldarg_0);
                    emitter.Emit(OpCodes.Ldc_I4, i);
                    emitter.Emit(OpCodes.Ldelem_Ref);
                    emitter.Emit(OpCodes.Isinst, item.GetParameters()[i].ParameterType);
                    emitter.Emit(OpCodes.Ldnull);
                    emitter.Emit(OpCodes.Ceq);
                    emitter.Emit(OpCodes.Ldc_I4, 0);
                    emitter.Emit(OpCodes.Ceq);
                    emitter.Emit(OpCodes.Ldloc_2);
                    emitter.Emit(OpCodes.Ceq);
                    emitter.Emit(OpCodes.Stloc_2);

                }
                emitter.Emit(OpCodes.Ldloc_2);
                emitter.Emit(OpCodes.Brfalse_S, continueLabel);

                //emitter.EmitWriteLine("Calling gay ctor with " + parametersCount + " parameters");
                for (int i = 0; i < item.GetParameters().Count(); i++)
                {
                    emitter.Emit(OpCodes.Ldarg_0);
                    emitter.Emit(OpCodes.Ldc_I4, i);
                    emitter.Emit(OpCodes.Ldelem_Ref);
                }
                emitter.Emit(OpCodes.Newobj, item);
                emitter.Emit(OpCodes.Stloc_1);
                emitter.Emit(OpCodes.Br, exit);

                emitter.MarkLabel(continueLabel);
            }


            emitter.Emit(OpCodes.Ldnull);
            emitter.Emit(OpCodes.Stloc_1);
            emitter.Emit(OpCodes.Br, exit);
            emitter.MarkLabel(exit);
            emitter.Emit(OpCodes.Ldloc_1);
            emitter.Emit(OpCodes.Ret);

        }

        private void EmitMethodCode(Type forType, ILGenerator emitter)
        {
            emitter.DeclareLocal(typeof(int));//0 parameters array size
            emitter.DeclareLocal(typeof(object));//1 return value
            emitter.DeclareLocal(typeof(bool));//2 storage for the "if"s
            emitter.DeclareLocal(typeof(string));//3 storage for the method name

            emitter.Emit(OpCodes.Ldarg_2);
            emitter.Emit(OpCodes.Ldlen);
            emitter.Emit(OpCodes.Conv_I4);
            emitter.Emit(OpCodes.Stloc_0);


            var exit = emitter.DefineLabel();
            Dictionary<ConstructorInfo, Label> labels = new Dictionary<ConstructorInfo, Label>();
            var methods = forType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).OrderByDescending(x => x, new OverloadComparer());
            foreach (var item in methods)
            {
                var continueLabel = emitter.DefineLabel();
                emitter.Emit(OpCodes.Ldstr, item.Name);
                emitter.Emit(OpCodes.Ldarg_1);
                emitter.Emit(OpCodes.Ceq);
                emitter.Emit(OpCodes.Stloc_2);
                emitter.Emit(OpCodes.Ldloc_2);
                emitter.Emit(OpCodes.Brfalse_S, continueLabel);


                var parametersCount = item.GetParameters().Count();
                emitter.Emit(OpCodes.Ldloc_0);
                emitter.Emit(OpCodes.Ldc_I4, parametersCount);
                emitter.Emit(OpCodes.Ceq);
                emitter.Emit(OpCodes.Stloc_2);
                emitter.Emit(OpCodes.Ldloc_2);
                emitter.Emit(OpCodes.Brfalse_S, continueLabel);
                for (int i = 0; i < parametersCount; i++)
                {
                    emitter.Emit(OpCodes.Ldarg_2);
                    emitter.Emit(OpCodes.Ldc_I4, i);
                    emitter.Emit(OpCodes.Ldelem_Ref);
                    emitter.Emit(OpCodes.Isinst, item.GetParameters()[i].ParameterType);
                    emitter.Emit(OpCodes.Ldnull);
                    emitter.Emit(OpCodes.Ceq);
                    emitter.Emit(OpCodes.Ldc_I4, 0);
                    emitter.Emit(OpCodes.Ceq);
                    emitter.Emit(OpCodes.Ldloc_2);
                    emitter.Emit(OpCodes.Ceq);
                    emitter.Emit(OpCodes.Stloc_2);

                }
                emitter.Emit(OpCodes.Ldloc_2);
                emitter.Emit(OpCodes.Brfalse_S, continueLabel);
                emitter.Emit(OpCodes.Ldarg_0);
                for (int i = 0; i < item.GetParameters().Count(); i++)
                {
                    emitter.Emit(OpCodes.Ldarg_2);
                    emitter.Emit(OpCodes.Ldc_I4, i);
                    emitter.Emit(OpCodes.Ldelem_Ref);
                }

                emitter.Emit(OpCodes.Call, item);
                if (item.ReturnType == null || item.ReturnType == typeof(void))
                {
                    emitter.Emit(OpCodes.Ldnull);

                }
                emitter.Emit(OpCodes.Stloc_1);
                emitter.Emit(OpCodes.Br, exit);

                emitter.MarkLabel(continueLabel);
            }


            emitter.Emit(OpCodes.Ldnull);
            emitter.Emit(OpCodes.Stloc_1);
            emitter.Emit(OpCodes.Br, exit);
            emitter.MarkLabel(exit);
            emitter.Emit(OpCodes.Ldloc_1);
            emitter.Emit(OpCodes.Ret);

        }



    }
}
