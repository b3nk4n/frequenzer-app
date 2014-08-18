using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Frequenzer.App.Resources;
using PhoneKit.Framework.Support;
using Ninject;
using Frequenzer.App.ViewModels;
using System.Windows.Media;
using System.Diagnostics;

namespace Frequenzer.App
{
    public partial class MainPage : PhoneApplicationPage
    {
        private IMainViewModel _mainViewModel;

        // Konstruktor
        public MainPage()
        {
            InitializeComponent();

            // register startup actions
            StartupActionManager.Instance.Register(5, ActionExecutionRule.Equals, () =>
            {
                FeedbackManager.Instance.StartFirst();
            });
            StartupActionManager.Instance.Register(10, ActionExecutionRule.Equals, () =>
            {
                FeedbackManager.Instance.StartSecond();
            });

            _mainViewModel = App.Injector.Get<IMainViewModel>();
            this.DataContext = _mainViewModel;

            // startup animation
            StartupAnimation.Begin();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ApplySettings();

            // fire startup events
            StartupActionManager.Instance.Fire();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            Settings.LastTimerState.Value = _mainViewModel.State;
        }

        private void ApplySettings()
        {
            PhoneApplicationService.Current.UserIdleDetectionMode = (Settings.PreventLockScreen.Value ? IdleDetectionMode.Disabled : IdleDetectionMode.Enabled);
            PhoneApplicationService.Current.ApplicationIdleDetectionMode = IdleDetectionMode.Disabled;

            PhoneApplicationFrame rootFrame = App.Current.RootVisual as PhoneApplicationFrame;
            if (rootFrame != null)
            {
                rootFrame.Obscured += (s, e) =>
                {

                };
                rootFrame.Unobscured += (s, e) =>
                {

                };
            }

            if (Settings.LastStartTimeInSeconds.Value != DateTime.MaxValue)
            {
                _mainViewModel.State = Settings.LastTimerState.Value;

                // ensure no longer than 1 day
                DateTime startTime = Settings.LastStartTimeInSeconds.Value;
                if ((DateTime.Now - startTime).TotalDays > 1)
                {
                    _mainViewModel.State = TimerState.Stopped;
                }

                switch (_mainViewModel.State)
                {
                    case TimerState.Running:
                        VisualStateManager.GoToState(this, "RunningState", true);
                        break;
                    case TimerState.Stopped:
                        // NOP
                        break;
                    case TimerState.Paused:
                        VisualStateManager.GoToState(this, "PausedState", true);
                        break;
                }     
                _mainViewModel.StartTime = startTime;
                _mainViewModel.PauseStartTime = Settings.LastPauseTimeInSeconds.Value;
                _mainViewModel.UpdateCommands();
            }
        }
    }
}