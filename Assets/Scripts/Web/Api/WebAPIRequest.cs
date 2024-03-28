using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SO.Data.Characters;
using UnityEngine;
using Util;
using VillageGame.Data;
using VillageGame.Logic.Tutorial;
using VillageGame.Services;
using VillageGame.Services.CutScenes;
using Web.ResponesStructs;
using Web.ResponseStructs;
using Web.ResponseStructs.PayloadValues;

namespace Web.Api
{
    public partial class WebAPI
    {
        public void SendBuildingsData(List<BuildData> buildings)
        {
            SendUpdateData(WebField.building, buildings);
        }

        public void SendExitTimeData(string date)
        {
            SendUpdateData(WebField.exit_time, date);
        }

        public void SendCurrencyData(int value)
        {
            SendUpdateData(WebField.currency, value);
        }

        public void SendWinNumberData(WinNumberData winNumber)
        {
            SendUpdateData(WebField.win_number, winNumber);
        }

        public void SendPresentData( List<OpenedPresentBoxData> presents)
        {
            SendUpdateData(WebField.opened_presents, presents);
        }
        
        public void SendPlayerProgressData(PlayerProgressData playerProgressData)
        {
            SendUpdateData(WebField.player_progress, playerProgressData);
        }

        public void SendCoreGameLevel(int level)
        {
            SendUpdateData(WebField.core_game_level, level);
        }

        public void SendCutScenesData(List<WatchedScenesData> scenesWatchedData)
        {
            SendUpdateData(WebField.scenes_watched_indexes, scenesWatchedData);
        }
        
        public void SendPlayerTimeZone(TimeZoneInfo timeZoneInfo)
        {
            SendUpdateData(WebField.player_time_zone, timeZoneInfo.Id);
        }

        public void SendBaseUtcOffset(int baseUtсOffset)
        {
            SendUpdateData(WebField.base_utc_offset, baseUtсOffset);
        }
        
        public void SendDailyTasks(TodayTasksState _todayTasksState)
        {
            SendUpdateData(WebField.daily_tasks, _todayTasksState);
        }

        public void SendChristmasTreeBank(TreeBank christmasTreeBank)
        {
            SendUpdateData(WebField.tree_bank,christmasTreeBank);
        }
        
        public void SendDailyVisitsPresentData(DailyVisitsPresentData dailyVisitsPresentData)
        {
            SendUpdateData(WebField.visit_presents,dailyVisitsPresentData);
        }
        
        public void SendExistingCharacters(List<CharacterType> existingCharacters)
        {
            SendUpdateData(WebField.existing_characters, existingCharacters);
        }

        public void SendConstructionSite(ConstructionSiteData[] constructionSites)
        {
            SendUpdateData(WebField.construction_site, constructionSites);
        }
        
        public void SendPurchasedBuildings(List<PurchasedBuildingData> purchasedBuildings)
        {
            SendUpdateData(WebField.purchased_buildings, purchasedBuildings);
        }
        
        public void SendEnergy(EnergyData energyData)
        {
            SendUpdateData(WebField.energy_data, energyData);
        }

        public void SendCoreGameShuffles(int coreGameShuffles)
        {
            SendUpdateData(WebField.core_game_shuffles, coreGameShuffles);
        }

        public void SendCoreGameHints(int coreGameHints)
        {
            SendUpdateData(WebField.core_game_hints, coreGameHints);
        }

        public void SendNewYearProjectsUnlocked(bool isNewYearProjectsUnlocked)
        {
            SendUpdateData(WebField.new_year_projects_unlocked, isNewYearProjectsUnlocked);
        }

        public void SendNewYearProjectsData(List<int> projectsStates)
        {
            SendUpdateData(WebField.new_year_projects_states, projectsStates);
        }

        public void SendPlayerName(string name)
        {
            
             SendUpdateData(WebField.player_name,Extensions.EncodeString(name));
        }

        public void SendVillageTutorial(VillageTutorialData[] tutorialData)
        {
            SendUpdateData(WebField.village_tutorials, tutorialData);
        }
        
        public void SendBirdGiftsUpdated(List<DodoBirdsGiftData> gifts)
        {
            SendUpdateData(WebField.received_birds_gifts,gifts);
        }

        public void SendBirdsBalanceUpdated(int balance)
        {
            SendUpdateData(WebField.dodo_birds_balance,balance);
        }

        public void SendLastStartedCoreGameLevelUpdated(int coreGameLevel)
        {
            SendUpdateData(WebField.last_started_core_game_level, coreGameLevel);
        }

        public void SendYearPizzaInfoWatched()
        {
            SendUpdateData(WebField.year_pizza_info_watched,true);
        }

        public void SendWherePresentsWatched()
        {
            SendUpdateData(WebField.where_presents_watched_2,true);
        }

        public void SendIceCubesDataUpdated(IceCubesData iceCubesData)
        {
            SendUpdateData(WebField.ice_cubes_data, iceCubesData);
        }

        public void ResetData()
        {
            SendPlayerProgressData(new PlayerProgressData());
            SendBuildingsData(new List<BuildData>());
            SendCurrencyData(0);
            SendExitTimeData("");
            SendCoreGameLevel(0);
            SendWinNumberData(null);
            SendPresentData(new  List<OpenedPresentBoxData>());
            SendDailyTasks(new TodayTasksState());
            SendCutScenesData(new List<WatchedScenesData>());
            SendChristmasTreeBank(new TreeBank());
            SendDailyVisitsPresentData(new DailyVisitsPresentData());
            SendExistingCharacters(null);
            SendConstructionSite(null);
            SendPurchasedBuildings(null);
            SendEnergy(new EnergyData(Constance.MAX_ENERGY, new DateTime()));
            SendCoreGameShuffles(3);
            SendCoreGameHints(3);
            SendNewYearProjectsUnlocked(false);
            SendNewYearProjectsData(new List<int>());
            SendVillageTutorial(null);
            SendBirdGiftsUpdated(null);
            SendBirdsBalanceUpdated(0);
            SendLastStartedCoreGameLevelUpdated(-1);
            SendIceCubesDataUpdated(new IceCubesData(false,new bool[10]));
        }

        private void SendUpdateData<T>(WebField field, T payload) 
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new OrderedContractResolver(),
            };
            
            var valueJson = JsonConvert.SerializeObject(payload, jsonSerializerSettings);
            
            var signatureData = new SignatureData
            {
                field = field.ToString(),
                value = valueJson,
                uid = JSAPI.UID
            };

            var data = new UpdateResponesData<T>();

            data.SetData(field.ToString(), payload, HMAC.CreateHMAC(signatureData.GetSignature()));
            
            var json = JsonConvert.SerializeObject(data, new JsonSerializerSettings 
                { ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            
            WebDEBUG.Log($"[UNITY -> GAME_SERVER] {field.ToString()} data: {json} ");
            WebSocketClient.SendMessageToServer(json);
        }

        public void SendAddDodoCoins(AddCoinsType coinAddType, string idempotencyKey)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new OrderedContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            var signature = $"{JSAPI.UID};{(int)coinAddType};{idempotencyKey};add_dodo_coins_for_user";
            var hmac = HMAC.CreateHMAC(signature);

            var data = new DodoCoinResponseData<AddIdempotentPayload>
            {
                request = "add_coins",
                payload = new AddIdempotentPayload
                {
                    type = (int)coinAddType,
                    idempotency_key = idempotencyKey,
                    signature = hmac
                }
            };
            
            var json = JsonConvert.SerializeObject(data, jsonSerializerSettings);
            
            WebDEBUG.Log($"[UNITY -> GAME_SERVER] add_coins data: {json} ");
            WebSocketClient.SendMessageToServer(json);
        }
        
        public void SendRemoveDodoCoins(int amountToRemove, string idempotencyKey)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new OrderedContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            var signature = $"{JSAPI.UID};{amountToRemove};{idempotencyKey};withdraw_dodo_coins_for_user";
            var hmac = HMAC.CreateHMAC(signature);

            var data = new DodoCoinResponseData<RemoveIdempotentPayload>
            {
                request = "remove_coins",
                payload = new RemoveIdempotentPayload
                {
                    amount = amountToRemove,
                    country_code = 643,
                    idempotency_key = idempotencyKey,
                    signature = hmac
                }
            };
            
            var json = JsonConvert.SerializeObject(data, jsonSerializerSettings);
            
            WebDEBUG.Log($"[UNITY -> GAME_SERVER] remove_coins data: {json} ");
            WebSocketClient.SendMessageToServer(json);
        }
        
        public void SendIssuePromocode(int promoID, string idempotencyKey)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new OrderedContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            var signature = $"{JSAPI.UID};{promoID};{idempotencyKey};issue_me_a_new_promocode_please";
            var hmac = HMAC.CreateHMAC(signature);

            var data = new DodoCoinResponseData<PromocodeIdempotentPayload>
            {
                request = "issue_promocode",
                payload = new PromocodeIdempotentPayload
                {
                    type = promoID,
                    country_code = 643,
                    idempotency_key = idempotencyKey,
                    signature = hmac
                }
            };
            
            var json = JsonConvert.SerializeObject(data, jsonSerializerSettings);
            
            WebDEBUG.Log($"[UNITY -> GAME_SERVER] issue_promocode data: {json} ");
            WebSocketClient.SendMessageToServer(json);
        }

        public void SendTrackProductID(int taskId, string productGuid)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new OrderedContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var signature = $"{JSAPI.UID};{taskId};{productGuid};start_product_id_tracking";
            var hmac = HMAC.CreateHMAC(signature);
            
            var data = new TrackingProductRequest
            {
                request = "set_tracking_product",
                payload = new TrackingProductRequestPayload
                {
                    task_id = taskId.ToString(),
                    guid = productGuid,
                    signature = hmac
                }
            };
            var json = JsonConvert.SerializeObject(data, jsonSerializerSettings);
            WebDEBUG.Log($"[UNITY -> GAME_SERVER] set_tracking_product data: {json} ");
            WebSocketClient.SendMessageToServer(json);
        }
        
        public void SendTrackOrderPrice(int taskId, int orderPrice)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new OrderedContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            
            var signature = $"{JSAPI.UID};{taskId};{orderPrice};start_price_tracking";
            var hmac = HMAC.CreateHMAC(signature);
            
            var data = new TrackingOrderPriceRequest
            {
                request = "set_tracking_price",
                payload = new TrackingOrderPricePayload
                {
                    task_id = taskId.ToString(),
                    price = orderPrice,
                    signature = hmac
                }
            };
            
            var json = JsonConvert.SerializeObject(data, jsonSerializerSettings);
            WebDEBUG.Log($"[UNITY -> GAME_SERVER] set_tracking_price data: {json} ");
            WebSocketClient.SendMessageToServer(json);
        }

        public void SendCoreGameLevelAnalytics(bool isWin, int level)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new OrderedContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            var data = new CoreGameAnalyticsData
            {
                request = "core_level_end",
                payload = new CoreLevelEndData
                {
                    level = level,
                    victory = isWin,
                }
            };

            var json = JsonConvert.SerializeObject(data, jsonSerializerSettings);
            Debug.Log($"[UNITY -> GAME_SERVER] core_level_end data: {json}");
            WebSocketClient.SendMessageToServer(json);
        }
    }
    
    public class OrderedContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
    {
        protected override IList<Newtonsoft.Json.Serialization.JsonProperty> CreateProperties(System.Type type, MemberSerialization memberSerialization)
        {
            var @base = base.CreateProperties(type, memberSerialization);
            var ordered = @base
                .OrderBy(p => p.Order ?? int.MaxValue)
                .ThenBy(p => p.PropertyName)
                .ToList();
            return ordered;
        }
    }
}