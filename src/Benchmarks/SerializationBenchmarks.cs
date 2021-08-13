﻿using BenchmarkDotNet.Attributes;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Utility;
using System.IO;
using System.Text.Json;

namespace Firely.Sdk.Benchmarks.Common
{
    [MemoryDiagnoser]
    public class SerializationBenchmarks
    {
        internal TestPatient Patient;
        JsonSerializerOptions Options;
        XmlFhirDictionarySerializer XmlSerializer;


        [GlobalSetup]
        public void BenchmarkSetup()
        {
            var filename = Path.Combine("TestData", "fp-test-patient.json");
            var data = File.ReadAllText(filename);
            // For now, deserialize with the existing deserializer, until we have completed
            // the dynamicserializer too.
            Patient = TypedSerialization.ToPoco<TestPatient>(FhirJsonNode.Parse(data));
            Options = new JsonSerializerOptions().ForFhir(typeof(TestPatient).Assembly);
            XmlSerializer = new XmlFhirDictionarySerializer(Hl7.Fhir.Specification.FhirRelease.STU3);
        }

        [Benchmark]
        public string JsonDictionarySerializer()
        {
            return JsonSerializer.Serialize(Patient, Options);
        }

        [Benchmark]
        public string XmlDictionarySerializer()
        {
            return SerializationUtil.WriteXmlToString(Patient, (o, w) => XmlSerializer.Serialize(o, w));
        }

        [Benchmark]
        public string TypedElementSerializerJson()
        {
            return TypedSerialization.ToTypedElement(Patient).ToJson();
        }

        [Benchmark]
        public string TypedElementSerializerXml()
        {
            return TypedSerialization.ToTypedElement(Patient).ToXml();
        }
    }
}
