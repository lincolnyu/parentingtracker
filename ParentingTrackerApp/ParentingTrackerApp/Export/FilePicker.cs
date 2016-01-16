using System;
using ParentingTrackerApp.ViewModels;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using ParentingTrackerApp.Helpers;
using Windows.Storage.Provider;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace ParentingTrackerApp.Export
{
    public class FilePicker
    {
        public FilePicker(CentralViewModel cvm)
        {
            CentralViewModel = cvm;
        }

        public StorageFile File { get; private set; }

        public CentralViewModel CentralViewModel { get; private set;}

        public async Task PickFile()
        {
            var fsp = new FileSavePicker();
            fsp.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            fsp.FileTypeChoices.Add("HTML files", new List<string>() { ".html" });
            fsp.SuggestedFileName = "New Document";
            File = await fsp.PickSaveFileAsync();
            if (File != null)
            {
                CentralViewModel.ExportPath = File.Path;
                CentralViewModel.ExportFileToken = StorageApplicationPermissions.FutureAccessList.Add(File);
            }
        }
        
        public async Task Merge()
        {
            string something = null;
            try
            {
                if (File == null)
                {
                    var fileNotFound = false;
                    try
                    {
                        File = await StorageFile.GetFileFromPathAsync(CentralViewModel.ExportPath);
                    }
                    catch (FileNotFoundException)
                    {
                        fileNotFound = true;
                    }
                    if (fileNotFound)
                    {
                        var dlg = new MessageDialog("File not found, please re-pick the file");
                        await dlg.ShowAsync();
                        return;
                    }
                }

                IEnumerable<EventViewModel> merged;
                var otherInfo = new HtmlExportHelper.DocInfo();
                try
                {
                    var lines = await FileIO.ReadLinesAsync(File);
                    var existing = lines.ReadFromTable(CentralViewModel, otherInfo);
                    merged = HtmlExportHelper.Merge(existing, CentralViewModel.LoggedEvents).ToList();
                }
                catch (Exception)
                {
                    merged = CentralViewModel.LoggedEvents;
                }

                if (string.IsNullOrWhiteSpace(otherInfo.Title))
                {
                    otherInfo.Title = File.DisplayName;
                }

                var wlines = merged.WriteToTable(otherInfo);
                var status = await WriteLinesToFile(wlines);
                if (status != FileUpdateStatus.Complete)
                {
                    something = "File " + File.Name + " couldn't be saved.";
                }
            }
            catch (Exception e)
            {
                something = e.Message;
                File = null;
            }

            if (something != null)
            {
                var dlg = new MessageDialog(string.Format("Details: {0}", something), "Error writing to file");
                await dlg.ShowAsync();
            }
        }

        public async Task Clear()
        {
            string something = null;
            try
            {
                if (File == null)
                {
                    File = await StorageFile.GetFileFromPathAsync(CentralViewModel.ExportPath);
                }
                // empty the file
                var wlines = HtmlExportHelper.GetEmptyHtmlLines(File.DisplayName);
                var status = await WriteLinesToFile(wlines);
                if (status != FileUpdateStatus.Complete)
                {
                    something = "File " + File.Name + " couldn't be cleared.";
                }
            }
            catch (Exception e)
            {
                something = e.Message;
            }
            finally
            {
                File = null;
            }

            if (something != null)
            {
                var dlg = new MessageDialog(string.Format("Details: {0}", something), "Error deleting file");
                await dlg.ShowAsync();
            }
        }

        public async Task View(WebView nav)
        {
            Exception something = null;
            try
            {
                if (File == null)
                {
                    File = await StorageFile.GetFileFromPathAsync(CentralViewModel.ExportPath);
                }
                var html = await FileIO.ReadTextAsync(File);
                nav.NavigateToString(html);
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

        private async Task<FileUpdateStatus> WriteLinesToFile(IEnumerable<string> wlines)
        {
            CachedFileManager.DeferUpdates(File);// TODO is this ncessary?
            await FileIO.WriteLinesAsync(File, wlines);
            var status = await CachedFileManager.CompleteUpdatesAsync(File);
            return status;
        }
    }
}
