using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elders.Reflection;
using Machine.Specifications;

namespace Elders.Relfection.Tests
{
    [Subject("TypeFacotry")]
    public class When_calling_abstract_method
    {

        Establish context = () =>
        {
            factory = new Dynamics().GetFactory(typeof(TestClass));
            createdInstance = factory.CreateInstance(new object[] { }) as TestClass;
        };

        Because of = () => { factory.Invoke(createdInstance, "Test", new object[] { InitializedValue }); };

        It should_have_called_the_method = () => createdInstance.Value.ShouldEqual(InitializedValue);

        static IDynamicFactory factory;
        static TestClass createdInstance;
        static string InitializedValue = "string overload";
        public abstract class BaseTestClass
        {
            public string Value { get { return value; } }
            protected string value;


            public void Test(int test)
            {
                value = "int overload";
            }

            public abstract void Test(string test);

            public void Test(object test)
            {
                value = "object overload";
            }
        }
        public class TestClass : BaseTestClass
        {
            public override void Test(string test)
            {
                value = InitializedValue;
            }


        }
    }

}
