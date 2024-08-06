using Autodesk.DataExchange.Core.Enums;
using Autodesk.DataExchange.DataModels;
using Autodesk.Parameters;
using System;
using System.Linq;
using Autodesk.DataExchange.ConsoleApp.Commands.Options;
using Parameter = Autodesk.Parameters.Parameter;
using System.Threading.Tasks;

namespace Autodesk.DataExchange.ConsoleApp.Helper
{
    internal class ParameterHelper
    {
        public async Task<DataModels.Parameter> AddCustomParameter(ElementDataModel elementDataModel, string parameterName, string schemaNamespace, Element element, string value, ParameterDataType parameterDataType,bool isTypeParameter)
        {
            if (parameterName.Contains(" "))
            {
                Console.WriteLine("Space in Parameter name is not allowed.\n");
                return null;
            }

            if (element.InstanceParameters.FirstOrDefault(n=>n.Name == parameterName) != null ||                
                element.TypeParameters.FirstOrDefault(n => n.Name == parameterName) != null)
            {
                Console.WriteLine("Parameter is already exist.\n");
                return null;
            }

            var instanceParameters = element.InstanceParameters.Count;
            var typeParameters = element.TypeParameters.Count;

            if (parameterDataType == ParameterDataType.String)
            {
                 await AddCustomParameter_String(elementDataModel,parameterName, schemaNamespace, element, value, isTypeParameter);
            }
            else if (parameterDataType == ParameterDataType.Int32)
            {
                await AddCustomParameter_Int32(elementDataModel, parameterName, schemaNamespace, element, value, isTypeParameter);
            }
            else if (parameterDataType == ParameterDataType.Int64)
            {
                await AddCustomParameter_Int64(elementDataModel, parameterName, schemaNamespace, element, value, isTypeParameter);
            }
            else if (parameterDataType == ParameterDataType.Bool)
            {
                await AddCustomParameter_Bool(elementDataModel, parameterName, schemaNamespace, element, value, isTypeParameter);
            }
            else if (parameterDataType == ParameterDataType.Float64)
            {
                await AddCustomParameter_Float64(elementDataModel, parameterName, schemaNamespace, element, value, isTypeParameter);
            }

            var instanceParameters2 = element.InstanceParameters.Count;
            var typeParameters2 = element.TypeParameters.Count;

            // this is workaround until we get DEXC-1855 fix.
            if (instanceParameters != instanceParameters2)
                return element.InstanceParameters.LastOrDefault();
            if (typeParameters != typeParameters2)
                return element.TypeParameters.LastOrDefault();            
            return null;
        }

        public async Task<DataModels.Parameter> AddBuiltInParameter(ElementDataModel elementDataModel, Element element, Autodesk.Parameters.Parameter parameterType, string value, ParameterDataType parameterDataType, bool isTypeParameter)
        {
            var instanceParameters = element.InstanceParameters.Count;
            var typeParameters = element.TypeParameters.Count;
            ParameterDefinition parameter = null;
            if (parameterDataType == ParameterDataType.String)
            {
                parameter = ParameterDefinition.Create(parameterType, ParameterDataType.String);
                ((StringParameterDefinition)parameter).Value = value;
            }
            else if (parameterDataType == ParameterDataType.Float64)
            {
                var data = Convert.ToDouble(value);
                parameter = ParameterDefinition.Create(parameterType, ParameterDataType.Float64);
                ((MeasurableParameterDefinition)parameter).Value = data;
            }
            else if (parameterDataType == ParameterDataType.Int32)
            {
                var data = Convert.ToInt32(value);
                parameter = ParameterDefinition.Create(parameterType, ParameterDataType.Int32);
                ((Int32ParameterDefinition)parameter).Value = data;
            }
            else if (parameterDataType == ParameterDataType.Int64)
            {
                var data = Convert.ToInt64(value);
                parameter = ParameterDefinition.Create(parameterType, ParameterDataType.Int64);
                ((Int64ParameterDefinition)parameter).Value = data;
            }
            else if (parameterDataType == ParameterDataType.Bool)
            {
                var data = Convert.ToBoolean(value);
                parameter = ParameterDefinition.Create(parameterType, ParameterDataType.Bool);
                ((BoolParameterDefinition)parameter).Value = data;
            }

            if (isTypeParameter)
                await elementDataModel.CreateTypeParameterAsync(element.Type, parameter);
            else
                await element.CreateInstanceParameterAsync(parameter);


            var instanceParameters2 = element.InstanceParameters.Count;
            var typeParameters2 = element.TypeParameters.Count;

            // this is workaround until we get DEXC-1855 fix.

            if (instanceParameters != instanceParameters2)
                return element.InstanceParameters.LastOrDefault();
            if (typeParameters != typeParameters2)
                return element.TypeParameters.LastOrDefault();            
            return null;
        }


        private static async Task AddCustomParameter_Float64(ElementDataModel elementDataModel, string parameterName, string schemaNamespace, Element element, string value, bool isTypeParameter)
        {
            var data = Convert.ToDouble(value);
            var schemaId = "exchange.parameter." + schemaNamespace + ":Float64" + parameterName + "TestCustomParameter-1.0.0";
            ParameterDefinition customParameterFloat = ParameterDefinition.Create(schemaId, ParameterDataType.Float64);
            customParameterFloat.Name = parameterName;
            customParameterFloat.SampleText = "SampleText-FloatCustomParam";
            customParameterFloat.Description = "Description-FloatCustomParam";
            customParameterFloat.ReadOnly = false;            
            customParameterFloat.GroupID = Group.Dimensions.DisplayName();
            customParameterFloat.SpecID = Spec.Number.DisplayName();
            ((MeasurableParameterDefinition)customParameterFloat).Value = data;
            if (isTypeParameter)
            {
                await elementDataModel.CreateTypeParameterAsync(element.Type, customParameterFloat);
            }
            else
            {
                await element.CreateInstanceParameterAsync(customParameterFloat);
            }
        }

        private static async Task AddCustomParameter_Bool(ElementDataModel elementDataModel, string parameterName, string schemaNamespace, Element element, string value, bool isTypeParameter)
        {
            var data = Convert.ToBoolean(value);
            string schemaId = "exchange.parameter." + schemaNamespace + ":Bool" + parameterName + "TestCustomParameter-1.0.0";
            ParameterDefinition customParameter = ParameterDefinition.Create(schemaId, ParameterDataType.Bool);
            customParameter.Name = parameterName;
            customParameter.SampleText = "";
            customParameter.Description = "";            
            customParameter.ReadOnly = false;
            customParameter.GroupID = Group.General.DisplayName();
            ((BoolParameterDefinition)customParameter).Value = data;
            if (isTypeParameter)
            {
                await elementDataModel.CreateTypeParameterAsync(element.Type, customParameter);
            }
            else
            {
                await element.CreateInstanceParameterAsync(customParameter);
            }
        }

        private static async Task AddCustomParameter_Int64(ElementDataModel elementDataModel, string parameterName, string schemaNamespace, Element element, string value, bool isTypeParameter)
        {
            var data = Convert.ToInt64(value);
            var schemaId = "exchange.parameter." + schemaNamespace + ":Int64T" + parameterName + "estCustomParameter-1.0.0";
            ParameterDefinition customParameterInt = ParameterDefinition.Create(schemaId, ParameterDataType.Int64);
            customParameterInt.Name = parameterName;
            customParameterInt.SampleText = "";
            customParameterInt.Description = "";            
            customParameterInt.ReadOnly = false;
            customParameterInt.GroupID = Group.General.DisplayName();
            ((Int64ParameterDefinition)customParameterInt).Value = data;
            if (isTypeParameter)
            {
                await elementDataModel.CreateTypeParameterAsync(element.Type, customParameterInt);
            }
            else
            {
                await element.CreateInstanceParameterAsync(customParameterInt);
            }
        }

        private static async Task AddCustomParameter_String(ElementDataModel elementDataModel, string parameterName, string schemaNamespace, Element element, string value, bool isTypeParameter)
        {
            var schemaId = "exchange.parameter." + schemaNamespace + ":String"+ parameterName + "TestCustomParameter-1.0.0";
            ParameterDefinition customParameterString = ParameterDefinition.Create(schemaId, ParameterDataType.String);
            customParameterString.Name = parameterName;
            customParameterString.SampleText = "SampleTest-String";
            customParameterString.Description = "Description-String";
            customParameterString.ReadOnly = false;            
            customParameterString.GroupID = Group.Graphics.DisplayName();
            ((StringParameterDefinition)customParameterString).Value = value;
            if (isTypeParameter)
            {
                await elementDataModel.CreateTypeParameterAsync(element.Type, customParameterString);
            }
            else
            {
                await element.CreateInstanceParameterAsync(customParameterString);
            }
        }

        private async Task AddCustomParameter_Int32(ElementDataModel elementDataModel, string parameterName, string schemaNamespace, Element element, string value, bool isTypeParameter)
        {
            var data = Convert.ToInt32(value);
            var schemaId = "exchange.parameter." + schemaNamespace + ":Int32"+parameterName+"TestCustomParameter-1.0.0";
            ParameterDefinition customParameterInt = ParameterDefinition.Create(schemaId, ParameterDataType.Int32);
            customParameterInt.Name = parameterName;
            customParameterInt.SampleText = "";
            customParameterInt.Description = "";
            customParameterInt.ReadOnly = false;
            customParameterInt.GroupID = Group.General.DisplayName();
            ((Int32ParameterDefinition)customParameterInt).Value = data;
            if (isTypeParameter)
            {
                await elementDataModel.CreateTypeParameterAsync(element.Type, customParameterInt);
            }
            else
            {
                await element.CreateInstanceParameterAsync(customParameterInt);
            }
        }
    }
}
