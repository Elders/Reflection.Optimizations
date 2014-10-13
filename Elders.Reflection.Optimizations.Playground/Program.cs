using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Elders.Cronus;
using Elders.Reflection;
using Elders.Relfection.Playground;

namespace Elders.Relfection.Optimizations
{

    public class ProjectionAFactory : IDynamicFactory
    {

        public object CreateInstance(object[] parmeters)
        {
            if (parmeters.Length == 0)
                return new ProjectionA();
            if (parmeters.Length == 1 && parmeters[0] is int)
            {
                return new ProjectionA(parmeters[0]);
            }
            if (parmeters.Length == 1 && parmeters[0] is string)
            {
                return new ProjectionA(parmeters[0]);
            }
            if (parmeters.Length == 1 && parmeters[0] is object)
            {
                return new ProjectionA(parmeters[0]);
            }
            if (parmeters.Length == 1 && parmeters[0] is object)
            {
                return new ProjectionA(parmeters[0]);
            }
            return null;
        }
    }
    public class ProjectionA
    {
        public ProjectionA()
        {
            // Console.WriteLine("Instance Created");
        }
        public ProjectionA(int oww)
        {
            // Console.WriteLine(oww + 1);
        }
        public ProjectionA(object oww)
        {
            //  Console.WriteLine(oww);
        }

        public ProjectionA(string oww)
        {
            // Console.WriteLine(oww);
        }

        public ProjectionA(object oww, string ffs)
        {
            // Console.WriteLine(oww.ToString() + ffs);
        }

        //public Handler(string oww, string ffs)
        //{
        //    //Console.WriteLine(oww.ToString() + ffs);
        //}
        //public Handler(object oww)
        //{
        //    Console.WriteLine(oww);
        //}
    }
    public class ProjectionB
    {
        public ProjectionB()
        {
            // Console.WriteLine("Instance Created");
        }


        //public Handler(string oww, string ffs)
        //{
        //    //Console.WriteLine(oww.ToString() + ffs);
        //}
        //public Handler(object oww)
        //{
        //    Console.WriteLine(oww);
        //}
    }

    class Program
    {
        public static Type type1 = typeof(string);
        public static Type type2 = typeof(int);
        static object test(object[] parameters)
        {
            //var isOfType = (parameters[0] as string);
            //if (isOfType != null)
            //    return new Handler();

            return null;
        }

        static void Main(string[] args)
        {
            var type = typeof(ProjectionA);
            Dynamics dynamics = new Dynamics();
            
            dynamics.Warm(type);
            FastActivator.WarmInstanceConstructor(type);

            object[] parameters = new object[] { new object(), "gg" };
            int numberOfObjects = 300000;
            var obj = new object();
            var str = "gg";
            var concreteFactory = dynamics.GetFactory(typeof(ProjectionA));
            var concreteFactoryB = dynamics.GetFactory(typeof(ProjectionB));
            var info = MeasureExecutionTime.Start(() => dynamics.CreateInstance(type, parameters), numberOfObjects);
            Console.WriteLine("Elders Activator:" + info);

            var infob = MeasureExecutionTime.Start(() => concreteFactoryB.CreateInstance(parameters), numberOfObjects);
            Console.WriteLine("Elders Activator:" + infob);

            var ms = MeasureExecutionTime.Start(() => Activator.CreateInstance(type, parameters), numberOfObjects);
            Console.WriteLine("Microsoft Activator:" + ms);

            var inline = MeasureExecutionTime.Start(() => new ProjectionA(obj, str), numberOfObjects);
            Console.WriteLine("Inline creation:" + inline);

            var fastActivator = MeasureExecutionTime.Start(() => FastActivator.CreateInstance(type, parameters), numberOfObjects);
            Console.WriteLine("FastActivator creation:" + fastActivator);

            Console.ReadLine();
        }




    }

}
