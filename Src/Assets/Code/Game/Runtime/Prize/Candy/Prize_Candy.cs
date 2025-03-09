using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "Prize", menuName = "Game/Prize/Candy/Create")]
    public class Prize_Candy : Prize
    {
        [BlendableField("BalanceToDeal"), SerializeField]
        private int _balanceToDeal;
        [BlendableProperty("BalanceToDeal")]
        public int BalanceToDeal { get; set; }

        public override void CollectPrize(Statistics.Owner owner, Statistics_Key balanceKey) => CollectPrize(owner, balanceKey.Id, balanceKey.SaveOnDevice);
        public override void CollectPrize(Statistics.Owner owner, string balanceId, bool saveBalanceOnDevice)
        {
            if (!Statistics.LoadNumericalStatus(owner, balanceId, out double balance, out Statistics.ErrorCodes loadBalanceError))
            {
                switch (loadBalanceError)
                {
                    case Statistics.ErrorCodes.StatusNotFound:
                        balance = 0;
                        break;
                    case Statistics.ErrorCodes.StatusFoundWithDifferentType:
                        Debug.LogError("Unable to collect prize, because " + balanceId + " status is not type of double! Statistics owner: " + owner.Id, this);
                        return;
                    default:
                        Debug.LogError("Unable to collect prize, because " + balanceId + " status cannot be loaded! Error: " + loadBalanceError + " Statistics owner: " + owner.Id, this);
                        return;
                }
            }

            balance += BalanceToDeal;

            Statistics.UpdateAndSaveStatistics(owner, new Statistics.DataEntry(balanceId, balance, saveBalanceOnDevice));
        }
    }
}