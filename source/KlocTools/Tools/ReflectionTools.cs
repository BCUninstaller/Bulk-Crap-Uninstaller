/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Klocman.Extensions;

namespace Klocman.Tools
{
    public static class ReflectionTools
    {
        /// <summary>
        ///     Get the name of a static or instance property from a property access lambda.
        /// </summary>
        /// <typeparam name="TProperty">Type of the property</typeparam>
        /// <typeparam name="TClass">Type of a class that contains the property</typeparam>
        /// <param name="memberLamda">You must pass a lambda formed like this 'x => x.Property' or this 'x => class.Property'</param>
        /// <returns>The name of the property</returns>
        public static string GetPropertyName<TProperty, TClass>(Expression<Func<TClass, TProperty>> memberLamda)
        {
            return GetPropertyInfo(memberLamda).Name;
        }

        /// <summary>
        ///     Get the PropertyInfo of a static or instance property from a property access lambda.
        /// </summary>
        /// <typeparam name="TProperty">Type of the property</typeparam>
        /// <typeparam name="TClass">Type of a class that contains the property</typeparam>
        /// <param name="memberLamda">You must pass a lambda formed like this 'x => x.Property' or this 'x => class.Property'</param>
        /// <returns>The name of the property</returns>
        public static PropertyInfo GetPropertyInfo<TProperty, TClass>(Expression<Func<TClass, TProperty>> memberLamda)
        {
            if (memberLamda == null)
                throw new ArgumentNullException(nameof(memberLamda));

            if (!(memberLamda.Body is MemberExpression memberSelectorExpression))
                throw new ArgumentException(@"You must pass a lambda of the form: 'x => x.Property' or 'x => class.Property'", nameof(memberLamda));

            var property = memberSelectorExpression.Member as PropertyInfo;
            if (property == null)
                throw new ArgumentException(@"You must pass a lambda of the form: 'x => x.Property' or 'x => class.Property'", nameof(memberLamda));

            return property;
        }

        /// <summary>
        ///     Try to set specified property to the supplied object. Ignores setter access protection.
        ///     Will throw an exception if the setter doesn't exist or any of the parameters is invalid.
        /// </summary>
        /// <typeparam name="TClass">Type of a class that contains the property</typeparam>
        /// <param name="classInstance">Instance of the class that contains the property</param>
        /// <param name="memberLamda">You must pass a lambda formed like this 'x => x.Property' or this 'x => class.Property'</param>
        /// <param name="value">Value to set the property to.</param>
        public static void SetPropertyValue<TClass>(TClass classInstance, Expression<Func<TClass, object>> memberLamda,
            object value)
        {
            var memberSelectorExpression = memberLamda.Body as MemberExpression;
            var property = memberSelectorExpression?.Member as PropertyInfo;
            if (property != null)
            {
                property.SetValue(classInstance, value, null);
            }
        }

        /// <summary>
        /// Create compiled get and set methods with drastically improved performance.
        /// </summary>
        /// <typeparam name="TInstance">Type of the class containing this property</typeparam>
        /// <param name="propertyInfo">Property info to compile</param>
        public static CompiledPropertyInfo<TInstance> CompileAccessors<TInstance>(this PropertyInfo propertyInfo)
        {
            return new CompiledPropertyInfo<TInstance>(propertyInfo);
        }

        /// <summary>
        /// Search for types that implement TBase in all assemblies in current domain.
        /// </summary>
        /// <typeparam name="TBase">Base that returned types have to implement</typeparam>
        /// <param name="ignoreAbstract">Filter out abstract types</param>
        /// <param name="ignoreInterfaces">Filter out interfaces</param>
        public static IEnumerable<Type> GetTypesImplementingBase<TBase>(bool ignoreAbstract = true, bool ignoreInterfaces = true) where TBase : class
        {
            return GetTypesImplementingBase<TBase>(AppDomain.CurrentDomain.GetAssemblies(), ignoreAbstract, ignoreInterfaces);
        }

        /// <summary>
        /// Search for types that implement TBase in specified assemblies.
        /// </summary>
        /// <typeparam name="TBase">Base that returned types have to implement</typeparam>
        /// <param name="assembliesToSearch">Assemblies to search for types</param>
        /// <param name="ignoreAbstract">Filter out abstract types</param>
        /// <param name="ignoreInterfaces">Filter out interfaces</param>
        public static IEnumerable<Type> GetTypesImplementingBase<TBase>(Assembly[] assembliesToSearch,
            bool ignoreAbstract = true, bool ignoreInterfaces = true) where TBase : class 
        {
            var baseType = typeof(TBase);
            if (baseType.IsSealed)
                throw new TypeLoadException("TBase can't be a sealed type");

            var result = assembliesToSearch.Attempt(x => x.GetTypes()).SelectMany(x => x);
            if (ignoreAbstract) result = result.Where(x => !x.IsAbstract);
            if (ignoreInterfaces) result = result.Where(x => !x.IsInterface);

            return result.Where(x => baseType.IsAssignableFrom(x));
        }
    }
}