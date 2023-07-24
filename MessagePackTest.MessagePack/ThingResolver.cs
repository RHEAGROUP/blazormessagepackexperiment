//  -------------------------------------------------------------------------------------------------
//  <copyright file="ThingResolver.cs" company="RHEA System S.A.">
// 
//    Copyright 2023 RHEA System S.A.
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 
//  </copyright>
//  -------------------------------------------------------------------------------------------------

namespace MessagePackTest.MessagePack
{
    using System;
    using System.Collections.Generic;

    using global::MessagePack;
    using global::MessagePack.Formatters;

    using MessagePackTest.Model;

    /// <summary>
    /// The purpose of the <see cref="ThingFormatterResolver"/> is to provide the model
    /// <see cref="IMessagePackFormatter"/> for all concrete classes
    /// </summary>
    public class ThingFormatterResolver : IFormatterResolver
    {
        // Resolver should be singleton.
        public static readonly IFormatterResolver Instance = new ThingFormatterResolver();

        /// <summary>
        /// Initializes a new instance of the <see cref="ThingFormatterResolver"/> class
        /// </summary>
        private ThingFormatterResolver()
        {
        }

        /// <summary>
        /// Gets an <see cref="T:MessagePack.Formatters.IMessagePackFormatter`1" /> instance that can serialize or deserialize some type <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type of value to be serialized or deserialized.</typeparam>
        /// <returns>A formatter, if this resolver supplies one for type <typeparamref name="T" />; otherwise <see langword="null" />.</returns>
        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }
        
        private static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> Formatter;

            // generic's static constructor should be minimized for reduce type generation size!
            // use outer helper method.
            static FormatterCache()
            {
                var type = typeof(T);

                Formatter = (IMessagePackFormatter<T>)ThingResolverGetFormatterHelper.GetFormatter(type);
            }
        }
    }

	/// <summary>
	/// Helper class that resolves a <see cref="IMessagePackFormatter"/> based on a <see cref="Type"/>
	/// </summary>
	internal static class ThingResolverGetFormatterHelper
    {
        // If type is concrete type, use type-formatter map
        static readonly Dictionary<Type, object> FormatterMap = new Dictionary<Type, object>()
        {
            {typeof(ElementDefinition), new ElementDefinitionFormatter()},
            {typeof(Parameter), new ParameterFormatter()}
        };

        /// <summary>
        /// Gets a <see cref="IMessagePackFormatter"/> for the specific <see cref="Type"/>
        /// </summary>
        /// <param name="t">
        /// The subject <see cref="Type"/>
        /// </param>
        /// <returns>
        /// an instance of <see cref="IMessagePackFormatter"/>
        /// </returns>
        internal static object GetFormatter(Type t)
        {
            object formatter;
            if (FormatterMap.TryGetValue(t, out formatter))
            {
                return formatter;
            }

            // If type can not get, must return null for fallback mechanism.
            return null;
        }
    }
}