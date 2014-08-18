using Microsoft.Xna.Framework.Audio;
using PhoneKit.Framework.Audio;
using PhoneKit.Framework.Core.MVVM;
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
        private DelegateCommand _stopCommand;
        private DelegateCommand _pauseCommand;
        private DelegateCommand _continueCommand;
        private DelegateCommand _incrementRoundTimeCommand;
        private DelegateCommand _decrementRoundTimeCommand;

        private const int TIME_FACTOR = 10;

        public MainViewModel()
        {
            int lastSecValue = 0;
            _timer.Interval = TimeSpan.FromSeconds(1.0 / TIME_FACTOR);
            _timer.Tick += async (s, e) =>
            {
                UpdateTimer();
                double current = CurrentValue;

                // check for change in second value
                if (lastSecValue != (int)current)
                {
                    lastSecValue = (int)current;
                    if (current < 1 && TimeSinceStart > 1)
                    {
                        if (Settings.ReadRoundCounter.Value)
                        {
                            await Speech.Instance.Synthesizer.SpeakTextAsync(RoundCounter.ToString());
                        }
                        else
                        {
                            _alarmSound.Play(1.0f, -0.3f, 0.0f);
                        }
                    }
                    else if (RoundTime >= 3 && current > RoundTime - 2 && Settings.IndicateRoundEnd.Value)
                    {
                        _alarmSound.Play(0.05f, -0.5f, 0.0f);
                    }
                }
            };
            RoundTime = Settings.LastRoundTimeInSeconds.Value;

            InitializeCommands();
            InitializeSounds();
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
                if (RoundTime < 999)
                {
                    RoundTime++;
                }
                UpdateCommands();
            }, () =>
            {
                return RoundTime < 999;
            });

            _decrementRoundTimeCommand = new DelegateCommand(() =>
            {
                if (RoundTime > 3)
                {
                    RoundTime--;
                }
                UpdateCommands();
            }, () =>
            {
                return RoundTime > 3;
            });
        }

        public void UpdateCommands()
        {
            _startCommand.RaiseCanExecuteChanged();
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
                return (int)(TimeSinceStart / RoundTime);
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
                UpdateTimer();
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
                UpdateTimer();
            }
        }

        private void UpdateTimer()
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
                return ((double)TimeSinceStart / _roundTime) * 360.0;
            }
        }

        public ICommand StartCommand
        {
            get { return _startCommand; }
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
