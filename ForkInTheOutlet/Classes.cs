using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ModManagerBase
{
    public class Meta
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Authors { get; set; }
        public string Link { get; set; }
        public string ID { get; set; }
        [JsonIgnore]
        public bool IsChecked { get; set; }
        [JsonIgnore]
        public string LinkImage { get; set; }
        [JsonIgnore]
        public bool ArchiveImage { get; set; }
        public string Profile {get; set;}
    }
    public class Settings
    {
        public int DefaultImage { get; set; }
        public int CurrentProfile { get; set; }
    }

    public class Profile
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public string DeployPath { get; set; }
        public string BGColor { get; set; }
        public string SettingsColor { get; set; }
        public string GridColor { get; set; }
        public string DescColor { get; set; }
        public string ModsImage { get; set; }
        public string UnModsImage { get; set; }
        public string SettingsImage { get; set; }
        public string UnSettingsImage { get; set; }
        public string SwitchImage { get; set; }
        public string DownloadImage { get; set; }
        public string DeployImage { get; set; }
        public string NewImage { get; set; }
        public string RefreshImage { get; set; }
        public string OpenImage { get; set; }
    }
}
