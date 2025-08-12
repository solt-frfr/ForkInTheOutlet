using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Interactivity;
using Avalonia.Media;
using SharpCompress;
using Avalonia.Platform.Storage;
using ModManagerBase.ViewModels;
using SharpCompress.Archives;
using SharpCompress.Common;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;


namespace ModManagerBase.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.axaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        /// This is largely copied from Pulsar. It's software also developed by me.
        private List<string> enabledmods = new List<string>();
        private bool isInitialized = false;
        private Profile currentprofile;
        private MainWindowViewModel viewModel = new MainWindowViewModel();
        
        public MainWindow()
        {
            InitializeComponent();
            ModsWindow(true);
            SettingsWindow.IsVisible = false;
            Directory.CreateDirectory(Misc.Paths.mods);
            var jsonoptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            if (!System.IO.File.Exists(Misc.Jsons.settings))
            {
                Settings settings = new Settings();
                settings.DefaultImage = 0;
                settings.CurrentProfile = 0;
                string jsonString = JsonSerializer.Serialize<Settings>(settings, jsonoptions);
                System.IO.File.WriteAllText(Misc.Jsons.settings, jsonString);
            }
            if (!System.IO.File.Exists(Misc.Jsons.profiles))
            {
                List<Profile> profiles = new List<Profile>();
                string jsonString = JsonSerializer.Serialize<List<Profile>>(profiles, jsonoptions);
                System.IO.File.WriteAllText(Misc.Jsons.profiles, jsonString);
                var mp = new MakeProf();
                mp.Show();
            }
            if (!System.IO.File.Exists(Misc.Jsons.enabled))
            {
                string jsonString = JsonSerializer.Serialize<List<string>>(new List<string>(), jsonoptions);
                System.IO.File.WriteAllText(Misc.Jsons.enabled, jsonString);
            }
            Settings settings2 = JsonSerializer.Deserialize<Settings>(System.IO.File.ReadAllText(Misc.Jsons.settings), jsonoptions);
            List<Profile> profiles2 = JsonSerializer.Deserialize<List<Profile>>(System.IO.File.ReadAllText(Misc.Jsons.profiles), jsonoptions);
            currentprofile = profiles2[settings2.CurrentProfile];
            Other_Click(null, null);
            Refresh();
            isInitialized = true;
            DataContext = viewModel;
        }

        private string[] CountFolders(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                string[] directories = Directory.GetDirectories(folderPath);
                int folderCount = directories.Length;
                return directories;
            }
            else
            {
                return null;
            }
        }

        public string CreateLinkImage(string link)
        {
            try
            {
                if (link.Contains("gamebanana.com"))
                {
                    return "Assets/Gamebanana.png";
                }
                else if (link.Contains("nexusmods.com"))
                {
                    return "Assets/Nexus.png";
                }
                else if (link.Contains("github.com"))
                {
                    return "Assets/Github.png";
                }
                else if (!string.IsNullOrWhiteSpace(link))
                {
                    return "Assets/Web.png";
                }
                else
                {
                    return null;
                }
            }
            catch { return null; }
        }
        public void Refresh()
        {
            try
            {
                try
                {
                    enabledmods.Clear();
                }
                catch { }
                enabledmods = QuickJson(false, enabledmods, "enabledmods.json");
            }
            catch { }
            viewModel.AllMods.Clear();
            string[] griditems = CountFolders(Path.Combine(Misc.Paths.mods, currentprofile.Name));
            Settings settings = new Settings();
            if (System.IO.File.Exists(Misc.Jsons.settings))
            {
                var jsonoptions = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string jsonString = System.IO.File.ReadAllText(Misc.Jsons.settings);
                settings = JsonSerializer.Deserialize<Settings>(jsonString, jsonoptions);
                DefPrevBox.SelectedIndex = settings.DefaultImage; 
                if (DefPrevBox.SelectedIndex < 0 || settings.DefaultImage < 0)
                {
                    settings.DefaultImage = 0;
                    DefPrevBox.SelectedIndex = 0;
                }
                string[] previews = Directory.GetFiles(Misc.Paths.previews);
                try
                {
                    var sortedpreviews = previews.OrderBy(i => i).ToList();
                    viewModel.AllMods.Clear();
                    Preview.Source = new Bitmap(sortedpreviews[settings.DefaultImage]);
                    foreach (var item in sortedpreviews)
                    {
                        bool found = false;
                        foreach (ComboBoxItem current in DefPrevBox.Items)
                        {
                            if (current.Content.Equals(Path.GetFileName(item)))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            DefPrevBox.Items.Add(new ComboBoxItem
                            {
                                Content = Path.GetFileName(item),
                            });
                        }
                    }
                }
                catch (Exception e)
                {
                    Preview.Source = new Bitmap(AssetLoader.Open(new Uri($"avares://{App.projectName}/Assets/Preview0.png", UriKind.RelativeOrAbsolute)));
                }
            }
            foreach (string modpath in griditems)
            {
                Meta mod = new Meta();
                string filepath = Path.Combine(modpath, "meta.json");
                if (!System.IO.File.Exists(filepath))
                {
                    string genid = modpath.Replace(Path.Combine(Misc.Paths.mods, currentprofile.Name), "");
                    mod.Name = mod.ID = genid = genid.TrimStart(Path.DirectorySeparatorChar);
                    mod.Profile = currentprofile.Name;
                    var jsonoptions = new JsonSerializerOptions
                    {
                        WriteIndented = true
                    };
                    string jsonString = JsonSerializer.Serialize(mod, jsonoptions);
                    System.IO.File.WriteAllText(filepath, jsonString);
                }
                if (System.IO.File.Exists(filepath))
                {
                    var jsonoptions = new JsonSerializerOptions
                    {
                        WriteIndented = true
                    };
                    string jsonString = System.IO.File.ReadAllText(filepath);
                    mod = JsonSerializer.Deserialize<Meta>(jsonString, jsonoptions);
                    if (!viewModel.AllMods.Contains(mod) && mod.Profile == currentprofile.Name)
                    {
                        if (enabledmods.Contains(mod.ID))
                            mod.IsChecked = true;
                        else
                            mod.IsChecked = false;
                        mod.LinkImage = CreateLinkImage(mod.Link);
                        viewModel.AllMods.Add(mod);
                    }
                }
            }
            var sorted = viewModel.AllMods.OrderBy(i => i.Name).ToList();
            viewModel.AllMods.Clear();  // Remove all current items
            foreach (var item in sorted)
            {
                viewModel.AllMods.Add(item);  // Re-add in sorted order
            }
            this.DataContext = viewModel;
        }
        private void New_OnClick(object sender, RoutedEventArgs e)
        {
            var mp = new MakePack(currentprofile.Name);
            mp.Show();
        }

        private void Folder_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var item in ModDataGrid.SelectedItems)
            {
                Meta row = (Meta)item;
                if (row != null)
                {
                    if (Directory.Exists(Path.Combine(Misc.Paths.mods, row.ID)))
                    {
                        try
                        {
                            ProcessStartInfo StartInformation = new ProcessStartInfo();
                            StartInformation.FileName = Path.Combine(Misc.Paths.mods, row.ID);
                            StartInformation.UseShellExecute = true;
                            Process process = Process.Start(StartInformation);
                        }
                        catch { }
                    }
                }
            }
        }

        private async void Delete_OnClick(object sender, RoutedEventArgs e)
        {
            string modpath = "";
            foreach (var item in ModDataGrid.SelectedItems)
            {
                Meta row = (Meta)item;
                if (row != null)
                {
                    foreach (string path in CountFolders(Path.Combine(Misc.Paths.mods, currentprofile.Name)))
                    {
                        Meta mod = new Meta();
                        string filepath = Path.Combine(path, "meta.json");
                        if (!System.IO.File.Exists(filepath))
                        {
                            continue;
                        }
                        if (System.IO.File.Exists(filepath))
                        {
                            var jsonoptions = new JsonSerializerOptions
                            {
                                WriteIndented = true
                            };
                            string jsonString = System.IO.File.ReadAllText(filepath);
                            mod = JsonSerializer.Deserialize<Meta>(jsonString, jsonoptions);
                            if (mod.ID == row.ID)
                            {
                                modpath = path;
                            }
                        }
                    }
                    var box = MessageBoxManager.GetMessageBoxStandard(
                        $"Delete {row.Name}",
                        $"Are you sure you want to delete {row.Name}?",
                        ButtonEnum.YesNo,
                        MsBox.Avalonia.Enums.Icon.Question
                    );

                    var result = await box.ShowAsPopupAsync(this);

                    if (result == ButtonResult.Yes)
                    {
                        Directory.Delete(modpath, true);
                        break;
                    }
                }
            }
            Refresh();
        }

        private void currentrow(object sender, SelectionChangedEventArgs e)
        {
            Meta row = (Meta)ModDataGrid.SelectedItem;
            string modpath = "";
            try
            {
                if (string.IsNullOrWhiteSpace(row.Description))
                    DescBox.Text = "Create a mod manager yourself with this base. You're seeing this because this mod has no description, or no mod is selected.\n\nConfused about the buttons at the bottom? Hover over them for more info.";
                else
                    DescBox.Text = row.Description;
            }
            catch
            {
                DescBox.Text = "Create a mod manager yourself with this base. You're seeing this because this mod has no description, or no mod is selected.\n\nConfused about the buttons at the bottom? Hover over them for more info.";
            }
            try
            {
                foreach (string path in CountFolders(Misc.Paths.mods))
                {
                    Meta mod = new Meta();
                    string filepath = Path.Combine(path, "meta.json");
                    if (!System.IO.File.Exists(filepath))
                    {
                        continue;
                    }
                    if (System.IO.File.Exists(filepath))
                    {
                        var jsonoptions = new JsonSerializerOptions
                        {
                            WriteIndented = true
                        };
                        string jsonString = System.IO.File.ReadAllText(filepath);
                        mod = JsonSerializer.Deserialize<Meta>(jsonString, jsonoptions);
                        if (mod.ID == row.ID)
                        {
                            modpath = path;
                        }
                    }
                }
                if (System.IO.File.Exists(Path.Combine(modpath, "preview.webp")))
                {
                    string imagePath = Path.Combine(modpath, "preview.webp");

                    if (File.Exists(imagePath))
                    {
                        using var stream = File.OpenRead(imagePath);
                        Preview.Source = new Bitmap(stream);
                    }
                }
                else
                {
                    Preview.Source = new Bitmap(AssetLoader.Open(new Uri($"avares://{App.projectName}/Assets/Preview{DefPrevBox.SelectedIndex}.png", UriKind.RelativeOrAbsolute)));
                }
            }
            catch
            {
                Preview.Source = new Bitmap(AssetLoader.Open(new Uri($"avares://{App.projectName}/Assets/Preview{DefPrevBox.SelectedIndex}.png", UriKind.RelativeOrAbsolute)));
            }
        }

        private void Mods_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(currentprofile.ModsImage) && !string.IsNullOrWhiteSpace(currentprofile.ModsImage))
            {
                ModsImage.Source = new Bitmap(currentprofile.ModsImage);
            }
            else
            {
                ModsImage.Source = new Bitmap(AssetLoader.Open(new Uri($"avares://{App.projectName}/Assets/ModsSel.png", UriKind.RelativeOrAbsolute)));
            }
            if (File.Exists(currentprofile.UnSettingsImage) && !string.IsNullOrWhiteSpace(currentprofile.UnSettingsImage))
            {
                SettingsImage.Source = new Bitmap(currentprofile.UnSettingsImage);
            }
            else
            {
                SettingsImage.Source = new Bitmap(AssetLoader.Open(new Uri($"avares://{App.projectName}/Assets/SettingsUnsel.png", UriKind.RelativeOrAbsolute)));
            }
            ModsWindow(true);
            SettingsWindow.IsVisible = false;
        }
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(currentprofile.UnModsImage) && !string.IsNullOrWhiteSpace(currentprofile.UnModsImage))
            {
                ModsImage.Source = new Bitmap(currentprofile.UnModsImage);
            }
            else
            {
                ModsImage.Source = new Bitmap(AssetLoader.Open(new Uri($"avares://{App.projectName}/Assets/ModsUnsel.png", UriKind.RelativeOrAbsolute)));
            }
            if (File.Exists(currentprofile.SettingsImage) && !string.IsNullOrWhiteSpace(currentprofile.SettingsImage))
            {
                SettingsImage.Source = new Bitmap(currentprofile.SettingsImage);
            }
            else
            {
                SettingsImage.Source = new Bitmap(AssetLoader.Open(new Uri($"avares://{App.projectName}/Assets/SettingsSel.png", UriKind.RelativeOrAbsolute)));
            }
            ModsWindow(false);
            SettingsWindow.IsVisible = true;
        }
        private void Other_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Misc.Jsons.profiles) && File.Exists(Misc.Jsons.settings))
            {
                var jsonoptions = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                List<Profile> profiles = JsonSerializer.Deserialize<List<Profile>>(System.IO.File.ReadAllText(Misc.Jsons.profiles), jsonoptions);
                Settings settings = JsonSerializer.Deserialize<Settings>(System.IO.File.ReadAllText(Misc.Jsons.settings), jsonoptions);
                if (sender != null)
                {
                    settings.CurrentProfile += 1;
                    if (settings.CurrentProfile >= profiles.Count)
                    {
                        settings.CurrentProfile = 0;
                    } 
                }
                System.IO.File.WriteAllText(Misc.Jsons.settings, JsonSerializer.Serialize<Settings>(settings, jsonoptions));
                currentprofile = profiles[settings.CurrentProfile];
                GameName.Text = currentprofile.Name;
                if (File.Exists(currentprofile.SwitchImage) && !string.IsNullOrWhiteSpace(currentprofile.SwitchImage))
                {
                    MusicImage.Source = new Bitmap(currentprofile.SwitchImage);
                }
                else
                {
                    MusicImage.Source = new Bitmap(AssetLoader.Open(new Uri($"avares://{App.projectName}/Assets/OtherUnsel.png", UriKind.RelativeOrAbsolute)));
                }
                if (File.Exists(currentprofile.DownloadImage) && !string.IsNullOrWhiteSpace(currentprofile.DownloadImage))
                {
                    DownloadImage.Source = new Bitmap(currentprofile.DownloadImage);
                }
                else
                {
                    DownloadImage.Source = new Bitmap(AssetLoader.Open(new Uri($"avares://{App.projectName}/Assets/DownloadUnsel.png", UriKind.RelativeOrAbsolute)));
                }
                if (File.Exists(currentprofile.DeployImage) && !string.IsNullOrWhiteSpace(currentprofile.DeployImage))
                {
                    DeployImage.Source = new Bitmap(currentprofile.DeployImage);
                }
                else
                {
                    DeployImage.Source = new Bitmap(AssetLoader.Open(new Uri($"avares://{App.projectName}/Assets/Deploy.png", UriKind.RelativeOrAbsolute)));
                }
                if (File.Exists(currentprofile.OpenImage) && !string.IsNullOrWhiteSpace(currentprofile.OpenImage))
                {
                    OpenFolderImage.Source = new Bitmap(currentprofile.OpenImage);
                }
                else
                {
                    OpenFolderImage.Source = new Bitmap(AssetLoader.Open(new Uri($"avares://{App.projectName}/Assets/OpenFolder.png", UriKind.RelativeOrAbsolute)));
                }
                if (File.Exists(currentprofile.NewImage) && !string.IsNullOrWhiteSpace(currentprofile.NewImage))
                {
                    NewImage.Source = new Bitmap(currentprofile.NewImage);
                }
                else
                {
                    NewImage.Source = new Bitmap(AssetLoader.Open(new Uri($"avares://{App.projectName}/Assets/plus.png", UriKind.RelativeOrAbsolute)));
                }
                if (File.Exists(currentprofile.RefreshImage) && !string.IsNullOrWhiteSpace(currentprofile.RefreshImage))
                {
                    RefreshImage.Source = new Bitmap(currentprofile.RefreshImage);
                }
                else
                {
                    RefreshImage.Source = new Bitmap(AssetLoader.Open(new Uri($"avares://{App.projectName}/Assets/Refresh.png", UriKind.RelativeOrAbsolute)));
                }
                if (!string.IsNullOrWhiteSpace(currentprofile.BGColor) && Color.TryParse(currentprofile.BGColor, out Color color))
                {
                    Background = Brush.Parse(currentprofile.BGColor);
                }
                else
                {
                    Background = Brush.Parse("#483D8B");
                }
                if (!string.IsNullOrWhiteSpace(currentprofile.GridColor) && Color.TryParse(currentprofile.GridColor, out Color color2))
                {
                    ModDataGrid.Background = Brush.Parse(currentprofile.GridColor);
                    ModDataGrid.RowBackground = Brush.Parse(currentprofile.GridColor);
                }
                else
                {
                    ModDataGrid.Background = Brush.Parse("#557E8A");
                    ModDataGrid.RowBackground = Brush.Parse("#557E8A");
                }
                if (!string.IsNullOrWhiteSpace(currentprofile.DescColor) && Color.TryParse(currentprofile.DescColor, out Color color3))
                {
                    DescBox.Background = Brush.Parse(currentprofile.DescColor);
                }
                else
                {
                    DescBox.Background = Brush.Parse("#F04080");
                }
                if (!string.IsNullOrWhiteSpace(currentprofile.BGColor) && Color.TryParse(currentprofile.BGColor, out Color color4))
                {
                    SettingsWindow.Background = Brush.Parse(currentprofile.BGColor);
                }
                else
                {
                    SettingsWindow.Background = Brush.Parse("#005ADA");
                }
                Directory.CreateDirectory(Path.Combine(Misc.Paths.mods, currentprofile.Name));
                Mods_Click(null, null);
            }
        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new ProcessStartInfo
                {
                    FileName = currentprofile.Link,
                    UseShellExecute = true
                });
            }
            catch { }
        }
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private async void Deploy_Click(object sender, RoutedEventArgs e)
        {
            Directory.Delete(currentprofile.DeployPath, true);
            Directory.CreateDirectory(currentprofile.DeployPath);
            foreach (string path in CountFolders(Path.Combine(Misc.Paths.mods, currentprofile.Name)))
            {
                Meta mod = new Meta();
                string filepath = Path.Combine(path, "meta.json");
                if (!System.IO.File.Exists(filepath))
                {
                    continue;
                }
                if (System.IO.File.Exists(filepath))
                {
                    var jsonoptions = new JsonSerializerOptions
                    {
                        WriteIndented = true
                    };
                    string jsonString = System.IO.File.ReadAllText(filepath);
                    mod = JsonSerializer.Deserialize<Meta>(jsonString, jsonoptions);
                    if (enabledmods.Contains(mod.ID) && mod.Profile == currentprofile.Name)
                    {
                        Misc.BetterDirCopy(path, currentprofile.DeployPath, false);
                    }
                }
            }

            try
            {
                File.Delete(Path.Combine(currentprofile.DeployPath, "meta.json"));
                File.Delete(Path.Combine(currentprofile.DeployPath, "preview.webp"));
            }
            catch
            {
            }
        }

        private void ModsWindow(bool sender)
        {
            if (sender == false)
            {
                Mods.IsVisible = false;
                ModContent.IsVisible = false;
            }
            else
            {
                Mods.IsVisible = true;
                ModContent.IsVisible = true;
            }
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(Misc.Paths.mods))
            {
                ProcessStartInfo StartInformation = new ProcessStartInfo();
                StartInformation.FileName = Path.Combine(Misc.Paths.mods, currentprofile.Name);
                StartInformation.UseShellExecute = true;
                Process process = Process.Start(StartInformation);
            }
        }

        private void OpenLink_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is string url)
            {
                try
                {
                    System.Diagnostics.Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                catch { }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                var row = checkBox.DataContext as Meta;
                if (row != null)
                {
                    if (!enabledmods.Contains(row.ID))
                        enabledmods.Add(row.ID);
                    QuickJson(true, enabledmods, "enabledmods.json");
                    enabledmods = QuickJson(false, enabledmods, "enabledmods.json");
                }
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                var row = checkBox.DataContext as Meta;
                if (row != null)
                {
                    if (enabledmods.Contains(row.ID))
                        enabledmods.Remove(row.ID);
                    QuickJson(true, enabledmods, "enabledmods.json");
                    enabledmods = QuickJson(false, enabledmods, "enabledmods.json");
                }
            }
        }

        private List<string> QuickJson(bool write, List<string> what, string filename)
        {
            if (write)
            {
                var jsonoptions = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string jsonString = JsonSerializer.Serialize(what, jsonoptions);
                System.IO.File.WriteAllText(Path.Combine(Misc.Paths.program, filename), jsonString);
                return null;
            }
            else
            {
                var jsonoptions = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string jsonString = System.IO.File.ReadAllText(Path.Combine(Misc.Paths.program, filename));
                what = JsonSerializer.Deserialize<List<string>>(jsonString, jsonoptions);
                return what;
            }
        }

        public static string[] ListToArray(List<string> sender)
        {
            string[] send = new string[sender.Count];
            for (var i = 0; i < sender.Count; ++i)
                send[i] = sender[i];
            return send;
        }

        private void DefPrevBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInitialized) return;
            Settings settings = new Settings();
            settings.DefaultImage = DefPrevBox.SelectedIndex;
            string jsonString = System.IO.File.ReadAllText(Misc.Jsons.settings);
            var jsonoptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            jsonString = JsonSerializer.Serialize(settings, jsonoptions);
            System.IO.File.WriteAllText(Misc.Jsons.settings, jsonString);
            settings = JsonSerializer.Deserialize<Settings>(jsonString, jsonoptions);
            Refresh();
        }

        private void Edit_OnClick(object sender, RoutedEventArgs e)
        {
            string modpath = "";
            Meta row = (Meta)ModDataGrid.SelectedItem;
            foreach (string path in CountFolders(Path.Combine(Misc.Paths.mods, currentprofile.Name)))
            {
                Meta mod = new Meta();
                string filepath = Path.Combine(path, "meta.json");
                if (!System.IO.File.Exists(filepath))
                {
                    continue;
                }
                if (System.IO.File.Exists(filepath))
                {
                    var jsonoptions = new JsonSerializerOptions
                    {
                        WriteIndented = true
                    };
                    string jsonString = System.IO.File.ReadAllText(filepath);
                    mod = JsonSerializer.Deserialize<Meta>(jsonString, jsonoptions);
                    if (mod.ID == row.ID)
                    {
                        modpath = path;
                    }
                }
            }
            MakePack edit = new MakePack(row, currentprofile.Name, modpath);
            Preview.Source = new Bitmap(AssetLoader.Open(new Uri($"avares://{App.projectName}/Assets/Preview{DefPrevBox.SelectedIndex}.png", UriKind.RelativeOrAbsolute)));
            try
            {
                edit.ShowDialog(this);
            }
            catch { }
            Refresh();
        }

        private void NewProf_OnClick(object? sender, RoutedEventArgs e)
        {
            MakeProf mp = new MakeProf();
            mp.Show();
        }
    }
}

