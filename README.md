== Introduction

The **blazormessagepackexperiment** repository contains an experimental implementation of a simplified ECSS-E-TM-10-25 like serializer on the basis of [MessagePack](https://github.com/MessagePack-CSharp/MessagePack-CSharp). The solution contains a very minimal **MessagePackTest.Model** project that contains model classes, a **MessagePackTest.MessagePack** project that contains the serializer and the **MessagePackTest** project that contains a simple blazor application that is used to test the performance of Blazor deserialization using MessagePack.

ECSS-E-TM-10-25 uses JSON serialization where the objects are serialized to a JSON Array. The ClassKind property is used to denote the Type of object. The MessagePack serializer proposes a different data structure which is optimized for speed. A Payload class is defined which has a property for each of the concrete classes defined in ECSS-E-TM-10-25. 

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

> Note: the `RatioScale` and `CyclicRatioScale` must have special treatment such that `CyclicRatioScale` instances are not duplicated in the `RatioScale` property of the `Payload` class.

== Versions

The CDP4-COMET data-model provides versioned extensions of ECSS-E-TM-10-25. All version will need to be supported. MessagePack uses string and numerical keys to identify properties of an object when serialized to the binary format. The numneric keys provide best performance and are therefore used for serialization of the 10-25 objects. The value of the keys may not change over time. For each version that CDP4-COMET supports, a dedicated versioned `Payload` class and dedicated versioned `IMessagePackFormatter{Thing}` classes are generated. For each version a namespace is created. The baseline version is `1.0.0` which corresponds to version `2.4.1` of ECSS-E-TM-10-25 Annex A. The namespace for version `1.0.0` of the MessagePack serializer is the following: `CDP4MessagePackerializer.v1_0_0.`.

== Content Negoitation

The `Content-Type` HTTP header is the header that specifies the ECCS-E-TM-10-25 protocol and it's version. The CDP4-COMET Web Services support the following:
  
  * application/json
  * application/json; ecss-e-tm-10-25; version=1.0.0
  * application/msgpack; ecss-e-tm-10-25; version=1.0.0

The last 2 provide extra information, i.e. that the content is according to `ECSS-E-TM-10-25` and the version. The third is a new MIME type to support `MessagePack`