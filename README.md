# Autodesk Data Exchange Console Connector

[![oAuth2](https://img.shields.io/badge/oAuth2-v2-green.svg)](http://developer.autodesk.com/)
![.NET](https://img.shields.io/badge/.NET%20Framework-4.8-blue.svg)
![SDK Version](https://img.shields.io/badge/Data%20Exchange%20SDK-7.1.0-orange.svg)
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
- [💻 Usage Examples](#-usage-examples) - See the console connector in action
- [📚 Command Reference](#-command-reference) - Complete command documentation  
- [🔄 Migration Guide](#-migration-guide-sdk-710-upgrade) - **SDK 7.1.0 Upgrade Guide**
- [🏗️ Architecture](#️-architecture) - Understand the codebase structure
- [🔧 Extending the Application](#-extending-the-application) - Add custom functionality

## 🎯 Key Features

### Core Functionality
- ✅ **Exchange Management** - Create, update, and retrieve exchanges
- ✅ **Multi-Format Geometry Processing** - BREP, IFC, Mesh, and Primitive geometries
- ✅ **Parameter Operations** - Add, modify, and delete instance/type parameters
- ✅ **Version Control** - Exchange synchronization and versioning
- ✅ **Data Export** - Download exchanges as STEP or OBJ files
- ✅ **Folder Management** - Set and manage working directories

### Developer Experience
- 🚀 **Professional Console Interface** - Clean, categorized output messages
- 📋 **Comprehensive Workflow Testing** - Complete end-to-end validation
- 💻 **Command-Based Architecture** - Extensible command pattern
- 🔧 **Error Handling** - Robust error management and user feedback
- 📖 **Built-in Help System** - Detailed command documentation

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
   - Valid Autodesk account with appropriate permissions

## 🚀 Quick Start

### 1. Clone and Setup
```bash
git clone https://github.com/your-repo/aps-dataexchange-console.git
cd aps-dataexchange-console
```

### 2. Install Dependencies
Follow the [Data Exchange .NET SDK installation guide](https://aps.autodesk.com/en/docs/dx-sdk-beta/v1/developers_guide/installing_the_sdk/#procedure):

**Option A: Visual Studio**
- Open `ConsoleConnector.sln`
- Build the solution (packages restore automatically)

**Option B: Command Line**
```bash
# Run from project root
BuildSolution.bat
```

### 3. Configuration
Update `src/ConsoleConnector/App.config` with your app credentials:
```xml
<appSettings>
    <add key="APS_CLIENT_ID" value="your_client_id" />
    <add key="APS_CLIENT_SECRET" value="your_client_secret" />
    <add key="APS_CALLBACK_URL" value="your_callback_url" />
</appSettings>
```

### 4. Run the Application
- Build and run the console application
- Complete OAuth authentication in the browser
- Start using commands in the console interface

## 💻 Usage Examples

### Basic Commands
```bash
# Get help
>> help

# Set working folder (using folder URL)
>> SetFolder [FolderUrl]
# OR set working folder (using individual parameters)
>> SetFolder [HubId] [Region] [ProjectUrn] [FolderUrn]

# Create a new exchange
>> CreateExchange [ExchangeTitle]

# Add BREP geometry
>> AddBrep [ExchangeTitle]

# Add instance parameters
>> AddInstanceParameter [ExchangeTitle] [ElementId] [ParameterName] [ParameterSchema] [ParameterValue] [ParameterValueDataType]

# Sync changes
>> SyncExchange [ExchangeTitle]

# Download exchange
>> GetExchange [ExchangeId] [CollectionId] [HubId] [ExchangeFileFormat]
```

### Complete Workflow Test
```bash
# Run comprehensive end-to-end test
>> WorkFlowTest
```

This command executes a complete workflow that:
1. Creates a new exchange
2. Adds multiple geometry types (BREP, IFC, Mesh, Primitives)
3. Adds instance and type parameters
4. Syncs to Version 1
5. Deletes some parameters
6. Adds more geometries and parameters
7. Syncs to Version 2
8. Downloads the final exchange

## 📚 Command Reference

| Command | Description | Example |
|---------|-------------|---------|
| `help` | Display all commands | `help` |
| `help [command]` | Get command details | `help CreateExchange` |
| `CreateExchange` | Create new exchange | `CreateExchange [ExchangeTitle]` |
| `AddBrep` | Add BREP geometry | `AddBrep [ExchangeTitle]` |
| `AddIFC` | Add IFC geometry | `AddIFC [ExchangeTitle]` |
| `AddMesh` | Add mesh geometry | `AddMesh [ExchangeTitle]` |
| `AddPrimitive` | Add primitives | `AddPrimitive [ExchangeTitle] [PrimitiveGeometry]` |
| `AddInstanceParameter` | Add instance parameter | `AddInstanceParameter [ExchangeTitle] [ElementId] [ParameterName] [ParameterSchema] [ParameterValue] [ParameterValueDataType]` |
| `AddTypeParameter` | Add type parameter | `AddTypeParameter [ExchangeTitle] [ElementId] [ParameterName] [ParameterSchema] [ParameterValue] [ParameterValueDataType]` |
| `DeleteInstanceParam` | Remove instance parameter | `DeleteInstanceParam [ExchangeTitle] [ElementId] [ParameterName]` |
| `DeleteTypeParam` | Remove type parameter | `DeleteTypeParam [ExchangeTitle] [ElementId] [ParameterName]` |
| `SyncExchange` | Sync exchange | `SyncExchange [ExchangeTitle]` |
| `GetExchange` | Download exchange | `GetExchange [ExchangeId] [CollectionId] [HubId] [ExchangeFileFormat]` |
| `SetFolder` | Set working folder | `SetFolder [FolderUrl]` or `SetFolder [HubId] [Region] [ProjectUrn] [FolderUrn]` |
| `WorkFlowTest` | Run complete test | `WorkFlowTest` |
| `Exit` | Close application | `Exit` |

## 🏗️ Architecture

```
ConsoleConnector/
├── Commands/           # Command implementations
│   ├── CreateExchangeCommand.cs
│   ├── CreateBrepCommand.cs
│   ├── WorkFlowTestCommand.cs
│   └── ...
├── Helper/            # Utility classes
│   ├── ConsoleAppHelper.cs
│   ├── GeometryHelper.cs
│   └── ParameterHelper.cs
├── Interfaces/        # Abstractions
└── Assets/           # Sample geometry files
```

### Key Components

- **Command Pattern**: Each operation is implemented as a separate command class
- **Helper Classes**: Reusable utilities for geometry, parameters, and console operations
- **Interface Abstractions**: Clean separation of concerns
- **Asset Management**: Sample files for testing and demonstration

## 🔧 Extending the Application

### Adding New Commands

1. Create a new command class inheriting from `Command`
2. Implement required methods (`Execute`, `Clone`, `ValidateOptions`)
3. Register the command in `ConsoleAppHelper`

```csharp
public class MyCustomCommand : Command
{
    public override async Task<bool> Execute()
    {
        Console.WriteLine("[CUSTOM] Executing my command");
        // Implementation here
        return true;
    }
    
    public override Command Clone()
    {
        return new MyCustomCommand(this);
    }
}
```

### Adding Command Options

1. Create option class in `Commands/Options/`
2. Add to command's `Options` list
3. Use `GetOption<T>()` to access values

## 🔄 Migration Guide: SDK 7.1.0 Upgrade

This section documents the migration from SDK 6.3.0 to **Autodesk Data Exchange SDK 7.1.0**.

### 📋 Overview of Changes

This upgrade includes significant API changes:
- **SDK Version**: Upgraded to `Autodesk.DataExchange 7.1.0`
- **API Simplification**: Removed `GeometryProperties` wrapper class
- **Explicit Format Specification**: Geometry format now required as explicit parameter

### 🚀 Key Dependency Updates

| Package | Previous Version | New Version | Impact |
|---------|------------------|-------------|---------|
| `Autodesk.DataExchange` | `6.3.0` | `7.1.0` | **Major** - Breaking changes to geometry creation APIs |

### ⚠️ Breaking Changes

#### 1. `GeometryProperties` Class Removed

The `GeometryProperties` class that was previously used to wrap geometry parameters has been **removed entirely**. All methods that accepted `GeometryProperties` now take individual parameters directly.

#### 2. `CreateFileGeometry` API Change

**Before (SDK 6.3.0):**
```csharp
// Required wrapping path and render style in GeometryProperties
var geometry = ElementDataModel.CreateFileGeometry(
    new GeometryProperties(filePath, renderStyle));
```

**After (SDK 7.1.0):**
```csharp
// Pass parameters directly with explicit GeometryFormat
var geometry = ElementDataModel.CreateFileGeometry(
    filePath, 
    GeometryFormat.Step,  // Explicit format required
    renderStyle,          // Optional: RenderStyle
    units);               // Optional: Units
```

**Key differences:**
- No `GeometryProperties` wrapper
- `GeometryFormat` enum is now **required** as the second parameter
- `RenderStyle` and `Units` are optional parameters

#### 3. `CreatePrimitiveGeometry` API Change

**Before (SDK 6.3.0):**
```csharp
// For GeometryContainer
var geometry = ElementDataModel.CreatePrimitiveGeometry(
    new GeometryProperties(geomContainer, renderStyle));

// For DesignPoint
var geometry = ElementDataModel.CreatePrimitiveGeometry(
    new GeometryProperties(designPoint, renderStyle));
```

**After (SDK 7.1.0):**
```csharp
// For GeometryContainer - pass directly
var geometry = ElementDataModel.CreatePrimitiveGeometry(
    geomContainer,  // Autodesk.GeometryPrimitives.Data.Geometry
    renderStyle,    // Optional: RenderStyle
    units);         // Optional: Units

// For DesignPoint - pass directly
var geometry = ElementDataModel.CreatePrimitiveGeometry(
    designPoint,    // Autodesk.GeometryPrimitives.Data.Geometry
    renderStyle,    // Optional: RenderStyle
    units);         // Optional: Units
```

**Key differences:**
- No `GeometryProperties` wrapper
- Pass the geometry object (`GeometryContainer`, `DesignPoint`, etc.) directly
- `RenderStyle` and `Units` are optional parameters

### 🔧 Migration Steps

#### Step 1: Update Package References
Update your `packages.config` or project file:

```xml
<package id="Autodesk.DataExchange" version="7.1.0" targetFramework="net48" />
```

#### Step 2: Add New Using Statement
Add the following using statement to access the `GeometryFormat` enum:

```csharp
using Autodesk.DataExchange.Core.Enums;
```

#### Step 3: Update `CreateFileGeometry` Calls

**Before:**
```csharp
var path = "path/to/geometry.stp";
var geometry = ElementDataModel.CreateFileGeometry(
    new GeometryProperties(path, renderStyle));
```

**After:**
```csharp
var path = "path/to/geometry.stp";
var geometry = ElementDataModel.CreateFileGeometry(
    path, 
    GeometryFormat.Step, 
    renderStyle);
```

#### Step 4: Update `CreatePrimitiveGeometry` Calls

**Before (GeometryContainer):**
```csharp
var geomContainer = new GeometryContainer();
geomContainer.Curves.Add(new Line(...));
var geometry = ElementDataModel.CreatePrimitiveGeometry(
    new GeometryProperties(geomContainer, renderStyle));
```

**After (GeometryContainer):**
```csharp
var geomContainer = new GeometryContainer();
geomContainer.Curves.Add(new Line(...));
var geometry = ElementDataModel.CreatePrimitiveGeometry(
    geomContainer, 
    renderStyle);
```

**Before (DesignPoint):**
```csharp
var point = new DesignPoint(x, y, z);
var geometry = ElementDataModel.CreatePrimitiveGeometry(
    new GeometryProperties(point, renderStyle));
```

**After (DesignPoint):**
```csharp
var point = new DesignPoint(x, y, z);
var geometry = ElementDataModel.CreatePrimitiveGeometry(
    point, 
    renderStyle);
```

#### Step 5: Helper Method for File Format Detection (Optional)

If you need to determine the geometry format from file extensions dynamically:

```csharp
private GeometryFormat GetGeometryFormat(string fileName)
{
    var extension = Path.GetExtension(fileName).ToLowerInvariant();
    switch (extension)
    {
        case ".stp":
        case ".step":
            return GeometryFormat.Step;
        case ".ifc":
            return GeometryFormat.Ifc;
        case ".obj":
            return GeometryFormat.Obj;
        default:
            return GeometryFormat.Unknown;
    }
}
```

### 📚 GeometryFormat Enum Values

| Value | Description | File Extensions |
|-------|-------------|-----------------|
| `GeometryFormat.Unknown` | Unknown format | - |
| `GeometryFormat.Step` | STEP file format | `.stp`, `.step` |
| `GeometryFormat.Obj` | OBJ mesh format | `.obj` |
| `GeometryFormat.Ifc` | IFC file format | `.ifc` |
| `GeometryFormat.Bimdex` | Bimdex geometry format | - |
| `GeometryFormat.LargePrimitive` | Large primitive format | - |

### 📖 Method Signatures Reference

#### CreateFileGeometry
```csharp
public static FileGeometry CreateFileGeometry(
    string filePath, 
    GeometryFormat format, 
    RenderStyle renderStyle = null, 
    Units units = null)
```

There is also an overload that accepts a `MemoryStream`:
```csharp
public static FileGeometry CreateFileGeometry(
    MemoryStream geometryStream, 
    GeometryFormat format, 
    RenderStyle renderStyle = null, 
    Units units = null)
```

#### CreatePrimitiveGeometry
```csharp
public static PrimitiveGeometry CreatePrimitiveGeometry(
    Autodesk.GeometryPrimitives.Data.Geometry geometry, 
    RenderStyle renderStyle = null, 
    Units units = null)
```

#### CreateMeshGeometry
```csharp
public static MeshGeometry CreateMeshGeometry(
    Autodesk.GeometryUtilities.MeshAPI.Mesh mesh, 
    string meshName, 
    Units units = null)
```

### 🎯 Summary of Changes

| Aspect | SDK 6.3.0 | SDK 7.1.0 |
|--------|-----------|-----------|
| Geometry wrapper | `GeometryProperties` class | Direct parameters |
| File format | Inferred from file | Explicit `GeometryFormat` enum |
| API style | Wrapper object pattern | Direct parameter pattern |
| Required namespace | - | `Autodesk.DataExchange.Core.Enums` |

### 🧪 Testing Your Migration

After migration, run the comprehensive workflow test:

```bash
>> WorkFlowTest
```

This command validates:
- ✅ Exchange creation and management
- ✅ Geometry processing (BREP, IFC, Mesh, Primitives)
- ✅ Parameter operations
- ✅ Synchronization workflows
- ✅ File download capabilities

### 🐛 Troubleshooting Common Issues

#### Issue: `GeometryProperties` could not be found
**Error**: `The type or namespace name 'GeometryProperties' could not be found`
**Solution**: Update all `CreateFileGeometry` and `CreatePrimitiveGeometry` calls to use the new direct parameter syntax as shown in this guide.

#### Issue: Cannot convert from `DesignPoint` to `Point`
**Error**: `Argument 1: cannot convert from 'Autodesk.GeometryPrimitives.Data.DX.DesignPoint' to 'Autodesk.GeometryPrimitives.Data.Point'`
**Solution**: Pass `DesignPoint` directly to `CreatePrimitiveGeometry` instead of wrapping it in a `GeometryContainer`:

```csharp
// Correct
var point = new DesignPoint(x, y, z);
ElementDataModel.CreatePrimitiveGeometry(point, renderStyle);

// Incorrect - don't add DesignPoint to GeometryContainer.Points
var container = new GeometryContainer();
container.Points.Add(point); // This will cause a type error
```

#### Issue: Missing GeometryFormat namespace
**Error**: `The name 'GeometryFormat' does not exist in the current context`
**Solution**: Add the using statement: `using Autodesk.DataExchange.Core.Enums;`

---

**Migration Checklist:**
- [ ] Updated all package references to 7.1.0
- [ ] Added `using Autodesk.DataExchange.Core.Enums;` where needed
- [ ] Updated all `CreateFileGeometry` calls to use direct parameters with `GeometryFormat`
- [ ] Updated all `CreatePrimitiveGeometry` calls to pass geometry objects directly
- [ ] Removed all references to `GeometryProperties` class
- [ ] Tested core workflows with `WorkFlowTest`
- [ ] Verified all geometry types render correctly

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
