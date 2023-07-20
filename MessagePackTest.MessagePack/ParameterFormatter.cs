//  -------------------------------------------------------------------------------------------------
//  <copyright file="ParameterFormatter.cs" company="RHEA System S.A.">
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

    using global::MessagePack;
    using global::MessagePack.Formatters;

    using MessagePackTest.Model;

    /// <summary>
    /// The purpose of the <see cref="ParameterFormatter"/> is to provide
    /// the contract for serialization of the <see cref="ParameterFormatter"/> type
    /// </summary>
    public class ParameterFormatter : IMessagePackFormatter<Parameter>
    {
        /// <summary>
        /// Serializes an <see cref="Parameter"/>.
        /// </summary>
        /// <param name="writer">
        /// The writer to use when serializing the value.
        /// </param>
        /// <param name="parameter">
        /// The value to be serialized.
        /// </param>
        /// <param name="options">
        /// The serialization settings to use.
        /// </param>
        public void Serialize(ref MessagePackWriter writer, Parameter parameter, MessagePackSerializerOptions options)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter), "The Parameter may not be null");
            }
            
            writer.WriteArrayHeader(3);
            
            writer.Write(parameter.Iid.ToByteArray());
            writer.Write(parameter.Name);
            writer.Write(parameter.Value);

            writer.Flush();
        }

        /// <summary>
        /// Deserializes an <see cref="Parameter"/>.
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
        public Parameter Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            
            Parameter parameter = null;
            
            int count = reader.ReadArrayHeader();
            for (int i = 0; i < count; i++)
            {
                switch (i)
                {
                    case 0:
                        var iidBytes = reader.ReadBytes();
                        var iid = new Guid(iidBytes.Value.ToArray());
                        parameter = new Parameter(iid);
                        break;
                    case 1:
                        parameter.Name = reader.ReadString();
                        break;
                    case 2:
                        parameter.Value = reader.ReadDouble();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;

            return parameter;
        }
    }
}
