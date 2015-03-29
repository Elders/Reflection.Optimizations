using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Machine.Specifications;
namespace Elders.Relfection.Tests
{
    class Runner
    {
        static void Main(params string[] args)
        {
            var testTypes = typeof(Runner).Assembly.GetTypes().Where(x => x.GetCustomAttributes(false).Where(y => y is SubjectAttribute).Any());
            foreach (var item in testTypes)
            {
                RunTest(item);
            }
            Console.ReadLine();
        }
        static void RunTest(Type t)
        {
            var subject = t.GetCustomAttribute<SubjectAttribute>();
            var name = subject.ToString();
            var testInstace = Activator.CreateInstance(t);
            Console.WriteLine(subject.CreateSubject().FullConcern);
            var ctx = GetField<Establish>(t, testInstace).Single();
            var of = GetField<Because>(t, testInstace).Single();
            var its = GetField<It>(t, testInstace);
            ctx.Value();
            of.Value();
            Console.WriteLine(t.Name);
            foreach (var item in its)
            {
                var assertName = item.Key;
                try
                {
                    item.Value();
                    Console.WriteLine(assertName + "\t OK");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(assertName + "\t Fail");
                }
            }
        }

        static IEnumerable<KeyValuePair<string, T>> GetField<T>(Type t, object intstance)
        {
            var fields = t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Where(x => x.FieldType == typeof(T));
            foreach (var item in fields)
            {
                yield return new KeyValuePair<string, T>(item.Name, (T)item.GetValue(intstance));
            }
        }
    }
}
