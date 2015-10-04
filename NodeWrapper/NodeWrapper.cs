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
        private const BindingFlags bindingFlags =
            BindingFlags.Instance | BindingFlags.Public |
            BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy;

        public Task<object> Invoke(Envelope input)
        {
            var repository = new Repository((string) input["path"]);
            return ConvertObjectToInvokableMap(repository);
        }

        public static Task<object> ConvertObjectToInvokableMap(object target)
        {
            if (target is IEnumerable<object>)
            {
                return Task.FromResult<object>(target);
            }

            var type = target.GetType();
            var methodInfos = type.GetMethods(bindingFlags);
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
                    var result = method.Invoke(target, bindingFlags, null, (object[]) args, null);
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
                methods.Add(fullName, args =>
                {
                    var fullArgs = new[] { target }.Concat((object[]) args).ToArray();
                    var result = myMethod.Invoke(null, bindingFlags, null, fullArgs, null);
                    return result.GetType().IsValueType || result.GetType().IsActuallyValueType()
                        ? Task.FromResult(result)
                        : ConvertObjectToInvokableMap(result);
                });
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
            {
                var parms = m.GetParameters();
                return parms.Length > 0 && (parms[0].ParameterType == t || parms[0].ParameterType == t.BaseType || t.GetInterfaces().Contains(parms[0].ParameterType));
            }).ToArray();
        }
    }
}
