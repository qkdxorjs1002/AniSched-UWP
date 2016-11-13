using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace AniSched
{

    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
            MainFunction.Painter();
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += OnHardwareButtonsBackPressed;
            }

            MainFunction.LoadList(AniListView, JsType.List
                , (Days)Enum.Parse(typeof(Days), DateTime.Now.DayOfWeek.ToString().Remove(3)));

        }

        private void OnHardwareButtonsBackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;

            //if (ChildGrid.Visibility == Visibility.Visible)
            //{
            //    Image_Animation.Stop();
            //    ChildGrid.Visibility = Visibility.Collapsed;
            //    Grid_Animation.Stop();
            //    SearchList.IsEnabled = true;
            //}
            //else
            //{
            //    App.Current.Exit();
            //}
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            TabSplitView.IsPaneOpen = TabSplitView.IsPaneOpen == true ? false : true;

        }

        private void DayButton_Click(object sender, RoutedEventArgs e)
        {
            Days targetDay;
            String callerObjectName;
            Button callerObject = sender as Button;

            TabSplitView.IsPaneOpen = false;

            callerObjectName = callerObject.Name.Remove(3);
            targetDay = (Days)Enum.Parse(typeof(Days), callerObjectName).GetHashCode();

            MainFunction.LoadList(AniListView, JsType.List, targetDay);
            MainFunction.LoadList(SearchListView, JsType.List, targetDay);

        }

        private void AllButton_Click(object sender, RoutedEventArgs e)
        {
            MainFunction.LoadList(AniListView, JsType.List);

        }

        private void AniListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataTypes.ListData callerObject = e.ClickedItem as DataTypes.ListData;

            MainFunction.LoadList(CapListView, callerObject.i);
            CapListImage.Source = new BitmapImage(new Uri("http://anisched.moeru.ga/" + callerObject.i + ".jpg", UriKind.Absolute));
            CapListTitle.Text = callerObject.s;
            CapListGenre.Text = callerObject.g;
            CapListStart.Text = callerObject.sd;
            CapListEnd.Text = callerObject.ed;
            CapListLink.Text = callerObject.l;
            CapListStatus.Fill = MainFunction.ConvertColor(callerObject.a);
            MainSplitView.IsPaneOpen = true;

        }

        private async void CapListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataTypes.ListData callerObject = e.ClickedItem as DataTypes.ListData;

            await Windows.System.Launcher.LaunchUriAsync(new Uri(callerObject.a));
        }

        private void ExtraButton_Click(object sender, RoutedEventArgs e)
        {
            ExtraMenuFloyout.ShowAt(ExtraButton);

        }

        private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem callerObject = sender as MenuFlyoutItem;
            Uri url = new Uri((callerObject.Text == "나무위키에서 검색" ? ("https://namu.wiki/go/") 
                : ("https://search.naver.com/search.naver?&query=")) + CapListTitle.Text);

            await Windows.System.Launcher.LaunchUriAsync(url);
        }

        private void CapListImage_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            CapListRani.Source = new BitmapImage(new Uri("http://anisched.moeru.ga/" + MainFunction.RandomImage(), UriKind.Absolute));

        }
    }

    public static class MainFunction
    {
        static DataTypes.ListData[] listData;
        static DataTypes.HttpData httpData;

        static JsonRequester jsonRequester = new JsonRequester("http://www.anissia.net/anitime/", "list?w=", "end?p=", "cap?i=");
        static JsonParser jsonParser = new JsonParser();
        static DataRefiner dataRefiner = new DataRefiner();
        static DataBinder dataBinder = new DataBinder();

        public static void Painter()
        {
            var MAColor = Color.FromArgb(255, 254, 59, 114);     // TH
            var MBColor = Color.FromArgb(255, 250, 250, 250);    // FG
            var MCColor = Color.FromArgb(255, 25, 25, 25);       // BG

            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                ApplicationView AppVCon = ApplicationView.GetForCurrentView();

                AppVCon.TitleBar.BackgroundColor = MAColor;
                AppVCon.TitleBar.ButtonBackgroundColor = MAColor;
                AppVCon.TitleBar.ButtonHoverBackgroundColor = MAColor;
                AppVCon.TitleBar.ButtonInactiveBackgroundColor = MAColor;
                AppVCon.TitleBar.ButtonPressedBackgroundColor = MBColor;
                AppVCon.TitleBar.InactiveBackgroundColor = MAColor;
                AppVCon.TitleBar.ForegroundColor = MBColor;
                AppVCon.TitleBar.ButtonForegroundColor = MBColor;
                AppVCon.TitleBar.ButtonHoverForegroundColor = MCColor;
                AppVCon.TitleBar.ButtonInactiveForegroundColor = MBColor;
                AppVCon.TitleBar.ButtonPressedForegroundColor = MCColor;
                AppVCon.TitleBar.InactiveForegroundColor = MBColor;

            }

            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();

                if (statusBar != null)
                {
                    statusBar.BackgroundOpacity = 1.0f;
                    statusBar.BackgroundColor = MAColor;
                    statusBar.ForegroundColor = MBColor;

                }

            }

        }

        public static SolidColorBrush ConvertColor(string hexColor)
        {
            hexColor = hexColor.Replace("#", string.Empty);
            byte a = (byte)(Convert.ToUInt32(hexColor.Substring(0, 2), 16));
            byte r = (byte)(Convert.ToUInt32(hexColor.Substring(2, 2), 16));
            byte g = (byte)(Convert.ToUInt32(hexColor.Substring(4, 2), 16));
            byte b = (byte)(Convert.ToUInt32(hexColor.Substring(6, 2), 16));

            return new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
        }

        public static String RandomImage()
        {
            Random rand = new Random();
            int targetNum = rand.Next() % 20;
            
            return ("rand" + targetNum + ".png");
        }

        public async static void LoadList(ListView targetObject, JsType targetJsType, Days targetDay)
        {
            httpData = await jsonRequester.Request(targetJsType, (int)targetDay);
            listData = jsonParser.Parse(httpData.jsonData);
            dataRefiner.ListDataRefine(listData);
            dataBinder.DataBind(targetObject, listData);

        }

        public async static void LoadList(ListView targetObject, int targetId)
        {
            httpData = await jsonRequester.Request(JsType.Capton, targetId);
            listData = jsonParser.Parse(httpData.jsonData);
            dataRefiner.CaptionDataRefine(listData);
            dataBinder.DataBind(targetObject, listData);

        }

        public async static void LoadList(ListView targetObject, JsType targetJsType)
        {
            DataTypes.HttpData tmpData;
            httpData = new DataTypes.HttpData(0, "");

            for (int i = 0; i <= 8; i++)
            {
                tmpData = await jsonRequester.Request(targetJsType, i);
                httpData.jsonData += tmpData.jsonData;
                httpData.statusCode = tmpData.statusCode;
                Debug.WriteLine(httpData.statusCode);

            }

            listData = jsonParser.Parse(httpData.jsonData);
            dataRefiner.ListDataRefine(listData);
            dataBinder.DataBind(targetObject, listData);

        }

    }

}
