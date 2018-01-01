using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using ExifInfo.Controls;
using ExifInfo.Helpers;
using ExifInfo.Services;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace ExifInfo.Views
{
    public sealed partial class ShellPage : Page, INotifyPropertyChanged
    {
        private const string PanoramicStateName = "PanoramicState";
        private const string WideStateName = "WideState";
        private const string NarrowStateName = "NarrowState";
        private const double WideStateMinWindowWidth = 640;
        private const double PanoramicStateMinWindowWidth = 1024;

        string strCurrentLanguage = "en-us";
        bool isExit = false;
        Compositor _compositor;
        SpriteVisual _hostSprite;
        public static ShellPage CurrentPage;

        private bool _isPaneOpen;

        public bool IsPaneOpen
        {
            get { return _isPaneOpen; }
            set { Set(ref _isPaneOpen, value); }
        }

        private object _selected;

        public object Selected
        {
            get { return _selected; }
            set { Set(ref _selected, value); }
        }

        private SplitViewDisplayMode _displayMode = SplitViewDisplayMode.CompactInline;

        public SplitViewDisplayMode DisplayMode
        {
            get { return _displayMode; }
            set { Set(ref _displayMode, value); }
        }

        private object _lastSelectedItem;

        private ObservableCollection<ShellNavigationItem> _primaryItems = new ObservableCollection<ShellNavigationItem>();

        public ObservableCollection<ShellNavigationItem> PrimaryItems
        {
            get { return _primaryItems; }
            set { Set(ref _primaryItems, value); }
        }

        private ObservableCollection<ShellNavigationItem> _secondaryItems = new ObservableCollection<ShellNavigationItem>();

        public ObservableCollection<ShellNavigationItem> SecondaryItems
        {
            get { return _secondaryItems; }
            set { Set(ref _secondaryItems, value); }
        }

        public ShellPage()
        {
            InitializeComponent();
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(186, 186));

            DataContext = this;
            CurrentPage = this;
            Initialize();

            strCurrentLanguage = (Application.Current as App).strCurrentLanguage;
            if (strCurrentLanguage.ToLower().Equals("auto"))
                strCurrentLanguage = LanguageHelper.GetCurLanguage();

            if (!Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                TitleBarControl myTitleBar = new TitleBarControl();
                (this.Content as Grid).Children.Add(myTitleBar);
                Window.Current.SetTitleBar(myTitleBar);

                var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
                coreTitleBar.ExtendViewIntoTitleBar = true;

                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                titleBar.ButtonBackgroundColor = Colors.Transparent;
            }
            else if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
                coreTitleBar.ExtendViewIntoTitleBar = false;
            }

            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var view = ApplicationView.GetForCurrentView();
                view.TryEnterFullScreenMode();
            }

            GlassEffect();
            SetFrostedGlassOpacityAsync((Application.Current as App).dOpacity);
        }

        private void Initialize()
        {
            NavigationService.Frame = shellFrame;
            NavigationService.Navigated += Frame_Navigated;
            PopulateNavItems();

            InitializeState(Window.Current.Bounds.Width);
        }

        private void InitializeState(double windowWith)
        {
            if (windowWith < WideStateMinWindowWidth)
            {
                GoToState(NarrowStateName);
            }
            else if (windowWith < PanoramicStateMinWindowWidth)
            {
                GoToState(WideStateName);
            }
            else
            {
                GoToState(PanoramicStateName);
            }
        }

        private void PopulateNavItems()
        {
            _primaryItems.Clear();
            _secondaryItems.Clear();

            // TODO WTS: Change the symbols for each item as appropriate for your app
            // More on Segoe UI Symbol icons: https://docs.microsoft.com/windows/uwp/style/segoe-ui-symbol-font
            // Or to use an IconElement instead of a Symbol see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/projectTypes/navigationpane.md
            // Edit String/en-US/Resources.resw: Add a menu item title for each page
            _primaryItems.Add(ShellNavigationItem.FromType<MasterDetailPage>("Shell_MasterDetail".GetLocalized(), Symbol.Home));
            _primaryItems.Add(ShellNavigationItem.FromType<MapPage>("Shell_Map".GetLocalized(), Symbol.World));
            _primaryItems.Add(ShellNavigationItem.FromType<ImageGalleryPage>("Shell_ImageGallery".GetLocalized(), Symbol.Pictures));
            _primaryItems.Add(ShellNavigationItem.FromType<AdPage>("Shell_Ad".GetLocalized(), Symbol.TouchPointer));

            _secondaryItems.Add(ShellNavigationItem.FromType<SettingsPage>("Shell_Settings".GetLocalized(), Symbol.Setting));
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            var navigationItem = PrimaryItems?.FirstOrDefault(i => i.PageType == e?.SourcePageType);
            if (navigationItem == null)
            {
                navigationItem = SecondaryItems?.FirstOrDefault(i => i.PageType == e?.SourcePageType);
            }

            if (navigationItem != null)
            {
                ChangeSelected(_lastSelectedItem, navigationItem);
                _lastSelectedItem = navigationItem;
            }
        }

        private void ChangeSelected(object oldValue, object newValue)
        {
            if (oldValue != null)
            {
                (oldValue as ShellNavigationItem).IsSelected = false;
            }

            if (newValue != null)
            {
                (newValue as ShellNavigationItem).IsSelected = true;
                Selected = newValue;
            }
        }

        private void Navigate(object item)
        {
            var navigationItem = item as ShellNavigationItem;
            if (navigationItem != null)
            {
                NavigationService.Navigate(navigationItem.PageType);
            }
        }

        private void ItemInvoked(object sender, HamburgetMenuItemInvokedEventArgs e)
        {
            if (DisplayMode == SplitViewDisplayMode.CompactOverlay || DisplayMode == SplitViewDisplayMode.Overlay)
            {
                IsPaneOpen = false;
            }

            Navigate(e.InvokedItem);
        }

        private void OpenPane_Click(object sender, RoutedEventArgs e)
        {
            IsPaneOpen = !_isPaneOpen;
        }

        private void WindowStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e) => GoToState(e.NewState.Name);

        private void GoToState(string stateName)
        {
            switch (stateName)
            {
                case PanoramicStateName:
                    DisplayMode = SplitViewDisplayMode.CompactInline;
                    IsPaneOpen = true;
                    break;
                case WideStateName:
                    DisplayMode = SplitViewDisplayMode.CompactInline;
                    IsPaneOpen = false;
                    break;
                case NarrowStateName:
                    DisplayMode = SplitViewDisplayMode.Overlay;
                    IsPaneOpen = false;
                    break;
                default:
                    break;
            }
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

        private void BackgroundGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_hostSprite != null)
            {
                _hostSprite.Size = e.NewSize.ToVector2();
            }
            InitializeState(Window.Current.Bounds.Width);
        }

        private void GlassEffect()
        {
            if (!SystemHelper.Windows10Build15063)
                return;

            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            _hostSprite = _compositor.CreateSpriteVisual();
            _hostSprite.Size = new Vector2((float)GlassHostGrid.ActualWidth, (float)GlassHostGrid.ActualHeight);

            ElementCompositionPreview.SetElementChildVisual(GlassHostGrid, _hostSprite);
            _hostSprite.Brush = _compositor.CreateHostBackdropBrush();
        }

        public async void SetFrostedGlassOpacityAsync(double dOpacity)
        {
            ElementTheme theme = await ThemeSelectorService.LoadThemeFromSettingsAsync();

            if (!SystemHelper.Windows10Build15063)
            {
                if (theme == ElementTheme.Light)
                    GlassHostGrid.Background = new SolidColorBrush(Colors.White);
                else
                    GlassHostGrid.Background = new SolidColorBrush(Colors.Black);

                return;
            }

            if (!Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                if (theme == ElementTheme.Light)
                {
                    contentGrid.Background = new SolidColorBrush(Colors.Transparent);
                    GlassHostGrid.Opacity = dOpacity;
                }
                else
                {
                    GlassHostGrid.Opacity = dOpacity;
                    byte ss = Convert.ToByte((1 - dOpacity) * 255);
                    contentGrid.Background = new SolidColorBrush(Color.FromArgb(ss, 53, 53, 53));
                }
            }
            else
            {
                if (theme == ElementTheme.Light)
                    GlassHostGrid.Opacity = 1;
                else
                {
                    contentGrid.Background = new SolidColorBrush(Color.FromArgb(0, 53, 53, 53));
                }
            }
        }

    }
}
