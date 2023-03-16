using Common;
using Common.Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace GameServer.Managers
{
    public class DataManager : Singleton<DataManager>
    {
        internal string DataPath;
        internal Dictionary<int, MapDefine> Maps = null;
        internal Dictionary<int, CharacterDefine> Characters = null;
        internal Dictionary<int, TeleporterDefine> Teleporters = null;
        internal Dictionary<int, Dictionary<int, SpawnPointDefine>> SpawnPoints = null;
        internal Dictionary<int, Dictionary<int, SpawnRuleDefine>> SpawnRules = null;
        internal Dictionary<int, NpcDefine> Npcs = null;
        internal Dictionary<int, ItemDefine> Items = null;
        internal Dictionary<int, EquipDefine> Equips = null;
        internal Dictionary<int, ShopDefine> Shops = null;
        internal Dictionary<int, QuestDefine> Quests = null;
        internal Dictionary<int, Dictionary<int, ShopItemDefine>> ShopItems = null;

        public DataManager()
        {
            DataPath = "Data/"; // Debug目录
            Log.Info("DataManager > DataManager()");
        }

        internal void Load()
        {
            string json = File.ReadAllText(DataPath + "MapDefine.txt");
            Maps = JsonConvert.DeserializeObject<Dictionary<int, MapDefine>>(json);

            json = File.ReadAllText(DataPath + "CharacterDefine.txt");
            Characters = JsonConvert.DeserializeObject<Dictionary<int, CharacterDefine>>(json);

            json = File.ReadAllText(DataPath + "TeleporterDefine.txt");
            Teleporters = JsonConvert.DeserializeObject<Dictionary<int, TeleporterDefine>>(json);

            json = File.ReadAllText(DataPath + "SpawnPointDefine.txt");
            SpawnPoints = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SpawnPointDefine>>>(json);

            json = File.ReadAllText(DataPath + "SpawnRuleDefine.txt");
            SpawnRules = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SpawnRuleDefine>>>(json);

            json = File.ReadAllText(DataPath + "NpcDefine.txt");
            Npcs = JsonConvert.DeserializeObject<Dictionary<int, NpcDefine>>(json);

            json = File.ReadAllText(DataPath + "ItemDefine.txt");
            Items = JsonConvert.DeserializeObject<Dictionary<int, ItemDefine>>(json);

            json = File.ReadAllText(DataPath + "EquipDefine.txt");
            Equips = JsonConvert.DeserializeObject<Dictionary<int, EquipDefine>>(json);

            json = File.ReadAllText(DataPath + "ShopDefine.txt");
            Shops = JsonConvert.DeserializeObject<Dictionary<int, ShopDefine>>(json);

            json = File.ReadAllText(DataPath + "QuestDefine.txt");
            Quests = JsonConvert.DeserializeObject<Dictionary<int, QuestDefine>>(json);

            json = File.ReadAllText(DataPath + "ShopItemDefine.txt");
            ShopItems = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, ShopItemDefine>>>(json);
        }
    }
}