using System;
using System.Linq;
using UnityEngine;

namespace SO.Data.Characters
{
    [Serializable]
    public class CharacterPresentationModel
    {
        public string Name;
        public Color32 Color = new Color32(1, 1, 1, 255);
        public Sprite PortraitBody;
        public PortraitData[] PortraitEmotions;

        public Sprite GetPortraitEmotion(PortraitReaction portraitReaction)
        {
            if (PortraitEmotions == null)
            {
                return null;
            }
            var data = PortraitEmotions.FirstOrDefault(p => p.Reaction == portraitReaction);
            return data != null ? data?.Sprite : PortraitEmotions[0]?.Sprite;
        }

        [Serializable]
        public class PortraitData
        {
            public PortraitReaction Reaction;
            public Sprite Sprite;
        }
    }
}