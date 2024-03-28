using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SO
{
    [CreateAssetMenu(fileName = "DodoBirdsGiftsConfig", menuName = "SO/DodoBirdsGiftsConfig", order = 0)]
    public class DodoBirdsGiftsConfig : ScriptableObject
    {
        public List<DodoBirdGift> _gifts;

        public DodoBirdGift GetGiftByID(int id)
        {
            return _gifts.FirstOrDefault(g => g.Id == id);
        }
    }

    [Serializable]
    public class DodoBirdGift
    {
        public int Id;
        public Sprite Opened;
        public Sprite Closed;
        public Sprite PartnerLogo;
        [TextArea(2,5)]public string Title;
        [TextArea(2,5)]public string Description;
        public int Price;
    }
}