using System;
using System.Linq;
using SO;
using SO.Data;
using SO.Data.Characters;
using UnityEngine;
using Util;
using VContainer;
using VContainer.Unity;
using VillageGame.Logic.Curtains;
using VillageGame.Services;
using VillageGame.Services.CutScenes;
using VillageGame.Services.CutScenes.CustomActions;
using VillageGame.Services.LoadingData;
using VillageGame.Services.Storages;
using VillageGame.UI.Panels;
using Web.RequestStructs;

namespace VillageGame.UI.Controllers
{
    public class DialoguePanelController : ILoading, IInitializable, ITickable
    {
        private readonly CutSceneActionsExecutor _cutsceneActionExecutor;
        private readonly EnterNamePanelController _enterNamePanelController;
        private readonly DialogueUIPanel _dialoguePanel;
        private readonly CharactersConfig _charactersConfig;
        private readonly InputService _inputService;
        private readonly CharactersStorage _characterStorage;

        private CutSceneData _cutSceneData;
        private ReplicaData _currentReplicaData;
        private int _currentReplicaIndex;
        private int _activeSecondaryCharacterIndex;

        public Action<ReplicaData> StartNewReplicaEvent;

        private string _playerName;

        private bool _isWatchingAction = true;

        private const float SKIP_COOLDOWN = 0.35f;
        private float _currentSkipCooldown;

        [Inject]
        public DialoguePanelController(IObjectResolver resolver)
        {
            _dialoguePanel = resolver.Resolve<UIFacade>().FindPanel<DialogueUIPanel>();
            _inputService = resolver.Resolve<InputService>();
            _charactersConfig = resolver.Resolve<CharactersConfig>();
            _cutsceneActionExecutor = resolver.Resolve<CutSceneActionsExecutor>();
            _enterNamePanelController = resolver.Resolve<EnterNamePanelController>();
            _characterStorage = resolver.Resolve<CharactersStorage>();
        }

        public void Initialize()
        {
            SubscribeToEvents(true);
        }

        ~DialoguePanelController()
        {
            SubscribeToEvents(false);
        }

        public void Tick()
        {
            if (_currentSkipCooldown > 0)
            {
                _currentSkipCooldown -= Time.deltaTime;
            }
        }

        public void StartCutSceneDialogue(CutSceneData cutSceneData)
        {
            _currentReplicaIndex = 0;
            _cutSceneData = cutSceneData;
            _currentReplicaData = _cutSceneData.Replicas[_currentReplicaIndex];

            SubscribeToDialogueEvents(true);
                StartReplicaAction();
        }


        private void StartTouch(Vector2 _)
        {
            if (!_dialoguePanel.IsActive || CutSceneActionsExecutor.IsWatching || CloudCurtain.IsAnimating)
            {
                return;
            }

            if (!CutSceneStateObserver.IsWatching)
            {
                _dialoguePanel.Hide();
                return;
            }

            if (_currentSkipCooldown <= 0)
            {
                if (_dialoguePanel.MainText.IsTyping)
                {
                    _dialoguePanel.MainText.StopWrite();
                }
                else
                {
                    SetNextReplica();
                }
            }
        }

        private void SetNextReplica()
        {
            _currentReplicaIndex++;
            if (_currentReplicaIndex < _cutSceneData.Replicas.Length)
            {
                _currentReplicaData = _cutSceneData.Replicas[_currentReplicaIndex];
                StartReplicaAction();
            }
            else
            {
                EndDialogue();
            }
        }

        private void StartReplicaAction()
        {
            if (_currentReplicaData.CustomAction != CustomCutsceneActionType.None)
            {
                _dialoguePanel.HideNoAnimation();
                _isWatchingAction = true;
                _cutsceneActionExecutor.ExecuteAction(_currentReplicaData.CustomAction);
            }
            else
            {
                StartCurrentReplica();
            }
        }

        private void StartCurrentReplica()
        {
            if (_currentReplicaData.Character == CharacterType.None)
            {
                CutSceneStateObserver.InvokeEndEvent(_cutSceneData);
                return;
            }

            _currentSkipCooldown = SKIP_COOLDOWN;
            if (!_dialoguePanel.IsActive)
            {
                _dialoguePanel.Show();
            }

            _isWatchingAction = false;
            _dialoguePanel.SetCharacter(_currentReplicaData.Character,
                GetCharacterPresentationModel(_currentReplicaData.Character),
                _currentReplicaData.PortraitReaction,
                IsMainCharacter());
            _dialoguePanel.MainText.StartWrite(GetCurrentText(), delay: 0.25f);

            if (_currentReplicaData.CharacterReaction != CharacterReaction.None)
            {
                var character = _characterStorage.GetCharacter(_currentReplicaData.Character);
                character?.Animation.PlayCutSceneReaction(_currentReplicaData.CharacterReaction);
            }

            StartNewReplicaEvent?.Invoke(_currentReplicaData);
        }

        private string GetCurrentText()
        {
            var originalText = _currentReplicaData.Text;

            if (originalText.Contains("[Имя]"))
            {
                return originalText.Replace("[Имя]", $"<color=#ED8233>{_playerName}</color>");
            }
            else
            {
                return originalText;
            }
        }

        private void SubscribeToDialogueEvents(bool flag)
        {
            if (flag)
            {
                _inputService.NonBlockStartTouchEvent += StartTouch;
                CutSceneActionsExecutor.EndEvent += StartCurrentReplica;
            }
            else
            {
                _inputService.NonBlockStartTouchEvent -= StartTouch;
                CutSceneActionsExecutor.EndEvent -= StartCurrentReplica;
            }
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _enterNamePanelController.SetPlayerNameEvent += OnSetPlayerName;
            }
            else
            {
                _enterNamePanelController.SetPlayerNameEvent -= OnSetPlayerName;
            }
        }

        private void EndDialogue()
        {
            SubscribeToDialogueEvents(false);
            _dialoguePanel.HideNoAnimation();
            _cutsceneActionExecutor.ExecuteAction(_cutSceneData.EndAction);
            CutSceneStateObserver.InvokeEndEvent(_cutSceneData);
        }


        private void OnSetPlayerName(string name)
        {
            _playerName = name;
        }

        private bool IsMainCharacter()
        {
            return _currentReplicaData.Character == _cutSceneData.CharacterData[0].Character;
        }

        private CharacterPresentationModel GetCharacterPresentationModel(CharacterType characterType)
        {
            var characterData = _charactersConfig.Characters.FirstOrDefault(x => x.Type == characterType);
            return characterData != null ? characterData.PresentationModel : null;
        }

        public void Load(LoadData request)
        {
            _playerName = Extensions.DecodeString(request.data.player_name);
        }
    }
}