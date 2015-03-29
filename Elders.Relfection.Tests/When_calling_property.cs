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
    public class When_calling_property
    {

        Establish context = () =>
        {
            factory = new Dynamics().GetFactory(typeof(TestClass));
            createdInstance = factory.CreateInstance(new object[] { }) as TestClass;
        };

        Because of = () => { ReturnValue = (string)factory.Invoke(createdInstance, "get_Property", new object[] { }); };

        It should_have_called_the_method = () => ReturnValue.ShouldEqual(InitializedValue);

        static IDynamicFactory factory;
        static TestClass createdInstance;
        static string InitializedValue = "test";
        static string ReturnValue;

        public class TestClass
        {
            public string Property { get { return InitializedValue; } }

        }
        
    }

}
