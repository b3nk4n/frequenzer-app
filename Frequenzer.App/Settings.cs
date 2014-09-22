using Frequenzer.App.ViewModels;
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
        /// Whether the app prevents the lockscreen.
        /// </summary>
        public static StoredObject<bool> PreventLockScreen = new StoredObject<bool>("_preventLock", true);


        /// <summary>
        /// Whether the app runs under lockscreen.
        /// </summary>
        public static StoredObject<bool> RunUnderLockScreen = new StoredObject<bool>("_underLock", true);

        /// <summary>
        /// Whether the app should indicate the round end (silent peep on 3 and 2).
        /// </summary>
        public static StoredObject<bool> IndicateRoundEnd = new StoredObject<bool>("_indicateRoundEnd", true);

        /// <summary>
        /// Whether the app should read the round counter.
        /// </summary>
        public static StoredObject<bool> ReadRoundCounter = new StoredObject<bool>("_readRound", true);

        /// <summary>
        /// Whether the vibration is enabled.
        /// </summary>
        public static StoredObject<bool> VibrationEnabled = new StoredObject<bool>("_vibration", true);

        /// <summary>
        /// Whether the sound is enabled.
        /// </summary>
        public static StoredObject<bool> SoundEnabled = new StoredObject<bool>("_sound", true);

        /// <summary>
        /// The round time in seconds.
        /// </summary>
        internal static StoredObject<double> LastRoundTimeInSeconds = new StoredObject<double>("_lastRoundTime", 30.0);

        /// <summary>
        /// The last timer state.
        /// </summary>
        internal static StoredObject<TimerState> LastTimerState = new StoredObject<TimerState>("_timerState", TimerState.Stopped);

        /// <summary>
        /// The start time in seconds.
        /// </summary>
        internal static StoredObject<DateTime> LastStartTimeInSeconds = new StoredObject<DateTime>("_lastStartTime", DateTime.MaxValue);

        /// <summary>
        /// The pause time in seconds.
        /// </summary>
        internal static StoredObject<DateTime> LastPauseTimeInSeconds = new StoredObject<DateTime>("_lastPauseTime", DateTime.MaxValue);
    }
}
