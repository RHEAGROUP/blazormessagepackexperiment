//  -------------------------------------------------------------------------------------------------
//  <copyright file="Parameter.cs" company="RHEA System S.A.">
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

namespace MessagePackTest.Model
{
    using System;

    /// <summary>
    /// representation of a <see cref="Parameter"/> that defines a characteristic or property of an <see cref="ElementDefinition"/> 
    /// </summary>
    public class Parameter : Thing
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Parameter"/>
        /// </summary>
        public Parameter()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Parameter"/>
        /// </summary>
        /// <param name="iid">
        /// The unique identifier of the <see cref="Parameter"/>
        /// </param>
        public Parameter(Guid iid) : base(iid)
        {
        }

        /// <summary>
        /// Gets or sets a human readable name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the numeric value
        /// </summary>
        public double Value { get; set; }
    }
}