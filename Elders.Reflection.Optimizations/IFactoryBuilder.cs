using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Elders.Reflection
{
    public interface IFactoryBuilder
    {
        IDynamicFactory CreateFactory(Type forType);
    }
}
