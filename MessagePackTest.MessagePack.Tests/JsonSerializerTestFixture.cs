//  -------------------------------------------------------------------------------------------------
//  <copyright file="JsonSerializerTestFixture.cs" company="RHEA System S.A.">
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

namespace MessagePackTest.MessagePack.Tests
{
	using System.IO;
	using System.Linq;
	using Model;
	using NUnit.Framework;

	[TestFixture]
	public class JsonSerializerTestFixture
	{
		private string payloadPath;

		private MessagePackTest.MessagePack.JsonSerializer jsonSerializer;

		[SetUp]
		public void SetUp()
		{
			this.payloadPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "data", "payload.json");

			this.jsonSerializer = new MessagePackTest.MessagePack.JsonSerializer();
		}

		[Test]
		public async Task Verify_that_payload_can_be_deserialized()
		{
			var cts = new CancellationTokenSource();
			var token = cts.Token;

			using var stream = File.OpenRead(this.payloadPath);
			var things = await this.jsonSerializer.Deserialize(stream, token);

			var elementDefinition = things.OfType<ElementDefinition>().Single();

			Assert.That(elementDefinition.Iid, Is.EqualTo(Guid.Parse("85e98ec8-14d4-4f9c-b018-e87e8d5a938b")));
			Assert.That(elementDefinition.Name, Is.EqualTo("ed_0"));
			Assert.That(elementDefinition.Description, Is.EqualTo("this is an element definition description 0 in which the quick brown fox jumps over the lazy dog"));
			Assert.That(elementDefinition.Aliases, Is.EquivalentTo(new[] { "alias_0", "another alias" }));
			Assert.That(elementDefinition.Parameters, Is.EquivalentTo(new[] { Guid.Parse("d6fb93e9-7c5c-4294-9d7f-10f0ec71703f") }));

			var parameter = things.OfType<Parameter>().Single();

			Assert.That(parameter.Iid, Is.EqualTo(Guid.Parse("d6fb93e9-7c5c-4294-9d7f-10f0ec71703f")));
			Assert.That(parameter.Name, Is.EqualTo("p_0"));
			Assert.That(parameter.Value, Is.EqualTo(0));
		}
	}
}
