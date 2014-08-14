using Microsoft.Xna.Framework.Audio;
using PhoneKit.Framework.Audio;
using PhoneKit.Framework.Core.MVVM;
using System;
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

        private int _roundTime;
        private int _currentValue;
        private int _roundCounter;

        private DispatcherTimer _timer = new DispatcherTimer();

        private enum TimerState
        {
            Running,
            Stopped,
            Paused
        }

        /// <summary>
        /// The current timer state. Required because DispatcherTimer.IsEnabled has a short delay.
        /// </summary>
        private TimerState _timerState = TimerState.Stopped; 

        /// <summary>
        /// The alarm sound.
        /// </summary>
        private SoundEffectInstance _alarmSound;

        private DelegateCommand _startCommand;
        private DelegateCommand _stopCommand;
        private DelegateCommand _pauseCommand;
        private DelegateCommand _continueCommand;

        public MainViewModel()
        {
            _timer.Interval = TimeSpan.FromSeconds(1.0);
            _timer.Tick += (s, e) =>
            {
                CurrentValue--;

                if (CurrentValue <= 0)
                {
                    RoundCounter++;
                    CurrentValue = RoundTime;

                    _alarmSound.Play();
                }
            };
            RoundTime = Settings.LastIntervalInSeconds.Value;
            CurrentValue = RoundTime;

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
            SoundEffect sound = SoundEffects.Instance[ALARM_FILE_PATH];
            _alarmSound = sound.CreateInstance();
        }

        /// <summary>
        /// Initializes the commands
        /// </summary>
        private void InitializeCommands()
        {
            _startCommand = new DelegateCommand(() =>
            {
                Start();
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
        }

        private void UpdateCommands()
        {
            _startCommand.RaiseCanExecuteChanged();
            _stopCommand.RaiseCanExecuteChanged();
            _pauseCommand.RaiseCanExecuteChanged();
            _continueCommand.RaiseCanExecuteChanged();
        }

        public void Start()
        {
            RoundCounter = 0;
            _timer.Start();
            _timerState = TimerState.Running;
        }

        public void Stop()
        {
            _timer.Stop();
            _timerState = TimerState.Stopped;
            CurrentValue = RoundTime;
        }

        public void Pause()
        {
            _timer.Stop();
            _timerState = TimerState.Paused;
        }

        public void Continue()
        {
            _timer.Start();
            _timerState = TimerState.Running;
        }

        public int RoundTime
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
                return _roundCounter;
            }
            set
            {
                if (_roundCounter != value)
                {
                    _roundCounter = value;
                    NotifyPropertyChanged("RoundCounter");
                }
            }
        }

        public int CurrentValue
        {
            get
            {
                return _currentValue;
            }
            set
            {
                if (_currentValue != value)
                {
                    _currentValue = value;
                    NotifyPropertyChanged("CurrentValue");
                    NotifyPropertyChanged("CurrentValueAngle");
                }
            }
        }

        public double CurrentValueAngle
        {
            get
            {
                return (1 - ((double)_currentValue / _roundTime)) * 360.0;
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
    }
}
