using ExifInfo.Models;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.DataTransfer;
using Windows.Services.Store;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ExifInfo.Controls
{
    public sealed partial class PurchaseControl : UserControl
    {
        StoreProductQueryResult _queryResult;
        Popup m_Popup;

        public PurchaseControl(StoreProductQueryResult queryResult)
        {
            this.InitializeComponent();
            //m_Popup = new Popup();

            m_Popup = (Application.Current as App).gPopup;
            MeasurePopupSize();
            m_Popup.Child = this;
            this.Loaded += MessagePopupWindow_Loaded;
            this.Unloaded += MessagePopupWindow_Unloaded;
            _queryResult = queryResult;

            List<Product> lProducts = new List<Product>();
            foreach (KeyValuePair<string, StoreProduct> item in _queryResult.Products)
            {
                // Access the Store product info for the add-on.
                StoreProduct product = item.Value;

                // Use members of the product object to access listing info for the add-on...
                Product pd = new Product()
                {
                    StoreId = product.StoreId,
                    Title = product.Title,
                    Price = product.Price.FormattedPrice,
                    Description = product.Description,
                    ProductImage = product.Images[0].Uri.OriginalString
                };

                lProducts.Add(pd);
            }

            listProducts.ItemsSource = lProducts;
        }
        
        private async void listProducts_ItemClick(object sender, ItemClickEventArgs e)
        {
            StoreContext context = StoreContext.GetDefault();
            var product = e.ClickedItem as Product;
            var result = await context.RequestPurchaseAsync(product.StoreId);
            if (result.Status == StorePurchaseStatus.Succeeded)
            {
                // 成功购买
                textPurchaseResult.Text = "Thank you.";
            }
            else if (result.Status == StorePurchaseStatus.AlreadyPurchased)
            {
                // 已经购买过了
                textPurchaseResult.Text = "You have already purchased.";
            }
            else if (result.Status == StorePurchaseStatus.NotPurchased)
            {
                // 用户没购买，即用户中途取消了操作
                textPurchaseResult.Text = "You have canceled the purchase.";
            }
            else if (result.Status == StorePurchaseStatus.ServerError || result.Status == StorePurchaseStatus.NetworkError)
            {
                // 发生错误
                textPurchaseResult.Text = "Sorry, something went wrong with the microsoft server or something else.";
            }
        }

        public void ShowWindow()
        {
            m_Popup.IsOpen = true;
        }

        private void DismissWindow()
        {
            m_Popup.IsOpen = false;
        }

        //public PurchaseControl(StoreProductQueryResult queryResult) : this()
        //{
        //    this._queryResult = queryResult;
        //}

        private void OutBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            m_Popup.IsOpen = false;
        }

        private void MeasurePopupSize()
        {
            this.Width = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().VisibleBounds.Width;

            double marginTop = 0;
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                marginTop = Windows.UI.ViewManagement.StatusBar.GetForCurrentView().OccludedRect.Height;
            this.Height = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().VisibleBounds.Height;
            this.Margin = new Thickness(0, marginTop, 0, 0);
        }

        private void MessagePopupWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().VisibleBoundsChanged += MessagePopupWindow_VisibleBoundsChanged;
        }

        private void MessagePopupWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().VisibleBoundsChanged -= MessagePopupWindow_VisibleBoundsChanged;
        }

        private void MessagePopupWindow_VisibleBoundsChanged(Windows.UI.ViewManagement.ApplicationView sender, object args)
        {
            MeasurePopupSize();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            m_Popup.IsOpen = false;
        }

        private async void imgPayPal_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Uri uri = new Uri("https://www.paypal.me/hupo376787");
            await Windows.System.Launcher.LaunchUriAsync(uri);
            textPurchaseResult.Text = "Opening PayPal :)";
        }

        private void imgWeChat_Tapped(object sender, TappedRoutedEventArgs e)
        {
            textPurchaseResult.Text = "请直接扫码转账，谢谢 :)";
        }

        private void imgZhiFuBiao_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string str = "376787823@qq.com";
            DataPackage dp = new DataPackage();
            dp.SetText(str);
            Clipboard.SetContent(dp);

            textPurchaseResult.Text = "支付婊账号已复制 :)";
        }
    }
}
