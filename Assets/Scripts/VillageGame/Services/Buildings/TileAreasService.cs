using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using VillageGame.Data.Types;

namespace VillageGame.Services.Buildings
{
    public class TileAreasService : MonoBehaviour
    {
        public Tilemap Tilemap_main;
        public Tilemap Tilemap_characterSpawn;

        [SerializeField] private TilesConfig _tilesConfig;
        [SerializeField] private GridLayout _gridLayout;
        [SerializeField] private List<BoundsInt> _buildingTileAreas;
        [SerializeField] private List<BoundsInt> _decorationTileAreas;
        [SerializeField] private List<BoundsInt> _characterSpawnAreasTileAreas;
        public List<BoundsInt> BuildingTileAreas => _buildingTileAreas;
        public List<BoundsInt> DecorationTileAreas => _decorationTileAreas;
        public List<BoundsInt> CharacterSpawnAreas => _characterSpawnAreasTileAreas;


        public Vector3 GetRandomCharacterSpawnCellPosition()
        {
            if (CharacterSpawnAreas == null || CharacterSpawnAreas.Count == 0)
            {
                return Vector3.zero;
            }

            var cell = CharacterSpawnAreas[Random.Range(0, CharacterSpawnAreas.Count)];
       
            return      NavMesh.SamplePosition(_gridLayout.CellToWorld(cell.position), out var hit, 0.1f, NavMesh.AllAreas) ? hit.position :_gridLayout.CellToWorld(cell.position) ;
        }

        public Vector3 CellToLocalPosition(Vector3Int position)
        {
            return _gridLayout.CellToLocalInterpolated(position + new Vector3(0.5f, 0.5f, 0));
        }

        public Vector3Int LocalToCellPosition(Vector3 position)
        {
            return _gridLayout.LocalToCell(position);
        }

        public void ClearBuildsAreas()
        {
            _buildingTileAreas.Clear();
        }

        public void ClearDecorationAreas()
        {
            _decorationTileAreas.Clear();
        }

        public TileBase GetTile(TileType type)
        {
            return _tilesConfig.GetTile(type);
        }

        #region Editor
        public void AnalyzeSquredTileAreas(Tilemap tilemap, TileBase tileBase, List<BoundsInt> areasList, Vector2Int minSize)
        {
            BoundsInt bounds = tilemap.cellBounds;

            for (int x = bounds.x; x < bounds.x + bounds.size.x; x++)
            {
                for (int y = bounds.y; y < bounds.y + bounds.size.y; y++)
                {
                    TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));
                    if (tile == tileBase)
                    {
                        Vector3Int cellPosition = new Vector3Int(x, y, 0);

                        bool isPartOfExistingArea = false;
                        foreach (BoundsInt existingArea in areasList)
                        {
                            if (existingArea.Contains(cellPosition))
                            {
                                isPartOfExistingArea = true;
                                break;
                            }
                        }

                        if (!isPartOfExistingArea)
                        {
                            BoundsInt buildingBounds = FloodFill(x, y, tileBase, minSize);

                            if (buildingBounds.size.x >= minSize.x && buildingBounds.size.y >= minSize.y)
                            {
                                areasList.Add(buildingBounds);
                            }
                        }
                    }
                }
            }
        }

        public void AnalyzeTilemap(Tilemap tilemap, TileBase tileBase, List<BoundsInt> areasList, Vector2Int size)
        {
            BoundsInt bounds = tilemap.cellBounds;

            for (int x = bounds.x; x < bounds.x + bounds.size.x - size.x + 1; x++)
            {
                for (int y = bounds.y; y < bounds.y + bounds.size.y - size.y + 1; y++)
                {
                    BoundsInt buildingBounds = new BoundsInt(x, y, 0, size.x, size.y, 1);
                    bool isAreaValid = true;

                    foreach (BoundsInt existingArea in areasList)
                    {
                        if (IsBoundsOverlap(buildingBounds, existingArea))
                        {
                            isAreaValid = false;
                            break;
                        }
                    }

                    if (isAreaValid)
                    {
                        for (int i = 0; i < size.x; i++)
                        {
                            for (int j = 0; j < size.y; j++)
                            {
                                TileBase tile = tilemap.GetTile(new Vector3Int(x + i, y + j, 0));

                                if (tile != tileBase)
                                {
                                    isAreaValid = false;
                                    break;
                                }
                            }

                            if (!isAreaValid)
                            {
                                break;
                            }
                        }

                        if (isAreaValid)
                        {
                            areasList.Add(buildingBounds);
                        }
                    }
                }
            }
        }

        private bool IsBoundsOverlap(BoundsInt bounds1, BoundsInt bounds2)
        {
            return bounds1.x < bounds2.x + bounds2.size.x &&
                   bounds1.x + bounds1.size.x > bounds2.x &&
                   bounds1.y < bounds2.y + bounds2.size.y &&
                   bounds1.y + bounds1.size.y > bounds2.y;
        }

        private BoundsInt FloodFill(int startX, int startY, TileBase tileBase, Vector2Int minSize)
        {
            BoundsInt buildingBounds = new BoundsInt(startX, startY, 0, 1, 1, 1);
            Queue<Vector3Int> queue = new Queue<Vector3Int>();
            queue.Enqueue(new Vector3Int(startX, startY, 0));
            HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

            while (queue.Count > 0)
            {
                Vector3Int cell = queue.Dequeue();
                visited.Add(cell);

                if (Tilemap_main.GetTile(cell) == tileBase)
                {
                    buildingBounds.x = Mathf.Min(buildingBounds.x, cell.x);
                    buildingBounds.y = Mathf.Min(buildingBounds.y, cell.y);
                    buildingBounds.size = new Vector3Int(
                        Mathf.Max(cell.x - buildingBounds.x + 1, minSize.x),
                        Mathf.Max(cell.y - buildingBounds.y + 1, minSize.y),
                        1
                    );

                    Vector3Int[] neighbors = new Vector3Int[]
                    {
                        cell + Vector3Int.up,
                        cell + Vector3Int.down,
                        cell + Vector3Int.left,
                        cell + Vector3Int.right
                    };

                    foreach (Vector3Int neighbor in neighbors)
                    {
                        if (!Tilemap_main.HasTile(neighbor) || visited.Contains(neighbor))
                        {
                            continue;
                        }

                        queue.Enqueue(neighbor);
                    }
                }
            }

            return buildingBounds;
        }

        #endregion

        public void ClearCharacterSpawnAreas()
        {
            _characterSpawnAreasTileAreas.Clear();
        }
    }
}