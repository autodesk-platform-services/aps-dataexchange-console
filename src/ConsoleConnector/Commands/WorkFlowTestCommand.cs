using Autodesk.DataExchange.ConsoleApp.Commands.Options;
using Autodesk.DataExchange.ConsoleApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
            var randomSuffix = new Random().Next(100, 999);
            var exchangeTitle = $"WorkflowTest_{timestamp}_{randomSuffix}";
            Console.WriteLine($"[CREATING] Exchange: {exchangeTitle}");
            var createExchangeCommand = new CreateExchangeCommand(ConsoleAppHelper);
            createExchangeCommand.GetOption<ExchangeTitle>().SetValue(exchangeTitle);            
            await createExchangeCommand.Execute();        

            Console.WriteLine("[GEOMETRY] Adding BREP geometry #1");
            var addBrep = new CreateBrepCommand(ConsoleAppHelper);
            addBrep.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            await addBrep.Execute();

            Console.WriteLine("[GEOMETRY] Adding BREP geometry #2");
            var addBrep2 = new CreateBrepCommand(ConsoleAppHelper);
            addBrep2.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            await addBrep2.Execute();

            Console.WriteLine("[GEOMETRY] Adding IFC geometry #1");
            var addIFC = new CreateIfcCommand(ConsoleAppHelper);
            addIFC.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            await addIFC.Execute();

            Console.WriteLine("[GEOMETRY] Adding IFC geometry #2");
            addIFC = new CreateIfcCommand(ConsoleAppHelper);
            addIFC.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            await addIFC.Execute();

            Console.WriteLine("[GEOMETRY] Adding mesh geometry #1");
            var addMesh = new CreateMeshCommand(ConsoleAppHelper);
            addMesh.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            await addMesh.Execute();

            Console.WriteLine("[GEOMETRY] Adding mesh geometry #2");
            var addMesh2 = new CreateMeshCommand(ConsoleAppHelper);
            addMesh2.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            await addMesh2.Execute();

            Console.WriteLine("[PRIMITIVE] Adding primitive geometries (All)");
            var createPrimitive = new CreatePrimitiveGeometryCommand(ConsoleAppHelper);
            createPrimitive.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            createPrimitive.GetOption<PrimitiveGeometry>().SetValue("All");
            await createPrimitive.Execute();

            Console.WriteLine("[PRIMITIVE] Adding primitive geometry (Line)");
            createPrimitive = new CreatePrimitiveGeometryCommand(ConsoleAppHelper);
            createPrimitive.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            createPrimitive.GetOption<PrimitiveGeometry>().SetValue("Line");
            await createPrimitive.Execute();

            Console.WriteLine("[PRIMITIVE] Adding primitive geometry (Point)");
            createPrimitive = new CreatePrimitiveGeometryCommand(ConsoleAppHelper);
            createPrimitive.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            createPrimitive.GetOption<PrimitiveGeometry>().SetValue("Point");
            await createPrimitive.Execute();

            Console.WriteLine("[PRIMITIVE] Adding primitive geometry (Circle)");
            createPrimitive = new CreatePrimitiveGeometryCommand(ConsoleAppHelper);
            createPrimitive.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            createPrimitive.GetOption<PrimitiveGeometry>().SetValue("Circle");
            await createPrimitive.Execute();

            Console.WriteLine("[PRIMITIVE] Adding primitive geometry (Polyline)");
            createPrimitive = new CreatePrimitiveGeometryCommand(ConsoleAppHelper);
            createPrimitive.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            createPrimitive.GetOption<PrimitiveGeometry>().SetValue("Polyline");
            await createPrimitive.Execute();

            Console.WriteLine("[PARAMETER] Adding instance parameter (AllModelDescription)");
            var addInstanceParameter = new AddInstanceParamCommand(ConsoleAppHelper);
            addInstanceParameter.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            addInstanceParameter.GetOption<ElementId>().SetValue(addBrep.CommandOutput["ElementId"].ToString());
            addInstanceParameter.GetOption<ParameterName>().SetValue("allModelDescription");
            addInstanceParameter.GetOption<ParameterSchema>().SetValue("autodesk.revit.parameter:allModelDescription-1.0.0");
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

            Console.WriteLine("[PARAMETER] Adding type parameter (AllModelModel)");
            var addTypeParamCommand = new AddTypeParamCommand(ConsoleAppHelper);
            addTypeParamCommand.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            addTypeParamCommand.GetOption<ElementId>().SetValue(addBrep.CommandOutput["ElementId"].ToString());
            addTypeParamCommand.GetOption<ParameterName>().SetValue("allModelModel");
            addTypeParamCommand.GetOption<ParameterSchema>().SetValue("autodesk.revit.parameter:allModelModel-1.0.0");
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


            Console.WriteLine("[SYNC] Synchronizing exchange data to Version 1");
            var syncExchangeData = new SyncExchangeData(ConsoleAppHelper);
            syncExchangeData.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            await syncExchangeData.Execute();

            Console.WriteLine("[DELETE] Deleting instance parameter (AllModelDescription)");
            var deleteInstanceParameter = new DeleteInstanceParameter(ConsoleAppHelper);
            deleteInstanceParameter.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            deleteInstanceParameter.GetOption<ElementId>().SetValue(addBrep.CommandOutput["ElementId"].ToString());
            deleteInstanceParameter.GetOption<ParameterName>().SetValue("allModelDescription");
            await deleteInstanceParameter.Execute();

            Console.WriteLine("Delete Type Parameter 1");
            var deleteTypeParameter = new DeleteTypeParameter(ConsoleAppHelper);
            deleteTypeParameter.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            deleteTypeParameter.GetOption<ElementId>().SetValue(addBrep2.CommandOutput["ElementId"].ToString());
            deleteTypeParameter.GetOption<ParameterName>().SetValue("CustomParameter2");
            await deleteTypeParameter.Execute();

            Console.WriteLine("[VERSION 2] Preparing Version 2 updates");


            Console.WriteLine("[GEOMETRY] Adding BREP geometry #3 (V2)");
            addBrep = new CreateBrepCommand(ConsoleAppHelper);
            addBrep.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            await addBrep.Execute();

            Console.WriteLine("[GEOMETRY] Adding BREP geometry #4 (V2)");
            addBrep2 = new CreateBrepCommand(ConsoleAppHelper);
            addBrep2.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            await addBrep2.Execute();

            Console.WriteLine("[GEOMETRY] Adding mesh geometry #3 (V2)");
            addMesh = new CreateMeshCommand(ConsoleAppHelper);
            addMesh.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            await addMesh.Execute();

            Console.WriteLine("[GEOMETRY] Adding mesh geometry #4 (V2)");
            addMesh2 = new CreateMeshCommand(ConsoleAppHelper);
            addMesh2.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            await addMesh2.Execute();

            Console.WriteLine("[PRIMITIVE] Adding primitive geometries (All) - V2");
            createPrimitive = new CreatePrimitiveGeometryCommand(ConsoleAppHelper);
            createPrimitive.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            createPrimitive.GetOption<PrimitiveGeometry>().SetValue("All");
            await createPrimitive.Execute();

            Console.WriteLine("[PRIMITIVE] Adding primitive geometry (Line) - V2");
            createPrimitive = new CreatePrimitiveGeometryCommand(ConsoleAppHelper);
            createPrimitive.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            createPrimitive.GetOption<PrimitiveGeometry>().SetValue("Line");
            await createPrimitive.Execute();

            Console.WriteLine("[PRIMITIVE] Adding primitive geometry (Point) - V2");
            createPrimitive = new CreatePrimitiveGeometryCommand(ConsoleAppHelper);
            createPrimitive.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            createPrimitive.GetOption<PrimitiveGeometry>().SetValue("Point");
            await createPrimitive.Execute();

            Console.WriteLine("[PRIMITIVE] Adding primitive geometry (Circle) - V2");
            createPrimitive = new CreatePrimitiveGeometryCommand(ConsoleAppHelper);
            createPrimitive.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            createPrimitive.GetOption<PrimitiveGeometry>().SetValue("Circle");
            await createPrimitive.Execute();

            Console.WriteLine("[PARAMETER] Adding instance parameter (AllModelDescription) - V2");
            addInstanceParameter = new AddInstanceParamCommand(ConsoleAppHelper);
            addInstanceParameter.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            addInstanceParameter.GetOption<ElementId>().SetValue(addBrep.CommandOutput["ElementId"].ToString());
            addInstanceParameter.GetOption<ParameterName>().SetValue("allModelDescription");
            addInstanceParameter.GetOption<ParameterSchema>().SetValue("autodesk.revit.parameter:allModelDescription-1.0.0");
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

            Console.WriteLine("[PARAMETER] Adding type parameter (AllModelTypeComments) - V2");
            addTypeParamCommand = new AddTypeParamCommand(ConsoleAppHelper);
            addTypeParamCommand.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            addTypeParamCommand.GetOption<ElementId>().SetValue(addBrep.CommandOutput["ElementId"].ToString());
            addTypeParamCommand.GetOption<ParameterName>().SetValue("allModelTypeComments");
            addTypeParamCommand.GetOption<ParameterSchema>().SetValue("autodesk.revit.parameter:allModelTypeComments-1.0.0");
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

            Console.WriteLine("[SYNC] Synchronizing final exchange data to Version 2");
            var syncExchangeData2 = new SyncExchangeData(ConsoleAppHelper);
            syncExchangeData2.GetOption<ExchangeTitle>().SetValue(exchangeTitle);
            await syncExchangeData2.Execute();

            Console.WriteLine("[DOWNLOAD] Retrieving final exchange data as STEP file");
            var getExchangeCommand = new GetExchangeCommand(ConsoleAppHelper);
            getExchangeCommand.GetOption<ExchangeId>().SetValue(createExchangeCommand.CommandOutput["ExchangeId"].ToString());
            getExchangeCommand.GetOption<CollectionId>().SetValue(createExchangeCommand.CommandOutput["CollectionId"].ToString());
                        await getExchangeCommand.Execute();
            
            Console.WriteLine("[SUCCESS] Complete workflow test finished successfully!");
            
            return true;
        }       

        public override Command Clone()
        {
            return new WorkFlowTestCommand(this);
        }
    }
}
