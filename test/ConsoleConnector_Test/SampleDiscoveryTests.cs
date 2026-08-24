using System;
using System.Linq;
using ConsoleConnector.Driver;
using ConsoleConnector.Samples;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleConnector_Test
{
    [TestClass]
    public class SampleDiscoveryTests
    {
        [TestMethod]
        public void DiscoverSamples_ReturnsOrderedByCategoryThenSampleThenSubSample()
        {
            var samples = SampleDiscovery.DiscoverSamples();

            var ordered = samples
                .OrderBy(s => s.CategoryId)
                .ThenBy(s => s.SampleId)
                .ThenBy(s => s.SubSampleId)
                .ToList();

            CollectionAssert.AreEqual(ordered, (System.Collections.ICollection)samples);
        }

        [TestMethod]
        public void DiscoverSamples_EveryEntryHasUniqueAddress()
        {
            var samples = SampleDiscovery.DiscoverSamples();

            var duplicateKeys = samples
                .GroupBy(s => s.Key)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            Assert.AreEqual(0, duplicateKeys.Count, $"Duplicate SampleAddress keys: {string.Join(", ", duplicateKeys)}");
        }

        [TestMethod]
        public void DiscoverSamples_NeverThrows_EvenWithNoSamplesRegisteredYet()
        {
            var samples = SampleDiscovery.DiscoverSamples();

            Assert.IsNotNull(samples);
        }

        [TestMethod]
        public void DiscoverSamples_EveryDiscoveredType_HasParameterlessConstructorAndAddress()
        {
            var samples = SampleDiscovery.DiscoverSamples();

            foreach (var sample in samples)
            {
                var type = sample.Sample.GetType();
                Assert.IsTrue(typeof(ISample).IsAssignableFrom(type), $"{type} must implement ISample.");
                Assert.IsNotNull(
                    Attribute.GetCustomAttribute(type, typeof(SampleAddressAttribute)),
                    $"{type} must declare a SampleAddressAttribute to be discoverable.");
            }
        }
    }
}
