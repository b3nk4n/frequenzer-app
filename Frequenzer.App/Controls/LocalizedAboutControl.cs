using Frequenzer.App.Resources;
using PhoneKit.Framework.Controls;
using PhoneKit.Framework.Core.Themeing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Frequenzer.App.Controls
{
    public class LocalizedAboutControl : AboutControlBase
    {
        /// <summary>
        /// Localizes the user controls contents and texts.
        /// </summary>
        protected override void LocalizeContent()
        {
            // app
            if (PhoneThemeHelper.IsLightThemeActive)
            {
                ApplicationIconSource = new Uri("/Assets/ApplicationIcon.png", UriKind.Relative);
            }
            else
            {
                ApplicationIconSource = new Uri("/Assets/ApplicationIconBlack.png", UriKind.Relative);
            }
            BackgroundTheme.Color = (Color)Application.Current.Resources["PhoneForegroundColor"];
            ApplicationTitle = AppResources.ApplicationTitle;
            ApplicationVersion = AppResources.ApplicationVersion;
            ApplicationAuthor = AppResources.ApplicationAuthor;
            ApplicationDescription = AppResources.ApplicationDescription;

            // buttons
            SupportAndFeedbackText = AppResources.SupportAndFeedback;
            SupportAndFeedbackEmail = "apps@bsautermeister.de";
            PrivacyInfoText = AppResources.PrivacyInfo;
            PrivacyInfoLink = "http://bsautermeister.de/privacy.php";
            RateAndReviewText = AppResources.RateAndReview;
            MoreAppsText = AppResources.MoreApps;
            MoreAppsSearchTerms = "Benjamin Sautermeister";

            // contributors
            ContributorsListVisibility = System.Windows.Visibility.Visible;
            IList<ContributorModel> contributors = new List<ContributorModel>();
            contributors.Add(new ContributorModel("/Assets/Images/font.png", "Johan Aakerlund"));
            SetContributorsList(contributors);
        }
    }
}
