//  -------------------------------------------------------------------------------------------------
//  <copyright file="ElementDefinition.cs" company="RHEA System S.A.">
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
    using System.Collections.Generic;

    /// <summary>
    /// definition of an element in a design solution for a system-of-interest 
    /// </summary>
    public class ElementDefinition : Thing
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ElementDefinition"/>
        /// </summary>
        public ElementDefinition()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ElementDefinition"/>
        /// </summary>
        /// <param name="iid">
        /// The unique identifier of the <see cref="ElementDefinition"/>
        /// </param>
        public ElementDefinition(Guid iid) : base(iid)
        {
        }

        /// <summary>
        /// Gets or sets a human readable name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a human readable description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a representation of an alternative human-readable name for a concept 
        /// </summary>
        public string[] Aliases { get; set; }

        /// <summary>
        /// Gets or sets the unique identifiers of the contained <see cref="Parameter"/>s
        /// </summary>
        public List<Guid> Parameters { get; set; } = new List<Guid>();
    }
}
