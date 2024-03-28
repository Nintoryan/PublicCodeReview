using System.Collections.Generic;
using VContainer;
using VillageGame.Services.LoadingData;

namespace VillageGame.Services.Storages
{
    public class LoadingEntitiesStorage
    {
        public List<ILoading> LoadingsEntities { get; } = new();

        [Inject] public LoadingEntitiesStorage(){}
        public void Add(ILoading entity)
        {
            LoadingsEntities.Add(entity);    
        }
        
        
    }
}