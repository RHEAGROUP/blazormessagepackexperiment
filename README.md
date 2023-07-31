# Introduction

Blazor is proving to be a great technology stack for the [CDP4-COMET](https://www.rheagroup.com/services-solutions/system-engineering/concurrent-design/download-cdp4-comet/). CDP4-COMET is an open-source collaborative Model Based System Engineering (MBSE) application. It is an implementation of [ECSS-E-TM-10-25](https://ecss.nl/hbstms/ecss-e-tm-10-25a-engineering-design-model-data-exchange-cdf-20-october-2010/) and as such provides a JSON REST API, nothing out of the ordinary there. One peculiarity of CDP4-COMET is that caching a complete object graph is a requirement to provide users with a responsive application. That also means that the initial REST request loads a large data set, something in the order of 1 MB.

For the .NET Framework based desktop application this is not an issue. The code-generated [Json.NET](https://www.newtonsoft.com/json) implementation has been optimized and was never cause for any concern. But now that we are also developing for the web, and in particular using Blazor WebAssembly we've noticed poor performance for these large payloads. Deserialization may take anywhere from 30 to 60 seconds, which is far too slow to give users a smooth experience. Our original serializer is implemented using [Json.NET](https://www.newtonsoft.com/json), we've achieved a performance improvement using [System.Text.Json](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/how-to?pivots=dotnet-7-0), and even better performance using [MessagePack](https://github.com/MessagePack-CSharp/MessagePack-CSharp). This repository contains a prototype we've developed to compare [System.Text.Json](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/how-to?pivots=dotnet-7-0) vs [MessagePack](https://github.com/MessagePack-CSharp/MessagePack-CSharp) and that has given us the confidence that a full [MessagePack](https://github.com/MessagePack-CSharp/MessagePack-CSharp) implementation is required to give us the performance we need.

> NOTE: The prototype demonstrates that MessagePack is approximately 3 times faster in Blazor WebAssembly than System.Text.Json. FireFox outperforms Chrome and Edge when deserializing MessagePack and Json with a factor of 3 to 4.

# Background

[CDP4-COMET](https://www.rheagroup.com/services-solutions/system-engineering/concurrent-design/download-cdp4-comet/) is an open-source collaborative Model Based System Engineering (MBSE) application that allows a team of engineers to create both a model-based requirements specification and architecture. This type of application is typically used to support Concurrent Engineering or Concurrent Design. Organizations such as the [European Space Agency](https://www.esa.int/Enabling_Support/Space_Engineering_Technology/COMET_upgrade_for_ESA_s_mission_design_centre) make use of [CDP4-COMET](https://www.rheagroup.com/services-solutions/system-engineering/concurrent-design/download-cdp4-comet/) to perform early phase design of future space missions. CDP4-COMET is based on a Space Engineering standard called [ECSS-E-TM-10-25](https://ecss.nl/hbstms/ecss-e-tm-10-25a-engineering-design-model-data-exchange-cdf-20-october-2010/) (a technical memorandum). The standard prescribes a data model, expressed in UML, and a JSON REST API. Using both one can develop a standard compliant application.

[CDP4-COMET](https://www.rheagroup.com/services-solutions/system-engineering/concurrent-design/download-cdp4-comet/) exists out a C# SDK, a server implementation, a desktop application and recently also a web application. The server and desktop application are implemented using the dotnet framework, the web application is developed using Blazor WebAssembly. One peculiar aspect of ECSS-E-TM-10-25 is that on initial load of data from the server, a rather large payload is returned. This can be in the order of 1 MB. Subsequent reads only retrieve the delta between the last version on the server and the version of the client, resulting in very small updates. All the data is cached client side in a complete object graph. Due to the optimized JSON implementation this was never an issue, neither for the server nor for the Desktop application. Come Blazor WebAssembly, this design seems to be a challenge, deserializing a 1 MB JSON payload has become a bottleneck. The deserialization process takes far too much time, somewhere between 30 to 60 seconds, which for an end user is of course not acceptable.

After reading various articles on the web we came to the conclusion Json.NET is not the most performant JSON implementation anymore; and for WebAssembly, MessagePack seems to be an even better choice. So, we have adopted 2 approaches:
  - Replace Json.NET with System.Text.Json
  - Add a MessagePack based serialization implementation next to the standard based JSON API

# The Prototype

The prototype is implemented using VS2022. The solution contains the following projects:
  - MessagePackTest.Model: a C# library that contains 3 model classes that are similar to [ECSS-E-TM-10-25 classes](https://comet-dev-docs.mbsehub.org/).
  - MessagePackTest.MessagePack: A C# library that contains both a MessagePack and JSON serializer.
  - MessagePackTest.Server: A NET7.0 [CarterCommunity](https://github.com/CarterCommunity/Carter) based implementation of a REST API
  - messagepacktest: A NET7.0 Blazor WebAssemnbly application that queries data from the server using both JSON and MessagePack

## MessagePackTest.MessagePack

Both the **JSON** and the **MessagePack** serializers are implemented using the available low-level API's provided by System.Text.Json and MessagePack libraries respectively. For the prototype these are hand-coded, but for a CDP4-COMET implementation these will be code-generated from the ECSS-E-TM-10-25 UML model. The data that is to be transported over the wire is encapsulated using the `Payload`` class.

```
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

```

> Note: For the payload class we make use of the `MessagePackObject` and `Key` C# attributes to decorate the class and properties for automatic serialization and deserialization. The `ElementDefinition` and `Parameter` classes defined in the MessagePackTest.Model project are (de)serialized using the `IMessagePackFormatter` classes defined below.

### MessagePack

MessagePack is an efficient binary serialization format. MessagePack provides a low-level API in which serialization and deserialization.  This is done ny implementing the  `IMessagePackFormatter<T>` interface for each class that needs to serialized and deserialized. 

```
public class ElementDefinitionFormatter : IMessagePackFormatter<ElementDefinition>
{
    public void Serialize(ref MessagePackWriter writer, ElementDefinition elementDefinition, MessagePackSerializerOptions options)
    {
        if (elementDefinition == null)
        {
            throw new ArgumentNullException(nameof(elementDefinition), "The ElementDefinition may not be null");
        }

        writer.WriteArrayHeader(5);
        writer.Write(elementDefinition.Iid.ToByteArray());

        writer.WriteArrayHeader(elementDefinition.Aliases.Count);
        foreach (var alias in elementDefinition.Aliases)
        {
            writer.Write(alias);
        }
        writer.Write(elementDefinition.Description);
        writer.Write(elementDefinition.Name);

        writer.WriteArrayHeader(elementDefinition.Parameters.Count);
        foreach (var parameter in elementDefinition.Parameters)
        {
            writer.Write(parameter.ToByteArray());
        }

        writer.Flush();
    }

    public ElementDefinition Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        options.Security.DepthStep(ref reader);

        ElementDefinition elementDefinition = null;
            
        int count = reader.ReadArrayHeader();
        for (int counter = 0; counter < count; counter++)
        {
            switch (counter)
            {
                case 0:
                    var iidBytes = reader.ReadBytes();
                    var iid = new Guid(iidBytes.Value.ToArray());
                    elementDefinition = new ElementDefinition(iid);
                    break;
                case 1:
                    var aliasLength = reader.ReadArrayHeader();
                    elementDefinition.Aliases = new List<string>();
                    for (int aliasCounter = 0; aliasCounter < aliasLength; aliasCounter++)
                    {
                        elementDefinition.Aliases.Add(reader.ReadString());
                    }
                    break;
                case 2:
                    elementDefinition.Description = reader.ReadString();
                    break;
                case 3:
                    elementDefinition.Name = reader.ReadString();
                    break;
                case 4:
                    var parametersLength = reader.ReadArrayHeader();
                    for (int parametersCounter = 0; parametersCounter < parametersLength; parametersCounter++)
                    {
                        var parameterIidBytes = reader.ReadBytes();

                        var parameterIid = new Guid(parameterIidBytes.Value.ToArray());

                        elementDefinition.Parameters.Add(parameterIid);
                    }
                    break;
                default:
                    reader.Skip();
                    break;
            }
        }

        reader.Depth--;
        return elementDefinition;
    }
}
```

To complete the MessagePack, the `ThingFormatterResolvers` need to be registered, for this we implement the `ThingFormatterResolver`:

```

public class ThingFormatterResolver : IFormatterResolver
{
    // Resolver should be singleton.
    public static readonly IFormatterResolver Instance = new ThingFormatterResolver();

    private ThingFormatterResolver()
    {
    }

    public IMessagePackFormatter<T> GetFormatter<T>()
    {
        return FormatterCache<T>.Formatter;
    }
    
    private static class FormatterCache<T>
    {
        public static readonly IMessagePackFormatter<T> Formatter;

        // generic's static constructor should be minimized for reduce type generation size!
        // use outer helper method.
        static FormatterCache()
        {
            var type = typeof(T);

            Formatter = (IMessagePackFormatter<T>)ThingResolverGetFormatterHelper.GetFormatter(type);
        }
    }
}

internal static class ThingResolverGetFormatterHelper
{
    // If type is concrete type, use type-formatter map
    static readonly Dictionary<Type, object> FormatterMap = new Dictionary<Type, object>()
    {
        {typeof(ElementDefinition), new ElementDefinitionFormatter()},
        {typeof(Parameter), new ParameterFormatter()}
    };

    internal static object GetFormatter(Type t)
    {
        object formatter;
        if (FormatterMap.TryGetValue(t, out formatter))
        {
            return formatter;
        }

        // If type can not get, must return null for fallback mechanism.
        return null;
    }
}

```

Last but not least we need to implement the MessagePackSerializer that in makes use of a `CompositeResolver` in which the `ThingFormatterResolver` is registered:

```
public class MessagePackSerializer
{
    public async Task SerializeToStream(List<Thing> things, Stream outputStream, CancellationToken cancellationToken)
    {
        var resolver = CompositeResolver.Create(
            ThingFormatterResolver.Instance,
            StandardResolver.Instance);

        var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);
        
        var payload = PayloadFactory.Create(things);

        await global::MessagePack.MessagePackSerializer.SerializeAsync(outputStream, payload, options, cancellationToken);
    }

    public void SerializeToPipeWriter(List<Thing> things, PipeWriter pipeWriter, CancellationToken cancellationToken)
    {
        var resolver = CompositeResolver.Create(
            ThingFormatterResolver.Instance,
            StandardResolver.Instance);

        var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);

        var payload = PayloadFactory.Create(things);

        global::MessagePack.MessagePackSerializer.Serialize(pipeWriter, payload, options, cancellationToken);
    }

    public async Task <IEnumerable<Thing>> Deserialize(Stream contentStream, CancellationToken cancellationToken)
    {
        var resolver = CompositeResolver.Create(
            ThingFormatterResolver.Instance,
            StandardResolver.Instance);

        var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);

        var payload = await global::MessagePack.MessagePackSerializer.DeserializeAsync<Payload>(contentStream, options, cancellationToken);
        
        var result = PayloadFactory.Create(payload);
        
        return result;
    }
}

```

### JSON

System.Text.JSON provides a low-level API with which custom serializers can be created, similar to the MessagePack formatter these are implemented to have a fair comparison between the two implementations. See an example below:

```
public class JsonSerializer
{
       
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

    public async Task<IEnumerable<Thing>> Deserialize(Stream contentStream, CancellationToken cancellationToken)
    {
        var things = new List<Thing>();

        var sw = Stopwatch.StartNew();
        using var document = await JsonDocument.ParseAsync(contentStream, default(JsonDocumentOptions), cancellationToken);
        var root = document.RootElement;

        var jsonParseTime = sw.ElapsedMilliseconds;
            
        sw = Stopwatch.StartNew();
            
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

        var jsonDeserializeTime = sw.ElapsedMilliseconds;

        Console.WriteLine("JsonDocument Content Parsed:Deserialized in {0}:{1} [ms]", jsonParseTime, jsonDeserializeTime);

        return things;
    }
}
```

Both the `JsonSerializer` and `MessagePackSerializer` classes are used in the prototype server and Blazor application.

## MessagePackTest.Server

This is a very simple [Carter](https://github.com/CarterCommunity/Carter) implementation that only contains one `Module` that exposes one route that returns a list of `Thing` classes (the abstract super class of all domain model classes in the MessagePackTest.Model project). The server generates random instances, the route allows a caller to specify the amount of objects that should be created. The following example will return a total of 20 objects comprised of 10 `ElementDefinition`s and 10 `Parameter`s.

```
http://127.0.0.1:5000/things/10
```

The route definition looks like this:

```
public override void AddRoutes(IEndpointRouteBuilder app)
{
    app.MapGet("/", () => "MessagePack TEST using Carter");

    app.MapGet("/things/{amount:int}", (int amount, HttpResponse res) =>
    {
        var things = this.GenerateThings(amount);
        res.Negotiate(things);
     });
}
```

The `res.Negotiate(things);` statement uses response negotation using the Accept HTTP header. The following headers are supported:
  - JSON response: application/json; ecss-e-tm-10-25; version=1.0.0
  - JSON response: text/html
  - MessagePack response: application/msgpack; ecss-e-tm-10-25; version=1.0.0

Carter makes it very easy to implement content negotation, it only requires the implementation and registration of `IResponseNegotiator`, see both implementations below:

```
public class JsonSerializerResponseNegotiator : IResponseNegotiator
{
    public bool CanHandle(MediaTypeHeaderValue accept)
    {
        var mediaType = accept.ToString();

        var contentTypeArray = mediaType.Split(';');
        if (contentTypeArray[0].Trim() == "application/json"
            && contentTypeArray[1].Trim() == "ecss-e-tm-10-25"
            && contentTypeArray[2].Trim() == "version=1.0.0")
        {
            return true;
        }

        if (contentTypeArray[0].Trim() == "text/html")
        {
            return true;
        }

        return false;
    }

    public Task Handle<T>(HttpRequest req, HttpResponse res, T model, CancellationToken cancellationToken)
    {
        var things = model as List<Thing>;

        if (things == null)
        {
            throw new NotSupportedException("Only List<Thing> is supported");
        }
        
        res.ContentType = "application/json";

        var jsonSerializer = new MessagePack.JsonSerializer();

        var jsonWriterOptions = new JsonWriterOptions
        {
            Indented = false
        };

        jsonSerializer.SerializeToPipeWriter(things, res.BodyWriter, jsonWriterOptions, cancellationToken);

        return Task.CompletedTask;
    }
}

public class MessagePackResponseNegotiator : IResponseNegotiator
{
    public bool CanHandle(MediaTypeHeaderValue accept)
    {
        var mediaType = accept.ToString();
            
        var contentTypeArray = mediaType.Split(';');
        if (contentTypeArray[0].Trim() == "application/msgpack"
            && contentTypeArray[1].Trim() == "ecss-e-tm-10-25"
            && contentTypeArray[2].Trim() == "version=1.0.0")
        {
            return true;
        }

         return false;
    }

    public Task Handle<T>(HttpRequest req, HttpResponse res, T model, CancellationToken cancellationToken)
    {
        var things = model as List<Thing>;

        if (things == null)
        {
            throw new NotSupportedException("Only List<Thing> is supported");
        }
            
         res.ContentType = "application/msgpack; ecss-e-tm-10-25; version=1.0.0";

         var serializer = new MessagePack.MessagePackSerializer();
            
         serializer.SerializeToPipeWriter(things, res.BodyWriter, cancellationToken);

         return Task.CompletedTask;
        }
    }

```

Both `IResponseNegotiator` classes are registered with Carter in the `Program.cs` class as follows:

```
builder.Services.AddCarter(configurator: c =>
{
    c.WithResponseNegotiator<MessagePackResponseNegotiator>();
    c.WithResponseNegotiator<JsonSerializerResponseNegotiator>();
});
```

# Performance Comparison

The Blazor WebAssembly application uses both the MessagePack and Json serilizer to deserialize the response from the server. The application has been tested using
  - FireFox - version 115.0.2 (64-bit)
  - Chrome - version 114.0.5735.248 (Official Build) (64-bit)
  - Edge - version 115.0.1901.183 (Official build) (64-bit)
The results are presented below. The numbers per iteration are not absolutes, the data we are after is the difference between MessagePack and JSON.

The REST requests are performed 50 times using varrious payload sizes of 2, 20, 200, 2000, and 20000 objects. The table below shows the results for the 3 browsers and 2 deserialization formats. Even though the table does show a difference in performance between the browsers, the focus here is on the performance difference between System.Text.Json and MessagePack.

> RESULT: from the numbers we can state that MessagePack deserialization is approximately 3 times faster than System.Text.Json deserialization. For large payloads, Firefox outperforms Chrome and Edge in both the MessagePack and Json case with a factor of 3 to 4.

| Brosser | Serialization | nr of objects | payload size [bytes] | mean deserilization time [ms] |
| ------- | ------------- | --------------| -------------------- | ----------------------------- |
| FireFox | MessagePack   | 2             | 245                  | 0.54                          |
| FireFox | JSON          | 2             | 414                  | 1.28                          |
| FireFox | MessagePack   | 20            | 2009                 | 1.52                          |
| FireFox | JSON          | 20            | 3438                 | 4.24                          |
| FireFox | MessagePack   | 200           | 20013                | 6.10                          |
| FireFox | JSON          | 200           | 34128                | 27.92                         |
| FireFox | MessagePack   | 2000          | 203613               | 54.62                         |
| FireFox | JSON          | 2000          | 345528               | 191.64                        |
| FireFox | MessagePack   | 20000         | 2075613              | 558.52                        |
| FireFox | JSON          | 20000         | 3504528              | 1716.32                       |
| Chrome  | MessagePack   | 2             | 245                  | 0.46                          |
| Chrome  | JSON          | 2             | 414                  | 1.34                          |
| Chrome  | MessagePack   | 20            | 2009                 | 3.56                          |
| Chrome  | JSON          | 20            | 3438                 | 8.78                          |
| Chrome  | MessagePack   | 200           | 20013                | 29.56                         |
| Chrome  | JSON          | 200           | 34128                | 62.84                         |
| Chrome  | MessagePack   | 2000          | 203613               | 198.60                        |
| Chrome  | JSON          | 2000          | 345528               | 514.22                        |
| Chrome  | MessagePack   | 20000         | 2075613              | 1847.36                       |
| Chrome  | JSON          | 20000         | 3504528              | 6487.52                       |
| Edge    | MessagePack   | 2             | 245                  | 0.48                          |
| Edge    | JSON          | 2             | 414                  | 1.54                          |
| Edge    | MessagePack   | 20            | 2009                 | 3.04                          |
| Edge    | JSON          | 20            | 3438                 | 9.28                          |
| Edge    | MessagePack   | 200           | 20013                | 21.44                         |
| Edge    | JSON          | 200           | 34128                | 54.60                         |
| Edge    | MessagePack   | 2000          | 203613               | 192.62                        |
| Edge    | JSON          | 2000          | 345528               | 464.40                        |
| Edge    | MessagePack   | 20000         | 2075613              | 1671.08                       |
| Edge    | JSON          | 20000         | 3504528              | 4905.02                       |