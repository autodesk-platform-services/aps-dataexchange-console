using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.DataExchange.DataModels;
using Autodesk.DataExchange.Exceptions;
using Autodesk.DataExchange.Interface;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Which typed exceptions the SDK raises for bad input and failed calls, and how to handle each one.
    /// SDK: AttributeValidationException (IElement.AddAttributes), CyclicDesignReferenceException (InstantiateDesign),
    /// IStatusCodeException.StatusCode (IError.Exception from a failed IResponse).
    /// Console plumbing: ElementDataModel.Create (scratch model), DesignSampleHelper.CreateDefinitionWithMesh, DiagnosticsSampleHelper.FindStatusCode.
    /// Prerequisites: 2.3 Load Exchange for the status-code part. Nothing is synced.
    /// </summary>
    [SampleAddress(10, 6)]
    public sealed class HandleSdkErrorsSample : ISample
    {
        // Deliberately far over any limit so the demo does not depend on the exact values; the SDK message reports the real limit.
        private const int OversizedLength = 1000;
        private const int OversizedAttributeCount = 100;

        public string Name => "Handle SDK Errors";
        public string Description => "Catch attribute-limit, cyclic-design and HTTP status errors";

        public async Task RunAsync(SampleContext ctx)
        {
            TerminalUi.Info("This sample works on a scratch in-memory model, so no loaded exchange is changed.");
            // SDK: an empty model that is never synced
            var scratch = ElementDataModel.Create(DiagnosticsSampleHelper.RequireClient(ctx));

            ShowAttributeLimits(scratch);
            ShowCyclicDesignReference(scratch);
            await ShowStatusCodeAsync(ctx);
        }

        private static void ShowAttributeLimits(ElementDataModel model)
        {
            TerminalUi.Section("AttributeValidationException");
            TerminalUi.Info("Element attribute names, values and the number of attributes per element are limited.");
            TerminalUi.Info("Writes over a limit throw AttributeValidationException (an ArgumentException) instead of being trimmed. The message states the limit.");
            var element = model.AddElement("attribute-limits", "Attribute limits");

            TryAddAttributes(element, "name too long", new Dictionary<string, string>
            {
                [new string('n', OversizedLength)] = "value",
            });
            TryAddAttributes(element, "value too long", new Dictionary<string, string>
            {
                ["Comment"] = new string('v', OversizedLength),
            });
            TryAddAttributes(element, "too many attributes", Enumerable
                .Range(1, OversizedAttributeCount)
                .ToDictionary(i => $"Key{i}", i => $"Value{i}"));
        }

        private static void TryAddAttributes(IElement element, string label, IDictionary<string, string> attributes)
        {
            try
            {
                // SDK: write attributes; limits are checked before anything is stored
                element.AddAttributes(attributes);
                TerminalUi.Warning($"  {label}: accepted (no exception).");
            }
            catch (AttributeValidationException ex)
            {
                TerminalUi.Chat($"  {label}: AttributeValidationException - {ex.Message}");
            }
        }

        private static void ShowCyclicDesignReference(ElementDataModel model)
        {
            TerminalUi.Section("CyclicDesignReferenceException");
            TerminalUi.Info("A design cannot be instantiated on an element that is already inside that design.");
            var definition = DesignSampleHelper.CreateDefinitionWithMesh(model, "loop-definition", "Loop definition");
            // SDK: a design reference built from the definition element
            var design = model.GetOrCreateDesignRef(definition, "Loop", "loop-design");
            var child = model.AddElement("loop-child", "Loop child", definition);

            try
            {
                // SDK: instantiating the design on its own child would make the design contain itself
                model.InstantiateDesign(design, child);
                TerminalUi.Warning("  InstantiateDesign: accepted (no exception).");
            }
            catch (CyclicDesignReferenceException ex)
            {
                TerminalUi.Chat($"  InstantiateDesign: CyclicDesignReferenceException - {ex.Message}");
            }
        }

        private static async Task ShowStatusCodeAsync(SampleContext ctx)
        {
            TerminalUi.Section("IStatusCodeException");
            TerminalUi.Info("Exchange (ApiException) and hosting (HTTPResponseException) errors both implement IStatusCodeException.");
            TerminalUi.Info("Read StatusCode from it instead of type-checking each exception class.");
            // Console plumbing (not SDK): borrow the collection id of any loaded exchange
            var collectionId = ctx.Exchanges.Values.Select(e => e.CollectionId).FirstOrDefault();
            if (string.IsNullOrEmpty(collectionId))
            {
                TerminalUi.Warning("No loaded exchange. Run 2.3 Load Exchange first to try this part.");
                return;
            }

            const string missingUrn = "urn:adsk.wipprod:dm.lineage:does-not-exist";
            Exception? failure = null;
            try
            {
                // SDK: ask for an exchange that does not exist
                var response = await ctx.Client.GetExchangeDetailsAsync(collectionId, missingUrn);
                if (response.IsSuccess)
                {
                    TerminalUi.Warning("  Unexpected success.");
                    return;
                }

                // SDK: a failed IResponse carries the original exception on IError.Exception
                failure = response.Errors.Select(e => e.Exception).FirstOrDefault(e => e != null);
                TerminalUi.Chat($"  Failed response: {response.Errors.FirstOrDefault()?.Message}");
            }
            catch (Exception ex)
            {
                failure = ex;
            }

            var status = DiagnosticsSampleHelper.FindStatusCode(failure);
            TerminalUi.Chat(status.HasValue
                ? $"  IStatusCodeException.StatusCode = {status.Value} ({failure!.GetType().Name})"
                : $"  No IStatusCodeException in the failure ({failure?.GetType().Name ?? "no exception"}).");
        }
    }
}
