using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.HubHop
{
public class Msfs2020HubhopPreset
    {
        public String path;
        public String vendor;
        public String aircraft;
        public String system;
        public String code { get; set; }
        public String label { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public HubHopType presetType;
        [JsonConverter(typeof(StringEnumConverter))]
        public HubHopAction? codeType;
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

    public class Msfs2020HubhopPresetListSingleton
    {
        public static Msfs2020HubhopPresetList Instance { get; } = new Msfs2020HubhopPresetList();
    }

    public class XplaneHubhopPresetListSingleton
    {
        public static Msfs2020HubhopPresetList Instance { get; } = new Msfs2020HubhopPresetList();
    }

    public class Msfs2020HubhopPresetList
    {
        public List<Msfs2020HubhopPreset> Items = new List<Msfs2020HubhopPreset>();
        String LoadedFile = null;

        public void Clear()
        {
            if (Items != null)
            {
                for (int i = 0; i != Items.Count; i++)
                {
                    Items[i] = null;
                }
                Items = null;
            }
            LoadedFile = null;
        }

        public void Load(String Msfs2020HubhopPreset)
        {
            if (LoadedFile == Msfs2020HubhopPreset) return;

            Clear();
            try
            {
                Items = JsonConvert.DeserializeObject<List<Msfs2020HubhopPreset>>
                                (File.ReadAllText(Msfs2020HubhopPreset), new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                LoadedFile = Msfs2020HubhopPreset;
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Unable to load {Msfs2020HubhopPreset}: {ex.Message}", LogSeverity.Error);
            }
        }

        public List<String> AllVendors(HubHopType presetType)
        {

            return Items
                .FindAll(x => (x.presetType & presetType) > 0)
                .GroupBy(x => x.vendor)
                .Select(g => g.FirstOrDefault().vendor)
                .OrderBy(x => x)
                .ToList();
        }

        public List<String> AllAircraft(HubHopType presetType)
        {

            return Items
                .FindAll(x => (x.presetType & presetType) > 0)
                .GroupBy(x => x.aircraft)
                .Select(g => g.FirstOrDefault().aircraft)
                .OrderBy(x => x)
                .ToList();
        }

        public List<String> AllSystems(HubHopType presetType)
        {
            return Items
                .FindAll(x => (x.presetType & presetType) > 0)
                .GroupBy(x => x.system)
                .Select(g => g.FirstOrDefault().system)
                .OrderBy(x => x)
                .ToList();
        }

        public List<Msfs2020HubhopPreset> Filtered(HubHopType presetType, string selectedVendor, string selectedAircraft, string selectedSystem, string filterText)
        {
            List<Msfs2020HubhopPreset> temp;

                temp = Items.FindAll(x => (x.presetType & presetType) > 0);
            
            if (selectedVendor != null)
                temp = temp.FindAll(x => x.vendor == selectedVendor);

            if (selectedAircraft != null)
                temp = temp.FindAll(x => x.aircraft == selectedAircraft);

            if (selectedSystem != null)
                temp = temp.FindAll(x => x.system == selectedSystem);

            if (filterText != null)
                temp = temp.FindAll(x=>(x.vendor.ToLower().IndexOf(filterText.ToLower()) >= 0) ||
                                        x.aircraft.ToLower().IndexOf(filterText.ToLower()) >= 0 ||
                                        x.system.ToLower().IndexOf(filterText.ToLower()) >= 0 ||
                                        x.label.ToLower().IndexOf(filterText.ToLower()) >= 0 ||
                                        x.description?.ToLower().IndexOf(filterText.ToLower()) >= 0 ||
                                        x.code?.ToLower().IndexOf(filterText.ToLower()) >= 0);

            return new List<Msfs2020HubhopPreset>(
                temp.OrderBy(x => x.label)
                    .ToArray()
            );
        }

        public Msfs2020HubhopPreset FindByCode(HubHopType presetType, string code)
        {
            Msfs2020HubhopPreset result = null;
            String trimmedCode = code.Trim();
            
            result = Items.Find(x => (x.presetType & presetType) > 0 && x.code.Replace('\n', ' ').Replace("  ", " ").TrimEnd() == trimmedCode);

            return result;
        }

        public Msfs2020HubhopPreset FindByUUID(HubHopType presetType, string UUID)
        {
            Msfs2020HubhopPreset result = null;
            
            result = Items.Find(x => (x.presetType & presetType) > 0 && x.id == UUID);

            return result;
        }
    }
}
