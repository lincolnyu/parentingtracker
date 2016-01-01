using ParentingTrackerApp.ViewModels;
using System;
using System.Collections.Generic;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
            fsp.FileTypeChoices.Add("Excel files", new List<string>() { ".xlsx" });
            fsp.SuggestedFileName = "New Document";
            var file = await fsp.PickSaveFileAsync();
            if (file != null)
            {
                var central = (CentralViewModel)DataContext;
                central.ExportPath = file.Path;
            }
        }

        private void ExportOnClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
