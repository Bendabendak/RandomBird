using SadJam;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "Map Config", menuName = "Game/MapShop/Config")]
    public class Map_Config : GameConfig, IGameConfig_Shop_WithBuyables, IGameConfig_Shop_WithChoosables
    {
        [Serializable]
        public class Bought
        {
            public string Id = "";
            public bool Chosen = false;
        }

        [BlendableField("Items"), SerializeField]
        private List<Map> _items;
        [BlendableProperty("Items")]
        [BlendablePropertyCorrespondingInterface(nameof(IGameConfig_Shop_WithBuyables), nameof(IGameConfig_Shop_WithBuyables.Items))]
        [BlendablePropertyCorrespondingInterface(nameof(IGameConfig_Shop_WithChoosables), nameof(IGameConfig_Shop_WithChoosables.Items))]
        [BlendablePropertyCorrespondingInterface(nameof(IGameConfig_Shop), nameof(IGameConfig_Shop.Items))]
        public List<Map> Items { get; set; }
        IEnumerable<IGameConfig_Shop_Buyable> IGameConfig_Shop_WithBuyables.Items => Items;
        IEnumerable<IGameConfig_Shop_Choosable> IGameConfig_Shop_WithChoosables.Items => Items;
        IEnumerable<IGameConfig> IGameConfig_Shop.Items => Items;

        [BlendableField("DefaultMap"), SerializeField]
        private Map _defaultMap;
        [BlendableProperty("DefaultMap")]
        public Map DefaultMap { get; set; }

        [BlendableField("SaveKey"), Space, SerializeField]
        private Statistics_Key _saveKey;
        [BlendableProperty("SaveKey")]
        public Statistics_Key SaveKey { get; set; }

        [BlendableField("BalanceKey"), SerializeField]
        private Statistics_Key _balanceKey;
        [BlendableProperty("BalanceKey")]
        public Statistics_Key BalanceKey { get; set; }

        public bool Buy(Statistics.Owner owner, IGameConfig_Shop_Buyable item)
        {
            if (item is not Map map || !Items.Contains(map) || map == DefaultMap) return false;

            if (!Statistics.LoadStatus(owner, SaveKey, out List<Bought> bought, out Statistics.ErrorCodes loadMapsError))
            {
                switch (loadMapsError)
                {
                    case Statistics.ErrorCodes.StatusNotFound:
                        bought = new();
                        break;
                    case Statistics.ErrorCodes.StatusFoundWithDifferentType:
                        Debug.LogError("Unable to buy map, because " + SaveKey.Id + " status is not type of List<Bought>! Statistics owner: " + owner.Id, this);
                        return false;
                    default:
                        Debug.LogError("Unable to buy map, because " + SaveKey.Id + " status cannot be loaded! Error: " + loadMapsError + " Statistics owner: " + owner.Id, this);
                        return false;
                }
            }

            if (bought.Exists(b => b.Id == map.Id)) return false;

            if (map.Price > 0)
            {
                if (!Statistics.LoadNumericalStatus(owner, BalanceKey, out double balance, out Statistics.ErrorCodes loadBalanceError))
                {
                    switch (loadBalanceError)
                    {
                        case Statistics.ErrorCodes.StatusNotFound:
                            balance = 0;
                            break;
                        case Statistics.ErrorCodes.StatusFoundWithDifferentType:
                            Debug.LogError("Unable to buy map, because " + BalanceKey.Id + " status is not type of double! Statistics owner: " + owner.Id, this);
                            return false;
                        default:
                            Debug.LogError("Unable to buy map, because " + BalanceKey.Id + " status cannot be loaded! Error: " + loadBalanceError + " Statistics owner: " + owner.Id, this);
                            return false;
                    }
                }

                if (balance < map.Price) return false;

                balance -= map.Price;

                bought.Add(new() { Id = map.Id });

                Statistics.UpdateAndSaveStatistics(owner, out Statistics.ErrorCodes error, new Statistics.DataEntry(BalanceKey, balance), new Statistics.DataEntry(SaveKey, bought));

                if (error != Statistics.ErrorCodes.None)
                {
                    return false;
                }

                return true;
            }
            else
            {
                bought.Add(new() { Id = map.Id });

                Statistics.UpdateAndSaveStatistics(owner, out Statistics.ErrorCodes error, new Statistics.DataEntry(SaveKey, bought));

                if (error != Statistics.ErrorCodes.None)
                {
                    return false;
                }

                return true;
            }
        }

        public IEnumerable<IGameConfig_Shop_Buyable> GetBought(Statistics.Owner owner) => GetBoughtMaps(owner);
        public IEnumerable<Map> GetBoughtMaps(Statistics.Owner owner)
        {
            yield return DefaultMap;

            if (!Statistics.LoadStatus(owner, SaveKey, out List<Bought> bought, out Statistics.ErrorCodes loadBirdsError))
            {
                switch (loadBirdsError)
                {
                    case Statistics.ErrorCodes.StatusFoundWithDifferentType:
                        Debug.LogError("Unable to get bought maps, because " + SaveKey.Id + " status is not type of List<Bought>! Statistics owner: " + owner.Id, this);
                        break;
                }

                yield break;
            }

            foreach (Bought b in bought)
            {
                foreach (Map map in Items)
                {
                    if (map.Id == b.Id)
                    {
                        yield return map;
                    }
                }
            }
        }

        public bool Choose(Statistics.Owner owner, IGameConfig_Shop_Choosable item)
        {
            if (item is not Map map || !Items.Contains(map)) return false;

            if (!Statistics.LoadStatus(owner, SaveKey, out List<Bought> bought, out Statistics.ErrorCodes loadMapsError))
            {
                switch (loadMapsError)
                {
                    case Statistics.ErrorCodes.StatusFoundWithDifferentType:
                        Debug.LogError("Unable to choose map, because bought data are not type of List<Bought>! Statistics owner: " + owner.Id, this);
                        break;
                }

                if (map == DefaultMap) return true;

                return false;
            }

            if (map == DefaultMap)
            {
                foreach (Bought b in bought)
                {
                    b.Chosen = false;
                }
            }
            else
            {
                Bought target;

                target = bought.Find(b => b.Id == map.Id);

                if (target == null) return false;
                if (bought.Exists(b => b.Id == map.Id && b.Chosen)) return false;

                foreach (Bought b in bought)
                {
                    b.Chosen = false;
                }

                target.Chosen = true;
            }

            Statistics.UpdateAndSaveStatistics(owner, out Statistics.ErrorCodes error, new Statistics.DataEntry(SaveKey, bought));

            if (error != Statistics.ErrorCodes.None)
            {
                return false;
            }

            return true;
        }

        public IEnumerable<IGameConfig_Shop_Choosable> GetChosen(Statistics.Owner owner) => GetChosenMaps(owner);
        public IEnumerable<Map> GetChosenMaps(Statistics.Owner owner)
        {
            if (!Statistics.LoadStatus(owner, SaveKey, out List<Bought> bought, out Statistics.ErrorCodes loadBirdsError))
            {
                switch (loadBirdsError)
                {
                    case Statistics.ErrorCodes.StatusFoundWithDifferentType:
                        Debug.LogError("Unable to get chosen map, because " + SaveKey.Id + " status is not type of List<Bought>! Statistics owner: " + owner.Id, this);
                        break;
                }

                yield return DefaultMap;
                yield break;
            }

            int count = 0;
            foreach (Bought b in bought)
            {
                if (!b.Chosen) continue;

                foreach (Map map in Items)
                {
                    if (map.Id == b.Id)
                    {
                        count++;
                        yield return map;
                    }
                }
            }

            if (count <= 0)
            {
                yield return DefaultMap;
            }
        }
    }
}
