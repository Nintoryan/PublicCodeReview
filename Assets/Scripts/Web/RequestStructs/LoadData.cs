using System;
using System.Collections.Generic;
using SO.Data.Characters;
using UnityEngine.Serialization;
using Util;
using VillageGame.Data;
using VillageGame.Logic.Tutorial;
using VillageGame.Services;
using VillageGame.Services.CutScenes;
using Web.ResponseStructs;
using Web.ResponseStructs.PayloadValues;

namespace Web.RequestStructs
{
    [Serializable]
    public class LoadData
    {
        public RequestData data;
        public DateTime time;

        public LoadData()
        {
            data = new RequestData();
        }
    }

    [Serializable]
    public class RequestData
    {
        public string player_name;
        public string exit_time;
        public int currency;
        public int core_game_level; 
        public string player_time_zone;
        public int base_utc_offset;
        public WinNumberData win_number;
        public DailyVisitsPresentData visit_presents;
        public PlayerProgressData player_progress;
        public List<BuildData> building;
        public List<CharacterType> existing_characters;
        public List<OpenedPresentBoxData> opened_presents;
        public List<WatchedScenesData> scenes_watched_indexes;
        public ConstructionSiteData[] construction_site;
        public TodayTasksState daily_tasks;
        public TreeBank tree_bank;
        public List<PurchasedBuildingData> purchased_buildings;
        public EnergyData energy_data;
        public int core_game_shuffles = 3;
        public int core_game_hints = 3;
        public bool new_year_projects_unlocked;
        public List<int> new_year_projects_states;
        public List<NotificationStateData> notification;
        public VillageTutorialData[] village_tutorials;
        
        public int dodo_birds_balance;
        public List<DodoBirdsGiftData> received_birds_gifts;
        
        public int coins_balance; //баланс додо коинов
        
        public List<string> complete_tracking_tasks; //ВОТ ЭТУ ХУЙНЮ на сервер не отправлять! Она приходит только с сервера.

        public int last_started_core_game_level = -1;

        public bool where_presents_watched_2;
        public bool year_pizza_info_watched;

        public IceCubesData ice_cubes_data;

        public RequestData()
        {
            energy_data = new(Constance.MAX_ENERGY, new DateTime());
            player_progress = new PlayerProgressData();
            new_year_projects_states = new List<int>();
            last_started_core_game_level = -1;
            ice_cubes_data = new(false, new bool[10]);
        }
    }
}