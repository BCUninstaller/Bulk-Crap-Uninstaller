using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Klocman.Binding.Settings
{
    internal static class ReflectionTools
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
                throw new ArgumentException(
                    "You must pass a lambda of the form: 'x => x.Property' or 'x => class.Property'",
                    nameof(memberLamda));

            var property = memberSelectorExpression.Member as PropertyInfo;
            if (property == null)
                throw new ArgumentException(
                    "You must pass a lambda of the form: 'x => x.Property' or 'x => class.Property'",
                    nameof(memberLamda));

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
            property?.SetValue(classInstance, value, null);
        }
    }
}