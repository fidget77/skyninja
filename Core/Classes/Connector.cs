﻿using System;
using System.Threading.Tasks;

using NLog;

namespace SkyNinja.Core.Classes
{
    public abstract class Connector: IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public abstract Task Open();

        public abstract void Close();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
        }
    }
}
