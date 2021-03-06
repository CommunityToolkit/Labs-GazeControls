// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CommunityToolkit.Labs.Uwp.GazeControls
{
    /// <summary>
    /// This class provides a file-save picker dialog for UWP apps that is optimized for gaze input
    /// </summary>
    public sealed class GazeFileSavePicker : GazeFilePicker
    {
        private bool _newFolderMode;
        private Button _newFolderButton;
        private Button _enterFilenameButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="GazeFileSavePicker"/> class.
        /// </summary>
        public GazeFileSavePicker()
        {
            Title = GetString("FileSave/Title");
            FilePickerInitialized += OnGazeFileSavePickerInitialized;
        }

        private void ShowCommandspaceButtons(bool show)
        {
            var visibility = show ? Visibility.Visible : Visibility.Collapsed;
            _newFolderButton.Visibility = visibility;
            _enterFilenameButton.Visibility = visibility;

            _newFolderButton.Content = GetString("NewFolder");
            _enterFilenameButton.Content = GetString("EnterFilename");
            SelectButton.Content = GetString("Save");
            Button3.Content = GetString("Cancel");
        }

        private void OnEnterFilenameClicked(object sender, RoutedEventArgs e)
        {
            FilenameTextbox.Text = string.Empty;
            FilenameTextbox.TextChanged += OnFilenameChanged;
            ShowCommandspaceButtons(false);
            SetFilePickerView(FilePickerView.FilenameEntry);
        }

        private void OnFilenameChanged(object sender, TextChangedEventArgs e)
        {
            SelectButton.IsEnabled = FilenameTextbox.Text.Length > 0;
        }

        private void OnNewFolderClicked(object sender, RoutedEventArgs e)
        {
            _newFolderMode = true;
            FilenameTextbox.Text = string.Empty;
            FilenameTextbox.TextChanged += OnFilenameChanged;
            ShowCommandspaceButtons(false);
            SetFilePickerView(FilePickerView.FilenameEntry);
        }

        private void OnGazeFileSavePickerInitialized(object sender, EventArgs e)
        {
            _newFolderButton = Button0;
            _newFolderButton.Click += OnNewFolderClicked;

            _enterFilenameButton = Button1;
            _enterFilenameButton.Click += OnEnterFilenameClicked;

            SelectButton = Button2;
            SelectButton.Click += OnSaveButtonClicked;

            Button3.Click += OnCloseButtonClicked;

            YesButton.Click += OnSaveButtonClicked;
            NoButton.Click += OnCloseButtonClicked;

            ShowCommandspaceButtons(true);
        }

        private async void OnSaveButtonClicked(object sender, RoutedEventArgs e)
        {
            if (_newFolderMode)
            {
                _newFolderMode = false;
                await CreateFolderAsync();
                SetFilePickerView(FilePickerView.FileListing);
                ShowCommandspaceButtons(true);
            }
            else if (CurrentView == FilePickerView.FilenameEntry)
            {
                SelectedItem = await CreateFileAsync(true);
                Hide();
            }
            else if (CurrentView == FilePickerView.FileListing)
            {
                SetOverwriteWarningText();
                SetFilePickerView(FilePickerView.FileOverwriteConfirmation);
            }
            else
            {
                SelectedItem = await CreateFileAsync(false);
                Hide();
            }

            FilenameTextbox.TextChanged -= OnFilenameChanged;
        }

        private void OnCloseButtonClicked(object sender, RoutedEventArgs e)
        {
            if (CurrentView == FilePickerView.FileListing)
            {
                Hide();
            }
            else
            {
                SetFilePickerView(FilePickerView.FileListing);
                ShowCommandspaceButtons(true);
            }
        }
    }
}