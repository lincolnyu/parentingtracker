﻿using ParentingTrackerApp.Helpers;
using ParentingTrackerApp.ViewModels;
using System;
using System.Collections.Generic;
using Windows.Storage.AccessCache;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Linq;
using Windows.UI.Popups;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ParentingTrackerApp.Views
{
    public sealed partial class ExportView : UserControl
    {
        public ExportView()
        {
            InitializeComponent();
        }

        private async void SelectFileOnClick(object sender, RoutedEventArgs e)
        {
            var fsp = new FileSavePicker();
            fsp.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            fsp.FileTypeChoices.Add("HTML files", new List<string>() { ".html" });
            fsp.SuggestedFileName = "New Document";
            var file = await fsp.PickSaveFileAsync();
            if (file != null)
            {
                var central = (CentralViewModel)DataContext;
                central.ExportPath = file.Path;
                central.ExportFileToken = StorageApplicationPermissions.FutureAccessList.Add(file);
            }
        }

        private async void ExportOnClick(object sender, RoutedEventArgs args)
        {
            var central = (CentralViewModel)DataContext;

            StorageFile file;

            IEnumerable<EventViewModel> merged;
            var otherInfo = new HtmlExportHelper.DocInfo();
            try
            {
                file = await StorageFile.GetFileFromPathAsync(central.ExportPath);
                var lines = await FileIO.ReadLinesAsync(file);
                var existing = lines.ReadFromTable(central, otherInfo);
                merged = HtmlExportHelper.Merge(existing, central.LoggedEvents).ToList();
            }
            catch (Exception)
            {
                merged = central.LoggedEvents;
            }

            file = await StorageFile.GetFileFromPathAsync(central.ExportPath);
            if (string.IsNullOrWhiteSpace(otherInfo.Title))
            {
                otherInfo.Title = file.DisplayName;
            }
            var wlines = merged.WriteToTable(otherInfo);
            await FileIO.WriteLinesAsync(file, wlines);
        }

        private async void ViewOnClick(object sender, RoutedEventArgs args)
        {
            var central = (CentralViewModel)DataContext;
            var file = await StorageFile.GetFileFromPathAsync(central.ExportPath);
            var html = await FileIO.ReadTextAsync(file);
            Nav.NavigateToString(html);
                #if false
            var central = (CentralViewModel)DataContext;
            string path = central.ExportPath;
            var uriStr = new Uri(path).AbsoluteUri;
            var uri = new Uri(uriStr);
            var succ = await Windows.System.Launcher.LaunchUriAsync(uri);
            if (!succ)
            {
                var dlg = new MessageDialog("Cannot find the file", "Information");
                await dlg.ShowAsync();
            }
#endif
        }

        private void UserControlOnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            var upperHeight = InfoText.ActualHeight + Header.ActualHeight + InputGrid.ActualHeight + MergeButton.ActualHeight;
            UpperRow.Height = new GridLength(upperHeight);
            LowerRow.Height = new GridLength(MainGrid.ActualHeight - upperHeight);
        }
    }
}
