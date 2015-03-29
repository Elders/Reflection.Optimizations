using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Elders.Reflection
{
    public class FactoryBuilder : IFactoryBuilder
    {
        ModuleBuilder moduleBuilder;
        public FactoryBuilder(ModuleBuilder moduleBuilder)
        {
            this.moduleBuilder = moduleBuilder;
        }
        public IDynamicFactory CreateFactory(Type forType)
        {
            var factoryTypeBuilder = moduleBuilder.DefineType(forType.FullName.Replace(".", "_"), TypeAttributes.Class | TypeAttributes.Public, null, new Type[] { typeof(IDynamicFactory) });
            factoryTypeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
            var factoryMethod = factoryTypeBuilder.DefineMethod("CreateInstance", MethodAttributes.Public | MethodAttributes.Virtual, typeof(object), new Type[1] { typeof(object[]) });
            var emitter = factoryMethod.GetILGenerator();
            EmitFactoryCode(forType, emitter);
            factoryTypeBuilder.DefineMethodOverride(factoryMethod, typeof(IDynamicFactory).GetMethods().Where(x => x.Name == "CreateInstance").Single());
            var type = factoryTypeBuilder.CreateType();
            var instace = Activator.CreateInstance(type);
            return instace as IDynamicFactory;
        }

        private void EmitFactoryCode(Type forType, ILGenerator emitter)
        {
            emitter.BeginScope();
            emitter.DeclareLocal(typeof(int));//0 parameters array size
            emitter.DeclareLocal(typeof(object));//1 return value
            emitter.DeclareLocal(typeof(bool));//2 storage for the "if"s

            emitter.Emit(OpCodes.Ldarg_1);
            emitter.Emit(OpCodes.Ldlen);
            emitter.Emit(OpCodes.Conv_I4);
            emitter.Emit(OpCodes.Stloc_0);


            var exit = emitter.DefineLabel();
            Dictionary<ConstructorInfo, Label> labels = new Dictionary<ConstructorInfo, Label>();
            var constructors = forType.GetConstructors().OrderByDescending(x => x, new OverloadComparer());
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
                    emitter.Emit(OpCodes.Ldarg_1);
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
                    emitter.Emit(OpCodes.Ldarg_1);
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


            emitter.EndScope();
        }





    }
}
