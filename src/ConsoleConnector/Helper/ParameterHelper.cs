using Autodesk.DataExchange.Core.Enums;
using Autodesk.DataExchange.DataModels;
using Autodesk.Parameters;
using System;
using System.Linq;
using Autodesk.DataExchange.ConsoleApp.Commands.Options;
//using Parameter = Autodesk.Parameters.Parameter;
using System.Threading.Tasks;
using Autodesk.DataExchange.Interface;
using Google.Protobuf.WellKnownTypes;
using Microsoft.IdentityModel.Tokens;

namespace Autodesk.DataExchange.ConsoleApp.Helper
{
    internal class ParameterHelper
    {
        public async Task<IParameter> AddCustomParameter(ElementDataModel elementDataModel, string parameterName, string parameterValue, Element element, ParameterDataTypeEnum parameterDataType, bool isTypeParameter)
        {
            if (parameterName.Contains(" "))
            {
                Console.WriteLine("[ERROR] Spaces in parameter names are not allowed\n");
                return null;
            }

            if (element.InstanceParameters.FirstOrDefault(n=>n.Name == parameterName) != null ||                
                element.TypeParameters.FirstOrDefault(n => n.Name == parameterName) != null)
            {
                Console.WriteLine("[WARNING] Parameter already exists\n");
                return null;
            }

            var instanceParameters = element.InstanceParameters.Count();
            var typeParameters = element.TypeParameters.Count();

            ParameterDataType ParameterDataType = null;
            if (parameterDataType == ParameterDataTypeEnum.Int32)
                ParameterDataType = new ParameterDataType(int.Parse(parameterValue));
            else if (parameterDataType == ParameterDataTypeEnum.Int64)
                ParameterDataType = new ParameterDataType(long.Parse(parameterValue));
            else if (parameterDataType == ParameterDataTypeEnum.Float64)
                ParameterDataType = new ParameterDataType(double.Parse(parameterValue));
            else if (parameterDataType == ParameterDataTypeEnum.Bool)
                ParameterDataType = new ParameterDataType(bool.Parse(parameterValue));
            else
                ParameterDataType = new ParameterDataType(parameterValue);

            var customParameterInt = new Autodesk.DataExchange.DataModels.Parameter(parameterName, parameterValue);
            customParameterInt.Name = parameterName;
            customParameterInt.SampleText = "SampleText";
            customParameterInt.Description = "Description";
            customParameterInt.ReadOnly = false;
            customParameterInt.GroupID = Group.General.DisplayName();
            customParameterInt.IsCustomParameter = true;
            if (isTypeParameter)
            {
                await elementDataModel.CreateTypeParameterAsync(element.Type, customParameterInt);
            }
            else
            {
                await element.CreateInstanceParameterAsync(customParameterInt);
            }

            var instanceParameters2 = element.InstanceParameters.Count();
            var typeParameters2 = element.TypeParameters.Count();

            // this is workaround until we get DEXC-1855 fix.
            if (instanceParameters != instanceParameters2)
                return element.InstanceParameters.LastOrDefault();
            if (typeParameters != typeParameters2)
                return element.TypeParameters.LastOrDefault();            
            return null;
        }

        public async Task<IParameter> AddBuiltInParameter(ElementDataModel elementDataModel,string name, string schemaId, Element element,string parameterValue, ParameterDataTypeEnum parameterDataType, bool isTypeParameter)
        {            
            var instanceParameters = element.InstanceParameters.Count();
            var typeParameters = element.TypeParameters.Count();

            var parameterSchemaId = new ParameterSchemaId(schemaId);
            ParameterDataType ParameterDataType = null;
            if (parameterDataType == ParameterDataTypeEnum.Int32)
                ParameterDataType = new ParameterDataType(int.Parse(parameterValue));
            else if (parameterDataType == ParameterDataTypeEnum.Int64)
                ParameterDataType = new ParameterDataType(long.Parse(parameterValue));
            else if (parameterDataType == ParameterDataTypeEnum.Float64)
                ParameterDataType = new ParameterDataType(double.Parse(parameterValue));
            else if (parameterDataType == ParameterDataTypeEnum.Bool)
                ParameterDataType = new ParameterDataType(bool.Parse(parameterValue));
            else
                ParameterDataType = new ParameterDataType(parameterValue);

            var parameter = new Autodesk.DataExchange.DataModels.Parameter(parameterSchemaId, ParameterDataType);
            parameter.Name = name;

            if (isTypeParameter)
                await elementDataModel.CreateTypeParameterAsync(element.Type, parameter);
            else
                await element.CreateInstanceParameterAsync(parameter);


            var instanceParameters2 = element.InstanceParameters.Count();
            var typeParameters2 = element.TypeParameters.Count();

            // this is workaround until we get DEXC-1855 fix.

            if (instanceParameters != instanceParameters2)
                return element.InstanceParameters.LastOrDefault();
            if (typeParameters != typeParameters2)
                return element.TypeParameters.LastOrDefault();            
            return null;
        }
    }

}
