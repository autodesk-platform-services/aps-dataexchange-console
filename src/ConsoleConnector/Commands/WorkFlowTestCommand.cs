using Autodesk.DataExchange.ConsoleApp.Commands.Options;
using Autodesk.DataExchange.ConsoleApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodesk.DataExchange.ConsoleApp.Commands
{
    internal class WorkFlowTestCommand : Command
    {
        public IConsoleAppHelper ConsoleAppHelper { get; }

        public WorkFlowTestCommand(IConsoleAppHelper consoleAppHelper) : base(consoleAppHelper)
        {
            this.Name = "WorkFlowTest";
            Description = "WorkFlowTest command.";
            ConsoleAppHelper = consoleAppHelper;
        }

        public WorkFlowTestCommand(WorkFlowTestCommand workFlorTestCommand) : base(workFlorTestCommand)
        {
            this.ConsoleAppHelper = workFlorTestCommand.ConsoleAppHelper;
        }

        public override async Task<bool> Execute()
        {
            var random = new Random();
            var exchangeTitle = System.Text.RegularExpressions.Regex.Replace(DateTime.Now.ToString("d") + "-" + random.Next(1, 10000), @"[<>:""/\\|?.*`]", " ");
            Console.WriteLine("Creating exchange : " + exchangeTitle);
            var createExchangeCommand = new CreateExchangeCommand(ConsoleAppHelper);
            createExchangeCommand.GetOption<ExchangeTitle>().SetValue(exchangeTitle);            
            await createExchangeCommand.Execute();        

            Console.WriteLine("Adding Bre 1");
            var addBrep = new CreateBrepCommand(ConsoleAppHelper);
            addBrep.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            await addBrep.Execute();

            Console.WriteLine("Adding Bre 2");
            var addBrep2 = new CreateBrepCommand(ConsoleAppHelper);
            addBrep2.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            await addBrep2.Execute();

            Console.WriteLine("Adding Mesh 1");
            var addMesh = new CreateMeshCommand(ConsoleAppHelper);
            addMesh.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            await addMesh.Execute();

            Console.WriteLine("Adding Mesh 2");
            var addMesh2 = new CreateMeshCommand(ConsoleAppHelper);
            addMesh2.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            await addMesh2.Execute();

            Console.WriteLine("Adding Primitives All");
            var createPrimitive = new CreatePrimitiveGeometryCommand(ConsoleAppHelper);
            createPrimitive.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            createPrimitive.GetOption<PrimitiveGeometry>().SetValue("All");
            await createPrimitive.Execute();

            Console.WriteLine("Adding Primitives Line");
            createPrimitive = new CreatePrimitiveGeometryCommand(ConsoleAppHelper);
            createPrimitive.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            createPrimitive.GetOption<PrimitiveGeometry>().SetValue("Line");
            await createPrimitive.Execute();

            Console.WriteLine("Adding Primitives Point");
            createPrimitive = new CreatePrimitiveGeometryCommand(ConsoleAppHelper);
            createPrimitive.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            createPrimitive.GetOption<PrimitiveGeometry>().SetValue("Point");
            await createPrimitive.Execute();

            Console.WriteLine("Adding Primitives Circle");
            createPrimitive = new CreatePrimitiveGeometryCommand(ConsoleAppHelper);
            createPrimitive.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            createPrimitive.GetOption<PrimitiveGeometry>().SetValue("Circle");
            await createPrimitive.Execute();

            Console.WriteLine("Adding Primitives Polyline");
            createPrimitive = new CreatePrimitiveGeometryCommand(ConsoleAppHelper);
            createPrimitive.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            createPrimitive.GetOption<PrimitiveGeometry>().SetValue("Polyline");
            await createPrimitive.Execute();

            Console.WriteLine("Adding Instance Parameter 1");
            var addInstanceParameter = new AddInstanceParamCommand(ConsoleAppHelper);
            addInstanceParameter.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            addInstanceParameter.GetOption<ElementId>().SetValue(addBrep.CommandOutput["ElementId"].ToString());
            addInstanceParameter.GetOption<ParameterName>().SetValue(Autodesk.Parameters.Parameter.AllModelDescription.ToString());
            addInstanceParameter.GetOption<ParameterValue>().SetValue("Testing AllModelDescription");
            addInstanceParameter.GetOption<ParameterValueDataType>().SetValue("String");
            await addInstanceParameter.Execute();

            Console.WriteLine("Adding Instance Parameter 2");
            var addInstanceParameter2 = new AddInstanceParamCommand(ConsoleAppHelper);
            addInstanceParameter2.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            addInstanceParameter2.GetOption<ElementId>().SetValue(addBrep2.CommandOutput["ElementId"].ToString());
            addInstanceParameter2.GetOption<ParameterName>().SetValue("CustomParameter");
            addInstanceParameter2.GetOption<ParameterValue>().SetValue("Testing CustomParameter");
            addInstanceParameter2.GetOption<ParameterValueDataType>().SetValue("String");
            await addInstanceParameter2.Execute();

            Console.WriteLine("Adding Type Parameter 1");
            var addTypeParamCommand = new AddTypeParamCommand(ConsoleAppHelper);
            addTypeParamCommand.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            addTypeParamCommand.GetOption<ElementId>().SetValue(addBrep.CommandOutput["ElementId"].ToString());
            addTypeParamCommand.GetOption<ParameterName>().SetValue(Autodesk.Parameters.Parameter.AllModelModel.ToString());
            addTypeParamCommand.GetOption<ParameterValue>().SetValue("Testing AllModelModel");
            addTypeParamCommand.GetOption<ParameterValueDataType>().SetValue("String");
            await addTypeParamCommand.Execute();

            Console.WriteLine("Adding Type Parameter 2");
            var addTypeParamCommand2 = new AddTypeParamCommand(ConsoleAppHelper);
            addTypeParamCommand2.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            addTypeParamCommand2.GetOption<ElementId>().SetValue(addBrep2.CommandOutput["ElementId"].ToString());
            addTypeParamCommand2.GetOption<ParameterName>().SetValue("CustomParameter2");
            addTypeParamCommand2.GetOption<ParameterValue>().SetValue("Testing CustomParameter");
            addTypeParamCommand2.GetOption<ParameterValueDataType>().SetValue("String");
            await addTypeParamCommand2.Execute();


            Console.WriteLine("Syncing to V1");
            var syncExchangeData = new SyncExchangeData(ConsoleAppHelper);
            syncExchangeData.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            await syncExchangeData.Execute();

            Console.WriteLine("Syncing to V2");


            Console.WriteLine("Adding Bre 1");
            addBrep = new CreateBrepCommand(ConsoleAppHelper);
            addBrep.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            await addBrep.Execute();

            Console.WriteLine("Adding Bre 2");
            addBrep2 = new CreateBrepCommand(ConsoleAppHelper);
            addBrep2.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            await addBrep2.Execute();

            Console.WriteLine("Adding Mesh 1");
            addMesh = new CreateMeshCommand(ConsoleAppHelper);
            addMesh.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            await addMesh.Execute();

            Console.WriteLine("Adding Mesh 2");
            addMesh2 = new CreateMeshCommand(ConsoleAppHelper);
            addMesh2.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            await addMesh2.Execute();

            Console.WriteLine("Adding Primitives All");
            createPrimitive = new CreatePrimitiveGeometryCommand(ConsoleAppHelper);
            createPrimitive.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            createPrimitive.GetOption<PrimitiveGeometry>().SetValue("All");
            await createPrimitive.Execute();

            Console.WriteLine("Adding Primitives Line");
            createPrimitive = new CreatePrimitiveGeometryCommand(ConsoleAppHelper);
            createPrimitive.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            createPrimitive.GetOption<PrimitiveGeometry>().SetValue("Line");
            await createPrimitive.Execute();

            Console.WriteLine("Adding Primitives Point");
            createPrimitive = new CreatePrimitiveGeometryCommand(ConsoleAppHelper);
            createPrimitive.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            createPrimitive.GetOption<PrimitiveGeometry>().SetValue("Point");
            await createPrimitive.Execute();

            Console.WriteLine("Adding Primitives Circle");
            createPrimitive = new CreatePrimitiveGeometryCommand(ConsoleAppHelper);
            createPrimitive.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            createPrimitive.GetOption<PrimitiveGeometry>().SetValue("Circle");
            await createPrimitive.Execute();

            Console.WriteLine("Adding Instance Parameter 1");
            addInstanceParameter = new AddInstanceParamCommand(ConsoleAppHelper);
            addInstanceParameter.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            addInstanceParameter.GetOption<ElementId>().SetValue(addBrep.CommandOutput["ElementId"].ToString());
            addInstanceParameter.GetOption<ParameterName>().SetValue(Autodesk.Parameters.Parameter.AllModelDescription.ToString());
            addInstanceParameter.GetOption<ParameterValue>().SetValue("Testing AllModelDescription");
            addInstanceParameter.GetOption<ParameterValueDataType>().SetValue("String");
            await addInstanceParameter.Execute();

            Console.WriteLine("Adding Instance Parameter 2");
            addInstanceParameter2 = new AddInstanceParamCommand(ConsoleAppHelper);
            addInstanceParameter2.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            addInstanceParameter2.GetOption<ElementId>().SetValue(addBrep2.CommandOutput["ElementId"].ToString());
            addInstanceParameter2.GetOption<ParameterName>().SetValue("CustomParameter");
            addInstanceParameter2.GetOption<ParameterValue>().SetValue("Testing CustomParameter");
            addInstanceParameter2.GetOption<ParameterValueDataType>().SetValue("String");
            await addInstanceParameter2.Execute();

            Console.WriteLine("Adding Type Parameter 1");
            addTypeParamCommand = new AddTypeParamCommand(ConsoleAppHelper);
            addTypeParamCommand.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            addTypeParamCommand.GetOption<ElementId>().SetValue(addBrep.CommandOutput["ElementId"].ToString());
            addTypeParamCommand.GetOption<ParameterName>().SetValue(Autodesk.Parameters.Parameter.AllModelModel.ToString());
            addTypeParamCommand.GetOption<ParameterValue>().SetValue("Testing AllModelModel");
            addTypeParamCommand.GetOption<ParameterValueDataType>().SetValue("String");
            await addTypeParamCommand.Execute();

            Console.WriteLine("Adding Type Parameter 2");
            addTypeParamCommand2 = new AddTypeParamCommand(ConsoleAppHelper);
            addTypeParamCommand2.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            addTypeParamCommand2.GetOption<ElementId>().SetValue(addBrep2.CommandOutput["ElementId"].ToString());
            addTypeParamCommand2.GetOption<ParameterName>().SetValue("CustomParameter2");
            addTypeParamCommand2.GetOption<ParameterValue>().SetValue("Testing CustomParameter");
            addTypeParamCommand2.GetOption<ParameterValueDataType>().SetValue("String");
            await addTypeParamCommand2.Execute();

            var syncExchangeData2 = new SyncExchangeData(ConsoleAppHelper);
            syncExchangeData2.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            await syncExchangeData2.Execute();
            
            return true;
        }

        public override Command Clone()
        {
            return new WorkFlowTestCommand(this);
        }
    }
}
