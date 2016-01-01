using ParentingTrackerApp.Helpers;
using ParentingTrackerApp.ViewModels;
using System;
using System.Collections.Generic;
using Windows.Storage.AccessCache;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Linq;

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
                var existing = lines.ReadFromTable(central.EventTypes, otherInfo);
                merged = HtmlExportHelper.Merge(existing, central.LoggedEvents).ToList();
            }
            catch (Exception)
            {
                merged = central.LoggedEvents;
            }

            var wlines = merged.WriteToTable(otherInfo);

            file = await StorageFile.GetFileFromPathAsync(central.ExportPath);
            await FileIO.WriteLinesAsync(file, wlines);
        }
    }
}
