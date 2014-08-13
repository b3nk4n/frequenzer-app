using PhoneKit.Framework.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frequenzer.App
{
    /// <summary>
    /// The app persistent settings.
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// The last interval in seconds.
        /// </summary>
        public static StoredObject<int> LastIntervalInSeconds = new StoredObject<int>("_lastIntervalInSeconds", 30);
    }
}
