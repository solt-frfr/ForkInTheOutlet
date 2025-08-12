using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Reflection;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using System.Diagnostics;
using ModManagerBase;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using Avalonia.Platform.Storage;

namespace ModManagerBase
{
    /// <summary>
    /// Interaction logic for MakePack.xaml
    /// </summary>
    public partial class MakePack : Window
    {
        private Meta modmetadata = new Meta();
        private bool UserID = false;
        private string Profile;
        private string FilePath;

        public MakePack() // Don't use. This is for the designer.
        {
            InitializeComponent();
            this.Topmost = true;
        }
        public MakePack(string profile)
        {
            InitializeComponent();
            Profile = profile;
            this.Topmost = true;
        }

        public MakePack(Meta sender, string profile, string filepath)
        {
            Profile = profile;
            InitializeComponent();
            this.Topmost = true;
            modmetadata = sender;
            FilePath = filepath;
            try
            {
                if (sender.Name != null || sender.ID != null)
                {
                    Title = $"Edit {sender.Name}";
                    NameBox.Text = sender.Name;
                    DescBox.Text = sender.Description;
                    AuthorBox.Text = sender.Authors;
                    LinkBox.Text = sender.Link;
                    IDBox.Text = sender.ID;
                    if (!string.IsNullOrWhiteSpace(sender.ID))
                    {
                        IDBox.IsEnabled = false;
                        UserID = true;
                    }
                    OpenButton.IsEnabled = !sender.ArchiveImage;
                }
            }
            catch
            {
                Close();
            }
        }
        private async void Open_Click(object sender, RoutedEventArgs e)
        {
            var files = await this.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select Preview",
                FileTypeFilter = new List<FilePickerFileType>
                {
                    new FilePickerFileType("Preview Image")
                    {
                        Patterns = new List<string> { "*.*" }
                    }
                },
                AllowMultiple = false
            });
            if (files != null)
            {
                PreviewBox.Text = files[0].Path.LocalPath;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FilePath))
            {
                FilePath = Path.Combine(Misc.Paths.mods, Profile, IDBox.Text);
            }
            modmetadata.Name = NameBox.Text;
            modmetadata.Description = DescBox.Text;
            modmetadata.Authors = AuthorBox.Text;
            modmetadata.Link = LinkBox.Text;
            modmetadata.ID = IDBox.Text;
            modmetadata.Profile = Profile;
            if (!string.IsNullOrWhiteSpace(modmetadata.ID))
            {
                var jsonoptions = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string jsonString = JsonSerializer.Serialize(modmetadata, jsonoptions);
                string filepath = Path.Combine(FilePath, "meta.json");
                Directory.CreateDirectory(FilePath);
                File.WriteAllText(filepath, jsonString);
                filepath = Path.Combine(FilePath, "preview.webp");
                if (File.Exists(PreviewBox.Text))
                {
                    using (SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(PreviewBox.Text))
                    {
                        image.Save(filepath, new WebpEncoder());
                    }
                }
                Close();
            }
        }

        private void NameChanged(object sender, TextChangedEventArgs e)
        {
            string idtext = NameBox.Text.Trim();
            idtext = idtext.ToLower();
            idtext = idtext.Replace(" ", string.Empty);
            if (UserID == false)
                IDBox.Text = idtext;
        }

        private void IDChanged(object sender, TextChangedEventArgs e)
        {
            string idtext = IDBox.Text.Trim();
            idtext = idtext.ToLower();
            idtext = idtext.Replace(" ", string.Empty);
            IDBox.Text = idtext;
        }
        private void IDBox_KeyDown(object sender, TextInputEventArgs e)
        {
            UserID = true;
            if (string.IsNullOrEmpty(e.Text) || e.Text.Length != 1)
            {
                e.Handled = true;
                return;
            }
            char keyChar = e.Text[0];
            e.Handled = !char.IsLetterOrDigit(keyChar) || !char.IsPunctuation(keyChar);
            if (e.Handled == false)
            {
                IDBox.Text = IDBox.Text.TrimEnd(keyChar);
            }
        }

        private void IDBox_KeyDown(object sender, KeyEventArgs e)
        {
            UserID = true;
        }
    }
}
