using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitImage.Services
{
    internal static class SelectDirectoryDialog
    {
        /// <summary>
        /// Shows the directory selection dialog to a user
        /// </summary>
        /// <param name="title"></param>
        /// <returns>Directory path if succeed or an empty string if failed</returns>
        public static string ShowDialog(string title)
        {
            var dlg = new CommonOpenFileDialog();
            dlg.Title = title;
            dlg.IsFolderPicker = true;

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return dlg.FileName;
            } else
            {
                return string.Empty;
            }
        }
    }
}
