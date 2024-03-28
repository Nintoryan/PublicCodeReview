using Web.RequestStructs;

namespace VillageGame.Services.LoadingData
{
    public interface ILoading
    {
         void Load(LoadData request);
    }
}