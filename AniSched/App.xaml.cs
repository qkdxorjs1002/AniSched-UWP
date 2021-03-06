﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace AniSched
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            //MainFunction.localSettings.Values.Clear();
            if (MainFunction.localSettings.Values.Keys.Count < 6)
            {
                MainFunction.localSettings.Values["customBackgroundColor"] = "#FFFAFAFA";
                MainFunction.localSettings.Values["customBaseColor"] = "#FFFE3B72";
                MainFunction.localSettings.Values["customAccentForgroundColor"] = "#FFFAFAFA";
                MainFunction.localSettings.Values["customAccentForgroundColor_alt"] = "#FF000000";
                MainFunction.localSettings.Values["customAccentBackgroundColor"] = "#FFF02F65";
                MainFunction.localSettings.Values["customAccentBackgroundColor_alt"] = "#FFE6E6E6";
            }

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            Frame rootFrame = Window.Current.Content as Frame;

            MainFunction.Painter();
            ApplicationView.PreferredLaunchViewSize = new Size(500, 850);
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(500, 850));

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                try
                {
                    Current.Resources["customBackgroundColor"] = new SolidColorBrush(MainFunction.ConvertColor(MainFunction.localSettings.Values["customBackgroundColor"].ToString()));
                    Current.Resources["customBaseColor"] = new SolidColorBrush(MainFunction.ConvertColor(MainFunction.localSettings.Values["customBaseColor"].ToString()));
                    Current.Resources["customAccentForgroundColor"] = new SolidColorBrush(MainFunction.ConvertColor(MainFunction.localSettings.Values["customAccentForgroundColor"].ToString()));
                    Current.Resources["customAccentForgroundColor_alt"] = new SolidColorBrush(MainFunction.ConvertColor(MainFunction.localSettings.Values["customAccentForgroundColor_alt"].ToString()));
                    Current.Resources["customAccentBackgroundColor"] = new SolidColorBrush(MainFunction.ConvertColor(MainFunction.localSettings.Values["customBaseColor"].ToString(), -15));
                    Current.Resources["customAccentBackgroundColor_alt"] = new SolidColorBrush(MainFunction.ConvertColor(MainFunction.localSettings.Values["customAccentBackgroundColor_alt"].ToString()));
                    if (MainFunction.localSettings.Values["favoriteList"] != null && MainFunction.localSettings.Values.Keys.Contains("favoriteList") != false)
                    {
                        MainFunction.favList = ((string[])MainFunction.localSettings.Values["favoriteList"]).ToList();
                    }

                }
                catch (Exception)
                {
                    MainFunction.localSettings.Values.Clear();

                }

                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            if (MainFunction.favList.Count == 0)
            {
                MainFunction.localSettings.Values.Remove("favoriteList");
            }
            else
            {
                MainFunction.localSettings.Values["favoriteList"] = MainFunction.favList.ToArray();
            }

            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
        
    }
}
