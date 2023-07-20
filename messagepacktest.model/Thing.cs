//  -------------------------------------------------------------------------------------------------
//  <copyright file="Thing.cs" company="RHEA System S.A.">
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
    /// top level abstract superclass from which all domain concept classes in the model inherit 
    /// </summary>
    public abstract class Thing
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Thing"/> class.
        /// </summary>
        protected Thing()
        {
            this.ClassKind = this.ComputeCurrentClassKind();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Thing"/>
        /// </summary>
        /// <param name="iid">
        /// The unique identifier of the <see cref="Thing"/>
        /// </param>
        protected Thing(Guid iid) : this()
        {
            this.Iid = iid;
        }

        /// <summary>
        /// Gets the assertion of the ClassKind of this Thing, denoting its actual class.
        /// Note: Typically this is used internally by the implementing software to improve classification of instances and optimise performance when moving data between different programming environments. In an object-oriented software engineering environment that supports reflection such information would be redundant.
        /// </summary>
        public readonly ClassKind ClassKind;

        /// <summary>
        /// Gets or sets the unique identifier
        /// </summary>
        public Guid Iid { get; set; }

        /// <summary>
        /// Computes the ClassKind of the current object based on it's type
        /// </summary>
        /// <returns>
        /// the <see cref="ClassKind"/> of the current object
        /// </returns>
        protected virtual ClassKind ComputeCurrentClassKind()
        {
            ClassKind classKind;
            var type = this.GetType();
            if (Enum.TryParse(type.Name, out classKind))
            {
                return classKind;
            }

            throw new InvalidOperationException($"The current Thing {type} does not have a corresponding ClassKind");
        }
    }
}