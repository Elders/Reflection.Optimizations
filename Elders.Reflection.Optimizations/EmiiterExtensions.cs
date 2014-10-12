using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Elders.Reflection
{
    public static class EmiiterExtensions
    {
        public static void WriteValue<T>(this ILGenerator emiter)
        {
            var cw = typeof(Console).GetMethods().Where(x => x.Name == "WriteLine" && x.GetParameters().Count() == 1 && x.GetParameters().Single().ParameterType == typeof(T)).Single();

            emiter.Emit(OpCodes.Call, cw);
        }
    }
}
