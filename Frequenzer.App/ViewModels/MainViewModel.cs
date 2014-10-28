using Microsoft.Xna.Framework.Audio;
using PhoneKit.Framework.Audio;
using PhoneKit.Framework.Core.MVVM;
using PhoneKit.Framework.OS;
using PhoneKit.Framework.Voice;
using System;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Resources;
using System.Windows.Threading;

namespace Frequenzer.App.ViewModels
{
    /// <summary>
    /// The main pages view model.
    /// </summary>
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        private const string ALARM_FILE_PATH = "Assets/Audio/alarm1.wav";

        private double _roundTime;
        private DateTime _startTime = DateTime.MaxValue;
        private DateTime _pauseStartTime = DateTime.MaxValue; // pause time has to be subtracted

        private DispatcherTimer _timer = new DispatcherTimer();

        /// <summary>
        /// The current timer state. Required because DispatcherTimer.IsEnabled has a short delay.
        /// </summary>
        private TimerState _timerState = TimerState.Stopped; 

        /// <summary>
        /// The alarm sound.
        /// </summary>
        private SoundEffect _alarmSound;

        private DelegateCommand _startCommand;
        private DelegateCommand _restartCommand;
        private DelegateCommand _stopCommand;
        private DelegateCommand _pauseCommand;
        private DelegateCommand _continueCommand;
        private DelegateCommand _incrementRoundTimeCommand;
        private DelegateCommand _decrementRoundTimeCommand;

        /// <summary>
        /// The number of ticks per second
        /// </summary>
        private const int TIME_FACTOR = 40;

        /// <summary>
        /// The current value of the last tick.
        /// </summary>
        private double _lastValue = -1.0;

        /// <summary>
        /// The current value millisecond part of the last tick.
        /// </summary>
        private double _lastValueMillisPart = -1.0;

        public MainViewModel()
        {
            _timer.Interval = TimeSpan.FromSeconds(1.0 / TIME_FACTOR);
            _timer.Tick += (s, e) =>
            {
                double current = CurrentValue;
                double currentMillisPart = current - (int)current;

                // check second change
                if (currentMillisPart < _lastValueMillisPart)
                {
                    _lastValueMillisPart = currentMillisPart;
                    FullSecondsEvent(current);
                }

                // check round change
                if (current < _lastValue)
                {
                    _lastValue = current;
                    RoundEndedEvent();
                }

                _lastValue = current;
                _lastValueMillisPart = currentMillisPart;

                // update ui
                UpdateTimerUI();

            };
            RoundTime = Settings.LastRoundTimeInSeconds.Value;

            InitializeCommands();
            InitializeSounds();
        }

        /// <summary>
        /// Called when a full seconds has elapsed.
        /// </summary>
        /// <param name="current">The current time value counting upwards.</param>
        private void FullSecondsEvent(double current)
        {
            if (Settings.SoundEnabled.Value)
            {
                if (RoundTime >= 3 && current > RoundTime - 2 && Settings.IndicateRoundEnd.Value)
                {
                    // pre-alarm
                    _alarmSound.Play(0.05f, -0.5f, 0.0f);
                }
            }
        }

        /// <summary>
        /// Called when a round ends.
        /// </summary>
        private async void RoundEndedEvent()
        {
            if (Settings.VibrationEnabled.Value)
            {
                VibrationHelper.Vibrate(0.33);
            }

            if (Settings.SoundEnabled.Value)
            {
                if (Settings.ReadRoundCounter.Value && RoundTime >= 3)
                {
                    try
                    {
                        await Speech.Instance.Synthesizer.SpeakTextAsync(RoundCounter.ToString());
                    }
                    catch (Exception ex) // SPERR_SYSTEM_CALL_INTERRUPTED 0x80045508
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
                else
                {
                    // alarm
                    _alarmSound.Play(1.0f, -0.3f, 0.0f);
                }
            }
            
        }

        /// <summary>
        /// Initializes the sound effect.
        /// </summary>
        private void InitializeSounds()
        {
            StreamResourceInfo alarmResource = App.GetResourceStream(new Uri(ALARM_FILE_PATH, UriKind.Relative));
            SoundEffects.Instance.Load(ALARM_FILE_PATH, alarmResource.Stream);
            _alarmSound = SoundEffects.Instance[ALARM_FILE_PATH];
        }

        /// <summary>
        /// Initializes the commands
        /// </summary>
        private void InitializeCommands()
        {
            _startCommand = new DelegateCommand(() =>
            {
                Start();
                Settings.LastRoundTimeInSeconds.Value = RoundTime;
                UpdateCommands();
            }, () =>
            {
                return _timerState == TimerState.Stopped;
            });

            _restartCommand = new DelegateCommand(() =>
            {
                Start();
                UpdateCommands();
            }, () =>
            {
                return _timerState != TimerState.Stopped;
            });

            _stopCommand = new DelegateCommand(() =>
            {
                Stop();
                UpdateCommands();
            }, () =>
            {
                return _timerState != TimerState.Stopped;
            });

            _pauseCommand = new DelegateCommand(() =>
            {
                Pause();
                UpdateCommands();
            }, () =>
            {
                return _timerState == TimerState.Running;
            });

            _continueCommand = new DelegateCommand(() =>
            {
                Continue();
                UpdateCommands();
            }, () =>
            {
                return _timerState == TimerState.Paused;
            });

            _incrementRoundTimeCommand = new DelegateCommand(() =>
            {
                if (RoundTime < 3599)
                {
                    RoundTime++;
                }
                UpdateCommands();
            }, () =>
            {
                return RoundTime < 3599;
            });

            _decrementRoundTimeCommand = new DelegateCommand(() =>
            {
                if (RoundTime > 1)
                {
                    RoundTime--;
                }
                UpdateCommands();
            }, () =>
            {
                return RoundTime > 1;
            });
        }

        public void UpdateCommands()
        {
            _startCommand.RaiseCanExecuteChanged();
            _restartCommand.RaiseCanExecuteChanged();
            _stopCommand.RaiseCanExecuteChanged();
            _pauseCommand.RaiseCanExecuteChanged();
            _continueCommand.RaiseCanExecuteChanged();
            _incrementRoundTimeCommand.RaiseCanExecuteChanged();
            _decrementRoundTimeCommand.RaiseCanExecuteChanged();
        }

        public TimerState State
        {
            get
            {
                return _timerState;
            }
            set
            {
                _timerState = value;
                if (_timerState == TimerState.Running)
                {
                    _timer.Start();
                }
                else
                {
                    _timer.Stop();
                }
            }
        }

        public void Start()
        {
            _lastValue = -1.0;
            StartTime = DateTime.Now;
            State = TimerState.Running;
        }

        public void Stop()
        {
            // no UI update here!
            _startTime = DateTime.MaxValue;

            State = TimerState.Stopped;
        }

        public void Pause()
        {
            State = TimerState.Paused;
            _pauseStartTime = DateTime.Now;
        }

        public void Continue()
        {
            var pauseTime = DateTime.Now - _pauseStartTime;

            // no UI updates here!
            _startTime = StartTime.Add(pauseTime);
            _pauseStartTime = DateTime.MaxValue;

            State = TimerState.Running;
        }

        public double RoundTime
        {
            get
            {
                return _roundTime;
            }
            set
            {
                if (_roundTime != value)
                {
                    _roundTime = value;
                    NotifyPropertyChanged("RoundTime");
                }

            }
        }

        public int RoundCounter
        {
            get
            {
                return (int)Math.Max(TimeSinceStart / RoundTime, 0);
            }
        }

        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                _startTime = value;
                UpdateTimerUI();
            }
        }

        public DateTime PauseStartTime
        {
            get
            {
                return _pauseStartTime;
            }
            set
            {
                _pauseStartTime = value;
                UpdateTimerUI();
            }
        }

        private void UpdateTimerUI()
        {
            NotifyPropertyChanged("RoundCounter");
            NotifyPropertyChanged("CurrentValue");
            NotifyPropertyChanged("CurrentValueAngle");
        }

        public double TimeSinceStart
        {
            get
            {
                if (State == TimerState.Paused && _pauseStartTime != DateTime.MaxValue)
                {
                    return (_pauseStartTime - _startTime).TotalSeconds;
                }
                else
                {
                    return (DateTime.Now - _startTime).TotalSeconds;
                }
            }
        }

        public double CurrentValue
        {
            get
            {
                return (TimeSinceStart % RoundTime);
            }
        }

        public double CurrentValueAngle
        {
            get
            {
                return 10 + (CurrentValue / RoundTime) * 340.0;
            }
        }

        public ICommand StartCommand
        {
            get { return _startCommand; }
        }

        public ICommand RestartCommand
        {
            get { return _restartCommand; }
        }

        public ICommand StopCommand
        {
            get { return _stopCommand; }
        }

        public ICommand PauseCommand
        {
            get { return _pauseCommand; }
        }

        public ICommand ContinueCommand
        {
            get { return _continueCommand; }
        }


        public ICommand IncrementRoundTimeCommand
        {
            get { return _incrementRoundTimeCommand; }
        }

        public ICommand DecrementRoundTimeCommand
        {
            get { return _decrementRoundTimeCommand; }
        }
    }
}
