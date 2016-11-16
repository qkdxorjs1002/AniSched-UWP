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

            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed += OnHardwareButtonsBackPressed;
            }

            MainFunction.gridList = new Grid[] { SunIndicator, MonIndicator, TueIndicator, WedIndicator, ThuIndicator, FriIndicator, SatIndicator, OvaIndicator, NewIndicator };

            MainFunction.LoadList(AniListView, JsType.List
                , (Days)Enum.Parse(typeof(Days), DateTime.Now.DayOfWeek.ToString().Remove(3)));
            MainFunction.InitIndicator(DateTime.Now.DayOfWeek.ToString().Remove(3));

        }

        private void OnHardwareButtonsBackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;

            if (MainSplitView.IsPaneOpen == true || TabSplitView.IsPaneOpen == true)
            {
                MainSplitView.IsPaneOpen = false;
                TabSplitView.IsPaneOpen = false;

            }
            else
            {
                App.Current.Exit();

            }

        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (TabSplitView.IsPaneOpen == false)
            {
                MainFunction.LoadList(SearchListView, JsType.List);
            }

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

            MainFunction.InitIndicator(callerObjectName);
            MainFunction.LoadList(AniListView, JsType.List, targetDay);

        }

        private void AniListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataTypes.ListData callerObject = e.ClickedItem as DataTypes.ListData;

            if (callerObject.iis == 404)
            {
                return;
            }

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

            if (callerObject.a != null)
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri(callerObject.a));
            }

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
            CapListImage.Source = new BitmapImage(new Uri("http://anisched.moeru.ga/" + MainFunction.RandomImage(), UriKind.Absolute));

        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox callerObject = sender as TextBox;

            MainFunction.SearchList(SearchListView, JsType.List, callerObject.Text);

        }
    }

    public static class MainFunction
    {
        const String STRING_LE404 = "[{\"s\":\"오류\",\"t\":\"원인\",\"g\":\"서버에서 목록을 받아올 수 없습니다.\"}]";
        const String STRING_CE404 = "[{\"s\":\"오류\",\"d\":\"원인\",\"n\":\"서버에서 목록을 받아올 수 없습니다.\"}]";

        static DataTypes.ListData[] listData;
        static DataTypes.ListData[] allListData;
        static DataTypes.HttpData httpData;
        public static Grid[] gridList;

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
                var statusBar = StatusBar.GetForCurrentView();

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

            return new SolidColorBrush(Color.FromArgb(a, r, g, b));
        }

        public static String RandomImage()
        {
            Random rand = new Random();
            int targetNum = rand.Next() % 50;

            return ("rand" + targetNum + ".png");
        }

        public static void InitIndicator(String targetDay)
        {
            foreach (Grid ld in gridList)
            {
                ld.Visibility = (targetDay == ld.Name.Remove(3) ? Visibility.Visible : Visibility.Collapsed);

            }

        }

        public async static void LoadList(ListView targetObject, JsType targetJsType, Days targetDay)
        {
            try
            {
                httpData = await jsonRequester.Request(targetJsType, (int)targetDay);

            }
            catch (System.Net.Http.HttpRequestException)
            {
                httpData = new DataTypes.HttpData((int)targetDay, 404, STRING_LE404);
            }

            listData = jsonParser.Parse(httpData);
            dataRefiner.ListDataRefine(listData);
            dataBinder.DataBind(targetObject, listData);

        }

        public async static void LoadList(ListView targetObject, int targetId)
        {
            try
            {
                httpData = await jsonRequester.Request(JsType.Capton, targetId);
            }
            catch (System.Net.Http.HttpRequestException)
            {
                httpData = new DataTypes.HttpData(targetId, 404, STRING_CE404);
            }

            listData = jsonParser.Parse(httpData);
            dataRefiner.CaptionDataRefine(listData);
            dataBinder.DataBind(targetObject, listData);

        }

        public async static void LoadList(ListView targetObject, JsType targetJsType)
        {
            DataTypes.HttpData tmpData;
            httpData = new DataTypes.HttpData();

            try
            {
                for (int i = 0; i <= 8; i++)
                {
                    tmpData = await jsonRequester.Request(targetJsType, i);
                    httpData.jsonData += tmpData.jsonData;
                    httpData.statusCode = tmpData.statusCode;

                }
            }
            catch (System.Net.Http.HttpRequestException)
            {
                httpData = new DataTypes.HttpData(-1, 404, STRING_LE404);
            }

            allListData = jsonParser.Parse(httpData);
            dataRefiner.ListDataRefine(allListData);
            dataBinder.DataBind(targetObject, allListData, null);

        }

        public static void SearchList(ListView targetObject, JsType targetJsType, String targetStr)
        {
            dataBinder.DataBind(targetObject, allListData, targetStr);

        }

    }

}
