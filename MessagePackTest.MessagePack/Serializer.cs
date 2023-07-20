//  -------------------------------------------------------------------------------------------------
//  <copyright file="Serializer.cs" company="RHEA System S.A.">
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
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using global::MessagePack;
    using global::MessagePack.Resolvers;

    using Model;

    /// <summary>
    /// The purpose of the <see cref="Serializer"/> is to serialize a <see cref="List{Thing}"/>
    /// to a <see cref="Stream"/> using MessagePack serialization and deserialization 
    /// </summary>
    public class Serializer
    {
        /// <summary>
        /// Serializes the <paramref name="things"/> to the provided <paramref name="outputStream"/>
        /// </summary>
        /// <param name="things">
        /// The <see cref="List{Thing}"/> that is to be serialized
        /// </param>
        /// <param name="outputStream">
        /// The output <see cref="Stream"/> that to which the <paramref name="things"/> are to be serialized
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// an awaitable <see cref="Task"/>
        /// </returns>
        public async Task SerializeToStream(List<Thing> things, Stream outputStream, CancellationToken cancellationToken)
        {
            var resolver = CompositeResolver.Create(
                ThingFormatterResolver.Instance,
                StandardResolver.Instance);

            var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);
            
            var payload = PayloadFactory.Create(things);

            await MessagePackSerializer.SerializeAsync(outputStream, payload, options, cancellationToken);
        }

        /// <summary>
        /// Deserializes <see cref="Thing"/>a from the <paramref name="contentStream"/>
        /// </summary>
        /// <param name="contentStream">
        /// A stream containing <see cref="Thing"/>a
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{Thing}"/>
        /// </returns>
        public async Task <IEnumerable<Thing>> Deserialize(Stream contentStream, CancellationToken cancellationToken)
        {
            var resolver = CompositeResolver.Create(
                ThingFormatterResolver.Instance,
                StandardResolver.Instance);

            var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);

            var payload = await MessagePackSerializer.DeserializeAsync<Payload>(contentStream, options, cancellationToken);
            
            var result = PayloadFactory.Create(payload);
            
            return result;
        }
    }
}
