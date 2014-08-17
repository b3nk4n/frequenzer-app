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

namespace Frequenzer.App
{
    public partial class MainPage : PhoneApplicationPage
    {
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

            IKernel kernel = new StandardKernel(new MainModule());
            this.DataContext = kernel.Get<IMainViewModel>();

            // startup animation
            StartupAnimation.Begin();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // fire startup events
            StartupActionManager.Instance.Fire();
        }
    }
}