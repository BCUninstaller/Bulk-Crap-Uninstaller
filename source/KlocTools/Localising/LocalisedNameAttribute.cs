/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Reflection;

namespace Klocman.Localising
{
    /// <summary>
    ///     Used alongside GetFancyName method to give custom names to properties, fields and enums
    /// </summary>
    public class LocalisedNameAttribute : Attribute
    {
        public LocalisedNameAttribute(string name)
        {
            NameOverride = name;
        }

        public LocalisedNameAttribute(Type resourceType, string resourceName)
        {
            ResourceName = resourceName;
            ResourceType = resourceType;
        }

        private string NameOverride { get; }
        private string ResourceName { get; }
        private Type ResourceType { get; }

        public string GetName()
        {
            if (NameOverride != null)
                return NameOverride;

            if ((ResourceType != null) && (ResourceName != null))
            {
                var property = ResourceType.GetProperty(ResourceName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (property == null)
                {
                    throw new InvalidOperationException("Resource of this type doesn\'t contain specified property");
                }
                if (property.PropertyType != typeof (string))
                {
                    throw new InvalidOperationException("Specified !(property is of string) type");
                }
                return (string) property.GetValue(null, null);
            }
            return string.Empty;
        }
    }
}