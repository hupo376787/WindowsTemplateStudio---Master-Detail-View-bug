using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.ViewManagement;
using ExifInfo.Helpers;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ExifInfo.Controls
{
    public sealed partial class MessagePopup : UserControl
    {
        private Popup m_Popup;
        private string m_TextBlockContent;
        string strCurrentLanguage = "en-us";

        public MessagePopup()
        {
            strCurrentLanguage = (Application.Current as App).strCurrentLanguage;
            if (strCurrentLanguage.ToLower().Equals("auto"))
                strCurrentLanguage = LanguageHelper.GetCurLanguage();

            this.InitializeComponent();
            SetOKCancelContent();
            //m_Popup = new Popup();
            m_Popup = (Application.Current as App).gPopup;
            MeasurePopupSize();
            m_Popup.Child = this;
            this.Loaded += MessagePopupWindow_Loaded;
            this.Unloaded += MessagePopupWindow_Unloaded;
        }

        //多语言
        private void SetOKCancelContent()
        {
            if (strCurrentLanguage.ToLower().Equals("zh-cn"))
            {
                btnLeft.Content = LanguageHelper.strOK_zhcn;
                btnRight.Content = LanguageHelper.strCancel_zhcn;
            }
            else
            {
                btnLeft.Content = LanguageHelper.strOK_en;
                btnRight.Content = LanguageHelper.strCancel_en;
            }
        }

        private void MeasurePopupSize()
        {
            this.Width = ApplicationView.GetForCurrentView().VisibleBounds.Width;

            double marginTop = 0;
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                marginTop = StatusBar.GetForCurrentView().OccludedRect.Height;
            this.Height = ApplicationView.GetForCurrentView().VisibleBounds.Height;
            this.Margin = new Thickness(0, marginTop, 0, 0);
        }

        public MessagePopup(string showMsg) : this()
        {
            this.m_TextBlockContent = showMsg;
        }

        private void MessagePopupWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.tbContent.Text = m_TextBlockContent;
            ApplicationView.GetForCurrentView().VisibleBoundsChanged += MessagePopupWindow_VisibleBoundsChanged;
        }

        private void MessagePopupWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            ApplicationView.GetForCurrentView().VisibleBoundsChanged -= MessagePopupWindow_VisibleBoundsChanged;
        }

        private void MessagePopupWindow_VisibleBoundsChanged(ApplicationView sender, object args)
        {
            MeasurePopupSize();
        }

        public void ShowWindow()
        {
            m_Popup.IsOpen = true;
        }

        private void DismissWindow()
        {
            m_Popup.IsOpen = false;
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            DismissWindow();
            LeftClick?.Invoke(this, e);
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            DismissWindow();
            RightClick?.Invoke(this, e);
        }

        public event EventHandler<RoutedEventArgs> LeftClick;
        public event EventHandler<RoutedEventArgs> RightClick;

        private void OutBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            DismissWindow();
            RightClick?.Invoke(this, e);
        }
    }
}
