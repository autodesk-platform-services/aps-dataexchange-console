using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConsoleConnector.Samples;

namespace ConsoleConnector.Driver
{
    internal static class SampleDiscovery
    {
        public static IReadOnlyList<SampleRef> DiscoverSamples()
        {
            var list = new List<SampleRef>();

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsAbstract || type.IsInterface || !typeof(ISample).IsAssignableFrom(type))
                    continue;

                var attr = type.GetCustomAttribute<SampleAddressAttribute>();
                if (attr == null)
                    continue;

                if (Activator.CreateInstance(type) is not ISample sample)
                    continue;

                list.Add(new SampleRef(attr.CategoryId, attr.SampleId, attr.SubSampleId, sample));
            }

            return list
                .OrderBy(s => s.CategoryId)
                .ThenBy(s => s.SampleId)
                .ThenBy(s => s.SubSampleId)
                .ToList();
        }
    }

    internal sealed class SampleRef
    {
        public SampleRef(int categoryId, int sampleId, int subSampleId, ISample sample)
        {
            CategoryId = categoryId;
            SampleId = sampleId;
            SubSampleId = subSampleId;
            Sample = sample;
        }

        public int CategoryId { get; }
        public int SampleId { get; }
        public int SubSampleId { get; }
        public ISample Sample { get; }
        public string Key => SubSampleId == 0
            ? $"{CategoryId}.{SampleId}"
            : $"{CategoryId}.{SampleId}.{SubSampleId}";
    }
}
