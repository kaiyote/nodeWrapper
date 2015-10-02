using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodeWrapper;

namespace NodeWrapperTests
{
    [TestClass]
    public class NodeWrapperTests
    {
        [TestMethod]
        public void TypeExtension_GetExtensionMethods_Should_Return_Extension_Methods()
        {
            var type = typeof (string);
            var mi = typeof (StringExtensions).GetMethod("Echo");
            Assert.IsTrue(type.GetExtensionMethods(Assembly.GetExecutingAssembly()).Contains(mi));
        }
    }

    public static class StringExtensions
    {
        public static string Echo(this string s)
        {
            return s;
        }
    }
}
