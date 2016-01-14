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
using Windows.UI.Popups;
using Windows.Storage.Provider;
using System.Threading.Tasks;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ParentingTrackerApp.Views
{
    public sealed partial class ExportView : UserControl
    {
        private StorageFile _file;

        public ExportView()
        {
            InitializeComponent();
        }

        private async void SelectFileOnClick(object sender, RoutedEventArgs e)
        {
            await UseFileOpen();
        }

        private async Task UseFileSave()
        {
            var fsp = new FileSavePicker();
            fsp.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            fsp.FileTypeChoices.Add("HTML files", new List<string>() { ".html" });
            fsp.SuggestedFileName = "New Document";
            _file = await fsp.PickSaveFileAsync();
            if (_file != null)
            {
                var central = (CentralViewModel)DataContext;
                central.ExportPath = _file.Path;
                central.ExportFileToken = StorageApplicationPermissions.FutureAccessList.Add(_file);
            }
        }

        private async Task UseFileOpen()
        {
            var fsp = new FileOpenPicker();
            fsp.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            fsp.FileTypeFilter.Add(".html");
            _file = await fsp.PickSingleFileAsync();
            if (_file != null)
            {
                var central = (CentralViewModel)DataContext;
                central.ExportPath = _file.Path;
                central.ExportFileToken = StorageApplicationPermissions.FutureAccessList.Add(_file);
            }
        }

        private async void ExportOnClick(object sender, RoutedEventArgs args)
        {
            var central = (CentralViewModel)DataContext;

            IEnumerable<EventViewModel> merged;
            var otherInfo = new HtmlExportHelper.DocInfo();

            string something = null;
            try
            {
                if (_file == null)
                {
                    _file = await StorageFile.GetFileFromPathAsync(central.ExportPath);
                }

                try
                {
                    var lines = await FileIO.ReadLinesAsync(_file);
                    var existing = lines.ReadFromTable(central, otherInfo);
                    merged = HtmlExportHelper.Merge(existing, central.LoggedEvents).ToList();
                }
                catch (Exception)
                {
                    merged = central.LoggedEvents;
                }

                if (string.IsNullOrWhiteSpace(otherInfo.Title))
                {
                    otherInfo.Title = _file.DisplayName;
                }
                var wlines = merged.WriteToTable(otherInfo);
                CachedFileManager.DeferUpdates(_file);
                await FileIO.WriteLinesAsync(_file, wlines);
                var status = await CachedFileManager.CompleteUpdatesAsync(_file);
                if (status != FileUpdateStatus.Complete)
                {
                    something = "File " + _file.Name + " couldn't be saved.";
                }
            }
            catch (Exception e)
            {
                something = e.Message;
            }

            if (something != null)
            {
                var dlg = new MessageDialog(string.Format("Details: {0}", something), "Error writing to file");
                await dlg.ShowAsync();
            }
        }

        private async void ViewOnClick(object sender, RoutedEventArgs args)
        {
            Exception something = null;
            try
            {
                var central = (CentralViewModel)DataContext;
                if (_file == null)
                {
                    _file = await StorageFile.GetFileFromPathAsync(central.ExportPath);
                }
                var html = await FileIO.ReadTextAsync(_file);
                Nav.NavigateToString(html);
            }
            catch (Exception e)
            {
                something = e;
            }
            if (something != null)
            {
                var dlg = new MessageDialog(string.Format("Details: {0}", something.Message), "Error accessing file");
                await dlg.ShowAsync();
            }
        }

        private void UserControlOnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            var upperHeight = InfoText.ActualHeight + Header.ActualHeight + InputGrid.ActualHeight + MergeButton.ActualHeight;
            UpperRow.Height = new GridLength(upperHeight);
            LowerRow.Height = new GridLength(MainGrid.ActualHeight - upperHeight);
        }
    }
}
