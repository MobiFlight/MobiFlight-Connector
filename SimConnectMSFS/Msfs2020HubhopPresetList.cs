using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.SimConnectMSFS
{
    public class Msfs2020HubhopPreset
    {
        public String path;
        public String vendor;
        public String aircraft;
        public String system;
        public String code { get; set; }
        public String label { get; set; }
        public String presetType;
        public int version;
        public String status;
        public String description;
        public String createdDate;
        public String author;
        public String updatedBy;
        public int reported;
        public int score;
        public String id { get; set; }
    }
    public class Msfs2020HubhopPresetList
    {
        public List<Msfs2020HubhopPreset> Items = new List<Msfs2020HubhopPreset>();

        public void Load(String Msfs2020HubhopPreset)
        {
            Items.Clear();
            try
            {
                var presets = JsonConvert.DeserializeObject<List<Msfs2020HubhopPreset>>
                                (File.ReadAllText(Msfs2020HubhopPreset), new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });  ;
                Items.AddRange(presets);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Unable to load {Msfs2020HubhopPreset}: {ex.Message}", LogSeverity.Error);
            }
        }

        public List<String> AllVendors(String presetType)
        {
                return Items
                    .FindAll(x => x.presetType == presetType)
                    .GroupBy(x => x.vendor)
                    .Select(g => g.FirstOrDefault().vendor).ToList();
        }

        public List<String> AllAircraft(String presetType)
        {
            return Items
                .FindAll(x => x.presetType == presetType)
                .GroupBy(x => x.aircraft)
                .Select(g => g.FirstOrDefault().aircraft).ToList();
        }

        public List<String> AllSystems(String presetType)
        {
            return Items
                .FindAll(x => x.presetType == presetType)
                .GroupBy(x => x.system)
                .Select(g => g.FirstOrDefault().system).ToList();
        }

        public List<String> FilteredVendors(String Aircraft, String System)
        {
            List<String> result = new List<string>();

            if (Aircraft != null)
            {
                if (System != null)
                    result = Items.FindAll(x => x.aircraft == Aircraft && x.system == System)
                        .GroupBy(x => x.vendor)
                        .Select(g => g.FirstOrDefault().vendor).ToList();
                else
                    result = Items.FindAll(x => x.aircraft == Aircraft)
                        .GroupBy(x => x.vendor)
                        .Select(g => g.FirstOrDefault().vendor).ToList();
            }
            else if (System != null)
            {
                result = Items.FindAll(x => x.system == System)
                        .GroupBy(x => x.vendor)
                        .Select(g => g.FirstOrDefault().vendor).ToList();
            }

            return result;
        }

        public List<String> FilteredAircraft(String Vendor, String System)
        {
            List<String> result = new List<string>();

            if (Vendor != null)
            {
                if (System != null)
                    result = Items.FindAll(x => x.vendor == Vendor && x.system == System)
                        .GroupBy(x => x.aircraft)
                        .Select(g => g.FirstOrDefault().aircraft).ToList();
                else
                    result = Items.FindAll(x => x.vendor == Vendor)
                        .GroupBy(x => x.aircraft)
                        .Select(g => g.FirstOrDefault().aircraft).ToList();
            }
            else if (System != null)
            {
                result = Items.FindAll(x => x.system == System)
                        .GroupBy(x => x.aircraft)
                        .Select(g => g.FirstOrDefault().aircraft).ToList();
            }

            return result;
        }

        public List<String> FilteredSystems(String Vendor, String Aircraft)
        {
            List<String> result = new List<string>();
            
            if (Vendor != null)
            {
                if (Aircraft != null) 
                    result = Items.FindAll(x => x.vendor==Vendor && x.aircraft==Aircraft)
                        .GroupBy(x => x.system)
                        .Select(g => g.FirstOrDefault().system).ToList();
                else
                    result = Items.FindAll(x => x.vendor == Vendor)
                        .GroupBy(x => x.system)
                        .Select(g => g.FirstOrDefault().system).ToList();
            } else if (Aircraft != null)
            {
                result = Items.FindAll(x => x.aircraft == Aircraft)
                        .GroupBy(x => x.system)
                        .Select(g => g.FirstOrDefault().system).ToList();
            }
            
            return result;
        }

        public List<Msfs2020HubhopPreset> FilteredByText(String type, String filterText)
        {
            if (filterText == null || filterText == "") return Items;

            return new List<Msfs2020HubhopPreset>(
                Items.FindAll(x => x.presetType == type)
                     .FindAll(x => x.vendor.IndexOf(filterText) >= 0 ||
                                   x.aircraft.IndexOf(filterText) >= 0 ||
                                   x.system.IndexOf(filterText) >= 0 ||
                                   x.description?.IndexOf(filterText) >= 0)
                     .ToArray()
            );
        }

        internal List<Msfs2020HubhopPreset> Filtered(string type, string selectedVendor, string selectedAircraft, string selectedSystem, string filterText)
        {
            List<Msfs2020HubhopPreset> temp;
            temp = Items.FindAll(x => x.presetType == type);
            if (selectedVendor != null)
                temp = temp.FindAll(x => x.vendor == selectedVendor);

            if (selectedAircraft != null)
                temp = temp.FindAll(x => x.aircraft == selectedAircraft);

            if (selectedSystem != null)
                temp = temp.FindAll(x => x.system == selectedSystem);

            if (filterText != null && filterText.Length > 2)
                temp = temp.FindAll(x => x.vendor.IndexOf(filterText) >= 0 ||
                                   x.aircraft.IndexOf(filterText) >= 0 ||
                                   x.system.IndexOf(filterText) >= 0 ||
                                   x.description?.IndexOf(filterText) >= 0);

            return new List<Msfs2020HubhopPreset>(
                temp.ToArray()
            );
        }
    }

   
}
