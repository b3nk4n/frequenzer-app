using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Frequenzer.App.ViewModels
{
    public interface IMainViewModel
    {
        void Start();

        void Stop();

        void Pause();

        void Continue();

        int RoundTime { get; set; }

        ICommand StartCommand { get; }

        ICommand StopCommand { get; }

        ICommand PauseCommand { get; }

        ICommand ContinueCommand { get; }
    }
}
