//  -------------------------------------------------------------------------------------------------
//  <copyright file="PayloadFactory.cs" company="RHEA System S.A.">
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

    using Model;

    /// <summary>
    /// The purpose of the <see cref="PayloadFactory"/> class is to create an
    /// instance of <see cref="Payload"/>
    /// </summary>
    public static class PayloadFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="Payload"/> from the provided <see cref="IEnumerable{Thing}"/>
        /// </summary>
        /// <param name="things">
        /// The <see cref="IEnumerable{Thing}"/> on the bases of which the <see cref="Payload"/> will
        /// be created
        /// </param>
        /// <returns>
        /// An instance of <see cref="Payload"/>
        /// </returns>
        public static Payload Create(IEnumerable<Thing> things)
        {
            if (things == null)
            {
                throw new ArgumentNullException(nameof(things));
            }

            var payload = new Payload
            {
                Created = DateTime.UtcNow
            };

            foreach (var thing in things)
            {
                switch (thing.ClassKind)
                {
                    case ClassKind.ElementDefinition:
                        payload.ElementDefinition.Add((ElementDefinition)thing);
                        break;
                    case ClassKind.Parameter:
                        payload.Parameter.Add((Parameter)thing);
                        break;
                }
            }

            return payload;
        }

        /// <summary>
        /// Creates an <see cref="IEnumerable{Thing}"/> from the provided <see cref="Payload"/>.
        /// </summary>
        /// <param name="payload">
        /// The <see cref="Payload"/> that carries the <see cref="Thing"/>s.
        /// </param>
        /// <returns>
        /// an <see cref="IEnumerable{Thing}"/>.
        /// </returns>
        public static IEnumerable<Thing> Create(Payload payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            var result = new List<Thing>();
            result.AddRange(payload.ElementDefinition);
            result.AddRange(payload.Parameter);

            return result;
        }
    }
}
