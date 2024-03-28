using UnityEngine;

namespace SO.Data.Presents
{
    [CreateAssetMenu(fileName = "PresentValue_Currency_", menuName = "SO/Data/Presenets/Value/PresentValue_Currency")]
    public class CurrencyPresentValueData : PresentValueData
    {
        public Type ValueType;
        public int Count;
        
        public enum Type
        {
            None,
            ActionPoint,
            Rating,
            Shuffle,
            Hint
        }

        public override bool Equivalent(PresentValueData other)
        {
            if (other != null && other is CurrencyPresentValueData currencyPresentValueData)
            {
                return ValueType == currencyPresentValueData.ValueType && Count == currencyPresentValueData.Count;
            }

            return false;
        }
    }
  
}