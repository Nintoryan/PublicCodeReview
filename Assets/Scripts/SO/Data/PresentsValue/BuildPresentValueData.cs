using System;
using UnityEngine;
using VillageGame.Data.Types;

namespace SO.Data.Presents
{
    [CreateAssetMenu(fileName = "PresentValue_Build_", menuName = "SO/Data/Presenets/Value/PresentValue_Build")]
    public class BuildPresentValueData : PresentValueData 
    {
        public BuildingType Type;
        public int BuildingID;


        public override bool Equivalent(PresentValueData other)
        {
            if (other != null && other is BuildPresentValueData buildPresentValueData)
            {
                return Type == buildPresentValueData.Type && BuildingID == buildPresentValueData.BuildingID;
            }

            return false;
        }
    }
}