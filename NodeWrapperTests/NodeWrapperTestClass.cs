using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeWrapperTests
{
    public interface INodeWrapperTestClass
    {
        string Method1();
    }

    public abstract class AbstractNodeWrapperTestClass : INodeWrapperTestClass
    {
        public virtual string Method1()
        {
            return "Method 1";
        }
    }

    public class NodeWrapperTestClass : AbstractNodeWrapperTestClass
    {
        public String Method2(string s)
        {
            return "Method 2: " + s;
        }

        public string[] StringArray
        {
            get
            {
                return new[] { "test1", "test2", "test3" };
            }
        }

        public List<string> StringList
        {
            get
            {
                return new List<string> { "test1", "test2", "test3" };
            }
        }
    }

    public static class NodeWrapperTestClassExtensions
    {
        public static int ExtensionMethod1(this INodeWrapperTestClass nw, string s)
        {
            return int.Parse(s);
        }

        public static Int32 ExtensionMethod2(this NodeWrapperTestClass nw, string s)
        {
            return int.Parse(s) + 3;
        }

        public static Int64 ExtensionMethod3(this AbstractNodeWrapperTestClass nw, string s)
        {
            return int.Parse(s) + 5;
        }
    }
}
