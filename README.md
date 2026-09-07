# Autodesk Data Exchange Console Connector

[![oAuth2](https://img.shields.io/badge/oAuth2-v2-green.svg)](http://developer.autodesk.com/)
![.NET](https://img.shields.io/badge/.NET%20Framework-4.8-blue.svg)
![SDK Version](https://img.shields.io/badge/Data%20Exchange%20SDK-7.7.0--alpha.1-orange.svg)
![Intermediary](https://img.shields.io/badge/Level-Intermediary-lightblue.svg)
[![License](https://img.shields.io/badge/License-Autodesk%20SDK-blue.svg)](LICENSE)

## 📋 Overview

This is a **sample console connector** that demonstrates how to use the Autodesk Data Exchange SDK without the UI component. It serves as a reference implementation for developers building their own Data Exchange integrations and provides a comprehensive example of SDK capabilities through a professional command-line interface.

**Perfect for:**
- Service-based integrations
- Custom UI development
- Learning Data Exchange SDK patterns
- Automated workflow testing
- Headless data processing

![Console Interface](images/thumbnail.png)

## 🔗 Quick Navigation

- [🚀 Quick Start](#-quick-start) - Get up and running quickly
- [🔐 Credential Setup](#-credential-setup) - Env vars, App.config, and safe defaults
- [💻 Usage Examples](#-usage-examples) - See the console connector in action
- [📚 Sample Catalog](#-sample-catalog) - Complete list of categories and samples
- [🔄 Migration Guide](#-migration-guide-console-connector-architecture-revamp) - **Architecture Revamp Guide**
- [🏗️ Architecture](#️-architecture) - Understand the codebase structure
- [🔧 Extending the Application](#-extending-the-application) - Add custom functionality

## 🎯 Key Features

### Core Functionality
- ✅ **Exchange Management** - Create, update, and retrieve exchanges
- ✅ **Multi-Format Geometry Processing** - BREP, IFC, Mesh, and Primitive geometries
- ✅ **Parameter Operations** - Add, modify, and delete instance/type parameters
- ✅ **Version Control** - Exchange synchronization and versioning
- ✅ **Data Export** - Download exchanges as STEP, IFC, or OBJ files
- ✅ **Folder Management** - Navigate hubs, projects, and folders

### Developer Experience
- 🚀 **Interactive Console Menu** - Categorized, navigable sample catalog (11 categories, 100+ samples)
- 📋 **End-to-End Workflow Scenarios** - Self-contained, runnable demonstrations of complete flows
- 💻 **Sample-Based Architecture** - Every capability is a standalone `ISample` you can jump to directly (e.g. `4.1`) or run non-interactively via `--run-all`
- 🔧 **Error Handling** - Robust error management and user feedback
- 🔐 **Safe Credential Handling** - Environment variables first; nothing written to disk unless you opt in via `App.config`

## 🛠️ Prerequisites

1. **Autodesk Platform Services App**
   - [Register an app](https://aps.autodesk.com/myapps/)
   - Select **Data Management** and **Data Exchange** APIs
   - Note your **Client ID**, **Client Secret**, and **Auth Callback**

2. **Development Environment**
   - Visual Studio 2019 or later
   - .NET Framework 4.8
   - Basic knowledge of C#

3. **Access Requirements**
   - [Autodesk Construction Cloud](https://acc.autodesk.com/) (ACC) access
   - Valid Autodesk account with membership on the target hub/project
   - **Custom Integration** — your Forge app's **Client ID** must be added and approved under **ACC Hub Admin → Custom Integrations** for each hub you want to use (required for hub lookup from ACC/Forma URLs and for Data Management / Data Exchange API calls). See [ACC Custom Integration](#-acc-custom-integration) below.

## 🚀 Quick Start

### 1. Clone and Setup
```bash
git clone https://github.com/your-repo/aps-dataexchange-console.git
cd aps-dataexchange-console
```

### 2. Install Dependencies
Follow the [Data Exchange .NET SDK installation guide](https://aps.autodesk.com/en/docs/dx-sdk-beta/v1/developers_guide/installing_the_sdk/#procedure) to obtain the beta SDK nupkgs (they aren't on public nuget.org). `BuildSolution.bat` restores everything else from nuget.org automatically, but these packages must be dropped as loose `.nupkg` files in the **parent directory of your repo checkout** first:
- `Autodesk.DataExchange` (7.7.0-alpha.1)
- `Autodesk.DataExchange.ADPAnalytics.Abstractions` (1.0.0)
- `Autodesk.DataExchange.GeometryDefinitions` (0.9.4)
- `ForgeParameters-csharp_win_release_intel64_v140` (3.0.6)
- `ForgeUnits-csharp_win_release_intel64_v140` (5.1.4)

**Option A: Visual Studio**
- Open `ConsoleConnector.sln`
- Build the solution (packages restore automatically)

**Option B: Command Line**
```bash
# Run from project root
BuildSolution.bat
```

### 3. Run the Application
- Build and run the console application (`ConsoleConnector.exe`)
- Complete OAuth authentication in the browser
- Navigate the interactive menu, or jump straight to a sample by its key (e.g. `4.1`)

See [🔐 Credential Setup](#-credential-setup) below before your first run.

## 🔐 Credential Setup

Credentials are resolved in this order, and **nothing is written to disk unless you explicitly opt into `App.config`**:

1. **Environment variables (recommended)** — `DXSDK_CLIENT_ID` / `DXSDK_CLIENT_SECRET`. Safe for CI and shared machines; never touches the filesystem.

   PowerShell:
   ```powershell
   $env:DXSDK_CLIENT_ID = "your_client_id"
   $env:DXSDK_CLIENT_SECRET = "your_client_secret"
   ```

   POSIX shells:
   ```bash
   export DXSDK_CLIENT_ID=your_client_id
   export DXSDK_CLIENT_SECRET=your_client_secret
   ```

2. **`App.config` (local-dev fallback)** — if the environment variables aren't set, the app reads `AuthClientId` / `AuthClientSecret` from `src/ConsoleConnector/App.config`:
   ```xml
   <appSettings>
       <add key="AuthClientId" value="" />
       <add key="AuthClientSecret" value="" />
       <add key="AuthCallback" value="http://localhost:8080/" />
       <add key="ConnectorName" value="Sample Connector" />
       <add key="ConnectorVersion" value="1.0.0" />
       <add key="HostApplicationName" value="Sample Host App" />
       <add key="HostApplicationVersion" value="2.0.0" />
   </appSettings>
   ```
   **Never commit real values here** — this file is tracked in git. Run `git diff`/`git status` before pushing if you edit it locally.

3. **One-off interactive prompt** — if neither of the above is set, the app prompts for credentials for that run only. Nothing is saved; you'll see a reminder of how to persist them via env vars or `App.config`.

Register your app and callback URL at [aps.autodesk.com/myapps](https://aps.autodesk.com/myapps/) with **Data Management** and **Data Exchange** APIs selected. The default callback is `http://localhost:8080/`, with `http://127.0.0.1:63212/`, `http://localhost:9090/`, and `http://localhost:3000/` as fallbacks if that port is in use.

## 🏢 ACC Custom Integration

Hub lookup (when you paste a Forma or ACC docs URL on first run) calls the APS API to find which hub owns a project. That only works when **both** of the following are true:

1. Your **Forge app is authorized** for the hub (Custom Integration).
2. Your **Autodesk account** must have access to the hub (and project).

### Steps (hub admin)

1. Open [ACC](https://acc.autodesk.com/) and select the **hub** that owns the project you need.
2. Go to **Hub Admin** → **Custom Integrations**.
3. Click **Add Custom Integration** (or **Add app**).
4. Paste your Forge app's **Client ID** from [aps.autodesk.com/myapps](https://aps.autodesk.com/myapps/) — the same value as `DXSDK_CLIENT_ID` / `AuthClientId`.
5. **Approve** the integration.
6. Sign in to the console with an account that has access to that hub.
7. Set the session folder again — choose **Retry (added Custom Integration?)** after pasting the ACC URL, or enter Hub Id / Project URN / Folder URN manually. Use **1.1 List Hubs** and **1.2 List Projects** to confirm access.

### If hub lookup still fails

- Do **not** reuse a Hub Id from a different account — each project belongs to one hub.
- ACC docs URLs include `projects/{id}` and `folderUrn=…` but **not** `hubId`; lookup depends on Custom Integration + user access.
- Enter Hub Id manually only after **1.2 List Projects** shows the project under that hub.

The console prints these steps automatically when hub lookup fails.

## 💻 Usage Examples

### Interactive Menu

Launch `ConsoleConnector.exe` and navigate the category menu (Navigation, Exchange Lifecycle, Elements, Attach Geometry, Parameters, Design References, Download Exchange, Revisions, SDK Events, Debug & Telemetry, E2E Workflow Scenarios). From any menu:
- **`?` Show all samples** — prints the full catalog with keys (e.g. `4.1`, `5.2.1`)
- **`›` Jump to sample key** — type a key directly to run that sample without navigating menus

### Run Everything Non-Interactively

```bash
ConsoleConnector.exe --run-all
```

Runs every registered sample once, using defaults for any prompts, and prints a pass/fail summary — useful for smoke-testing after a change or in CI.

### End-to-End Workflow Scenarios (category 11)

Each scenario is a self-contained, runnable demonstration of a complete flow (e.g. "create exchange → add geometry → add parameters → sync"). Jump to any of them directly:

```text
11.1  Simple Exchange              - create + sync (empty)
11.2  Single Element With Geometry - create, root element, BREP, sync
11.4  Multi Geometry Multi Parameter - BREP + mesh + primitive + custom params
11.7  Two Version Round Trip       - create v1, modify v2, reload v1, diff
11.10 End To End Create Sync Download - full create + geometry + param + sync + download
```

## 📚 Sample Catalog

| # | Category | Samples |
|---|----------|---------|
| 1 | Navigation | Hubs, Projects, Folders, Exchanges |
| 2 | Exchange Lifecycle | Create, Load, Sync, Refresh, Revisions, Delete |
| 3 | Elements | Add root/child, list, get by id, delete, attributes, modifications |
| 4 | Attach Geometry | BREP/STEP, IFC, Mesh, Primitives, Advanced (render style, transforms, units, streams) |
| 5 | Parameters | Instance (built-in/custom/batch/update), Type, Misc (model/geometry-level, references) |
| 6 | Design References | Create, instantiate, query by id/name |
| 7 | Download Exchange | STEP, IFC, OBJ |
| 8 | Revisions | Created/modified/deleted/all-changed elements |
| 9 | SDK Events | Subscribe/unsubscribe to exchange updates, viewable generation progress |
| 10 | Debug & Telemetry | HTTP debug logging, feature flags, progress steps, telemetry sessions |
| 11 | E2E Workflow Scenarios | 10 self-contained end-to-end demonstrations |

Every sample implements `ISample` and is discovered automatically via reflection (`SampleAddressAttribute`) — see [Extending the Application](#-extending-the-application).

## 🏗️ Architecture

```
ConsoleConnector/
├── Driver/              # App plumbing: menu, bootstrap, credentials, terminal UI
│   ├── Menu.cs
│   ├── CredentialsBootstrap.cs
│   ├── SdkBootstrap.cs
│   ├── SampleDiscovery.cs
│   ├── SampleRunner.cs
│   └── ...
├── Common/              # Shared helpers reused across samples
│   ├── ExchangeSessionHelper.cs
│   ├── ElementSampleHelper.cs
│   ├── GeometrySampleHelper.cs
│   ├── ParameterSampleHelper.cs
│   └── ...
├── Samples/             # One ISample per capability, organized by category
│   ├── Navigation/
│   ├── ExchangeLifecycle/
│   ├── Elements/
│   ├── AttachGeometry/
│   ├── Parameters/
│   ├── DesignReferences/
│   ├── DownloadExchange/
│   ├── Revisions/
│   ├── SdkEvents/
│   ├── DebugTelemetry/
│   └── E2EWorkflowScenarios/
└── Assets/              # Sample geometry files
```

### Key Components

- **`ISample`**: every capability implements this interface (`Name`, `Description`, `RunAsync(ctx)`); discovered automatically via the `[SampleAddress(category, sample, subSample)]` attribute
- **`Common/*Helper` classes**: the only thing samples are allowed to compose — no sample ever instantiates another sample directly, keeping every example runnable and readable on its own
- **`SampleContext`**: carries the SDK client, session folder, and defaults between samples in a single run
- **`Driver/`**: everything that isn't part of the SDK story — menu navigation, credential resolution, session persistence, terminal rendering

## 🔧 Extending the Application

### Adding a New Sample

1. Create a class implementing `ISample` under the appropriate `Samples/<Category>/` folder
2. Tag it with `[SampleAddress(categoryId, sampleId, subSampleId = 0)]`
3. Implement the SDK call using existing `Common/*Helper` methods where possible

```csharp
[SampleAddress(3, 10)]
public sealed class MyCustomSample : ISample
{
    public string Name => "My Custom Sample";
    public string Description => "Demonstrates my custom SDK capability";

    public async Task RunAsync(SampleContext ctx)
    {
        var session = await ElementSampleHelper.BeginAsync(ctx);
        if (session == null)
            return;

        // Your SDK call here.

        await ElementSampleHelper.SyncAsync(ctx, session);
    }
}
```

Add the file to `ConsoleConnector.csproj`'s `<Compile Include="..." />` list (the project uses an old-style `.csproj` without globbing) — the sample is then automatically discovered and appears in the menu under its category.

### Adding Shared Logic

If a piece of logic is needed by more than one sample, add it to the relevant `Common/*Helper` class instead of duplicating it — samples should stay thin wrappers over `Common` helpers.

## 🔄 Migration Guide: Console Connector Architecture Revamp

This section documents replacing the Command-pattern console app with a menu-driven, sample-based architecture, landed as a sequence of category-wise PRs.

### 📋 Overview of Changes

- **Architecture**: `Commands/`/`Helper/`/`Interfaces/` replaced by `Driver/` (menu, bootstrap, terminal UI) + `Common/` (shared helpers) + `Samples/` (one `ISample` per capability, organized by category)
- **Credentials**: no longer persisted to a plaintext session file. Resolved from `DXSDK_CLIENT_ID`/`DXSDK_CLIENT_SECRET` env vars → `App.config` → a one-off prompt that is never saved (see [🔐 Credential Setup](#-credential-setup))
- **Tests**: added an MSTest+Moq scaffold covering the `Driver`/`Common` surface (credential resolution, session state mapping, sample discovery/runner, and the pure helper logic)
- **Breaking Changes**: Yes — the CLI-args command interface (`>> CreateExchange [Title]`, `>> WorkFlowTest`, etc.) is replaced entirely by the interactive menu / `--run-all` / jump-to-key interface described in [Usage Examples](#-usage-examples)
- **Build result**: 0 errors after fixes applied

### 🚀 Key Dependency Updates

| Package | Reason |
|---------|--------|
| `Autodesk.DataExchange.ADPAnalytics.Abstractions` (1.0.0) | `Client.Initialize()` requires this companion assembly at runtime against 7.6.0-beta; not previously declared |
| `Autodesk.DataExchange.GeometryDefinitions` (bumped to 0.9.4) | Exact version pinned by the SDK's own nuspec for 7.6.0-beta |
| `Spectre.Console` / `Spectre.Console.Ansi` (0.57.2) | Powers the interactive menu, tables, and prompts |
| `Microsoft.Bcl.TimeProvider`, `System.Memory`/`System.Buffers`/`System.Numerics.Vectors`/`System.Runtime.CompilerServices.Unsafe` (bumped) | Transitive requirements of `Spectre.Console` on net48 |
| `IndexRange` | Polyfills `System.Index`/`System.Range` (`^1`, `a..b` syntax) on net48 |

### ⚠️ Breaking Changes

#### 1. The CLI-args command interface is gone

**Before:**
```bash
>> CreateExchange [ExchangeTitle]
>> AddBrep [ExchangeTitle]
>> SyncExchange [ExchangeTitle]
```

**After:** navigate the interactive menu, or jump directly to a sample key:
```text
4.1   (jump to "Add BREP from STEP file")
```
or run everything non-interactively:
```bash
ConsoleConnector.exe --run-all
```

**Migration Action:** if you had scripts driving the old CLI syntax, replace them with `--run-all` for full-catalog runs, or automate via the `ISample` API directly (each sample is a public class with a `RunAsync(SampleContext)` method).

#### 2. Credential storage changed

**Before:** `App.config` was the only credential source, and real values were checked into git history.

**After:** environment variables are checked first and are never written to disk; `App.config` is a documented fallback that ships with empty values by default.

**Migration Action:** set `DXSDK_CLIENT_ID`/`DXSDK_CLIENT_SECRET`, or fill in your own local (never committed) `App.config` values.

### 🔧 Migration Steps

#### Step 1: Set credentials

See [🔐 Credential Setup](#-credential-setup).

#### Step 2: Restore and Rebuild

```bash
BuildSolution.bat
```

#### Step 3: Verify

```bash
ConsoleConnector.exe --run-all
```

Confirms every registered sample runs without an unhandled exception and prints a pass/fail summary.

### 🎯 Summary of Changes

| Aspect | Before | After |
|--------|--------|-------|
| Entry point | CLI-args commands (`>> CreateExchange ...`) | Interactive menu, jump-to-key, or `--run-all` |
| Credential storage | `App.config` only (real values in git history) | Env vars → `App.config` (empty by default) → one-off prompt |
| Extensibility | New `Command` subclass + `Options/` + registration in `ConsoleAppHelper` | New `ISample` + `[SampleAddress]`, auto-discovered |
| Test coverage | 11 MSTest cases against the Command pattern | 32 MSTest cases against `Driver`/`Common` |
| Upgrade effort | - | 14 stacked PRs — foundation, tests, E2E scenarios, then one PR per sample category |

### 🧪 Testing Your Migration

```bash
ConsoleConnector.exe --run-all
```

This validates every registered sample across all 11 categories runs without throwing. For unit-level coverage, run the MSTest suite in `test/ConsoleConnector_Test`.

---

**Migration Checklist:**
- [x] Replaced `Commands`/`Helper`/`Interfaces` with `Driver`/`Common`/`Samples`
- [x] Credentials resolved via env vars → `App.config` → one-off prompt, nothing persisted by default
- [x] Added MSTest+Moq coverage for the new `Driver`/`Common` surface
- [x] Made all 10 E2E workflow scenarios self-contained (no sample instantiates another sample)
- [x] Ported all 11 sample categories (104 samples total)
- [x] Ran `--run-all` to confirm full-catalog coverage

---

## 🔄 Migration Guide: SDK 7.7.0 Upgrade

This section documents the migration from SDK 7.6.0-beta to **Autodesk Data Exchange SDK 7.7.0-alpha.1**.

### Overview of Changes

- **SDK Version**: Upgraded to `Autodesk.DataExchange 7.7.0-alpha.1`
- **Geometry attach**: Use `AddElementGeometry` instead of `SetElementGeometry` when appending geometry to an element (preserves existing geometry)
- **ACC version display**: Loaded panel and post-sync output show the ACC file version number parsed from `FileVersionUrn`

### Key API Change: AddElementGeometry

`SetElementGeometry` replaces the element's entire geometry set. To append new geometry without deleting what is already attached, use `AddElementGeometry`:

**Before (7.6.0-beta):**
```csharp
model.SetElementGeometry(element, new List<IElementGeometry> { geometry });
```

**After (7.7.0-alpha.1):**
```csharp
model.AddElementGeometry(element, new List<IElementGeometry> { geometry });
```

### Migration Steps

1. Update `packages.config` and `.csproj` HintPaths to `7.7.0-alpha.1`
2. Replace append-style `SetElementGeometry` calls with `AddElementGeometry`
3. Restore NuGet packages and rebuild

---

## 🔄 Migration Guide: SDK 7.6.0 Upgrade

This section documents the migration from SDK 7.5.0 to **Autodesk Data Exchange SDK 7.6.0-beta**.

### 📋 Overview of Changes

- **SDK Version**: Upgraded to `Autodesk.DataExchange 7.6.0-beta`
- **Breaking Changes**: Yes — several `Element`/`ElementDataModel` APIs moved from concrete classes to interfaces (see below)
- **Build result**: 0 errors after fixes applied

### 🚀 Key Dependency Updates

| Package | Previous Version | New Version | Impact |
|---------|------------------|-------------|---------|
| `Autodesk.DataExchange` | `7.5.0-beta` | `7.6.0-beta` | **Minor** - a few breaking changes |

### ⚠️ Breaking Changes

#### 1. `Client.GetElementDataModelAsync` now returns `IElementDataModel` instead of `ElementDataModel`

The concrete `Autodesk.DataExchange.DataModels.ElementDataModel` class still exists and still implements `IElementDataModel`, so an explicit cast is sufficient — no data model changes required.

**Before (7.5.0):**
```csharp
currentExchangeData = (await Client.GetElementDataModelAsync(exchangeIdentifier)).Value;
```

**After (7.6.0-beta):**
```csharp
currentExchangeData = (ElementDataModel)(await Client.GetElementDataModelAsync(exchangeIdentifier)).Value;
```

**Migration Action:** Add an explicit cast to `ElementDataModel` wherever `Client.GetElementDataModelAsync(...).Value` is assigned to an `ElementDataModel`-typed variable or dictionary.

#### 2. `IElementDataModel.Elements` yields `IElement`, not `Element`

Iterating `elementDataModel.Elements` (e.g. via `FirstOrDefault`) now produces `IElement` instances. Code paths that pass a retrieved element into a method typed to accept the concrete `Element` class no longer compile.

**Migration Action:** Change parameter types that receive a *retrieved* element (as opposed to one just created via `AddElement`) from `Element` to `IElement` — e.g. `ParameterHelper.AddCustomParameter`/`AddBuiltInParameter` in `Helper/ParameterHelper.cs`.

#### 3. `IElement.Type` / `Element.Type` is now `IElementType`, not `string`

`Element.Type` used to be a plain string (e.g. `"Walls"`). The interface-typed `IElement.Type` returns `IElementType`, which exposes the same value via `IClassification.Value`.

**Before (7.5.0):**
```csharp
elementDataModel.DeleteTypeParameter(element.Type, parameterName.Value);
```

**After (7.6.0-beta):**
```csharp
elementDataModel.DeleteTypeParameter(element.Type.Value, parameterName.Value);
```

Similarly, prefer the new handle-based `AddTypeParameterAsync(IElementType, IParameter)` over the by-name `CreateTypeParameterAsync(string, IParameter)` when a type handle is already available (avoids a global name lookup):

```csharp
await elementDataModel.AddTypeParameterAsync(element.Type, parameter);
```

**Migration Action:** Replace `element.Type` usages that need a string with `element.Type.Value`, and switch `CreateTypeParameterAsync(element.Type, ...)` calls to `AddTypeParameterAsync(element.Type, ...)`.

### 🔧 Migration Steps

#### Step 1: Update Package References

Update your `packages.config`:

```xml
<package id="Autodesk.DataExchange" version="7.6.0-beta" targetFramework="net48" />
```

Update `ConsoleConnector.csproj` and `ConsoleConnector_Test.csproj`:
- Assembly version: `Version=7.5.0.0` → `Version=7.6.0.0`
- HintPaths: `Autodesk.DataExchange.7.5.0-beta\` → `Autodesk.DataExchange.7.6.0-beta\`
- Build target imports: same substitution

#### Step 2: Apply the Code Fixes

1. **`ConsoleAppHelper.cs`** — cast `Client.GetElementDataModelAsync(...).Value` (`IElementDataModel`) to `ElementDataModel` at both call sites.
2. **`ParameterHelper.cs`** — change `AddCustomParameter`/`AddBuiltInParameter` element parameters from `Element` to `IElement`; switch `CreateTypeParameterAsync(element.Type, ...)` to `AddTypeParameterAsync(element.Type, ...)`.
3. **`DeleteParameter.cs`** — change `element.Type` to `element.Type.Value` in both `DeleteTypeParameter` calls.

#### Step 3: Restore and Rebuild

**Command Line:**
```bash
BuildSolution.bat
```

#### Step 4: Verify

Run the comprehensive workflow test to confirm everything works as expected:

```bash
>> WorkFlowTest
```

### 🎯 Summary of Changes

| Aspect | SDK 7.5.0 | SDK 7.6.0-beta |
|--------|-----------|-----------------|
| Element retrieval | Returns `ElementDataModel`/concrete `Element` | Returns `IElementDataModel`/`IElement` |
| Element type | `Element.Type` is `string` | `IElement.Type` is `IElementType` (use `.Value` for the string) |
| Upgrade effort | - | ~20 min — 3 files changed |

### 🧪 Testing Your Migration

After upgrading, run the comprehensive workflow test:

```bash
>> WorkFlowTest
```

This command validates:
- ✅ Exchange creation and management
- ✅ Geometry processing (BREP, IFC, Mesh, Primitives)
- ✅ Parameter operations
- ✅ Synchronization workflows
- ✅ File download capabilities

---

**Migration Checklist:**
- [x] Updated all package references to 7.6.0-beta
- [x] Fixed `Element`/`ElementDataModel` → `IElement`/`IElementDataModel` breaking changes
- [x] Restored NuGet packages and rebuilt the solution (0 errors)
- [x] Ran the MSTest unit test suite (11/11 passed)
- [ ] Tested core workflows with `WorkFlowTest`

---

## 🔄 Migration Guide: SDK 7.5.0 Upgrade

This section documents the migration from SDK 7.4.0 to **Autodesk Data Exchange SDK 7.5.0**.

### 📋 Overview of Changes

- **SDK Version**: Upgraded to `Autodesk.DataExchange 7.5.0-beta`
- **Breaking Changes**: Yes — 1 code change required (see below)
- **Build result**: 0 errors after fixes applied

### 🚀 Key Dependency Updates

| Package | Previous Version | New Version | Impact |
|---------|------------------|-------------|---------|
| `Autodesk.DataExchange` | `7.4.0-beta` | `7.5.0-beta` | **Minor** - 1 breaking change |

### ⚠️ Breaking Changes

#### 1. `Client.GenerateViewableAsync` removed — viewable generation is now server-side

In SDK 7.5.0 viewable generation is handled **server-side**. The client-side
`Client.GenerateViewableAsync` API was removed with **no replacement** — after
`SyncExchangeDataAsync` completes, the service generates the viewable automatically.

**Before (7.4.0):**
```csharp
await Client.SyncExchangeDataAsync(dataExchangeIdentifier, exchangeData);
await Client.GenerateViewableAsync(exchangeDetails.ExchangeID, exchangeDetails.CollectionID);
```

**After (7.5.0):**
```csharp
await Client.SyncExchangeDataAsync(dataExchangeIdentifier, exchangeData);
// Viewable generation is handled server-side in SDK 7.5.0; the client-side
// Client.GenerateViewableAsync API was removed (no replacement).
```

**Migration Action:** Remove all calls to `Client.GenerateViewableAsync`.

### 🔧 Migration Steps

#### Step 1: Update Package References

Update your `packages.config`:

```xml
<package id="Autodesk.DataExchange" version="7.5.0-beta" targetFramework="net48" />
```

Update `ConsoleConnector.csproj` and `ConsoleConnector_Test.csproj`:
- Assembly version: `Version=7.4.0.0` → `Version=7.5.0.0`
- HintPaths: `Autodesk.DataExchange.7.4.0-beta\` → `Autodesk.DataExchange.7.5.0-beta\`
- Build target imports: same substitution

#### Step 2: Apply the Code Fix

1. **`ConsoleAppHelper.cs`** — remove the `Client.GenerateViewableAsync` call after `SyncExchangeDataAsync`.
2. **`SyncExchangeGeometry.cs`** — remove the `Client.GenerateViewableAsync` call after `SyncExchangeDataAsync`.

#### Step 3: Restore and Rebuild

**Command Line:**
```bash
BuildSolution.bat
```

#### Step 4: Verify

Run the comprehensive workflow test to confirm everything works as expected:

```bash
>> WorkFlowTest
```

### 🎯 Summary of Changes

| Aspect | SDK 7.4.0 | SDK 7.5.0 |
|--------|-----------|-----------|
| Viewable generation | Client-side via `GenerateViewableAsync` | Server-side; API removed |
| Upgrade effort | - | ~10 min — 2 files changed |

### 🧪 Testing Your Migration

After upgrading, run the comprehensive workflow test:

```bash
>> WorkFlowTest
```

This command validates:
- ✅ Exchange creation and management
- ✅ Geometry processing (BREP, IFC, Mesh, Primitives)
- ✅ Parameter operations
- ✅ Synchronization workflows
- ✅ File download capabilities

---

**Migration Checklist:**
- [x] Updated all package references to 7.5.0-beta
- [x] Restored NuGet packages and rebuilt the solution (0 errors)
- [x] Removed `Client.GenerateViewableAsync` calls
- [ ] Tested core workflows with `WorkFlowTest`

---

## 🔄 Migration Guide: SDK 7.4.0 Upgrade

This section documents the migration from SDK 7.2.1 to **Autodesk Data Exchange SDK 7.4.0**.

### 📋 Overview of Changes

- **SDK Version**: Upgraded to `Autodesk.DataExchange 7.4.0-beta`
- **Breaking Changes**: Yes — 3 code changes required (see below)
- **Build result**: 0 errors after fixes applied

### 🚀 Key Dependency Updates

| Package | Previous Version | New Version | Impact |
|---------|------------------|-------------|---------|
| `Autodesk.DataExchange` | `7.2.1-beta` | `7.4.0-beta` | **Minor** - 3 breaking changes |

### ⚠️ Breaking Changes

#### 1. `SDKOptionsDefaultSetup` — auth properties removed from initializer

`ConnectorName`, `ConnectorVersion`, `HostApplicationName`, `HostApplicationVersion` still exist on the base `SDKOptions` class and must be set there, but can still be set in the `SDKOptionsDefaultSetup` initializer. A new `FallbackRedirectUrls` property was added for port-conflict handling.

**Before (7.2.1):**
```csharp
var sdkOptions = new SDKOptionsDefaultSetup()
{
    ClientId = authClientId,
    CallBack = authCallBack,
    ClientSecret = authClientSecret,
    ConnectorName = "ConsoleConnector",
    ConnectorVersion = "1.0.0",
    HostApplicationName = "ConsoleConnector",
    HostApplicationVersion = "1.0",
};
Client = new Client(sdkOptions);
```

**After (7.4.0):**
```csharp
var sdkOptions = new SDKOptionsDefaultSetup()
{
    ClientId = authClientId,
    CallBack = authCallBack,
    ClientSecret = authClientSecret,
    ConnectorName = "ConsoleConnector",
    ConnectorVersion = "1.0.0",
    HostApplicationName = "ConsoleConnector",
    HostApplicationVersion = "1.0",
};

// Auth must be obtained BEFORE new Client() — the ctor calls Initialize() → GetHashedUserId() internally
var auth = new Auth(new AuthOptions { ClientId = authClientId, ClientSecret = authClientSecret, CallBack = authCallBack });
await auth.GetAuthTokenAsync();
sdkOptions.AuthProvider = auth;   // pre-assign so Client ctor reuses it

await Task.Run(() => { Client = new Client(sdkOptions); });
```

#### 2. `Client` constructor now calls `Initialize()` internally

`Client.Initialize()` is called inside the constructor — it requires a valid auth token at construction time. **Auth must be completed before `new Client(sdkOptions)` is called.**

The fix is to pre-build an `Auth` instance, call `GetAuthTokenAsync()`, assign it to `sdkOptions.AuthProvider`, and then construct the client. `InitializeSDKOptions` (called inside the ctor) skips creating a new `AuthProvider` if one is already set.

#### 3. `RenderStyle` and `RGBA` default constructors marked `[Obsolete]`

Property setters are deprecated. Use the parameterized constructors instead.

**Before (7.2.1):**
```csharp
RenderStyle renderStyle = new RenderStyle()
{
    Name = "My Style",
    RGBA = new RGBA() { Red = 255, Green = 0, Blue = 0, Alpha = 255 },
    Transparency = 1
};
```

**After (7.4.0):**
```csharp
RenderStyle renderStyle = new RenderStyle("My Style", new RGBA(255, 0, 0, 255), 1);
```

### 🔧 Migration Steps

#### Step 1: Update Package References

Update your `packages.config`:

```xml
<package id="Autodesk.DataExchange" version="7.4.0-beta" targetFramework="net48" />
```

Update `ConsoleConnector.csproj` and `ConsoleConnector_Test.csproj`:
- Assembly version: `Version=7.2.1.0` → `Version=7.4.0.0`
- HintPaths: `Autodesk.DataExchange.7.2.1-beta\` → `Autodesk.DataExchange.7.4.0-beta\`
- Build target imports: same substitution

#### Step 2: Apply the 3 Code Fixes

1. **`ConsoleAppHelper.cs`** — make `CreateClient` async, pre-authenticate before `new Client()`
2. **`GeometryHelper.cs`** — replace `new RenderStyle() { ... }` with `new RenderStyle(name, rgba, transparency)`

#### Step 3: Restore and Rebuild

**Command Line:**
```bash
BuildSolution.bat
```

#### Step 4: Verify

Run the comprehensive workflow test to confirm everything works as expected:

```bash
>> WorkFlowTest
```

### 🎯 Summary of Changes

| Aspect | SDK 7.2.1 | SDK 7.4.0 |
|--------|-----------|-----------|
| API surface | Stable | 3 breaking changes |
| `SDKOptionsDefaultSetup` | Sets all fields | Auth-only; metadata via `SDKOptions` base |
| `Client` construction | Sync, no pre-auth needed | Requires auth token before `new Client()` |
| `RenderStyle` / `RGBA` | Default constructor + setters | Parameterized constructors required |
| Auth flow | Token fetched lazily | Must call `GetAuthTokenAsync()` explicitly before `new Client()` |
| Upgrade effort | - | ~30 min — 2 files changed |

### 🧪 Testing Your Migration

After upgrading, run the comprehensive workflow test:

```bash
>> WorkFlowTest
```

This command validates:
- ✅ Exchange creation and management
- ✅ Geometry processing (BREP, IFC, Mesh, Primitives)
- ✅ Parameter operations
- ✅ Synchronization workflows
- ✅ File download capabilities

---

**Migration Checklist:**
- [x] Updated all package references to 7.4.0-beta
- [x] Restored NuGet packages and rebuilt the solution (0 errors)
- [x] Fixed `RenderStyle`/`RGBA` to use parameterized constructors
- [x] Fixed `CreateClientAsync` to pre-authenticate before `new Client()`
- [ ] Tested core workflows with `WorkFlowTest`
- [ ] Verified all geometry types render correctly

---

## 🔄 Migration Guide: SDK 7.2.1 Upgrade

This section documents the migration from SDK 7.2.0 to **Autodesk Data Exchange SDK 7.2.1**.

### 📋 Overview of Changes

This is a **non-breaking** upgrade focused on bug fixes and minor improvements:
- **SDK Version**: Upgraded to `Autodesk.DataExchange 7.2.1-beta`
- **No Breaking Changes**: All existing APIs remain fully compatible
- **Bug Fixes**: Various stability and reliability improvements

### 🚀 Key Dependency Updates

| Package | Previous Version | New Version | Impact |
|---------|------------------|-------------|---------|
| `Autodesk.DataExchange` | `7.2.0-beta` | `7.2.1-beta` | **Patch** - Bug fixes, no breaking changes |

### 🔧 Migration Steps

#### Step 1: Update Package References

Update your `packages.config`:

```xml
<package id="Autodesk.DataExchange" version="7.2.1-beta" targetFramework="net48" />
```

Or if using a `.csproj` `PackageReference`:

```xml
<PackageReference Include="Autodesk.DataExchange" Version="7.2.1-beta" />
```

#### Step 2: Restore and Rebuild

**Visual Studio:**
- Open `ConsoleConnector.sln`
- Rebuild the solution (packages restore automatically)

**Command Line:**
```bash
BuildSolution.bat
```

#### Step 3: Verify

Run the comprehensive workflow test to confirm everything works as expected:

```bash
>> WorkFlowTest
```

### 🎯 Summary of Changes

| Aspect | SDK 7.2.0 | SDK 7.2.1 |
|--------|-----------|-----------|
| API surface | Stable | No changes |
| Breaking changes | - | None |
| Upgrade effort | - | Version bump only |
| Key focus | Bug fixes & improvements | Bug fixes & improvements |

### 🧪 Testing Your Migration

After upgrading, run the comprehensive workflow test:

```bash
>> WorkFlowTest
```

This command validates:
- ✅ Exchange creation and management
- ✅ Geometry processing (BREP, IFC, Mesh, Primitives)
- ✅ Parameter operations
- ✅ Synchronization workflows
- ✅ File download capabilities

---

**Migration Checklist:**
- [ ] Updated all package references to 7.2.1-beta
- [ ] Restored NuGet packages and rebuilt the solution
- [ ] Tested core workflows with `WorkFlowTest`
- [ ] Verified all geometry types render correctly

---

<details>
<summary><strong>📜 Historical: SDK 7.1.0 → 7.2.0 Migration Guide</strong></summary>

This section documents the migration from SDK 7.1.0 to **Autodesk Data Exchange SDK 7.2.0**.

### 📋 Overview of Changes

This is a **non-breaking** upgrade focused on bug fixes and minor improvements:
- **SDK Version**: Upgraded to `Autodesk.DataExchange 7.2.0`
- **No Breaking Changes**: All existing APIs remain fully compatible
- **Bug Fixes**: Various stability and reliability improvements
- **Minor Improvements**: Internal enhancements to SDK performance

### 🚀 Key Dependency Updates

| Package | Previous Version | New Version | Impact |
|---------|------------------|-------------|---------|
| `Autodesk.DataExchange` | `7.1.0` | `7.2.0` | **Minor** - Bug fixes and improvements, no breaking changes |

### 🔧 Migration Steps

#### Step 1: Update Package References

Update your `packages.config`:

```xml
<package id="Autodesk.DataExchange" version="7.2.0" targetFramework="net48" />
```

Or if using a `.csproj` `PackageReference`:

```xml
<PackageReference Include="Autodesk.DataExchange" Version="7.2.0" />
```

#### Step 2: Restore and Rebuild

**Visual Studio:**
- Open `ConsoleConnector.sln`
- Rebuild the solution (packages restore automatically)

**Command Line:**
```bash
BuildSolution.bat
```

#### Step 3: Verify

Run the comprehensive workflow test to confirm everything works as expected:

```bash
>> WorkFlowTest
```

### 🎯 Summary of Changes

| Aspect | SDK 7.1.0 | SDK 7.2.0 |
|--------|-----------|-----------|
| API surface | Stable | No changes |
| Breaking changes | - | None |
| Upgrade effort | - | Version bump only |
| Key focus | API simplification | Bug fixes & improvements |

### 🧪 Testing Your Migration

After upgrading, run the comprehensive workflow test:

```bash
>> WorkFlowTest
```

This command validates:
- ✅ Exchange creation and management
- ✅ Geometry processing (BREP, IFC, Mesh, Primitives)
- ✅ Parameter operations
- ✅ Synchronization workflows
- ✅ File download capabilities

---

**Migration Checklist:**
- [ ] Updated all package references to 7.2.0
- [ ] Restored NuGet packages and rebuilt the solution
- [ ] Tested core workflows with `WorkFlowTest`
- [ ] Verified all geometry types render correctly

</details>

---

<details>
<summary><strong>📜 Historical: SDK 6.3.0 → 7.1.0 Migration Guide</strong></summary>

This section documents the previous migration from SDK 6.3.0 to **Autodesk Data Exchange SDK 7.1.0** and is preserved here for reference.

#### Overview of Changes

This upgrade included significant API changes:
- **SDK Version**: Upgraded to `Autodesk.DataExchange 7.1.0`
- **API Simplification**: Removed `GeometryProperties` wrapper class
- **Explicit Format Specification**: Geometry format now required as explicit parameter

#### Key Dependency Updates

| Package | Previous Version | New Version | Impact |
|---------|------------------|-------------|---------|
| `Autodesk.DataExchange` | `6.3.0` | `7.1.0` | **Major** - Breaking changes to geometry creation APIs |

#### Breaking Changes

##### 1. `GeometryProperties` Class Removed

The `GeometryProperties` class that was previously used to wrap geometry parameters was **removed entirely**. All methods that accepted `GeometryProperties` now take individual parameters directly.

##### 2. `CreateFileGeometry` API Change

**Before (SDK 6.3.0):**
```csharp
var geometry = ElementDataModel.CreateFileGeometry(
    new GeometryProperties(filePath, renderStyle));
```

**After (SDK 7.1.0):**
```csharp
var geometry = ElementDataModel.CreateFileGeometry(
    filePath,
    GeometryFormat.Step,
    renderStyle,
    units);
```

Key differences:
- No `GeometryProperties` wrapper
- `GeometryFormat` enum is **required** as the second parameter
- `RenderStyle` and `Units` are optional parameters

##### 3. `CreatePrimitiveGeometry` API Change

**Before (SDK 6.3.0):**
```csharp
var geometry = ElementDataModel.CreatePrimitiveGeometry(
    new GeometryProperties(geomContainer, renderStyle));
```

**After (SDK 7.1.0):**
```csharp
var geometry = ElementDataModel.CreatePrimitiveGeometry(
    geomContainer,
    renderStyle,
    units);
```

Key differences:
- No `GeometryProperties` wrapper
- Pass the geometry object (`GeometryContainer`, `DesignPoint`, etc.) directly
- `RenderStyle` and `Units` are optional parameters

#### Migration Steps (6.3.0 → 7.1.0)

1. Update package references to 7.1.0
2. Add `using Autodesk.DataExchange.Core.Enums;` where needed
3. Update all `CreateFileGeometry` calls to use direct parameters with `GeometryFormat`
4. Update all `CreatePrimitiveGeometry` calls to pass geometry objects directly
5. Remove all references to `GeometryProperties` class

#### GeometryFormat Enum Values

| Value | Description | File Extensions |
|-------|-------------|-----------------|
| `GeometryFormat.Unknown` | Unknown format | - |
| `GeometryFormat.Step` | STEP file format | `.stp`, `.step` |
| `GeometryFormat.Obj` | OBJ mesh format | `.obj` |
| `GeometryFormat.Ifc` | IFC file format | `.ifc` |
| `GeometryFormat.Bimdex` | Bimdex geometry format | - |
| `GeometryFormat.LargePrimitive` | Large primitive format | - |

#### Method Signatures Reference

```csharp
public static FileGeometry CreateFileGeometry(
    string filePath,
    GeometryFormat format,
    RenderStyle renderStyle = null,
    Units units = null)

public static FileGeometry CreateFileGeometry(
    MemoryStream geometryStream,
    GeometryFormat format,
    RenderStyle renderStyle = null,
    Units units = null)

public static PrimitiveGeometry CreatePrimitiveGeometry(
    Autodesk.GeometryPrimitives.Data.Geometry geometry,
    RenderStyle renderStyle = null,
    Units units = null)

public static MeshGeometry CreateMeshGeometry(
    Autodesk.GeometryUtilities.MeshAPI.Mesh mesh,
    string meshName,
    Units units = null)
```

#### Summary (6.3.0 → 7.1.0)

| Aspect | SDK 6.3.0 | SDK 7.1.0 |
|--------|-----------|-----------|
| Geometry wrapper | `GeometryProperties` class | Direct parameters |
| File format | Inferred from file | Explicit `GeometryFormat` enum |
| API style | Wrapper object pattern | Direct parameter pattern |
| Required namespace | - | `Autodesk.DataExchange.Core.Enums` |

</details>

## 📖 Documentation

- [Autodesk Data Exchange SDK](https://aps.autodesk.com/en/docs/dx-sdk-beta/v1/developers_guide/overview/)
- [SDK Without UI Tutorial](https://aps.autodesk.com/en/docs/dx-sdk-beta/v1/tutorials/sdk-without-ui/create-an-exchange-container/)
- [Authentication Guide](https://aps.autodesk.com/en/docs/oauth/v2/developers_guide/overview/)

## 🤝 Contributing

This is a sample project for reference purposes. While direct contributions may not be accepted, you're encouraged to:

1. Fork the repository for your own modifications
2. Report issues or suggestions
3. Share improvements with the community

## 📄 License

This sample code is part of the Autodesk Data Exchange .NET SDK (Software Development Kit) beta. It is subject to the license covering the Autodesk Data Exchange .NET SDK (Software Development Kit) beta.

## ✍️ Authors

**Dhiraj Lotake** - *Autodesk*
**Hariom Sharma** - *Autodesk*

---

## 🆘 Support

For SDK-related questions and support:
- [Autodesk Platform Services Documentation](https://aps.autodesk.com/)
- [Community Forums](https://forums.autodesk.com/)
- [SDK Issues](https://github.com/autodesk-platform-services)

---

*This sample demonstrates the power and flexibility of the Autodesk Data Exchange SDK for building custom integrations and automating design data workflows.*
