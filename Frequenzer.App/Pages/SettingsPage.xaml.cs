using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Frequenzer.App.Pages
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // load settings
            //CheckBoxRunUnderLock.IsChecked = Settings.RunUnderLockScreen.Value;
            CheckBoxPreventLock.IsChecked = Settings.PreventLockScreen.Value;
            CheckBoxSoundEnabled.IsChecked = Settings.SoundEnabled.Value;
            CheckBoxIndicateRoundEnd.IsChecked = Settings.IndicateRoundEnd.Value;
            CheckBoxReadRoundCounter.IsChecked = Settings.ReadRoundCounter.Value;
            CheckBoxVibrationEnabled.IsChecked = Settings.VibrationEnabled.Value;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // save settings
            //Settings.RunUnderLockScreen.Value = CheckBoxRunUnderLock.IsChecked.Value;
            Settings.PreventLockScreen.Value = CheckBoxPreventLock.IsChecked.Value;
            Settings.SoundEnabled.Value = CheckBoxSoundEnabled.IsChecked.Value;
            Settings.IndicateRoundEnd.Value = CheckBoxIndicateRoundEnd.IsChecked.Value;
            Settings.ReadRoundCounter.Value = CheckBoxReadRoundCounter.IsChecked.Value;
            Settings.VibrationEnabled.Value = CheckBoxVibrationEnabled.IsChecked.Value;
        }
    }
}