using SadJam;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "BirdShop Config", menuName = "Game/BirdShop/Config")]
    public class BirdShop_Config : GameConfig, IGameConfig_Shop_WithBuyables, IGameConfig_Shop_WithChoosables, IGameConfig_Shop_WithShowables
    {
        [Serializable]
        public class Bought
        {
            public string Id = "";
            public bool Chosen = false;
        }

        [BlendableField("Items"), SerializeField]
        private List<Bird> _items;
        [BlendableProperty("Items")]
        [BlendablePropertyCorrespondingInterface(nameof(IGameConfig_Shop_WithBuyables), nameof(IGameConfig_Shop_WithBuyables.Items))]
        [BlendablePropertyCorrespondingInterface(nameof(IGameConfig_Shop_WithChoosables), nameof(IGameConfig_Shop_WithChoosables.Items))]
        [BlendablePropertyCorrespondingInterface(nameof(IGameConfig_Shop), nameof(IGameConfig_Shop.Items))]
        [BlendablePropertyCorrespondingInterface(nameof(IGameConfig_Shop_WithShowables), nameof(IGameConfig_Shop_WithShowables.Items))]
        public List<Bird> Items { get; set; }
        IEnumerable<IGameConfig_Shop_Buyable> IGameConfig_Shop_WithBuyables.Items => Items;
        IEnumerable<IGameConfig_Shop_Choosable> IGameConfig_Shop_WithChoosables.Items => Items;
        IEnumerable<IGameConfig> IGameConfig_Shop.Items => Items;
        IEnumerable<IGameConfig_Shop_Showable> IGameConfig_Shop_WithShowables.Items => Items;

        [BlendableField("DefaultBird"), SerializeField]
        private Bird _defaultBird;
        [BlendableProperty("DefaultBird")]
        public Bird DefaultBird { get; set; }

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
            if (item is not Bird bird || !Items.Contains(bird) || bird == DefaultBird) return false;

            if (!Statistics.LoadStatus(owner, SaveKey, out List<Bought> bought, out Statistics.ErrorCodes loadBirdsError))
            {
                switch (loadBirdsError)
                {
                    case Statistics.ErrorCodes.StatusNotFound:
                        bought = new();
                        break;
                    case Statistics.ErrorCodes.StatusFoundWithDifferentType:
                        Debug.LogError("Unable to buy bird, because " + SaveKey.Id + " status is not type of List<Bought>! Statistics owner: " + owner.Id, this);
                        return false;
                    default:
                        Debug.LogError("Unable to buy bird, because " + SaveKey.Id + " status cannot be loaded! Error: " + loadBirdsError + " Statistics owner: " + owner.Id, this);
                        return false;
                }
            }

            if (bought.Exists(b => b.Id == bird.Id)) return false;

            if (bird.Price > 0)
            {
                if (!Statistics.LoadNumericalStatus(owner, BalanceKey, out double balance, out Statistics.ErrorCodes loadBalanceError))
                {
                    switch (loadBalanceError)
                    {
                        case Statistics.ErrorCodes.StatusNotFound:
                            balance = 0;
                            break;
                        case Statistics.ErrorCodes.StatusFoundWithDifferentType:
                            Debug.LogError("Unable to buy bird, because " + BalanceKey.Id + " status is not type of double! Statistics owner: " + owner.Id, this);
                            return false;
                        default:
                            Debug.LogError("Unable to buy bird, because " + BalanceKey.Id + " status cannot be loaded! Error: " + loadBalanceError + " Statistics owner: " + owner.Id, this);
                            return false;
                    }
                }

                if (balance < bird.Price) return false;

                balance -= bird.Price;

                bought.Add(new() { Id = bird.Id });

                Statistics.UpdateAndSaveStatistics(owner, out Statistics.ErrorCodes error, new Statistics.DataEntry(BalanceKey, balance), new Statistics.DataEntry(SaveKey, bought));

                if (error != Statistics.ErrorCodes.None)
                {
                    return false;
                }

                return true;
            }
            else
            {
                bought.Add(new() { Id = bird.Id });

                Statistics.UpdateAndSaveStatistics(owner, out Statistics.ErrorCodes error, new Statistics.DataEntry(SaveKey, bought));

                if (error != Statistics.ErrorCodes.None)
                {
                    return false;
                }

                return true;
            }
        }

        public IEnumerable<IGameConfig_Shop_Buyable> GetBought(Statistics.Owner owner) => GetBoughtBirds(owner);
        public IEnumerable<Bird> GetBoughtBirds(Statistics.Owner owner)
        {
            yield return DefaultBird;

            if (!Statistics.LoadStatus(owner, SaveKey, out List<Bought> bought, out Statistics.ErrorCodes loadBirdsError))
            {
                switch (loadBirdsError)
                {
                    case Statistics.ErrorCodes.StatusFoundWithDifferentType:
                        Debug.LogError("Unable to get bought birds, because " + SaveKey.Id + " status is not type of List<Bought>! Statistics owner: " + owner.Id, this);
                        break;
                }

                yield break;
            }

            foreach (Bought b in bought)
            {
                foreach (Bird bird in Items)
                {
                    if (bird.Id == b.Id)
                    {
                        yield return bird;
                    }
                }
            }
        }

        public bool Choose(Statistics.Owner owner, IGameConfig_Shop_Choosable item)
        {
            if (item is not Bird bird || !Items.Contains(bird)) return false;

            if (!Statistics.LoadStatus(owner, SaveKey, out List<Bought> bought, out Statistics.ErrorCodes loadBirdsError))
            {
                switch (loadBirdsError)
                {
                    case Statistics.ErrorCodes.StatusFoundWithDifferentType:
                        Debug.LogError("Unable to choose bird, because " + SaveKey.Id + " status is not type of List<Bought>! Statistics owner: " + owner.Id, this);
                        break;
                }

                if (bird == DefaultBird) return true;

                return false;
            }

            if (bird == DefaultBird)
            {
                foreach (Bought b in bought)
                {
                    b.Chosen = false;
                }
            }
            else
            {
                Bought target;

                target = bought.Find(b => b.Id == bird.Id);

                if (target == null) return false;
                if (bought.Exists(b => b.Id == bird.Id && b.Chosen)) return false;

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

        public IEnumerable<IGameConfig_Shop_Choosable> GetChosen(Statistics.Owner owner) => GetChosenBirds(owner);
        public IEnumerable<Bird> GetChosenBirds(Statistics.Owner owner)
        {
            if (!Statistics.LoadStatus(owner, SaveKey, out List<Bought> bought, out Statistics.ErrorCodes loadBirdsError))
            {
                switch (loadBirdsError)
                {
                    case Statistics.ErrorCodes.StatusFoundWithDifferentType:
                        Debug.LogError("Unable to get chosen bird, because bought data are not type of List<Bought>! Statistics owner: " + owner.Id, this);
                        break;
                }

                yield return DefaultBird;
                yield break;
            }

            int count = 0;
            foreach (Bought b in bought)
            {
                if (!b.Chosen) continue;

                foreach (Bird bird in Items)
                {
                    if (bird.Id == b.Id)
                    {
                        count++;
                        yield return bird;
                    }
                }
            }

            if (count <= 0)
            {
                yield return DefaultBird;
            }
        }
    }
}
