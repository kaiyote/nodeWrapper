using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using LibGit2Sharp;

namespace NodeWrapper
{
    using Envelope = IDictionary<string, object>;
    using NodeFunction = Func<object, Task<object>>;

    public class NodeWrapper
    {
        public Task<object> openRepository(string path)
        {
            var repo = new Repository(path);
            return Startup.ConvertObjectToInvokableMap(repo);
        }
    }

    public class Startup
    {
        private const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.InvokeMethod;

        public Task<object> Invoke(object input)
        {
            var adapter = new NodeWrapper();
            return ConvertObjectToInvokableMap(adapter);
        }

        internal static Task<object> ConvertObjectToInvokableMap(object target)
        {
            var type = target.GetType();
            var methodInfos = type.GetMethods(bindingFlags);

            var methods = methodInfos.ToDictionary(method =>
                {
                    var methodsWithName = methodInfos.Where(methodInfo => methodInfo.Name == method.Name).ToList();
                    return method.Name + (methodsWithName.IndexOf(method) == 0 ? "" : methodsWithName.IndexOf(method).ToString());
                }, method =>
                    (NodeFunction) (args =>
                        Task.FromResult(type.InvokeMember(method.Name, bindingFlags, null, target, (object[]) args))));

            return Task.FromResult<object>(methods);
        }
    }
}
