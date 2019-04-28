using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Notifications;
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
        //const string URL_IMGSOURCE = "http://anisched.moeru.me/";
        const string URL_IMGSOURCE = "http://test.istpikworld.net/~anisched/";

        public MainPage()
        {
            this.InitializeComponent();

            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed += OnHardwareButtonsBackPressed;
            }

            MainFunction.gridList = new Grid[] { SunIndicator, MonIndicator, TueIndicator, WedIndicator, ThuIndicator, FriIndicator, SatIndicator, OvaIndicator, NewIndicator, FavIndicator };

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
            JsType targetType = new JsType();

            if (TabSplitView.IsPaneOpen == false)
            {
                switch(IsEnd_CheckBox.IsChecked)
                {
                    case true:
                        targetType = JsType.End;
                        break;

                    case false:
                        targetType = JsType.List;
                        break;

                }
                MainFunction.LoadList(SearchListView, targetType, null);

            }

            TabSplitView.IsPaneOpen = TabSplitView.IsPaneOpen == true ? false : true;
            
        }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            SettingView.Visibility = Visibility.Visible;
            CaptionView.Visibility = Visibility.Collapsed;
            MainSplitView.IsPaneOpen = true;

        }

        private void FavButton_Click(object sender, RoutedEventArgs e)
        {
            string callerObjectName;
            Button callerObject = sender as Button;

            callerObjectName = callerObject.Name.Remove(3);

            MainFunction.InitIndicator(callerObjectName);
            MainFunction.LoadList(AniListView, JsType.List, "%1002&FAVLIST%1002&");

        }

        private void DayButton_Click(object sender, RoutedEventArgs e)
        {
            Days targetDay;
            string callerObjectName;
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

            int toDay = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
            int endDay = Convert.ToInt32(callerObject.ed.Replace("/", ""));

            if (callerObject.iis == 404)
            {
                return ;
            }

            CaptionView.Visibility = Visibility.Visible;
            SettingView.Visibility = Visibility.Collapsed;

            MainFunction.LoadList(CapListView, callerObject.i);
            CapListImage.Source = new BitmapImage(new Uri(URL_IMGSOURCE + callerObject.i + ".jpg", UriKind.Absolute));
            CaptionIDText.Text = callerObject.i.ToString();
            CapListTitle.Text = callerObject.s;
            CapListGenre.Text = callerObject.g;
            CapListStart.Text = callerObject.sd;
            CapListEnd.Text = callerObject.ed;
            CapListLink.Text = callerObject.l;
            CapListStatus.Fill = new SolidColorBrush(MainFunction.ConvertColor(callerObject.a));

            FavToggleButton.IsEnabled = toDay > endDay && endDay != 0 ? false : true;
            FavToggleButton.IsChecked = MainFunction.favList.Contains(CaptionIDText.Text);

            MainSplitView.IsPaneOpen = true;

        }

        private async void CapListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataTypes.ListData callerObject = e.ClickedItem as DataTypes.ListData;

            if (callerObject.a != "http://" && callerObject.a != null)
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

        private void SearchExtraButton_Click(object sender, RoutedEventArgs e)
        {
            SearchExtraMenuFloyout.ShowAt(SearchExtraButton);

        }

        private void ToggleMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            ToggleMenuFlyoutItem callerObject = sender as ToggleMenuFlyoutItem;

            if (callerObject.IsChecked == true)
            {
                MainFunction.LoadList(SearchListView, JsType.End, null);
            }
            else
            {
                MainFunction.LoadList(SearchListView, JsType.List, null);
            }

        }

        private void CapListImage_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            CapListImage.Source = new BitmapImage(new Uri(URL_IMGSOURCE + MainFunction.RandomImage() + ".png", UriKind.Absolute));

        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox callerObject = sender as TextBox;

            MainFunction.SearchList(SearchListView, JsType.List, callerObject.Text);

        }

        private async void CapListLink_Tapped(object sender, TappedRoutedEventArgs e)
        {
            TextBlock callerObject = sender as TextBlock;

            await Windows.System.Launcher.LaunchUriAsync(new Uri(callerObject.Text));
        }

        private void ResetSettings_Click(object sender, RoutedEventArgs e)
        {
            ColorSettings_Pink.IsChecked = true;
            MainFunction.favList.Clear();
        }

        private void ColorSettings_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton callerObject = sender as RadioButton;
            string targetBsColor;

            switch (callerObject.Name.Remove(0, 14))
            {
                case "Pink":
                    targetBsColor = "#FFFE3B72";
                    break;
                case "Purple":
                    targetBsColor = "#FFD500F9";
                    break;
                case "Blue":
                    targetBsColor = "#FF448AFF";
                    break;
                case "Cyan":
                    targetBsColor = "#FF00BCD4";
                    break;
                case "Teal":
                    targetBsColor = "#FF009688";
                    break;
                case "Amber":
                    targetBsColor = "#FFFF8F00";
                    break;
                case "Orange":
                    targetBsColor = "#FFFF5722";
                    break;
                case "Grey":
                    targetBsColor = "#FF607D8B";
                    break;
                default:
                    targetBsColor = "#FFFE3B72";
                    break;
            }
            
            if (MainFunction.localSettings.Values["customBaseColor"].ToString() != targetBsColor)
            {
                RestartAlert.Visibility = Visibility.Visible;
                MainFunction.localSettings.Values["customBaseColor"] = targetBsColor;
            }

        }

        private void ColorSettings_Loaded(object sender, RoutedEventArgs e)
        {
            switch (MainFunction.localSettings.Values["customBaseColor"].ToString())
            {
                case "#FFFE3B72":
                    ColorSettings_Pink.IsChecked = true;
                    break;
                case "#FFD500F9":
                    ColorSettings_Purple.IsChecked = true;
                    break;
                case "#FF448AFF":
                    ColorSettings_Blue.IsChecked = true;
                    break;
                case "#FF00BCD4":
                    ColorSettings_Cyan.IsChecked = true;
                    break;
                case "#FF009688":
                    ColorSettings_Teal.IsChecked = true;
                    break;
                case "#FFFF8F00":
                    ColorSettings_Amber.IsChecked = true;
                    break;
                case "#FFFF5722":
                    ColorSettings_Orange.IsChecked = true;
                    break;
                case "#FF607D8B":
                    ColorSettings_Grey.IsChecked = true;
                    break;
                default:
                    ColorSettings_Pink.IsChecked = true;
                    break;
            }

        }
        
        private void FavToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton callerObject = sender as ToggleButton;
            string targetId = CaptionIDText.Text;

            if (MainFunction.favList.Contains(targetId))
            {
                MainFunction.favList.Remove(targetId);
            }
            else
            {
                MainFunction.favList.Add(targetId);
            }
            
        }
    }

    public static class MainFunction
    {
        const string STRING_LE404 = "[{\"s\":\"오류\",\"t\":\"원인\",\"g\":\"서버에서 목록을 받아올 수 없습니다.\"}]";
        const string STRING_CE404 = "[{\"s\":\"오류\",\"d\":\"원인\",\"n\":\"서버에서 목록을 받아올 수 없습니다.\"}]";

        public static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public static StorageFolder localFolder = ApplicationData.Current.LocalFolder;

        static DataTypes.ListData[] listData;
        static DataTypes.ListData[] allListData;
        static DataTypes.HttpData httpData;
        public static Grid[] gridList;
        public static string[] settingList;
        public static List<string> favList = new List<string>();

        static JsonRequester jsonRequester = new JsonRequester("http://www.anissia.net/anitime/", "list?w=", "end?p=", "cap?i=");
        static JsonParser jsonParser = new JsonParser();
        static DataRefiner dataRefiner = new DataRefiner();
        static DataBinder dataBinder = new DataBinder();

        public static void Painter()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                ApplicationView AppVCon = ApplicationView.GetForCurrentView();

                AppVCon.TitleBar.BackgroundColor = ConvertColor(localSettings.Values["customBaseColor"].ToString());
                AppVCon.TitleBar.ButtonBackgroundColor = ConvertColor(localSettings.Values["customBaseColor"].ToString());
                AppVCon.TitleBar.ButtonHoverBackgroundColor = ConvertColor(localSettings.Values["customBaseColor"].ToString());
                AppVCon.TitleBar.ButtonInactiveBackgroundColor = ConvertColor(localSettings.Values["customBaseColor"].ToString());
                AppVCon.TitleBar.ButtonPressedBackgroundColor = ConvertColor(localSettings.Values["customAccentForgroundColor"].ToString());
                AppVCon.TitleBar.InactiveBackgroundColor = ConvertColor(localSettings.Values["customBaseColor"].ToString());
                AppVCon.TitleBar.ForegroundColor = ConvertColor(localSettings.Values["customAccentForgroundColor"].ToString());
                AppVCon.TitleBar.ButtonForegroundColor = ConvertColor(localSettings.Values["customAccentForgroundColor"].ToString());
                AppVCon.TitleBar.ButtonHoverForegroundColor = ConvertColor(localSettings.Values["customAccentForgroundColor_alt"].ToString());
                AppVCon.TitleBar.ButtonInactiveForegroundColor = ConvertColor(localSettings.Values["customAccentForgroundColor"].ToString());
                AppVCon.TitleBar.ButtonPressedForegroundColor = ConvertColor(localSettings.Values["customAccentForgroundColor_alt"].ToString());
                AppVCon.TitleBar.InactiveForegroundColor = ConvertColor(localSettings.Values["customAccentForgroundColor"].ToString());

            }

            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusBar = StatusBar.GetForCurrentView();

                if (statusBar != null)
                {
                    statusBar.BackgroundOpacity = 1.0f;
                    statusBar.BackgroundColor = ConvertColor(localSettings.Values["customBaseColor"].ToString());
                    statusBar.ForegroundColor = ConvertColor(localSettings.Values["customAccentForgroundColor"].ToString());

                }

            }

        }

        public static Color ConvertColor(string hexColor)
        {
            hexColor = hexColor.Replace("#", string.Empty);
            byte a = (byte)(Convert.ToUInt32(hexColor.Substring(0, 2), 16));
            byte r = (byte)(Convert.ToUInt32(hexColor.Substring(2, 2), 16));
            byte g = (byte)(Convert.ToUInt32(hexColor.Substring(4, 2), 16));
            byte b = (byte)(Convert.ToUInt32(hexColor.Substring(6, 2), 16));

            return Color.FromArgb(a, r, g, b);
        }

        public static Color ConvertColor(string hexColor, sbyte bright)
        {
            Color targetColor = ConvertColor(hexColor);
            
            targetColor.R = (byte)(targetColor.R + bright > 255 ? 255 : (targetColor.R + bright < 0 ? 0 : targetColor.R + bright));
            targetColor.G = (byte)(targetColor.G + bright > 255 ? 255 : (targetColor.G + bright < 0 ? 0 : targetColor.G + bright));
            targetColor.B = (byte)(targetColor.B + bright > 255 ? 255 : (targetColor.B + bright < 0 ? 0 : targetColor.B + bright));

            return targetColor;
        }

        public static String RandomImage()
        {
            Random rand = new Random();
            int targetNum = rand.Next() % 50;

            return ("rand" + targetNum);
        }

        public static void InitIndicator(string targetDay)
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

        public async static void LoadList(ListView targetObject, JsType targetJsType, string targetStr)
        {
            DataTypes.HttpData tmpData;
            httpData = new DataTypes.HttpData();
            for (int i = 0; i <= 8; i++)
            {
                tmpData = await jsonRequester.Request(targetJsType, i);
                httpData.jsonData += tmpData.jsonData;
                httpData.statusCode = tmpData.statusCode;
            }

            try
            {
                allListData = jsonParser.Parse(httpData);
            }
            catch (Exception)
            {
                httpData = new DataTypes.HttpData(-1, 404, STRING_LE404);
                allListData = jsonParser.Parse(httpData);
            }

            dataRefiner.ListDataRefine(allListData);
            dataBinder.DataBind(targetObject, allListData, targetStr);

        }

        public static void SearchList(ListView targetObject, JsType targetJsType, string targetStr)
        {
            dataBinder.DataBind(targetObject, allListData, targetStr);

        }

    }

}
