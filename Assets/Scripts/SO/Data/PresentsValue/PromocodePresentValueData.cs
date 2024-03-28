using UnityEngine;

namespace SO.Data.Presents
{
    [CreateAssetMenu(fileName = "PresentValue_Promocode_", menuName = "SO/Data/Presenets/Value/PresentValue_Promocode")]
    public class PromocodePresentValueData: PresentValueData
    { 
        public int PromocodeID;
        [TextArea(5,10)] public string Description;
        public override bool Equivalent(PresentValueData other)
        {
            if (other != null && other is PromocodePresentValueData promocodePresentValueData)
            {
                return PromocodeID == promocodePresentValueData.PromocodeID;
            }

            return false;
        }
    }
}