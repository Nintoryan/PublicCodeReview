using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VillageGame.Data;
using VillageGame.Data.Types;
using VillageGame.Services.Snowdrifts;
using Random = UnityEngine.Random;

namespace VillageGame.Services.Buildings
{
    public class BuildingAreaService : MonoBehaviour
    {
        [SerializeField] private BigSnowdrifts _bigSnowdrifts;
        [SerializeField] private TileAreasService _tileAreasService;
        [SerializeField] private IceCubesService _iceCubesService;

        [SerializeField] private List<BuildingAreaData> _allAreas;
        [SerializeField] private ClueAreasCollection _clue;

        private readonly Vector3 _offsetClue2x2 = Vector3.up * 0.65f;
        private readonly Vector3 _offsetClue5x5 = Vector3.up * 1.65f;
        private Camera _camera;


        private void Start()
        {
            _camera = Camera.main;

            HideClueAreas();
        }

        public bool HasFreeDecorationArea()
        {
            return _allAreas.Any(a => a.BuildingId is -1 or -2 && a.Type == BuildingType.Decoration);
        }

        public bool TryGetBuildingAreaByCoordinate(int x, int y, out BuildingAreaData area)
        {
            area = null;
            if (_bigSnowdrifts.IsActive && (Mathf.Abs(x) > 11 || Mathf.Abs(y) > 11))
            {
                return false;
            }

            area = _allAreas.FirstOrDefault(a => a.Area.x == x && a.Area.y == y);
            return area != null;
        }


        public bool TryGetBuildingAreaAtPosition(Vector3 position, out BuildingAreaData area)
        {
            area = null;

            Vector3 pos = _camera.ScreenToWorldPoint(position);
            Vector3Int cellPos = _tileAreasService.LocalToCellPosition(pos);
            cellPos.z = 0;

            if(_iceCubesService.IsActive && _iceCubesService.CheckPositionForIceCube(cellPos))
            {
                BuildingAreaData buildingAreaData = new BuildingAreaData();
                buildingAreaData.Type = BuildingType.IceCube;
                area = buildingAreaData;
                return true;
            }

            if (_bigSnowdrifts.IsActive && (Mathf.Abs(cellPos.x) > 11 || Mathf.Abs(cellPos.y) > 11))
            {
                return false;
            }

            foreach (var buildingArea in _allAreas)
            {
                if (buildingArea.Area.Contains(cellPos))
                {
                    area = buildingArea;
                    break;
                }
            }

            return area != null;
        }

        public List<BuildingAreaData> GetArea5X5()
        {
            return _allAreas.Where(b => b.Type == BuildingType.House).ToList();
        }

        public void ShowClueAreas(BuildingType buildingType)
        {
            if (buildingType != BuildingType.Decoration)
            {
                return;
            }
            var areas = _allAreas.Where(a => a.Type == BuildingType.Decoration);
            foreach (var area in areas)
            {
                if (area.IsBuildUp || IsDecorationAreaUnderBigSnowDrift(area.Area.position))
                {
                    area.ClueArea.SetActive(false);
                }
                else
                {
                    area.ClueArea.SetActive(true);
                }
            }
        }

        public void HideClueAreas()
        {
            foreach (var area in _allAreas.Where(a => a.Type == BuildingType.Decoration))
            {
                area.ClueArea.SetActive(false);
            }
        }

        public Vector3 GetRandomAreaPosition()
        {
            var areas = GetArea5X5();
            var randomArea = areas[Random.Range(0, areas.Count)].Area;
            return _tileAreasService.LocalToCellPosition(randomArea.position);
        }

        #region Editor

        public void CreateAllAreas()
        {
            _allAreas.Clear();
            CreateAreas2x2();
            CreateAreas5x5();
        }

        private void CreateAreas5x5()
        {
            for (var index = 0; index < _tileAreasService.BuildingTileAreas.Count; index++)
            {

                var area = _tileAreasService.BuildingTileAreas[index];
                var pos = _tileAreasService.CellToLocalPosition(area.position);

                GameObject clue = null;

                if (index < _clue.AreaObjects_5x5.Count)
                {
                    clue = _clue.AreaObjects_5x5[index];
                    clue.transform.position = pos + _offsetClue5x5;
                }
           
                var areaData = new BuildingAreaData()
                {
                    Area = area,
                    Type = BuildingType.House,
                    ClueArea = clue,
                    BuildingId = -2,
                };

                _allAreas.Add(areaData);
            }
        }

        private void CreateAreas2x2()
        {
            for (var index = 0; index < _tileAreasService.DecorationTileAreas.Count; index++)
            {
                var area = _tileAreasService.DecorationTileAreas[index];
                var pos = _tileAreasService.CellToLocalPosition(area.position);
                GameObject clue = null;

                if (index < _clue.AreaObjects_2x2.Count)
                {
                    clue = _clue.AreaObjects_2x2[index];
                    clue.transform.position = pos + _offsetClue2x2;
                }

                var areaData = new BuildingAreaData
                {
                    Area = area,
                    Type = BuildingType.Decoration,
                    ClueArea = clue,
                    BuildingId = -2,
                };

                _allAreas.Add(areaData);
            }
        }


        private bool IsDecorationAreaUnderBigSnowDrift(Vector3Int position)
        {
            if (!_bigSnowdrifts.IsActive) return false;
            
            Vector3Int[] decorationPosition =
            {
                new(-5, 3, 0), //37
                new(-5, -5, 0), //34
                new(-5, -3, 0), //35
                new(-5, 1, 0), //36
                new(-3, -5, 0), //43
                new(-3, 3, 0), //44
                new(3, -3, 0),
                new(1,-5,0),
                new(3,-5,0),
                new(3,-3,0)
            };

            return !decorationPosition.Contains(position);
        }

        #endregion

        
    }
}