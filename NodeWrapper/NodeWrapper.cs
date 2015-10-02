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
        public Repository openRepository(string path)
        {
            return new Repository(path);
        }
    }

    public class Startup
    {
        private const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly;

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
                    return method.Name[0].ToString().ToLower() + string.Join("", method.Name.Skip(1).Take(method.Name.Length - 1)) + (methodsWithName.IndexOf(method) == 0 ? "" : methodsWithName.IndexOf(method).ToString());
                }, method =>
                    (NodeFunction) (args =>
                    {
                        var result = type.InvokeMember(method.Name, bindingFlags, null, target, (object[]) args);
                        return result.GetType().IsValueType ? Task.FromResult(result) : ConvertObjectToInvokableMap(result);
                    }));

            return Task.FromResult<object>(methods);
        }
    }
}
