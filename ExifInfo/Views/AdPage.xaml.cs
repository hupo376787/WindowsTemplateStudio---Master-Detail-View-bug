using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Advertising.WinRT.UI;
using Windows.UI.Xaml.Shapes;
using Windows.UI;
using ExifInfo.Models;
using Windows.Storage;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ExifInfo.Views
{
    public sealed partial class AdPage : Page, INotifyPropertyChanged
    {

        private int adCount = 0;
        private InterstitialAd interstitialAd;
        string strCurrentLanguage = "en-us";

        private List<Staf> _staf = new List<Staf>();
        private double _width;
        private double _height;
        private Random ran = new Random();
        private DispatcherTimer _time;
        private Color accentColor = Colors.Green;

        public AdPage()
        {
            InitializeComponent();

            strCurrentLanguage = (Application.Current as App).strCurrentLanguage;
            accentColor = (Color)Application.Current.Resources["SystemAccentColor"];

            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                gridMain.Margin = new Thickness(0, 48, 0, 0);

            // Instantiate the interstitial video ad
            interstitialAd = new InterstitialAd();

            // Attach event handlers
            interstitialAd.ErrorOccurred += OnAdError;
            interstitialAd.AdReady += OnAdReady;
            interstitialAd.Cancelled += OnAdCancelled;
            interstitialAd.Completed += OnAdCompleted;

            if (ApplicationData.Current.LocalSettings.Values["nShowAdTimes"] != null)
                adCount = Convert.ToInt16(ApplicationData.Current.LocalSettings.Values["nShowAdTimes"]);
            else
                adCount = 0;

            _time = new DispatcherTimer();
            _time.Interval = TimeSpan.FromTicks(500);
            _time.Tick += Time_Tick;
            RandomStaf();
            _time.Start();
            _width = Window.Current.Bounds.Width;
            _height = Window.Current.Bounds.Height;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // This is an error handler for the interstitial ad.
        private void OnErrorOccurred(object sender, AdErrorEventArgs e)
        {
            textAdStatus.Text = "An error occurred while loading ad :(";
        }

        // This is an event handler for the ad control. It's invoked when the ad is refreshed.
        private void OnAdRefreshed(object sender, RoutedEventArgs e)
        {
            // We increment the ad count so that the message changes at every refresh.
            adCount++;
            textAdStatus.Text = "You have watched ad " + adCount.ToString() + " times :)";

            ApplicationData.Current.LocalSettings.Values["nShowAdTimes"] = adCount.ToString();
        }



        private void RunInterstitialAd(object sender, RoutedEventArgs e)
        {
            // Request an ad. When the ad is ready to show, the AdReady event will fire.
            // The application id and ad unit id are passed in here.
            // The application id and ad unit id can be obtained from Dev Center.
            // See "Monetize with Ads" at https://msdn.microsoft.com/en-us/library/windows/apps/mt170658.aspx

            textAdStatus.Text = "Loading interstitial ad...";
            interstitialAd.RequestAd(AdType.Video, "9n3x2jzp7b96", "1100000350");
        }

        // This is an event handler for the interstitial ad. It is triggered when the interstitial ad is ready to play.
        private void OnAdReady(object sender, object e)
        {
            textAdStatus.Text = "Interstitial ad is ready to show";
            // The ad is ready to show; show it.
            interstitialAd.Show();
        }

        // This is an event handler for the interstitial ad. It is triggered when the interstitial ad is cancelled.
        private void OnAdCancelled(object sender, object e)
        {
            textAdStatus.Text = "Interstitial Ad has been canceled";
        }

        // This is an event handler for the interstitial ad. It is triggered when the interstitial ad has completed playback.
        private void OnAdCompleted(object sender, object e)
        {
            adCount++;
            textAdStatus.Text = "Congratulations, you have watched an interstitial ad";
            textAdStatus.Text = "You have watched ad " + adCount.ToString() + " times :)";

            ApplicationData.Current.LocalSettings.Values["nShowAdTimes"] = adCount.ToString();
        }

        // This is an error handler for the interstitial ad.
        private void OnAdError(object sender, AdErrorEventArgs e)
        {
            textAdStatus.Text = "An error occurred while loading interstitial ad";
        }

        private void AdControl_AdLoadingError(object sender, AdDuplex.Common.Models.AdLoadingErrorEventArgs e)
        {
            textAdStatus.Text = "An error occurred while loading ad :(";
        }

        private void AdControl_AdLoaded(object sender, AdDuplex.Banners.Models.BannerAdLoadedEventArgs e)
        {
            adCount++;
            textAdStatus.Text = "You have watched ad " + adCount.ToString() + " times :)";

            ApplicationData.Current.LocalSettings.Values["nShowAdTimes"] = adCount.ToString();
        }

        private void RandomStaf()
        {
            const int count = 20;

            for (int i = 0; i < count; i++)
            {
                Staf staf = new Staf();
                staf.X = ran.Next((int)_width);
                staf.Y = ran.Next((int)_height);
                staf.Point = new Ellipse()
                {
                    Height = 20,
                    Width = 20,
                    Fill = new SolidColorBrush(accentColor),
                };
                staf.RandomStaf(ran);
                _staf.Add(staf);
            }

            foreach (var temp in _staf)
            {
                P.Children.Add(temp.Point);
            }
        }

        private void Time_Tick(object sender, object e)
        {
            foreach (var temp in _staf)
            {
                if (temp.X > _width || temp.Y > _height
                    || temp.X < 0 || temp.Y < 0)
                {
                    temp.X = ran.Next((int)_width);
                    temp.Y = ran.Next((int)_height);
                }

                temp.X -= temp.Vx;
                temp.Y -= temp.Vy;

                Canvas.SetLeft(temp.Point, temp.X);
                Canvas.SetTop(temp.Point, temp.Y);

                temp.Time--;
            }

            //Draw Line
            //Pw.Children.Clear();
            //const double distan = 200;
            //Line line = new Line();
            //foreach (var temp in _staf)
            //{
            //    foreach (var p in _staf)
            //    {
            //        line.X1 = temp.X + 5;
            //        line.Y1 = temp.Y + 5;
            //        line.X2 = p.X + 5;
            //        line.Y2 = p.Y + 5;
            //        double sqrt = Math.Sqrt(Math.Pow((line.X1 - line.X2), 2) +
            //          Math.Pow((line.Y1 - line.Y2), 2));
            //        if (sqrt < distan)
            //        {
            //            line.Stroke = new SolidColorBrush(Colors.Gray);
            //            line.StrokeThickness = 5 * (distan - sqrt) / distan;
            //            Pw.Children.Add(line);
            //            line = new Line();
            //        }
            //    }
            //}
        }

    }
}
