using Microsoft.OneDrive.Sdk;
using ParentingTrackerApp.ViewModels;
using System.Threading.Tasks;
using System;
using Windows.UI.Xaml.Controls;
using System.IO;
using System.Collections.Generic;
using ParentingTrackerApp.Helpers;
using System.Linq;
using Windows.UI.Popups;

namespace ParentingTrackerApp.Export
{
    public class OneDriveMobile
    {
        public OneDriveMobile(CentralViewModel cvm)
        {
            CentralViewModel = cvm;
        }

        public CentralViewModel CentralViewModel { get; }

        public AccountSession AccountSession { get; private set; }

        public IOneDriveClient OneDriveClient { get; private set; }

        public bool Connected
        {
            get
            {
                return OneDriveClient != null && AccountSession != null;
            }
        }

        public async Task Connect()
        {
            var scopes = new string[]
            {
                "wl.signin",
                "wl.offline_access",
                "onedrive.readwrite"
            };
            OneDriveClient = OneDriveClientExtensions.GetUniversalClient(scopes);
            AccountSession = await OneDriveClient.AuthenticateAsync();
        }

        private static void GetOneDrivePath(string userSpecifiedName, out string path, out string displayName,
            out string fileName)
        {
            if (string.IsNullOrWhiteSpace(userSpecifiedName))
            {
                throw new Exception("Please provide a valid path/file name");
            }
            displayName = Path.GetFileNameWithoutExtension(userSpecifiedName);
            fileName = displayName + ".html";
            path = "/drive/special/documents:/" + fileName;
        }

        private async Task CheckAndConnect()
        {
            if (!Connected)
            {
                await Connect();
            }
        }

        public async Task Merge()
        {
            string something = null;
            try
            {
                await CheckAndConnect();

                string path, displayName, fn;
                GetOneDrivePath(CentralViewModel.ExportPath, out path, out displayName, out fn);

                var lines = new List<string>();
                IEnumerable<EventViewModel> merged;
                var otherInfo = new HtmlExportHelper.DocInfo();
                try
                {
                    using (var stream = await OneDriveClient.ItemWithPath(path).Content.Request().GetAsync())
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            while (!sr.EndOfStream)
                            {
                                var line = await sr.ReadLineAsync();
                                if (line != null)
                                {
                                    lines.Add(line);
                                }
                            }
                        }
                    }
                    var existing = lines.ReadFromTable(CentralViewModel, otherInfo);
                    merged = HtmlExportHelper.Merge(existing, CentralViewModel.LoggedEvents).ToList();
                }
                catch (Exception)
                {
                    merged = CentralViewModel.LoggedEvents;
                }

                if (string.IsNullOrWhiteSpace(otherInfo.Title))
                {
                    otherInfo.Title = displayName;
                }

                var wlines = merged.WriteToTable(otherInfo);
                using (var memstream = new MemoryStream())
                {
                    var sw = new StreamWriter(memstream);
                    foreach (var line in wlines)
                    {
                        await sw.WriteLineAsync(line);
                    }
                    await sw.FlushAsync();
                    memstream.Position = 0;
                    await OneDriveClient.ItemWithPath(path).Content.Request().PutAsync<Item>(memstream);
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

        public async Task Clear()
        {
            string something = null;
            try
            {
                await CheckAndConnect();
                string path, displayName, fn;
                GetOneDrivePath(CentralViewModel.ExportPath, out path, out displayName, out fn);
                using (var memstream = new MemoryStream())
                {
                    // put an empty stream to clear the file
                    await OneDriveClient.ItemWithPath(path).Content.Request().PutAsync<Item>(memstream);
                }
            }
            catch (Exception e)
            {
                something = e.Message;
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
                await CheckAndConnect();

                string path, displayName, fn;
                GetOneDrivePath(CentralViewModel.ExportPath, out path, out displayName, out fn);
                var stream = await OneDriveClient.ItemWithPath(path).Content.Request().GetAsync();
                using (var sr = new StreamReader(stream))
                {
                    var html = sr.ReadToEnd();
                    nav.NavigateToString(html);
                }
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
    }
}
