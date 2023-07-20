//  -------------------------------------------------------------------------------------------------
//  <copyright file="Payload.cs" company="RHEA System S.A.">
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

    using Model;

    /// <summary>
    /// The purpose of the <see cref="Payload"/> class is to act as a wrapper class around
    /// the various model classes.
    /// </summary>
    [MessagePackObject]
    public class Payload
    {
        /// <summary>
        /// The <see cref="DateTime"/> the payload was created.
        /// </summary>
        [Key("Created")]
        public DateTime Created { get; set; }

        /// <summary>
        /// The <see cref="ElementDefinition"/> instances that are part of the payload
        /// </summary>
        [Key("ElementDefinition")]
        public List<ElementDefinition> ElementDefinition  { get; set; } = new List<ElementDefinition>();

        /// <summary>
        /// The <see cref="Parameter"/> instances that are part of the payload
        /// </summary>
        [Key("Parameter")]
        public List<Parameter> Parameter { get; set; } = new List<Parameter>();
    }
}
