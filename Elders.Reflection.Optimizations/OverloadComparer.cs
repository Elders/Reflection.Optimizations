using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Elders.Reflection
{
    public class OverloadComparer : IComparer<MethodBase>
    {

        ParameterTypeComparer typeComp = new ParameterTypeComparer();
        public int Compare(MethodBase x, MethodBase y)
        {
            if (x.DeclaringType != y.DeclaringType)
            {
                if (y.DeclaringType.IsAssignableFrom(x.DeclaringType))
                    return 1;
                if (x.DeclaringType.IsAssignableFrom(y.DeclaringType))
                    return -1;
            }
            if (x == y)
                return 0;
            var xParameters = x.GetParameters();
            var yParameters = y.GetParameters();
            var result = Comparer<int>.Default.Compare(xParameters.Length, yParameters.Length);
            if (result != 0)
                return result * -1;

            //if (xParameters.Length < yParameters.Length)
            //    return 1;
            //if (xParameters.Length > yParameters.Length)
            //    return -1;

            for (int i = 0; i < xParameters.Length; i++)
            {
                var compare = typeComp.Compare(xParameters[i].ParameterType, yParameters[i].ParameterType);
                if (compare != 0)
                    return compare;
            }

            return 0;

        }

        public class ParameterTypeComparer : IComparer<Type>
        {

            public int Compare(Type x, Type y)
            {
                if (x.IsAssignableFrom(y))
                    return -1;
                if (y.IsAssignableFrom(x))
                    return 1;
                return 1;
            }
        }
    }
}
