using System;
using Spine.Unity;
using UnityEngine;

namespace VillageGame.Data.PresentationModels
{
    [Serializable]
    public class BuildingPresentationModel
    {
        public string Name;
        [TextArea]public string Description;
        public int Price;
        public int RatingAfterConstruction = 10;
        public int RatingPerHour = 1;
        public Sprite Icon;
        public SkeletonDataAsset BigIcon;
    }
}