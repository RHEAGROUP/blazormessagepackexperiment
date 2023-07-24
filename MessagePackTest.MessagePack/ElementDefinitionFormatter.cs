//  -------------------------------------------------------------------------------------------------
//  <copyright file="ElementDefinitionFormatter.cs" company="RHEA System S.A.">
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
    using System.Buffers;
    using System.Collections.Generic;
    using global::MessagePack;
    using global::MessagePack.Formatters;

    using MessagePackTest.Model;

    /// <summary>
    /// The purpose of the <see cref="ElementDefinitionFormatter"/> is to provide
    /// the contract for serialization of the <see cref="ElementDefinition"/> type
    /// </summary>
    public class ElementDefinitionFormatter : IMessagePackFormatter<ElementDefinition>
    {
        /// <summary>
        /// Serializes an <see cref="ElementDefinition"/>.
        /// </summary>
        /// <param name="writer">
        /// The writer to use when serializing the value.
        /// </param>
        /// <param name="elementDefinition">
        /// The value to be serialized.
        /// </param>
        /// <param name="options">
        /// The serialization settings to use.
        /// </param>
        public void Serialize(ref MessagePackWriter writer, ElementDefinition elementDefinition, MessagePackSerializerOptions options)
        {
            if (elementDefinition == null)
            {
                throw new ArgumentNullException(nameof(elementDefinition), "The ElementDefinition may not be null");
            }

            writer.WriteArrayHeader(5);
            writer.Write(elementDefinition.Iid.ToByteArray());

            writer.WriteArrayHeader(elementDefinition.Aliases.Count);
            foreach (var alias in elementDefinition.Aliases)
            {
                writer.Write(alias);
            }
            writer.Write(elementDefinition.Description);
            writer.Write(elementDefinition.Name);

            writer.WriteArrayHeader(elementDefinition.Parameters.Count);
            foreach (var parameter in elementDefinition.Parameters)
            {
                writer.Write(parameter.ToByteArray());
            }

            writer.Flush();
        }

        /// <summary>
        /// Deserializes an <see cref="ElementDefinition"/>.
        /// </summary>
        /// <param name="reader">
        /// The reader to deserialize from.
        /// </param>
        /// <param name="options">
        /// The serialization settings to use.
        /// </param>
        /// <returns>
        /// The deserialized value.
        /// </returns>
        public ElementDefinition Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);

            ElementDefinition elementDefinition = null;
            
            int count = reader.ReadArrayHeader();
            for (int counter = 0; counter < count; counter++)
            {
                switch (counter)
                {
                    case 0:
                        var iidBytes = reader.ReadBytes();
                        var iid = new Guid(iidBytes.Value.ToArray());
                        elementDefinition = new ElementDefinition(iid);
                        break;
                    case 1:
                        var aliasLength = reader.ReadArrayHeader();
                        elementDefinition.Aliases = new List<string>();
                        for (int aliasCounter = 0; aliasCounter < aliasLength; aliasCounter++)
                        {
                            elementDefinition.Aliases.Add(reader.ReadString());
                        }
                        break;
                    case 2:
                        elementDefinition.Description = reader.ReadString();
                        break;
                    case 3:
                        elementDefinition.Name = reader.ReadString();
                        break;
                    case 4:
                        var parametersLength = reader.ReadArrayHeader();
                        for (int parametersCounter = 0; parametersCounter < parametersLength; parametersCounter++)
                        {
                            var parameterIidBytes = reader.ReadBytes();

                            var parameterIid = new Guid(parameterIidBytes.Value.ToArray());

                            elementDefinition.Parameters.Add(parameterIid);
                        }
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return elementDefinition;
        }
    }
}
