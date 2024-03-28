using System;
using System.Collections.Generic;
using SO.Data.Characters;
using UnityEngine;
using VillageGame.Data;
using VillageGame.Data.PresentationModels;
using VillageGame.Data.Types;
using VillageGame.Logic.Buildings;

namespace SO.Data
{
    [CreateAssetMenu(fileName = "BuildingData", menuName = "SO/Data/BuildingData")]
    public class BuildingData: ScriptableObject, IEquatable<BuildingData>
    {
        public BuildingType Type;
        public int ID = -1;
        public Building Prefab;
        public BuildingPresentationModel PresentationModel;
        
        [Header("Данные ниже необходимы только для зданий." +
                "\nДля декораций заполнять не нужно.")]
        public MainerData MainerData;
        public List<CharacterType> Characters;


        public bool Equals(BuildingData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Type == other.Type && ID == other.ID;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BuildingData)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), (int)Type, ID);
        }
    }
}