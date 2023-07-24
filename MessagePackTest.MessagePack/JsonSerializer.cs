//  -------------------------------------------------------------------------------------------------
//  <copyright file="JsonSerializer.cs" company="RHEA System S.A.">
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
	using System.IO;
	using System.IO.Pipelines;
	using System.Linq;
	using System.Text.Json;
	using System.Threading;
	using System.Threading.Tasks;
	
	using Model;

	/// <summary>
	/// A JSON serializer
	/// </summary>
	public class JsonSerializer
	{
		/// <summary>
		/// Serialize the data to a <see cref="PipeWriter"/>
		/// </summary>
		/// <param name="things">
		/// The <see cref="List{Thing}"/> that is to be serialized
		/// </param>
		/// <param name="pipeWriter">
		/// The target <see cref="PipeWriter"/>
		/// </param>
		/// <param name="jsonWriterOptions">
		/// The <see cref="JsonWriterOptions"/>
		/// </param>
		/// <param name="cancellationToken">
		/// A <see cref="CancellationToken"/>
		/// </param>
		public void SerializeToPipeWriter(List<Thing> things, PipeWriter pipeWriter, JsonWriterOptions jsonWriterOptions, CancellationToken cancellationToken)
		{
			using var writer = new Utf8JsonWriter(pipeWriter, jsonWriterOptions);
			writer.WriteStartObject();
			writer.WritePropertyName("Created"u8);
			writer.WriteStringValue(DateTime.UtcNow);

			writer.WriteStartArray("ElementDefinition"u8);

			var elementDefinitions = things.OfType<ElementDefinition>();
			foreach (var elementDefinition in elementDefinitions)
			{
				writer.WriteStartObject();

				writer.WritePropertyName("Iid"u8);
				writer.WriteStringValue(elementDefinition.Iid.ToString());
					
				writer.WritePropertyName("Name"u8);
				writer.WriteStringValue(elementDefinition.Name);
					
				writer.WritePropertyName("Description"u8);
				writer.WriteStringValue(elementDefinition.Description);
					
				writer.WriteStartArray("Aliases"u8);
				foreach (var alias in elementDefinition.Aliases)
				{
					writer.WriteStringValue(alias);
				}
				writer.WriteEndArray();
					
				writer.WriteStartArray("Parameters"u8);
				foreach (var parameter in elementDefinition.Parameters)
				{
					writer.WriteStringValue(parameter.ToString());
				}
				writer.WriteEndArray();

				writer.WriteEndObject();
			}

			writer.WriteEndArray();

				
			writer.WriteStartArray("Parameter"u8);

			var parameters = things.OfType<Parameter>();
			foreach (var parameter in parameters)
			{
				writer.WriteStartObject();

				writer.WritePropertyName("Iid"u8);
				writer.WriteStringValue(parameter.Iid.ToString());

				writer.WritePropertyName("Name"u8);
				writer.WriteStringValue(parameter.Name);

				writer.WritePropertyName("Value"u8);
				writer.WriteNumberValue(parameter.Value);

				writer.WriteEndObject();
			}

			writer.WriteEndArray();
				
			writer.WriteEndObject();
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
		public async Task<IEnumerable<Thing>> Deserialize(Stream contentStream, CancellationToken cancellationToken)
		{
			var things = new List<Thing>();

			using (var document = await JsonDocument.ParseAsync(contentStream, default(JsonDocumentOptions), cancellationToken))
			{
				var root = document.RootElement;

				if (root.TryGetProperty("Created"u8, out var createdProperty))
				{
					Console.WriteLine("created: ", createdProperty.GetDateTime());
				}

				if (root.TryGetProperty("ElementDefinition"u8, out var elementDefinitionProperty))
				{
					foreach (var jsonElement in elementDefinitionProperty.EnumerateArray())
					{
						var elementDefinition = new ElementDefinition();

						if (jsonElement.TryGetProperty("Iid"u8, out var iidProperty))
						{
							elementDefinition.Iid = iidProperty.GetGuid();
						}

						if (jsonElement.TryGetProperty("Name"u8, out var nameProperty))
						{
							elementDefinition.Name = nameProperty.GetString();
						}

						if (jsonElement.TryGetProperty("Description"u8, out var descriptionProperty))
						{
							elementDefinition.Description = descriptionProperty.GetString();
						}

						if (jsonElement.TryGetProperty("Aliases"u8, out var aliasesProperty))
						{
							foreach (var aliasJsonElement in aliasesProperty.EnumerateArray())
							{
								elementDefinition.Aliases.Add(aliasJsonElement.GetString());
							}
						}

						if (jsonElement.TryGetProperty("Parameters"u8, out var parametersProperty))
						{
							foreach (var parameterJsonElement in parametersProperty.EnumerateArray())
							{
								elementDefinition.Parameters.Add(parameterJsonElement.GetGuid());
							}
						}

						things.Add(elementDefinition);
					}
				}

				if (root.TryGetProperty("Parameter"u8, out var parameterProperty))
				{
					foreach (var jsonElement in parameterProperty.EnumerateArray())
					{
						var parameter = new Parameter();

						if (jsonElement.TryGetProperty("Iid"u8, out var iidProperty))
						{
							parameter.Iid = iidProperty.GetGuid();
						}

						if (jsonElement.TryGetProperty("Name"u8, out var nameProperty))
						{
							parameter.Name = nameProperty.GetString();
						}

						if (jsonElement.TryGetProperty("Value"u8, out var valueProperty))
						{
							parameter.Value = valueProperty.GetDouble();
						}

						things.Add(parameter);
					}
				}
			}

			return things;
		}
	}
}
