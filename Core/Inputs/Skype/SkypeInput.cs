﻿using System;
using System.Data.Common;
using System.Data.SQLite;
using System.Threading.Tasks;

using NLog;

using SkyNinja.Core.Classes;
using SkyNinja.Core.Exceptions;

namespace SkyNinja.Core.Inputs.Skype
{
    internal class SkypeInput: Input
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private const string GetChatsQuery = @"
            select id as id,
                   name as name,
                   friendlyname as friendlyName,
                   timestamp as timestamp,
                   activity_timestamp as activityTimestamp,
                   topic as topic
            from chats
        ";

        private readonly string databasePath;

        private SQLiteConnection connection;

        public SkypeInput(string databasePath)
        {
            this.databasePath = databasePath;
        }

        public override async Task Open()
        {
            string connectionString = String.Format("Data Source={0};Read Only=True", databasePath);
            Logger.Info("Connection string: {0}", connectionString);
            connection = new SQLiteConnection(connectionString);
            Logger.Info("Opening ...");
            try
            {
                await connection.OpenAsync();
            }
            catch (SQLiteException e)
            {
                throw new InternalException("Failed to open database.", e);
            }
            Logger.Info("Opened.");
            // Add trace handler.
            connection.Trace += ConnectionTrace;
        }

        public override async Task<ChatEnumerator> GetChatsAsync()
        {
            Logger.Info("Getting chats ...");
            using (SQLiteCommand command = new SQLiteCommand(GetChatsQuery, connection))
            {
                DbDataReader reader = await command.ExecuteReaderAsync();
                return new SkypeChatEnumerator(reader);
            }
        }

        public override void Close()
        {
            if (connection != null)
            {
                connection.Close();
            }
        }

        private void ConnectionTrace(object sender, TraceEventArgs e)
        {
            Logger.Trace("SQL: {0}", e.Statement);
        }
    }
}
