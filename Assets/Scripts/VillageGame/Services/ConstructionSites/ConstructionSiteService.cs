using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Febucci.UI;
using SO;
using UnityEngine;
using Util;
using VContainer;
using VillageGame.Data;
using VillageGame.Infrastructure.Factories;
using VillageGame.Logic.Snowdrifts;
using VillageGame.Services.Buildings;
using VillageGame.Services.LoadingData;
using VillageGame.UI.Controllers;
using Web.RequestStructs;

namespace VillageGame.Services.Snowdrifts
{
    public class ConstructionSiteService : MonoBehaviour, ILoading
    {
        [SerializeField] private BuildingAreaService _buildingAreaService;
        [SerializeField] private TileAreasService _tileAreasService;
        [SerializeField] private BuildingsConfig _buildingsConfig;
        [SerializeField] private BigSnowdrifts _bigSnowdrifts;
        
        [SerializeField] private List<ConstructionSite> _constructionSites;
        public List<ConstructionSite> ConstructionSites => _constructionSites;
        
        private  BuildingFactory _buildingFactory;
        private  SnowdriftPanelController _snowdriftPanelController;
        
        public Action<ConstructionSiteData[]> RefreshConstructionSitesEvent;
        public Action<ConstructionSite> ClearSnowdriftEvent;
        private TextAnimatorPlayer _player;
        
        [Inject]
        public void Construct(IObjectResolver objectResolver)
        {
            _buildingFactory = objectResolver.Resolve<BuildingFactory>();
            _snowdriftPanelController = objectResolver.Resolve<SnowdriftPanelController>();

            SubscribeToEvents(true);
        }

        ~ConstructionSiteService()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _snowdriftPanelController.SnowdriftClearedEvent += OnSnowdriftCleared;
            }
            else
            {
                if(_snowdriftPanelController != null)
                    _snowdriftPanelController.SnowdriftClearedEvent -= OnSnowdriftCleared;
            }
        }

        public int GetSnowDriftSiteCount()
        {
            return ConstructionSites.Count(c => c.Data.State is ConstructionSite.StateType.Snowdrift);
        }

        public int GetClearConstructionSiteCount()
        {
            return ConstructionSites.Count(c => 
                c.Data.State is ConstructionSite.StateType.ConstructionSite or ConstructionSite.StateType.Highlighted);
        }

        public int GetSnowdriftCount()
        {
            return ConstructionSites.Count(c => c.Data.State is ConstructionSite.StateType.Snowdrift);
        }

        public int GetNearblySnowdriftSiteCount()
        {
            return ConstructionSites.Count(c => c.Data.PositionType == ConstructionSite.PositionType.Nearby 
                                                &&  c.Data.State == ConstructionSite.StateType.Snowdrift);      
        }
        
        private void OnSnowdriftCleared(ConstructionSite site)
        {
            site.SetCurrentState(ConstructionSite.StateType.ConstructionSite);
            site.SetSprite(_buildingsConfig.ConstructionSiteSprite);
            RefreshConstructionSitesEvent?.Invoke(GetAllSitesData());
            ClearSnowdriftEvent?.Invoke(site);
        }

        public void SetConstructionSiteState(Vector3Int buildingAreaPosition, ConstructionSite.StateType stateType)
        {
            if (TryGetConstructionSite((Vector2Int)buildingAreaPosition, out var site))
            {
                site.SetCurrentState(stateType);
                SetSiteSprite(stateType, site);
            }
        }

        public void SetHighlightConstructionSites(bool value)
        {
            foreach (var site in ConstructionSites)
            {
                if (value)
                {
                    if (site.Data.State == ConstructionSite.StateType.ConstructionSite)
                    {
                        site.SetCurrentState(ConstructionSite.StateType.Highlighted);
                        site.SetSprite(_buildingsConfig.ConstructionSiteHighlighted);
                    } 
                }
                else
                {
                    if (site.Data.State == ConstructionSite.StateType.Highlighted)
                    {
                        site.SetCurrentState(ConstructionSite.StateType.ConstructionSite);
                        site.SetSprite(_buildingsConfig.ConstructionSiteSprite);
                    } 
                }
                
            }
        }
        

        private void SetSiteSprite(ConstructionSite.StateType stateType, ConstructionSite site)
        {
            switch (stateType)
            {
                case ConstructionSite.StateType.None:
                default:
                    site.SetSprite(null);
                    break;
                case ConstructionSite.StateType.Snowdrift:
                    site.SetSprite(_buildingsConfig.GetSnowdriftSprite(site.Data.SpriteIndex));
                    break;
                case ConstructionSite.StateType.ConstructionSite:
                    site.SetSprite(_buildingsConfig.ConstructionSiteSprite);
                    break;
                case ConstructionSite.StateType.Highlighted:
                    site.SetSprite(_buildingsConfig.ConstructionSiteHighlighted);
                    break;
            }
        }

        public bool TryGetSnowdrift(Vector2Int buildingAreaPosition, out ConstructionSite constructionSite)
        {
            constructionSite = ConstructionSites.FirstOrDefault(c => c.Data.BuildAreaPosition == buildingAreaPosition);
            return constructionSite != null && constructionSite.Data.State == ConstructionSite.StateType.Snowdrift;
        }

        public bool TryGetSnowdrift(Vector3Int buildingAreaPosition, out ConstructionSite constructionSite)
        {
            constructionSite =
                ConstructionSites.FirstOrDefault(c => c.Data.BuildAreaPosition == (Vector2Int)buildingAreaPosition);
            return constructionSite != null && constructionSite.Data.State == ConstructionSite.StateType.Snowdrift;
        }

        public ConstructionSiteData[] GetAllSitesData()
        {
            return ConstructionSites.Select(s => s.Data).ToArray();
        }

        public bool TryGetConstructionSite(Vector2Int buildingAreaPosition, out ConstructionSite site)
        {
            site = ConstructionSites.FirstOrDefault(c => c.Data.BuildAreaPosition == buildingAreaPosition);
            return site != null;
        }
        
        public void Load(LoadData request)
        {
            if (request.data.construction_site != null)
            {
                for (var index = 0; index < request.data.construction_site.Length; index++)
                {
                    var siteData = request.data.construction_site[index];
                    if (index >= _constructionSites.Count)
                    {
                        var site = _buildingFactory.InstantiateConstructionSite(siteData,root: transform);
                        _constructionSites.Add(site);
                    }
                    else
                    {
                        if (siteData.PositionType == ConstructionSite.PositionType.None &&
                            siteData.State == ConstructionSite.StateType.None)
                        {
                            continue;
                        }

                        if (siteData.State == ConstructionSite.StateType.Highlighted)
                        {
                            siteData.State = ConstructionSite.StateType.ConstructionSite;
                        }
                        _constructionSites[index].SetData(siteData);
                        SetSiteSprite(siteData.State, _constructionSites[index]);
                    }
                }
                if(GetNearblySnowdriftSiteCount() == 0)
                {
                    _bigSnowdrifts.DisableSnowdrifts();
                }
                _bigSnowdrifts.UpdateNavMesh();
            }
            
        }

        #region Editor

        public void RefreshSites()
        {
            var areas = _buildingAreaService.GetArea5X5();
            for (var index = 0; index <areas.Count; index++)
            {
                var areaData = areas[index];
                
                if (index >= _constructionSites.Count  )
                {
                    Debugging.Log($"Construction site service: cannot refresh sites. " +
                                  $"Please check  count Constuction Site List in inspector. " +
                                  $"We need has{areas.Count} sites prefabs into list");
                }
                var areaPos = areaData.Area.position;
                var position = _tileAreasService.CellToLocalPosition(areaPos);
                _constructionSites[index].transform.position = position;
                var sprite = _buildingsConfig.GetSnowdriftSprite(out var spriteIndex);

                ConstructionSiteData data = new ConstructionSiteData()
                {
                    BuildAreaPosition = new Vector2Int(areaPos.x,areaPos.y),
                    PositionType = Mathf.Abs(areaPos.x) > 11 || Mathf.Abs(areaPos.y) > 11  
                        ? ConstructionSite.PositionType.Distant
                        : ConstructionSite.PositionType.Nearby,
                    SpriteIndex = spriteIndex,
                    State = ConstructionSite.StateType.Snowdrift
                };

                _constructionSites[index].gameObject.name += $"_{data.PositionType}_{data.BuildAreaPosition}";
                _constructionSites[index].SetData(data);
                _constructionSites[index].SetSprite(sprite);
            }
        }

        #endregion
    }
}