using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Autodesk.DataExchange.Core.Enums;
using Autodesk.DataExchange.Models.Revit;
using Autodesk.GeometryPrimitives.Design;
using Autodesk.GeometryPrimitives.Geometry;
using Autodesk.Parameters;
using Parameter = Autodesk.Parameters.Parameter;
using PrimitiveGeometry = Autodesk.GeometryPrimitives;

namespace Autodesk.DataExchange.ConsoleApp.Helper
{
    internal class ParameterHelper
    {
        public void AddCustomParameter(string schemaNamespace, Element element, string value, ParameterDataType parameterDataType)
        {
            if (parameterDataType == ParameterDataType.String)
            {
                AddCustomParameter_String(schemaNamespace, element, value);
            }
            else if (parameterDataType == ParameterDataType.Int32)
            {
                AddCustomParameter_Int32(schemaNamespace, element, value);
            }
            else if (parameterDataType == ParameterDataType.Int64)
            {
                AddCustomParameter_Int64(schemaNamespace, element, value);
            }
            else if (parameterDataType == ParameterDataType.Bool)
            {
                AddCustomParameter_Bool(schemaNamespace, element, value);
            }
            else if (parameterDataType == ParameterDataType.Float64)
            {
                AddCustomParameter_Float64(schemaNamespace, element, value);
            }
        }

        public void AddInstanceParameter(Element element, Autodesk.Parameters.Parameter parameterType, string value, ParameterDataType parameterDataType)
        {
            if (parameterDataType == ParameterDataType.String)
            {
                AddInstanceParameter_String(element, parameterType, value);
            }
            else if (parameterDataType == ParameterDataType.Float64)
            {
                AddInstanceParameter_Float64(element, parameterType, value);
            }
            else if (parameterDataType == ParameterDataType.Int32)
            {
                AddInstanceParameter_Int32(element, parameterType, value);
            }
            else if (parameterDataType == ParameterDataType.Int64)
            {
                AddInstanceParameter_Int64(element, parameterType, value);
            }
            else if (parameterDataType == ParameterDataType.Bool)
            {
                AddInstanceParameter_Bool(element, parameterType, value);
            }
        }


        private static void AddCustomParameter_Float64(string schemaNamespace, Element element, string value)
        {
            var data = Convert.ToDouble(value);
            var schemaId = "exchange.parameter." + schemaNamespace + ":Float64TestCustomParameter-1.0.0";
            ParameterDefinition customParameterFloat = ParameterDefinition.Create(schemaId, ParameterDataType.Float64);
            customParameterFloat.Name = "TestFloat64" + Guid.NewGuid();
            customParameterFloat.SampleText = "SampleText-FloatCustomParam";
            customParameterFloat.Description = "Description-FloatCustomParam";
            customParameterFloat.ReadOnly = false;
            customParameterFloat.GroupID = Group.Dimensions.DisplayName();
            customParameterFloat.SpecID = Spec.Number.DisplayName();
            ((MeasurableParameterDefinition)customParameterFloat).Value = data;
            element.CreateParameter(customParameterFloat);
        }

        private static void AddCustomParameter_Bool(string schemaNamespace, Element element, string value)
        {
            var data = Convert.ToBoolean(value);
            string schemaId = "exchange.parameter." + schemaNamespace + ":BoolTestCustomParameter-1.0.0";
            ParameterDefinition customParameter = ParameterDefinition.Create(schemaId, ParameterDataType.Bool);
            customParameter.Name = "Test" + Guid.NewGuid();
            customParameter.SampleText = "";
            customParameter.Description = "";
            customParameter.ReadOnly = false;
            customParameter.GroupID = Group.General.DisplayName();
            ((BoolParameterDefinition)customParameter).Value = data;
            element.CreateParameter(customParameter);
        }

        private static void AddCustomParameter_Int64(string schemaNamespace, Element element, string value)
        {
            var data = Convert.ToInt64(value);
            var schemaId = "exchange.parameter." + schemaNamespace + ":Int64TestCustomParameter-1.0.0";
            ParameterDefinition customParameterInt = ParameterDefinition.Create(schemaId, ParameterDataType.Int64);
            customParameterInt.Name = "TestInt64" + Guid.NewGuid();
            customParameterInt.SampleText = "";
            customParameterInt.Description = "";
            customParameterInt.ReadOnly = false;
            customParameterInt.GroupID = Group.General.DisplayName();
            ((Int64ParameterDefinition)customParameterInt).Value = data;
            element.CreateParameter(customParameterInt);
        }

        private static void AddCustomParameter_String(string schemaNamespace, Element element, string value)
        {
            var schemaId = "exchange.parameter." + schemaNamespace + ":StringTestCustomParameter-1.0.0";
            ParameterDefinition customParameterString = ParameterDefinition.Create(schemaId, ParameterDataType.String);
            customParameterString.Name = "TestString" + Guid.NewGuid();
            customParameterString.SampleText = "SampleTest-String";
            customParameterString.Description = "Description-String";
            customParameterString.ReadOnly = false;
            customParameterString.GroupID = Group.Graphics.DisplayName();
            ((StringParameterDefinition)customParameterString).Value = value;
            element.CreateParameter(customParameterString);
        }

        public void AddCustomParameter_Int32(string schemaNamespace, Element element, string value)
        {
            var data = Convert.ToInt32(value);
            var schemaId = "exchange.parameter." + schemaNamespace + ":Int32TestCustomParameter-1.0.0";
            ParameterDefinition customParameterInt = ParameterDefinition.Create(schemaId, ParameterDataType.Int32);
            customParameterInt.Name = "TestInt32" + Guid.NewGuid();
            customParameterInt.SampleText = "";
            customParameterInt.Description = "";
            customParameterInt.ReadOnly = false;
            customParameterInt.GroupID = Group.General.DisplayName();
            ((Int32ParameterDefinition)customParameterInt).Value = data;
            element.CreateParameter(customParameterInt);
        }

        private static void AddInstanceParameter_Bool(Element element, Parameter parameterType, string value)
        {
            var data = Convert.ToBoolean(value);
            var parameter = ParameterDefinition.Create(parameterType, ParameterDataType.Bool);
            ((BoolParameterDefinition)parameter).Value = data;
            element.CreateParameter(parameter);
        }

        private static void AddInstanceParameter_Int64(Element element, Parameter parameterType, string value)
        {
            var data = Convert.ToInt64(value);
            var parameter = ParameterDefinition.Create(parameterType, ParameterDataType.Int64);
            ((Int64ParameterDefinition)parameter).Value = data;
            element.CreateParameter(parameter);
        }

        private static void AddInstanceParameter_Int32(Element element, Parameter parameterType, string value)
        {
            var data = Convert.ToInt32(value);
            var hostAreaComputed = ParameterDefinition.Create(parameterType, ParameterDataType.Int32);
            ((Int32ParameterDefinition)hostAreaComputed).Value = data;
            element.CreateParameter(hostAreaComputed);
        }

        private static void AddInstanceParameter_Float64(Element element, Parameter parameterType, string value)
        {
            var data = Convert.ToDouble(value);
            var parameter = ParameterDefinition.Create(parameterType, ParameterDataType.Float64);
            ((MeasurableParameterDefinition)parameter).Value = data;
            element.CreateParameter(parameter);
        }

        private static void AddInstanceParameter_String(Element element, Parameter parameterType, string value)
        {
            var parameter = ParameterDefinition.Create(parameterType, ParameterDataType.String);
            ((StringParameterDefinition)parameter).Value = value;
            element.CreateParameter(parameter);
        }
    }
}
