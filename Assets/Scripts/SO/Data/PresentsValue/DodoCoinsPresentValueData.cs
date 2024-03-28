using UnityEngine;
using Web.ResponseStructs;

namespace SO.Data.Presents
{
    [CreateAssetMenu(fileName = "PresentValue_DodoCoins_", 
        menuName = "SO/Data/Presenets/Value/PresentValue_DodoCoins")]
    public class DodoCoinsPresentValueData : PresentValueData
    {
        public AddCoinsType DodoCoinsValue;
        public override bool Equivalent(PresentValueData other)
        {
            if (other != null && other is DodoCoinsPresentValueData valueData)
            {
                return DodoCoinsValue == valueData.DodoCoinsValue;
            }

            return false;
        }
    }
}