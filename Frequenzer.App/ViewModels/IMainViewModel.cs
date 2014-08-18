using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Frequenzer.App.ViewModels
{
    public enum TimerState
    {
        Running,
        Stopped,
        Paused
    }

    public interface IMainViewModel
    {
        void Start();

        void Stop();

        void Pause();

        void Continue();

        void UpdateCommands();

        TimerState State { get; set; }

        DateTime StartTime { get; set; }

        DateTime PauseStartTime { get; set; }

        double RoundTime { get; set; }

        double CurrentValueAngle { get; } 

        ICommand StartCommand { get; }

        ICommand StopCommand { get; }

        ICommand PauseCommand { get; }

        ICommand ContinueCommand { get; }

        ICommand IncrementRoundTimeCommand { get; }

        ICommand DecrementRoundTimeCommand { get; }
    }
}
