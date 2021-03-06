﻿using System;
using System.IO;

using NLog;

using SkyNinja.Core.Classes;
using SkyNinja.Core.Classes.Factories;
using SkyNinja.Core.Exceptions;
using SkyNinja.Core.Helpers;

namespace SkyNinja.Core.Inputs.Skype
{
    /// <summary>
    /// Creates <see cref="SkypeInput"/> by Skype ID.
    /// </summary>
    internal class SkypeIdInputFactory : InputFactory
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public override Input CreateConnector(ParsedUri uri)
        {
            string skypeId = uri.Host;
            Logger.Debug("Trying Skype ID: {0} ...", skypeId);
            string applicationDataPath = Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData);
            string databasePath = Path.Combine(
                applicationDataPath, "Skype", skypeId, "main.db");
            Logger.Debug("Trying database path: {0} ...", databasePath);
            if (!File.Exists(databasePath))
            {
                throw new InvalidArgumentInternalException("Database file is not found for this Skype ID.");
            }
            Logger.Debug("Database file is found.");
            return new SkypeInput(databasePath);
        }
    }
}
