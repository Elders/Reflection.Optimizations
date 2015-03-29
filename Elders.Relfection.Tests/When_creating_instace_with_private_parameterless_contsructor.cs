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
    public class When_creating_instace_with_private_parameterless_contsructor
    {

        Establish context = () => factory = new Dynamics().GetFactory(typeof(TestClass));

        Because of = () => { createdInstance = factory.CreateInstance(new object[] { }) as TestClass; };

        It should_not_be_null = () => createdInstance.ShouldNotBeNull();
        It should_be_of_exact_type = () => createdInstance.ShouldBeOfExactType<TestClass>();
        It should_have_initialized = () => createdInstance.Value.ShouldEqual(InitializedValue);

        static IDynamicFactory factory;
        static TestClass createdInstance;
        static string InitializedValue = "test";
        public class TestClass
        {
            public string Value { get { return value; } }
            private string value;
            private TestClass()
            {

                value = InitializedValue;
            }
        }
    }

}
