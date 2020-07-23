using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SimpleGameEngine.Core
{
    public static class InheritedClassMethods
    {
        public static IEnumerable<Type> GetInheritedClasses<T>()
        {
            var result = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                result.AddRange(assembly
                    .GetTypes()
                    .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(T)))
                );   
            }
            return result;
        }

        public static IEnumerable<Type> GetInheritedClassesInAssembly<T>() =>
            Assembly.GetAssembly(typeof(T))
                .GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(T)))
                .ToList();

        public static IEnumerable<Type> GetInheritedClassesByInterface<T>()
        {
            var result = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                result.AddRange(assembly
                    .GetTypes()
                    .Where(type => type.IsClass && !type.IsAbstract && type.GetInterfaces().Contains(typeof(T)))
                );   
            }
            return result;
        }

        public static IEnumerable<Type> GetInheritedClassesByInterfaceInAssembly<T>() =>
            Assembly.GetAssembly(typeof(T))
                .GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && type.GetInterfaces().Contains(typeof(T)))
                .ToList();


        public static IEnumerable<MethodInfo> GetExtensionMethods(Assembly assembly, Type extendedType)
        {
            return (from t in assembly
                .GetTypes() 
                where t.IsDefined(typeof(ExtensionAttribute), false) 
                from mi in t.GetMethods() 
                where mi.IsDefined(typeof(ExtensionAttribute), false) 
                where mi.GetParameters()[0].ParameterType == extendedType 
                select mi).ToList();
        }
    }
}