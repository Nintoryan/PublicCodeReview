using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Analytics;
using Util;
using VContainer;
using VContainer.Unity;
using VillageGame.Data;
using VillageGame.Services.Presents;
using VillageGame.Services.Storages;
using Web.ResponseStructs;
using Web.ResponseStructs.PayloadValues;

namespace VillageGame.Services
{
    public class DodoAnalyticsService : IInitializable
    {
        private readonly CoreGameLoadService _coreGameService;
        private readonly CalendarPresentsStorage _calendarPresentsStorage;
        private readonly RatingStorage _ratingStorage;
        private readonly CoroutineRunner _coroutineRunner;

        [Inject]
        public DodoAnalyticsService(IObjectResolver objectResolver)
        {
            Debug.Log("Analytics service constructor done!");
            _coreGameService = objectResolver.Resolve<CoreGameLoadService>();
            _calendarPresentsStorage = objectResolver.Resolve<CalendarPresentsStorage>();
            _ratingStorage = objectResolver.Resolve<RatingStorage>();
            _coroutineRunner = objectResolver.Resolve<CoroutineRunner>();

            _coroutineRunner.StartCoroutine(InitializeServices());
        }
        
        private IEnumerator InitializeServices()
        {
            var t = UnityServices.InitializeAsync();
            while (!t.IsCompleted)
            {
                yield return null;
            }

            yield return new WaitForSecondsRealtime(Random.Range(0, 2f));
            if (Random.Range(0, 100) < 97)
            {
                AnalyticsService.Instance.StartDataCollection();
                Debug.Log("Analytics should work");
            }
        }

        ~DodoAnalyticsService()
        {
            _coreGameService.CoreGameWinEvent -= SendCoreLevelWin;
            _coreGameService.CoreGameLoseEvent -= SendCoreLevelLose;
            
            _coreGameService.DodoCombinationCompletedEvent -= SendDodoBirdReceived;
            
            _calendarPresentsStorage.TakeNewClosedPresentBoxEvent -= SendPresentReceived;
            _calendarPresentsStorage.OpenCalendarPresentEvent -= SendPresentOpened;
            _ratingStorage.ChangeLevel -= SendLevelUp;
        }

        public void Initialize()
        {
            _coreGameService.CoreGameWinEvent += SendCoreLevelWin;
            _coreGameService.CoreGameLoseEvent += SendCoreLevelLose;

            _coreGameService.DodoCombinationCompletedEvent += SendDodoBirdReceived;

            _calendarPresentsStorage.TakeNewClosedPresentBoxEvent += SendPresentReceived;
            _calendarPresentsStorage.OpenCalendarPresentEvent += SendPresentOpened;

            _ratingStorage.ChangeLevel += SendLevelUp;
        }

        private void SendCoreLevelWin(int level) => SendCoreLevelDone(level, true);

        private void SendCoreLevelLose(int level) => SendCoreLevelDone(level, false);
        

        private void SendCoreLevelDone(int level, bool isWin)
        {
            Debugging.Log($"[Unity -> Analytics] Level {level} Done isWin={isWin}");
#if ENABLE_CLOUD_SERVICES_ANALYTICS
            AnalyticsService.Instance.CustomData("CoreLevelDone", new Dictionary<string, object>
            {
                { "level", level },
                { "win", isWin }
            });
#endif
        }
        
        public static void SendTaskDone(ConditionData data)
        {
            Debugging.Log($"[Unity -> Analytics] Task Done type={data.Type} value={data.Value}");
#if ENABLE_CLOUD_SERVICES_ANALYTICS
            AnalyticsService.Instance.CustomData("TaskDone", new Dictionary<string, object>
            {
                { "type", data.Type.ToString() },
                { "value", data.Value },
                { "valueString", data.ValueString}
            });
#endif
        }

        private void SendDodoBirdReceived()
        {
            Debugging.Log($"[Unity -> Analytics] Dodo bird received");
#if ENABLE_CLOUD_SERVICES_ANALYTICS
            AnalyticsService.Instance.CustomData("DodoBirdReceived");
#endif
        }

        public static void SendRemovedDodoCoins(int amount, string place)
        {
            Debugging.Log($"[Unity -> Analytics] Dodo Coin Spend amount={amount} place={place}");
#if ENABLE_CLOUD_SERVICES_ANALYTICS
            AnalyticsService.Instance.CustomData("DodoCoinSpend", new Dictionary<string, object>
            {
                { "amount", amount },
                { "place", place }
            });
#endif
        }

        public static void SendAddDodoCoins(AddCoinsType addType, string place)
        {
            Debugging.Log($"[Unity -> Analytics] Dodo Coins added amount={addType} place={place}");
#if ENABLE_CLOUD_SERVICES_ANALYTICS
            AnalyticsService.Instance.CustomData("DodoCoinAddd", new Dictionary<string, object>
            {
                { "addType", addType.ToString() },
                { "place", place }
            });
#endif
        }

        private void SendPresentReceived(PresentBoxData present)
        {
            Debugging.Log($"[Unity -> Analytics] Present Received name={present.Title} {present.ID}");
#if ENABLE_CLOUD_SERVICES_ANALYTICS
            AnalyticsService.Instance.CustomData("PresentReceived", new Dictionary<string, object>
            {
                { "name", $"{present.Title} {present.ID}" },
                { "open_date", $"{present.OpenDate.GetFullYearDateText()}" }
            });
#endif
        }

        private void SendPresentOpened(List<OpenedPresentBoxData> _, PresentBoxData present)
        {
            Debugging.Log($"[Unity -> Analytics] Present Opened name={present.Title} {present.ID}");
#if ENABLE_CLOUD_SERVICES_ANALYTICS
            AnalyticsService.Instance.CustomData("PresentOpened", new Dictionary<string, object>
            {
                { "name", $"{present.Title} {present.ID}" },
                { "open_date", $"{present.OpenDate.GetFullYearDateText()}" }
            });
#endif
        }

        private void SendLevelUp(int newLevel)
        {
            Debugging.Log($"[Unity -> Analytics] Level Up level={newLevel} Event:{Analytics.IsCustomEventEnabled("LevelUp")}");
#if ENABLE_CLOUD_SERVICES_ANALYTICS
            AnalyticsService.Instance.CustomData("LevelUp", new Dictionary<string, object>
            {
                { "level", newLevel }
            });
#endif   
        }
    }
}