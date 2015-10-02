using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using LibGit2Sharp;

namespace NodeWrapper
{
    using Envelope = IDictionary<string, object>;
    using NodeFunction = Func<object, Task<object>>;

    public class Startup
    {
        private const BindingFlags BindingFlags =
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Static |
            System.Reflection.BindingFlags.FlattenHierarchy;

        public Task<object> Invoke(Envelope input)
        {
            var repository = new Repository((string) input["path"]);
            return ConvertObjectToInvokableMap(repository);
        }

        private static Task<object> ConvertObjectToInvokableMap(object target)
        {
            var type = target.GetType();
            var methodInfos = type.GetMethods(BindingFlags);
            var extensionMethodInfos = type.GetExtensionMethods(type.Assembly);

            var methods = methodInfos.ToDictionary(method =>
            {
                var baseName = char.ToLower(method.Name[0]) + method.Name.Substring(1);
                return method.GetParameters()
                    .Aggregate(baseName,
                        (name, next) => string.Format("{0}_{1}", name, next.ParameterType.Name.ToLower()));
            }, method =>
                (NodeFunction) (args =>
                {
                    var result = method.Invoke(target, BindingFlags, null, (object[]) args, null);
                    return result.GetType().IsValueType || result.GetType().IsActuallyValueType()
                        ? Task.FromResult(result)
                        : ConvertObjectToInvokableMap(result);
                }));

            foreach (var method in extensionMethodInfos)
            {
                var myMethod = method;
                var baseName = char.ToLower(myMethod.Name[0]) + myMethod.Name.Substring(1);
                var fullName = myMethod.GetParameters()
                    .Aggregate(baseName,
                        (name, next) => String.Format("{0}_{1}", name, next.ParameterType.Name.ToLower()));
                methods.Add(fullName, (args =>
                {
                    var fullArgs = new[] {target}.Concat((object[]) args).ToArray();
                    var result = myMethod.Invoke(null, BindingFlags, null, fullArgs, null);
                    return result.GetType().IsValueType || result.GetType().IsActuallyValueType()
                        ? Task.FromResult(result)
                        : ConvertObjectToInvokableMap(result);
                }));
            }

            return Task.FromResult<object>(methods);
        }
    }

    public static class TypeExtensions
    {
        public static bool IsActuallyValueType(this Type t)
        {
            var stupidClassIshValueTypes = new List<string>
            {
                "System.String",
                "System.Int16",
                "System.Int32",
                "System.Int64",
                "System.Boolean",
                "System.Double",
                "System.Char"
            };
            return stupidClassIshValueTypes.Contains(t.FullName);
        }

        public static MethodInfo[] GetExtensionMethods(this Type t, Assembly extensionAssembly)
        {
            var methods = extensionAssembly.GetTypes().SelectMany(type => type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));

            return methods.Where(m => 
                m.GetParameters().Length > 0 && (m.GetParameters()[0].ParameterType == t || t.GetInterfaces().Contains(m.GetParameters()[0].ParameterType))
            ).ToArray();
        }
    }
}
