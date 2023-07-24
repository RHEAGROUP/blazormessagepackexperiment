//  -------------------------------------------------------------------------------------------------
//  <copyright file="DeserializationMetrics.cs" company="RHEA System S.A.">
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

namespace MessagePackTest.Pages
{
	/// <summary>
	/// Represents the deserialization metrics
	/// </summary>
	public class DeserializationMetrics
	{
		/// <summary>
		/// Gets or sets the Size of the payload in bytes
		/// </summary>
		public long Size { get; set; }

		/// <summary>
		/// Gets or sets the amount of Thing objects in the payload
		/// </summary>
		public int Amount { get; set; }

		/// <summary>
		/// Gets or sets the amount of Ticks that elapsed during the deserialization
		/// </summary>
		public long ElapsedMilliseconds { get; set; }
	}
}
