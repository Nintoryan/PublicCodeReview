using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace VillageGame.Data.Types
{
    [CreateAssetMenu(fileName = "TilesConfig", menuName = "SO/TilesConfig")]
    public class TilesConfig : ScriptableObject
    {
        public TileBaseData[] Tiles;

        public TileBase GetTile(TileType type)
        {
            return Tiles.FirstOrDefault(t => t.Type == type)?.Base;
        }

        public TileType GetType(TileBase tileBase)
        {
            var data = Tiles.FirstOrDefault(t => t.Base == tileBase);
            return data?.Type ?? TileType.None;
        }
        
        public bool Contains(TileBase tileBase)
        {
            var data = Tiles.FirstOrDefault(t => t.Base == tileBase);
            return data != null;
        }
    }


    [Serializable]
    public class TileBaseData
    {
        public TileType Type;
        public TileBase Base;
    }
}