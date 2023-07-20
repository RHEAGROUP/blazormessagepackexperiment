//  -------------------------------------------------------------------------------------------------
//  <copyright file="SerializerTestFixture.cs" company="RHEA System S.A.">
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
    using Model;
    
    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="Serializer"/> class
    /// </summary>
    [TestFixture]
    public class SerializerTestFixture
    {
        private Serializer serializer;

        private List<Thing> things;
 
        [SetUp]
        public void Setup()
        {
            this.serializer = new Serializer();

            var elementDefinition_battery = new ElementDefinition
            {
                Iid = Guid.Parse("a668865d-15fd-4e6e-83ed-de8e19322bec"),
                Name = "battery",
                Description = "this is a battery",
                Aliases = new string[] {"alias_1", "alias_2"},
                Parameters = new List<Guid>{ Guid.Parse("9d471fae-010f-4bc2-b169-9d9ee833c5d4"), 
                    Guid.Parse("337187a1-5160-4495-a727-9b2f5f730a36") }
            };

            var elementDefinition_pcdu = new ElementDefinition
            {
                Iid = Guid.Parse("3157cb37-a632-4814-b605-b7a0bf258e3c"),
                Name = "PCDU",
                Description = "this is a PCDU",
                Aliases = new string[]{}
            };

            var parameter_mass = new Parameter
            {
                Iid = Guid.Parse("9d471fae-010f-4bc2-b169-9d9ee833c5d4"),
                Name = "mass",
                Value = 1234567.01
            };

            var parameter_length = new Parameter
            {
                Iid = Guid.Parse("337187a1-5160-4495-a727-9b2f5f730a36"),
                Name = "length",
                Value = 10
            };

            this.things = new List<Thing>
            {
                elementDefinition_battery,
                elementDefinition_pcdu,
                parameter_mass,
                parameter_length
            };
        }

        [Test]
        public async Task Verify_that_list_of_things_can_be_serialized_and_deserialized()
        {
            var cts = new CancellationTokenSource();

            var stream = new MemoryStream();

            await this.serializer.SerializeToStream(this.things, stream, cts.Token);

            stream.Position = 0;

            var deserializedThings = await this.serializer.Deserialize(stream, cts.Token);

            var parameter_mass = deserializedThings.OfType<Parameter>()
                .Single(x => x.Iid == Guid.Parse("9d471fae-010f-4bc2-b169-9d9ee833c5d4"));

            Assert.That(parameter_mass.Name, Is.EqualTo("mass"));
            Assert.That(parameter_mass.Value, Is.EqualTo(1234567.01));

            var elementDefinition_battery = deserializedThings.OfType<ElementDefinition>()
                .Single(x => x.Iid == Guid.Parse("a668865d-15fd-4e6e-83ed-de8e19322bec"));

            Assert.That(elementDefinition_battery.Name, Is.EqualTo("battery"));
            Assert.That(elementDefinition_battery.Description, Is.EqualTo("this is a battery"));
            Assert.That(elementDefinition_battery.Aliases, Is.EquivalentTo( new string[]{ "alias_1", "alias_2" }));
        }
    }
}
