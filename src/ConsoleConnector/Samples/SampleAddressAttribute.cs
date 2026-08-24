using System;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class SampleAddressAttribute : Attribute
    {
        public SampleAddressAttribute(int categoryId, int sampleId, int subSampleId = 0)
        {
            CategoryId = categoryId;
            SampleId = sampleId;
            SubSampleId = subSampleId;
        }

        public int CategoryId { get; }
        public int SampleId { get; }
        public int SubSampleId { get; }
    }
}
