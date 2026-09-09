using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using ConsoleConnector.Samples;

// System.Text.Json, not Autodesk.Newtonsoft.Json: the SDK's Release NuGet ILRepacks and
// internalizes Autodesk.Newtonsoft.Json, so JsonConvert is inaccessible there (CS0122).

namespace ConsoleConnector.Driver
{
    // Session state only — never credentials. See CredentialsBootstrap for how
    // Forge/APS credentials are resolved (environment variables, then App.config).
    internal sealed class SessionData
    {
        public FolderInfo? Folder { get; set; }
        public Defaults Defaults { get; set; } = new();
        public LoadedExchangeInfo? LastExchange { get; set; }
        public string? LastExchangeTitle { get; set; }
    }

    internal static class SessionStore
    {
        private const string AppFolderName = "ApsDxConsole";
        private const string SessionFileName = "session.json";

        public static string SessionDirectory
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppFolderName);

                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                        "Library", "Application Support", AppFolderName);

                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    ".config", AppFolderName);
            }
        }

        private static readonly JsonSerializerOptions SessionJsonOptions = new JsonSerializerOptions { WriteIndented = true };

        public static string SessionFilePath => Path.Combine(SessionDirectory, SessionFileName);

        public static SessionData Load()
        {
            if (!File.Exists(SessionFilePath))
                return new SessionData();

            var json = File.ReadAllText(SessionFilePath);
            return JsonSerializer.Deserialize<SessionData>(json, SessionJsonOptions) ?? new SessionData();
        }

        public static void Save(SessionData session)
        {
            Directory.CreateDirectory(SessionDirectory);
            var json = JsonSerializer.Serialize(session, SessionJsonOptions);
            File.WriteAllText(SessionFilePath, json);
        }

        public static void Apply(SessionData session, SampleContext ctx)
        {
            ctx.Folder = session.Folder;
            ctx.LastExchange = session.LastExchange;
            ctx.LastExchangeTitle = session.LastExchange?.Title ?? session.LastExchangeTitle;

            ctx.Defaults.StepPath = session.Defaults.StepPath;
            ctx.Defaults.IfcPath = session.Defaults.IfcPath;
            ctx.Defaults.ObjPath = session.Defaults.ObjPath;
            ctx.Defaults.BuiltInSchemaId = session.Defaults.BuiltInSchemaId;
        }

        public static void Capture(SampleContext ctx, SessionData session)
        {
            session.Folder = ctx.Folder;
            session.LastExchange = ctx.LastExchange;
            session.LastExchangeTitle = ctx.LastExchange?.Title ?? ctx.LastExchangeTitle;
            session.Defaults = new Defaults
            {
                StepPath = ctx.Defaults.StepPath,
                IfcPath = ctx.Defaults.IfcPath,
                ObjPath = ctx.Defaults.ObjPath,
                BuiltInSchemaId = ctx.Defaults.BuiltInSchemaId,
            };
        }
    }
}
