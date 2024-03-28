using SO.Data;
using SO.Data.Presents;
using VContainer;
using VContainer.Unity;
using VillageGame.Data.Types;
using VillageGame.Services.DailyTasks;
using VillageGame.Services.Presents;
using VillageGame.Services.Storages;
using VillageGame.UI.Controllers;

namespace VillageGame.Services
{
    public class RewardService : IInitializable
    {
        private readonly DailyTasksService _dailyTasksService;
        private readonly CurrencyStorage _currencyStorage;
        private readonly RatingStorage _ratingStorage;
        private readonly PresentInformationPanelController _presentInformationPanelController;
        private readonly PurchasedBuildingsStorage _purchasedBuildingStorage;
        private readonly CoreGameBonusStorage _coreGameBonusesStorage;
        private readonly DailyVisitService _dailyVisitService;

        [Inject]
        public RewardService(IObjectResolver objectResolver)
        {
            _currencyStorage = objectResolver.Resolve<CurrencyStorage>();
            _ratingStorage = objectResolver.Resolve<RatingStorage>();
            _presentInformationPanelController = objectResolver.Resolve<PresentInformationPanelController>();
            _dailyTasksService = objectResolver.Resolve<DailyTasksService>();
            _purchasedBuildingStorage = objectResolver.Resolve<PurchasedBuildingsStorage>();
            _coreGameBonusesStorage = objectResolver.Resolve<CoreGameBonusStorage>();
            _dailyVisitService = objectResolver.Resolve<DailyVisitService>();
        }

        public void Initialize()
        {
            SubscribeToEvents(true);
        }

        ~RewardService()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _dailyTasksService.CloseTaskEvent += OnCloseTaskEvent;
                _dailyVisitService.PressTakeDailyVisitPresentEvent += OpenPresent;
                _presentInformationPanelController.GetPresentEvent += OpenPresent;
            }
            else
            {
                _dailyTasksService.CloseTaskEvent -= OnCloseTaskEvent;
                _dailyVisitService.PressTakeDailyVisitPresentEvent -= OpenPresent;
                _presentInformationPanelController.GetPresentEvent -= OpenPresent;
            }
        }

        public void OpenPresent(PresentValueData presentValue)
        {
            switch (presentValue)
            {
                case BuildPresentValueData buildPresent:
                    _purchasedBuildingStorage.Add(buildPresent.Type, buildPresent.BuildingID, isPresent: true);
                    break;
                case CurrencyPresentValueData currencyPresent:
                    switch (currencyPresent.ValueType)
                    {
                        case CurrencyPresentValueData.Type.ActionPoint:
                            _currencyStorage.Add(currencyPresent.Count);
                            break;
                        case CurrencyPresentValueData.Type.Rating:
                            _ratingStorage.AddRating(currencyPresent.Count);
                            break;
                        case CurrencyPresentValueData.Type.Shuffle:
                            _coreGameBonusesStorage.AddShuffles(currencyPresent.Count);
                            break;
                        case CurrencyPresentValueData.Type.Hint:
                            _coreGameBonusesStorage.AddHints(currencyPresent.Count);
                            break;
                    }
                    break;
            }
        }

        private void OnCloseTaskEvent(DailyTaskData dailyTaskData)
        {
            if (dailyTaskData.CurrencyReward > 0)
            {
                _currencyStorage.Add(dailyTaskData.CurrencyReward);
            }

            if (dailyTaskData.RatingReward > 0)
            {
                _ratingStorage.AddRating(dailyTaskData.RatingReward);
            }

            if (dailyTaskData.ShuffleReward > 0)
            {
                _coreGameBonusesStorage.AddShuffles(dailyTaskData.ShuffleReward);
            }

            if (dailyTaskData.HintReward > 0)
            {
                _coreGameBonusesStorage.AddHints(dailyTaskData.HintReward);
            }

            if (dailyTaskData.DecorationRewardID != -1)
            {
                _purchasedBuildingStorage.Add(BuildingType.Decoration,dailyTaskData.DecorationRewardID, isPresent: true);
            }
        }
    }
}