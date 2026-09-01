using System;
using System.Collections.Generic;
using Autodesk.DataExchange.DataModels;
using Autodesk.DataExchange.Interface;
using Autodesk.DataExchange.SchemaObjects.Units;
using Autodesk.GeometryUtilities.PrimitivesAPI;
using Autodesk.GeometryUtilities.PrimitivesAPI.DX;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// Demo element/geometry builders for samples — SDK setup data used by sync demos.
    /// </summary>
    internal static class SampleDataFactory
    {
        private static readonly RenderStyle DemoLineStyle =
            new("ConsoleConnector Line", new RGBA(255, 0, 0, 255), 1);

        internal static IElement CreateDemoLine(ElementDataModel model)
        {
            var elementId = $"Line_{Guid.NewGuid():N}"[..12];
            var element = model.AddElement(
                elementId,
                "Sample Line",
                lengthUnit: UnitFactory.Centimeter,
                displayLengthUnit: UnitFactory.Centimeter);
            ElementSampleHelper.ClassifyGeneric(model, element);

            var geometryContainer = new GeometryContainer();
            var line = new Line(
                new Point3d { X = 200, Y = 200, Z = 200 },
                new Vector3d { X = 100, Y = 400, Z = 300 });
            line.Range = new ParamRange
            {
                High = 3.5,
                Low = 0,
                Type = ParamRange.RangeType.Finite,
            };
            geometryContainer.Curves.Add(line);

            var units = new Autodesk.DataExchange.DataModels.Units(UnitFactory.Centimeter, UnitFactory.Centimeter, UnitFactory.Centimeter);
            var geometry = ElementDataModel.CreatePrimitiveGeometry(geometryContainer, DemoLineStyle, units);
            model.AddElementGeometry(element, new List<IElementGeometry> { geometry });
            return element;
        }
    }
}
