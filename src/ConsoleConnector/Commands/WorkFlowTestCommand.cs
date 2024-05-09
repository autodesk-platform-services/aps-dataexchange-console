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
            var exchangeTitle = DateTime.Now.ToString("d")+"-"+ random.Next(1, 10000);
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

            Console.WriteLine("Adding Primitives Line");
            createPrimitive = new CreatePrimitiveGeometryCommand(ConsoleAppHelper);
            createPrimitive.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            createPrimitive.GetOption<PrimitiveGeometry>().SetValue("Line");

            Console.WriteLine("Adding Primitives Point");
            createPrimitive = new CreatePrimitiveGeometryCommand(ConsoleAppHelper);
            createPrimitive.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            createPrimitive.GetOption<PrimitiveGeometry>().SetValue("Point");

            Console.WriteLine("Adding Primitives Circle");
            createPrimitive = new CreatePrimitiveGeometryCommand(ConsoleAppHelper);
            createPrimitive.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            createPrimitive.GetOption<PrimitiveGeometry>().SetValue("Circle");

            Console.WriteLine("Adding Primitives Polyline");
            createPrimitive = new CreatePrimitiveGeometryCommand(ConsoleAppHelper);
            createPrimitive.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            createPrimitive.GetOption<PrimitiveGeometry>().SetValue("Polyline");

            var addInstanceParameter = new AddInstanceParamCommand(ConsoleAppHelper);
            addInstanceParameter.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            addInstanceParameter.GetOption<ElementId>().SetValue(addBrep.CommandOutput["ElementId"].ToString());
            addInstanceParameter.GetOption<ParameterName>().SetValue(Autodesk.Parameters.Parameter.AllModelDescription.ToString());
            addInstanceParameter.GetOption<ParameterValue>().SetValue("Testing AllModelDescription");
            addInstanceParameter.GetOption<ParameterValueDataType>().SetValue("String");
            await addInstanceParameter.Execute();

            var addInstanceParameter2 = new AddInstanceParamCommand(ConsoleAppHelper);
            addInstanceParameter2.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            addInstanceParameter2.GetOption<ElementId>().SetValue(addBrep2.CommandOutput["ElementId"].ToString());
            addInstanceParameter2.GetOption<ParameterName>().SetValue("CustomParameter");
            addInstanceParameter2.GetOption<ParameterValue>().SetValue("Testing CustomParameter");
            addInstanceParameter2.GetOption<ParameterValueDataType>().SetValue("String");
            await addInstanceParameter2.Execute();

            var addTypeParamCommand = new AddTypeParamCommand(ConsoleAppHelper);
            addTypeParamCommand.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            addTypeParamCommand.GetOption<ElementId>().SetValue(addBrep.CommandOutput["ElementId"].ToString());
            addTypeParamCommand.GetOption<ParameterName>().SetValue(Autodesk.Parameters.Parameter.AllModelModel.ToString());
            addTypeParamCommand.GetOption<ParameterValue>().SetValue("Testing AllModelModel");
            addTypeParamCommand.GetOption<ParameterValueDataType>().SetValue("String");
            await addTypeParamCommand.Execute();

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
            
            return true;
        }

        public override Command Clone()
        {
            return new WorkFlowTestCommand(this);
        }
    }
}
