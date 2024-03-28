using System.Linq;
using SO.Data;
using SO.Data.Characters;
using UnityEngine;
using VillageGame.Data.Types;
using VillageGame.Logic.Snowdrifts;

namespace SO
{
    [CreateAssetMenu(fileName = "BuildingsConfig", menuName = "SO/BuildingsConfig")]
    public class BuildingsConfig : ScriptableObject
    {
        [Header("BUILDINGS")] 
        public BuildingData[] Houses;
        public BuildingData[] Decorations;

        [Space, Header("CONSTRUCTION SITE")] 
        public ConstructionSite ConstructionSitePrefab;
        public Sprite[] SnowdriftSprites;
        public Sprite ConstructionSiteSprite;
        public Sprite ConstructionSiteHighlighted;
        public int NearbySnowdriftPrice = 1;
        public int DistantSnowdriftPrice = 5;


        public Sprite GetSnowdriftSprite(out int spriteIndex)
        {
            spriteIndex = Random.Range(0, SnowdriftSprites.Length);
            return SnowdriftSprites[spriteIndex];
        }

        public Sprite GetSnowdriftSprite(int spriteIndex)
        {
            return spriteIndex < SnowdriftSprites.Length ? SnowdriftSprites[spriteIndex] : SnowdriftSprites[0];
        }

        public BuildingData GetData(BuildingType buildingType, int id)
        {
            if (buildingType == BuildingType.House)
                return Houses.FirstOrDefault(d => d.ID == id);
            if (buildingType == BuildingType.Decoration)
                return Decorations.FirstOrDefault(d => d.ID == id);

            return null;
        }

        public BuildingData GetData(CharacterType characterType)
        {
            return Houses.FirstOrDefault(d => d.Characters.Contains(characterType));
        }

        public void OnValidate()
        {
            for (int i = 0; i < Houses.Length; i++)
            {
                if (Houses[i] != null)
                {
                    Houses[i].ID = i;
                }
            }

            for (int i = 0; i < Decorations.Length; i++)
            {
                if (Decorations[i] != null)
                {
                    Decorations[i].ID = i;
                }
            }
        }
    }
}