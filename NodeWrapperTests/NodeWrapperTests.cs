using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodeWrapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace NodeWrapperTests
{
    using NodeFunction = Func<object, Task<object>>;

    [TestClass]
    public class NodeWrapperTests
    {
        [TestMethod]
        public void TypeExtension_GetExtensionMethods_Should_Return_All_Extension_Methods()
        {
            var type = typeof(NodeWrapperTestClass);
            var extensionMethod1 = typeof(NodeWrapperTestClassExtensions).GetMethod("ExtensionMethod1");
            var extensionMethod2 = typeof(NodeWrapperTestClassExtensions).GetMethod("ExtensionMethod2");
            var extensionMethod3 = typeof(NodeWrapperTestClassExtensions).GetMethod("ExtensionMethod3");
            var extensionMethods = type.GetExtensionMethods(Assembly.GetExecutingAssembly());

            Assert.IsTrue(extensionMethods.Contains(extensionMethod1));
            Assert.IsTrue(extensionMethods.Contains(extensionMethod2));
            Assert.IsTrue(extensionMethods.Contains(extensionMethod3));
        }

        [TestMethod]
        public void Invoking_Methods_that_return_String_or_string_Should_Return_String()
        {
            var invokeMap = (Dictionary<string, NodeFunction>) Startup.ConvertObjectToInvokableMap(new NodeWrapperTestClass()).Result;

            var result = invokeMap["method1"](new object[] { }).Result;
            Assert.AreEqual(typeof(String), result.GetType());
            Assert.AreEqual("Method 1", result);

            result = invokeMap["method2_string"](new object[] { "test" }).Result;
            Assert.AreEqual(typeof(String), result.GetType());
            Assert.AreEqual("Method 2: test", result);
        }

        [TestMethod]
        public void Invoking_Methods_that_return_int_or_Class_Int_Should_Return_Int()
        {
            var invokeMap = (Dictionary<string, NodeFunction>) Startup.ConvertObjectToInvokableMap(new NodeWrapperTestClass()).Result;

            var result = invokeMap["extensionMethod1_inodewrappertestclass_string"](new object[] { "3" }).Result;
            Assert.AreEqual(typeof(Int32), result.GetType());
            Assert.AreEqual(3, result);

            result = invokeMap["extensionMethod2_nodewrappertestclass_string"](new object[] { "3" }).Result;
            Assert.AreEqual(typeof(Int32), result.GetType());
            Assert.AreEqual(6, result);

            result = invokeMap["extensionMethod3_abstractnodewrappertestclass_string"](new object[] { "3" }).Result;
            Assert.AreEqual(typeof(Int64), result.GetType());
            Assert.AreEqual((Int64) 8, result);
        }

        [TestMethod]
        public void Invoking_Methods_that_return_a_collection_Should_Return_A_Collection()
        {
            var invokeMap = (Dictionary<string, NodeFunction>) Startup.ConvertObjectToInvokableMap(new NodeWrapperTestClass()).Result;

            var result = invokeMap["get_StringArray"](new object[] { }).Result;
            Assert.AreEqual(typeof(String[]), result.GetType());
            CollectionAssert.AreEqual(new[] { "test1", "test2", "test3" }, (String[]) result);

            result = invokeMap["get_StringList"](new object[] { }).Result;
            Assert.AreEqual(typeof(List<string>), result.GetType());
            CollectionAssert.AreEqual(new List<string> { "test1", "test2", "test3" }, (List<string>) result);
        }
    }
}
