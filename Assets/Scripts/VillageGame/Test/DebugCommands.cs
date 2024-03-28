using System;
using IngameDebugConsole;
using SO;
using UnityEngine;
using VContainer;
using VillageGame.Services;
using VillageGame.Services.Storages;
using Web.Api;

namespace VillageGame.Test
{
    public class DebugCommands : MonoBehaviour
    {
        [SerializeField] private GameObject _ingameDebugConsole;

        [Inject] private DodoBirdsService _service;
        [Inject] private EnergyStorage _energyStorage;
        [Inject] private CoreGameLoadService _coreGameLoad;
        [Inject] private RatingStorage _progressionService;
        [Inject] private CurrencyStorage _currencyStorage;
        [Inject] private WebAPI _webApi;
        [Inject] private TimeObserver _timeObserver;

        private void Awake()
        {
            JSAPI.UIDSet += OnUIDSet;
        }

        private void Start()
        {
            EditCommands();
        }

        private void OnUIDSet(string uid)
        {
            if (uid is "000D3A25D54580E611E75743E72DEA64" 
                or "EAD5084517F89DE611EE8ECA3B08E378")
            {
                _ingameDebugConsole.SetActive(true);
            }
        }
        private void EditCommands()
        {
            DebugLogConsole.AddCommand<string>("Set.UID",
                "Устанавливает uid пользователя",
                JSAPI.SetCustomUID);
            
            DebugLogConsole.AddCommand("Set.TestUID",
                "Устанавливает uid тестового пользователя из БД додо (можно использовать додо коины), но он только один",
                JSAPI.SetTestUserUID);
            
            DebugLogConsole.AddCommand<int>("Set.GameLevel", 
                "Устанавливает текущий уровень кор игры. Не использовать внутри кор игры.",
                SetCoreGameLevel);
            
            DebugLogConsole.AddCommand<int>( "Add.Stars",
                "Добавляет чудеса в указаном количестве",
                _currencyStorage.Add);
            
            DebugLogConsole.AddCommand<int>( "Add.Exp",
                "Добавляет очки опыта в указаном количестве",
                _progressionService.AddRating);
            
            DebugLogConsole.AddCommand<int>( "Add.Energy",
                "Добавляет энергию в указаном количестве",
                _energyStorage.ConsoleAdd);
            
            DebugLogConsole.AddCommand( "Reset", 
                "Обнуляет весь прогресс игрока на сервера. После использования надо обновить страницу",
                _webApi.ResetData);
            
            DebugLogConsole.AddCommand<int>( "Add.DodoBird",
                "Добавляет додо птицы в указаном количестве",
                _service.AddDodoBirdAmount);

            DebugLogConsole.AddCommand<int>("Add.Hints",
                "Добавляет подсказки в указанном количество",
                StaticPrefs.AddHints);
            

            DebugLogConsole.AddCommand<string>("SendUrl","Opens deep link",JSAPI.SendURL);
            DebugLogConsole.AddCommand<string>("JSCALL","",ExecuteJs);
            

            DebugLogConsole.RemoveCommand("prefs.clear");
            DebugLogConsole.RemoveCommand("prefs.delete");
            DebugLogConsole.RemoveCommand("prefs.float");
            DebugLogConsole.RemoveCommand("prefs.int");
            DebugLogConsole.RemoveCommand("prefs.string");
            DebugLogConsole.RemoveCommand("scene.load");
            DebugLogConsole.RemoveCommand("scene.loadasync");
            DebugLogConsole.RemoveCommand("scene.unload");
            DebugLogConsole.RemoveCommand("scene.restart");
            DebugLogConsole.RemoveCommand("time.scale");
            DebugLogConsole.RemoveCommand("logs.save");
            DebugLogConsole.RemoveCommand("sysinfo");
        }

        private void ExecuteJs(string jsCode)
        {
            Application.ExternalEval(jsCode);
        }

        private void SetCoreGameLevel(int value)
        {
            StaticPrefs.SelectedCoreGameLevel = value;
            _coreGameLoad.CoreGameWinEvent?.Invoke(StaticPrefs.SelectedCoreGameLevel);
        }
    }
}