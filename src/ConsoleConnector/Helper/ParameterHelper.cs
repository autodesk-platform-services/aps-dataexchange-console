using Autodesk.DataExchange.Core.Enums;
using Autodesk.DataExchange.DataModels;
using Autodesk.Parameters;
using System;
using System.Linq;
using Autodesk.DataExchange.ConsoleApp.Commands.Options;
using Parameter = Autodesk.Parameters.Parameter;

namespace Autodesk.DataExchange.ConsoleApp.Helper
{
    internal class ParameterHelper
    {
        public DataModels.Parameter AddCustomParameter(string parameterName, string schemaNamespace, Element element, string value, ParameterDataType parameterDataType,bool isTypeParameter)
        {
            if(parameterName.Contains(" "))
            {
                Console.WriteLine("Space in Parameter name is not allowed.\n");
                return null;
            }

            if(element.InstanceParameters.FirstOrDefault(n=>n.Name == parameterName) != null ||
                element.ModelStructureParameters.FirstOrDefault(n => n.Name == parameterName) != null ||
                element.TypeParameters.FirstOrDefault(n => n.Name == parameterName) != null)
            {
                Console.WriteLine("Parameter is already exist.\n");
                return null;
            }

            var instanceParameters = element.InstanceParameters.Count;
            var modelStructureParameters = element.ModelStructureParameters.Count;
            var typeParameters = element.TypeParameters.Count;

            if (parameterDataType == ParameterDataType.String)
            {
                AddCustomParameter_String(parameterName, schemaNamespace, element, value, isTypeParameter);
            }
            else if (parameterDataType == ParameterDataType.Int32)
            {
                AddCustomParameter_Int32(parameterName, schemaNamespace, element, value, isTypeParameter);
            }
            else if (parameterDataType == ParameterDataType.Int64)
            {
                AddCustomParameter_Int64(parameterName, schemaNamespace, element, value, isTypeParameter);
            }
            else if (parameterDataType == ParameterDataType.Bool)
            {
                AddCustomParameter_Bool(parameterName, schemaNamespace, element, value, isTypeParameter);
            }
            else if (parameterDataType == ParameterDataType.Float64)
            {
                AddCustomParameter_Float64(parameterName,schemaNamespace, element, value, isTypeParameter);
            }

            var instanceParameters2 = element.InstanceParameters.Count;
            var modelStructureParameters2 = element.ModelStructureParameters.Count;
            var typeParameters2 = element.TypeParameters.Count;

            // this is workaround until we get DEXC-1855 fix.
            if (instanceParameters != instanceParameters2)
                return element.InstanceParameters.LastOrDefault();
            if (typeParameters != typeParameters2)
                return element.TypeParameters.LastOrDefault();
            if (modelStructureParameters != modelStructureParameters2)
                return element.ModelStructureParameters.LastOrDefault();
            return null;
        }

        public DataModels.Parameter AddBuiltInParameter(Element element, Autodesk.Parameters.Parameter parameterType, string value, ParameterDataType parameterDataType, bool isTypeParameter)
        {
            var instanceParameters = element.InstanceParameters.Count;
            var modelStructureParameters = element.ModelStructureParameters.Count;
            var typeParameters = element.TypeParameters.Count;

            if (parameterDataType == ParameterDataType.String)
            {
                AddBuiltInParameter_String(element, parameterType, value, isTypeParameter);
            }
            else if (parameterDataType == ParameterDataType.Float64)
            {
                AddBuiltInParameter_Float64(element, parameterType, value, isTypeParameter);
            }
            else if (parameterDataType == ParameterDataType.Int32)
            {
                AddIBuiltInParameter_Int32(element, parameterType, value, isTypeParameter);
            }
            else if (parameterDataType == ParameterDataType.Int64)
            {
                AddBuiltInParameter_Int64(element, parameterType, value, isTypeParameter);
            }
            else if (parameterDataType == ParameterDataType.Bool)
            {
                AddBuiltInParameter_Bool(element, parameterType, value, isTypeParameter);
            }

            var instanceParameters2 = element.InstanceParameters.Count;
            var modelStructureParameters2 = element.ModelStructureParameters.Count;
            var typeParameters2 = element.TypeParameters.Count;

            // this is workaround until we get DEXC-1855 fix.

            if (instanceParameters != instanceParameters2)
                return element.InstanceParameters.LastOrDefault();
            if (typeParameters != typeParameters2)
                return element.TypeParameters.LastOrDefault();
            if (modelStructureParameters != modelStructureParameters2)
                return element.ModelStructureParameters.LastOrDefault();
            return null;
        }


        private static void AddCustomParameter_Float64(string parameterName, string schemaNamespace, Element element, string value, bool isTypeParameter)
        {
            var data = Convert.ToDouble(value);
            var schemaId = "exchange.parameter." + schemaNamespace + ":Float64" + parameterName + "TestCustomParameter-1.0.0";
            ParameterDefinition customParameterFloat = ParameterDefinition.Create(schemaId, ParameterDataType.Float64);
            customParameterFloat.Name = parameterName;
            customParameterFloat.SampleText = "SampleText-FloatCustomParam";
            customParameterFloat.Description = "Description-FloatCustomParam";
            customParameterFloat.ReadOnly = false;
            customParameterFloat.IsTypeParameter = isTypeParameter;
            customParameterFloat.GroupID = Group.Dimensions.DisplayName();
            customParameterFloat.SpecID = Spec.Number.DisplayName();
            ((MeasurableParameterDefinition)customParameterFloat).Value = data;
            element.CreateParameter(customParameterFloat);
        }

        private static void AddCustomParameter_Bool(string parameterName, string schemaNamespace, Element element, string value, bool isTypeParameter)
        {
            var data = Convert.ToBoolean(value);
            string schemaId = "exchange.parameter." + schemaNamespace + ":Bool" + parameterName + "TestCustomParameter-1.0.0";
            ParameterDefinition customParameter = ParameterDefinition.Create(schemaId, ParameterDataType.Bool);
            customParameter.Name = parameterName;
            customParameter.SampleText = "";
            customParameter.Description = "";
            customParameter.IsTypeParameter = isTypeParameter;
            customParameter.ReadOnly = false;
            customParameter.GroupID = Group.General.DisplayName();
            ((BoolParameterDefinition)customParameter).Value = data;
            element.CreateParameter(customParameter);
        }

        private static void AddCustomParameter_Int64(string parameterName, string schemaNamespace, Element element, string value, bool isTypeParameter)
        {
            var data = Convert.ToInt64(value);
            var schemaId = "exchange.parameter." + schemaNamespace + ":Int64T" + parameterName + "estCustomParameter-1.0.0";
            ParameterDefinition customParameterInt = ParameterDefinition.Create(schemaId, ParameterDataType.Int64);
            customParameterInt.Name = parameterName;
            customParameterInt.SampleText = "";
            customParameterInt.Description = "";
            customParameterInt.IsTypeParameter = isTypeParameter;
            customParameterInt.ReadOnly = false;
            customParameterInt.GroupID = Group.General.DisplayName();
            ((Int64ParameterDefinition)customParameterInt).Value = data;
            element.CreateParameter(customParameterInt);
        }

        private static void AddCustomParameter_String(string parameterName, string schemaNamespace, Element element, string value, bool isTypeParameter)
        {
            var schemaId = "exchange.parameter." + schemaNamespace + ":String"+ parameterName + "TestCustomParameter-1.0.0";
            ParameterDefinition customParameterString = ParameterDefinition.Create(schemaId, ParameterDataType.String);
            customParameterString.Name = parameterName;
            customParameterString.SampleText = "SampleTest-String";
            customParameterString.Description = "Description-String";
            customParameterString.ReadOnly = false;
            customParameterString.IsTypeParameter = isTypeParameter;
            customParameterString.GroupID = Group.Graphics.DisplayName();
            ((StringParameterDefinition)customParameterString).Value = value;
            element.CreateParameter(customParameterString);
        }

        private void AddCustomParameter_Int32(string parameterName, string schemaNamespace, Element element, string value, bool isTypeParameter)
        {
            var data = Convert.ToInt32(value);
            var schemaId = "exchange.parameter." + schemaNamespace + ":Int32"+parameterName+"TestCustomParameter-1.0.0";
            ParameterDefinition customParameterInt = ParameterDefinition.Create(schemaId, ParameterDataType.Int32);
            customParameterInt.Name = parameterName;
            customParameterInt.SampleText = "";
            customParameterInt.Description = "";
            customParameterInt.IsTypeParameter = isTypeParameter;
            customParameterInt.ReadOnly = false;
            customParameterInt.GroupID = Group.General.DisplayName();
            ((Int32ParameterDefinition)customParameterInt).Value = data;
            element.CreateParameter(customParameterInt);
        }

        private static void AddBuiltInParameter_Bool(Element element, Parameter parameterType, string value, bool isTypeParameter)
        {
            var data = Convert.ToBoolean(value);
            var parameter = ParameterDefinition.Create(parameterType, ParameterDataType.Bool);
            parameter.IsTypeParameter = isTypeParameter;
            ((BoolParameterDefinition)parameter).Value = data;
            element.CreateParameter(parameter);
        }

        private static void AddBuiltInParameter_Int64(Element element, Parameter parameterType, string value, bool isTypeParameter)
        {
            var data = Convert.ToInt64(value);
            var parameter = ParameterDefinition.Create(parameterType, ParameterDataType.Int64);
            parameter.IsTypeParameter = isTypeParameter;
            ((Int64ParameterDefinition)parameter).Value = data;
            element.CreateParameter(parameter);
        }

        private static void AddIBuiltInParameter_Int32(Element element, Parameter parameterType, string value, bool isTypeParameter)
        {
            var data = Convert.ToInt32(value);
            var parameter = ParameterDefinition.Create(parameterType, ParameterDataType.Int32);
            parameter.IsTypeParameter = isTypeParameter;
            ((Int32ParameterDefinition)parameter).Value = data;
            element.CreateParameter(parameter);
        }

        private static void AddBuiltInParameter_Float64(Element element, Parameter parameterType, string value, bool isTypeParameter)
        {
            var data = Convert.ToDouble(value);
            var parameter = ParameterDefinition.Create(parameterType, ParameterDataType.Float64);
            parameter.IsTypeParameter = isTypeParameter;
            ((MeasurableParameterDefinition)parameter).Value = data;
            element.CreateParameter(parameter);
        }

        private static void AddBuiltInParameter_String(Element element, Parameter parameterType, string value,bool isTypeParameter)
        {
            var parameter = ParameterDefinition.Create(parameterType, ParameterDataType.String);
            parameter.IsTypeParameter = isTypeParameter;
            ((StringParameterDefinition)parameter).Value = value;
            element.CreateParameter(parameter);
        }
    }
}
