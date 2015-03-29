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
    public class When_calling_base_class_method
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
        public class BaseTestClass
        {
            public string Value { get { return value; } }
            private string value;


            public void Test(int test)
            {
                value = "int overload";
            }
            public void Test(string test)
            {
                value = test;
            }
            public void Test(object test)
            {
                value = "object overload";
            }
        }
        public class TestClass : BaseTestClass
        { }
    }

}
