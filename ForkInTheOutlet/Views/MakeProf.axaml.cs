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


namespace ModManagerBase.Views;

public partial class MakeProf : Window
{
    private Profile gameprofile = new Profile();
    private bool UserID = false;

    public MakeProf() // Don't use. This is for the designer.
    {
        InitializeComponent();
        this.Topmost = true;
    }

    public MakeProf(Profile sender)
    {
        InitializeComponent();
        this.Topmost = true;
        gameprofile = sender;
        try
        {
            if (!string.IsNullOrWhiteSpace(sender.Name))
            {
                Title = $"Edit {sender.Name}";
                NameBox.Text = sender.Name;
                PathBox.Text = sender.DeployPath;
                LinkBox.Text = sender.Link;
            }
        }
        catch
        {
            Close();
        }
    }
    private async void ModsOpen_Click(object sender, RoutedEventArgs e)
    {
        var files = await this.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select \"Mods\" Selected Image",
            FileTypeFilter = new List<FilePickerFileType>
            {
                new FilePickerFileType("\"Mods\" Selected Image")
                {
                    Patterns = new List<string> { "*.*" }
                }
            },
            AllowMultiple = false
        });
        if (File.Exists(files[0].Path.LocalPath))
        {
            ModsBox.Text = files[0].Path.LocalPath;
        }
    }
    
    private async void SwitchOpen_Click(object sender, RoutedEventArgs e)
    {
        var files = await this.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select \"Switch\" Selected Image",
            FileTypeFilter = new List<FilePickerFileType>
            {
                new FilePickerFileType("\"Switch\" Selected Image")
                {
                    Patterns = new List<string> { "*.*" }
                }
            },
            AllowMultiple = false
        });
        if (File.Exists(files[0].Path.LocalPath))
        {
            SwitchBox.Text = files[0].Path.LocalPath;
        }
    }
    
    private async void SettingsOpen_Click(object sender, RoutedEventArgs e)
    {
        var files = await this.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select \"Settings\" Selected Image",
            FileTypeFilter = new List<FilePickerFileType>
            {
                new FilePickerFileType("\"Settings\" Selected Image")
                {
                    Patterns = new List<string> { "*.*" }
                }
            },
            AllowMultiple = false
        });
        if (File.Exists(files[0].Path.LocalPath))
        {
            SettingsBox.Text = files[0].Path.LocalPath;
        }
    }
    
    private async void Mods2Open_Click(object sender, RoutedEventArgs e)
    {
        var files = await this.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select \"Mods\" Unselected Image",
            FileTypeFilter = new List<FilePickerFileType>
            {
                new FilePickerFileType("\"Mods\" Unselected Image")
                {
                    Patterns = new List<string> { "*.*" }
                }
            },
            AllowMultiple = false
        });
        if (File.Exists(files[0].Path.LocalPath))
        {
            Mods2Box.Text = files[0].Path.LocalPath;
        }
    }
    
    
    private async void Settings2Open_Click(object sender, RoutedEventArgs e)
    {
        var files = await this.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select \"Settings\" Unselected Image",
            FileTypeFilter = new List<FilePickerFileType>
            {
                new FilePickerFileType("\"Settings\" Unselected Image")
                {
                    Patterns = new List<string> { "*.*" }
                }
            },
            AllowMultiple = false
        });
        if (File.Exists(files[0].Path.LocalPath))
        {
            Settings2Box.Text = files[0].Path.LocalPath;
        }
    }
    
    private async void DownloadOpen_Click(object sender, RoutedEventArgs e)
    {
        var files = await this.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select \"Download\" Image",
            FileTypeFilter = new List<FilePickerFileType>
            {
                new FilePickerFileType("\"Download\" Image")
                {
                    Patterns = new List<string> { "*.*" }
                }
            },
            AllowMultiple = false
        });
        if (File.Exists(files[0].Path.LocalPath))
        {
            DownloadBox.Text = files[0].Path.LocalPath;
        }
    }
    
    private async void DeployOpen_Click(object sender, RoutedEventArgs e)
    {
        var files = await this.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select Deploy Image",
            FileTypeFilter = new List<FilePickerFileType>
            {
                new FilePickerFileType("Deploy Image")
                {
                    Patterns = new List<string> { "*.*" }
                }
            },
            AllowMultiple = false
        });
        if (File.Exists(files[0].Path.LocalPath))
        {
            DeployBox.Text = files[0].Path.LocalPath;
        }
    }
    
    private async void NewOpen_Click(object sender, RoutedEventArgs e)
    {
        var files = await this.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select New Mod Image",
            FileTypeFilter = new List<FilePickerFileType>
            {
                new FilePickerFileType("New Mod Image")
                {
                    Patterns = new List<string> { "*.*" }
                }
            },
            AllowMultiple = false
        });
        if (File.Exists(files[0].Path.LocalPath))
        {
            NewBox.Text = files[0].Path.LocalPath;
        }
    }
    
    private async void OpenOpen_Click(object sender, RoutedEventArgs e)
    {
        var files = await this.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select Open Folder Image",
            FileTypeFilter = new List<FilePickerFileType>
            {
                new FilePickerFileType("Open Folder Image")
                {
                    Patterns = new List<string> { "*.*" }
                }
            },
            AllowMultiple = false
        });
        if (File.Exists(files[0].Path.LocalPath))
        {
            OpenBox.Text = files[0].Path.LocalPath;
        }
    }
    
    private async void RefreshOpen_Click(object sender, RoutedEventArgs e)
    {
        var files = await this.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select Refresh Image",
            FileTypeFilter = new List<FilePickerFileType>
            {
                new FilePickerFileType("Refresh Image")
                {
                    Patterns = new List<string> { "*.*" }
                }
            },
            AllowMultiple = false
        });
        if (File.Exists(files[0].Path.LocalPath))
        {
            RefreshBox.Text = files[0].Path.LocalPath;
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Confirm_Click(object sender, RoutedEventArgs e)
    {
        gameprofile.Name = NameBox.Text;
        gameprofile.DeployPath = PathBox.Text;
        gameprofile.Link = LinkBox.Text;
        gameprofile.BGColor = BGCBox.Text;
        gameprofile.GridColor = MGCBox.Text;
        gameprofile.SettingsColor = SCBox.Text;
        gameprofile.DescColor = DCBox.Text;
        
        if (!string.IsNullOrWhiteSpace(gameprofile.Name))
        {
            if (File.Exists(ModsBox.Text))
            {
                Directory.CreateDirectory(Path.Combine(Misc.Paths.profileassets, gameprofile.Name));
                string filepath = Path.Combine(Misc.Paths.profileassets, gameprofile.Name, Path.GetFileNameWithoutExtension(ModsBox.Text) + ".webp"); 
                using (SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(ModsBox.Text))
                {
                    image.Save(filepath, new WebpEncoder());
                }
                gameprofile.ModsImage = filepath;
            }
            if (File.Exists(SwitchBox.Text))
            {
                Directory.CreateDirectory(Path.Combine(Misc.Paths.profileassets, gameprofile.Name));
                string filepath = Path.Combine(Misc.Paths.profileassets, gameprofile.Name, Path.GetFileNameWithoutExtension(SwitchBox.Text) + ".webp"); 
                using (SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(SwitchBox.Text))
                {
                    image.Save(filepath, new WebpEncoder());
                }
                gameprofile.SwitchImage = filepath;
            }
            if (File.Exists(SettingsBox.Text))
            {
                Directory.CreateDirectory(Path.Combine(Misc.Paths.profileassets, gameprofile.Name));
                string filepath = Path.Combine(Misc.Paths.profileassets, gameprofile.Name, Path.GetFileNameWithoutExtension(SettingsBox.Text) + ".webp"); 
                using (SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(SettingsBox.Text))
                {
                    image.Save(filepath, new WebpEncoder());
                }
                gameprofile.SettingsImage = filepath;
            }
            if (File.Exists(Mods2Box.Text))
            {
                Directory.CreateDirectory(Path.Combine(Misc.Paths.profileassets, gameprofile.Name));
                string filepath = Path.Combine(Misc.Paths.profileassets, gameprofile.Name, Path.GetFileNameWithoutExtension(Mods2Box.Text) + ".webp"); 
                using (SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(Mods2Box.Text))
                {
                    image.Save(filepath, new WebpEncoder());
                }
                gameprofile.UnModsImage = filepath;
            }
            if (File.Exists(Settings2Box.Text))
            {
                Directory.CreateDirectory(Path.Combine(Misc.Paths.profileassets, gameprofile.Name));
                string filepath = Path.Combine(Misc.Paths.profileassets, gameprofile.Name, Path.GetFileNameWithoutExtension(Settings2Box.Text) + ".webp"); 
                using (SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(Settings2Box.Text))
                {
                    image.Save(filepath, new WebpEncoder());
                }
                gameprofile.UnSettingsImage = filepath;
            }
            if (File.Exists(DownloadBox.Text))
            {
                Directory.CreateDirectory(Path.Combine(Misc.Paths.profileassets, gameprofile.Name));
                string filepath = Path.Combine(Misc.Paths.profileassets, gameprofile.Name, Path.GetFileNameWithoutExtension(DownloadBox.Text) + ".webp"); 
                using (SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(DownloadBox.Text))
                {
                    image.Save(filepath, new WebpEncoder());
                }
                gameprofile.DownloadImage = filepath;
            }
            if (File.Exists(DeployBox.Text))
            {
                Directory.CreateDirectory(Path.Combine(Misc.Paths.profileassets, gameprofile.Name));
                string filepath = Path.Combine(Misc.Paths.profileassets, gameprofile.Name, Path.GetFileNameWithoutExtension(DeployBox.Text) + ".webp"); 
                using (SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(DeployBox.Text))
                {
                    image.Save(filepath, new WebpEncoder());
                }
                gameprofile.DeployImage = filepath;
            }
            if (File.Exists(NewBox.Text))
            {
                Directory.CreateDirectory(Path.Combine(Misc.Paths.profileassets, gameprofile.Name));
                string filepath = Path.Combine(Misc.Paths.profileassets, gameprofile.Name, Path.GetFileNameWithoutExtension(NewBox.Text) + ".webp"); 
                using (SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(NewBox.Text))
                {
                    image.Save(filepath, new WebpEncoder());
                }
                gameprofile.NewImage = filepath;
            }
            if (File.Exists(OpenBox.Text))
            {
                Directory.CreateDirectory(Path.Combine(Misc.Paths.profileassets, gameprofile.Name));
                string filepath = Path.Combine(Misc.Paths.profileassets, gameprofile.Name, Path.GetFileNameWithoutExtension(OpenBox.Text) + ".webp"); 
                using (SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(OpenBox.Text))
                {
                    image.Save(filepath, new WebpEncoder());
                }
                gameprofile.OpenImage = filepath;
            }
            if (File.Exists(RefreshBox.Text))
            {
                Directory.CreateDirectory(Path.Combine(Misc.Paths.profileassets, gameprofile.Name));
                string filepath = Path.Combine(Misc.Paths.profileassets, gameprofile.Name, Path.GetFileNameWithoutExtension(RefreshBox.Text) + ".webp"); 
                using (SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(RefreshBox.Text))
                {
                    image.Save(filepath, new WebpEncoder());
                }
                gameprofile.RefreshImage = filepath;
            }
            var jsonoptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            List<Profile> profiles = new List<Profile>();
            profiles = JsonSerializer.Deserialize<List<Profile>>(File.ReadAllText(Misc.Jsons.profiles), jsonoptions);
            profiles.Add(gameprofile);
            string jsonString = JsonSerializer.Serialize(profiles, jsonoptions);
            File.WriteAllText(Misc.Jsons.profiles, jsonString);
            
            Close();
        }
    }
}