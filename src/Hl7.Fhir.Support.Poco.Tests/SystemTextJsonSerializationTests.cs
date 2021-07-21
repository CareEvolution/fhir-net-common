using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Hl7.Fhir.Support.Poco.Tests
{
    [TestClass]
    public class SystemTextJsonSerializationTests
    {
        private (TestPatient, string) getEdgecases()
        {
            var filename = Path.Combine("TestData", "json-edge-cases.json");
            var expected = File.ReadAllText(filename);

            // For now, deserialize with the existing deserializer, until we have completed
            // the dynamicserializer too.
            return (TypedSerialization.ToPoco<TestPatient>(FhirJsonNode.Parse(expected)), expected);
        }

        [TestMethod]
        public void CanSerializeEdgeCases()
        {
            var (poco, expected) = getEdgecases();

            var options = new JsonSerializerOptions().ForFhirCompact(typeof(TestPatient).Assembly);
            //Resource poco = JsonSerializer.Deserialize<Resource>(expected, options);
            string actual = JsonSerializer.Serialize(poco, options);

            var errors = new List<string>();
            JsonAssert.AreSame("edgecases", expected, actual, errors);
            Console.WriteLine(string.Join("\r\n", errors));
            Assert.AreEqual(0, errors.Count, "Errors were encountered comparing converted content");
        }

        [TestMethod]
        public void PrintsPretty()
        {
            var (poco, _) = getEdgecases();

            var optionsCompact = new JsonSerializerOptions().ForFhirCompact(typeof(TestPatient).Assembly);
            string compact = JsonSerializer.Serialize(poco, optionsCompact);
            var compactWS = compact.Where(c => char.IsWhiteSpace(c)).Count();

            var optionsPretty = new JsonSerializerOptions().ForFhirPretty(typeof(TestPatient).Assembly);
            string pretty = JsonSerializer.Serialize(poco, optionsPretty);
            var prettyWS = pretty.Where(c => char.IsWhiteSpace(c)).Count();

            // much more whitespace, in fact...
            Assert.IsTrue(prettyWS > compactWS * 2);
        }
    }

}
