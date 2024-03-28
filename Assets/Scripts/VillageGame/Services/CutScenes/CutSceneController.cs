using System;
using SO.Data;
using UnityEngine;
using Util;
using VContainer;
using VContainer.Unity;
using VillageGame.Data.Types;
using VillageGame.Logic.Buildings;
using VillageGame.Logic.Cameras;
using VillageGame.Logic.Curtains;
using VillageGame.Services.Storages;
using VillageGame.UI;
using VillageGame.UI.Controllers;

namespace VillageGame.Services.CutScenes
{
    public class CutSceneController : IInitializable
    {
        private readonly CutSceneStartController _cutSceneStartController;
        private readonly CharactersStorage _charactersStorage;
        private readonly DialoguePanelController _dialoguePanelController;
        private readonly CutSceneCamera _cutSceneCamera;
        private readonly CloudCurtain _cloudCurtain;

        private readonly BuildingOnMapStorage _buildingStorage;

        private readonly IceCubesService _iceCubesService;
 
        public Action<CutSceneData> EndCutSceneEvent;

        [Inject]
        private CutSceneController(IObjectResolver objectResolver)
        {
            _cutSceneStartController = objectResolver.Resolve<CutSceneStartController>();
            _charactersStorage = objectResolver.Resolve<CharactersStorage>();
            _dialoguePanelController = objectResolver.Resolve<DialoguePanelController>();
            _cutSceneCamera = objectResolver.Resolve<CutSceneCamera>();
            _buildingStorage = objectResolver.Resolve<BuildingOnMapStorage>();
            _cloudCurtain = objectResolver.Resolve<UIFacade>().CloudCurtain;
            _iceCubesService = objectResolver.Resolve<IceCubesService>();
        }

        public void Initialize()
        {
            SubscribeToEvents(true);
        }

        ~CutSceneController()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                CutSceneStateObserver.StartCutsceneEvent += OnStartCutSceneEvent;
                _dialoguePanelController.StartNewReplicaEvent += SetCameraPositionWithAnimation;
                CutSceneStateObserver.EndCutsceneEvent += OnDialogueEndEvent;
            }
            else
            {
                CutSceneStateObserver.StartCutsceneEvent -= OnStartCutSceneEvent;
                _dialoguePanelController.StartNewReplicaEvent -= SetCameraPositionWithAnimation;
                CutSceneStateObserver.EndCutsceneEvent -= OnDialogueEndEvent;
            }
        }

        private void OnStartCutSceneEvent(CutSceneData cutSceneData)
        {
            if (cutSceneData.CurtainParams.IsShowOnStart && cutSceneData.CurtainParams.Text != "")
            {
                _cloudCurtain.ShowWithText(cutSceneData.CurtainParams.Text,
                    onShown: () =>
                    {
                        SetPositions(cutSceneData);
                        _dialoguePanelController.StartCutSceneDialogue(cutSceneData);
                    },
                    onHidden: () => { _dialoguePanelController.StartCutSceneDialogue(cutSceneData); });
            }
            else if (cutSceneData.CurtainParams.IsShowOnStart)
            {
                _cloudCurtain.Show(
                    onShown: () =>
                    {
                        SetPositions(cutSceneData);
                        _dialoguePanelController.StartCutSceneDialogue(cutSceneData);
                    },
                    onHidden: () => { _dialoguePanelController.StartCutSceneDialogue(cutSceneData); });
            }
            else
            {
                SetPositions(cutSceneData);
                _dialoguePanelController.StartCutSceneDialogue(cutSceneData);
            }
        }

        private void SetPositions(CutSceneData cutSceneData)
        {
            SetCharactersPosition(cutSceneData);
            if (!cutSceneData.CameraIsStayOnCurrentPosition)
            {
                _cutSceneCamera.SaveStartPosition();
                SetCameraPosition(cutSceneData.Replicas[0]);
            }
        }

        private void SetCharactersPosition(CutSceneData cutSceneData)
        {
            if (!cutSceneData.IsCharacterTeleportToPosition)
            {
                return;
            }
            if (cutSceneData.Condition.Type != ConditionType.BuildHouse)
            {
                if (cutSceneData.Condition.Type == ConditionType.EndCutScene && cutSceneData.CurtainParams.IsShowOnStart)
                {
                    if (_buildingStorage.TryGetBuilding(BuildingType.House, cutSceneData.Replicas[0].Camera.PositionByBuildingID, out var house))
                    {
                        foreach (var characterData in cutSceneData.CharacterData)
                        {
                            var character = _charactersStorage.GetCharacter(characterData.Character);
                            if (character == null)
                            {
                                continue;
                            }

                            character.Move.BlockMove();

                            Vector3 position;

                            if (house.BuildData.x == -11 && house.BuildData.y == -11)
                            {
                                position = house.LookPosition.GetLastCaptureLookPoint(character);
                            }
                            else
                            {
                                position = house.LookPosition.GetCaptureLookPoint(character);
                            }

                            character.TeleportToPosition(position,
                                isUp: false,
                                isLeft: position.x > house.transform.position.x);
                        }
                    }
                    else
                    {
                        foreach (var characterData in cutSceneData.CharacterData)
                        {
                            var character = _charactersStorage.GetCharacter(characterData.Character);
                            if (character == null)
                            {
                                continue;
                            }

                            character.Move.BlockMove();
                            character.TeleportToPosition(characterData.CharacterPosition, characterData.IsUp,
                                characterData.isLeft);
                        }
                    }
                }
                else
                {
                    if (cutSceneData.Condition.Type == ConditionType.IceCubeRemoved)
                    {
                        BuildingLookPosition lookPosition = _iceCubesService.GetSelectedIceCubeLookPosition();
                        foreach (var characterData in cutSceneData.CharacterData)
                        {
                            var character = _charactersStorage.GetCharacter(characterData.Character);
                            if (character == null)
                            {
                                continue;
                            }

                            character.Move.BlockMove();

                            Vector3 position;

                            position = lookPosition.GetCaptureLookPoint(character);

                            character.TeleportToPosition(position,
                                isUp: false,
                                isLeft: position.x > _iceCubesService.GetPositionOfSelectedIceCube().x);
                        }
                    }
                    else
                    {
                        foreach (var characterData in cutSceneData.CharacterData)
                        {
                            var character = _charactersStorage.GetCharacter(characterData.Character);
                            if (character == null)
                            {
                                continue;
                            }

                            character.Move.BlockMove();
                            character.TeleportToPosition(characterData.CharacterPosition, characterData.IsUp,
                                characterData.isLeft);
                        }
                    }
                }
            }
            else
            {
                if (_buildingStorage.TryGetBuilding(BuildingType.House, cutSceneData.Condition.Value, out var house))
                {
                    foreach (var characterData in cutSceneData.CharacterData)
                    {
                        var character = _charactersStorage.GetCharacter(characterData.Character);
                        if (character == null)
                        {
                            continue;
                        }

                        character.Move.BlockMove();

                        Vector3 position;

                        if (house.BuildData.x == -11 && house.BuildData.y == -11)
                        {
                            position = house.LookPosition.GetLastCaptureLookPoint(character);
                        }
                        else
                        {
                            position = house.LookPosition.GetCaptureLookPoint(character);
                        }

                        character.TeleportToPosition(position,
                            isUp: false,
                            isLeft: position.x > house.transform.position.x);
                    }
                }
            }
        }

        private void SetCameraPositionWithAnimation(ReplicaData replicaData)
        {
            if (replicaData.Camera.IsIceBlock)
            {
                _cutSceneCamera.SetNewPosition(_iceCubesService.GetPositionOfSelectedIceCube(), replicaData.Camera.Zoom);
            }
            else
            {
                if (_buildingStorage.TryGetBuilding(BuildingType.House, replicaData.Camera.PositionByBuildingID,
                        out var house))
                {
                    _cutSceneCamera.SetNewPositionWithAnimation(house.transform.position, replicaData.Camera.Zoom);
                }
                else
                {
                    _cutSceneCamera.SetNewPositionWithAnimation(replicaData.Camera.Position, replicaData.Camera.Zoom);
                }
            }
        }


        private void SetCameraPosition(ReplicaData replicaData)
        {
            if (replicaData.Camera.IsIceBlock)
            {
                _cutSceneCamera.SetNewPosition(_iceCubesService.GetPositionOfSelectedIceCube(), replicaData.Camera.Zoom);
            }
            else
            {
                if (_buildingStorage.TryGetBuilding(BuildingType.House, replicaData.Camera.PositionByBuildingID,
                        out var house))
                {
                    _cutSceneCamera.SetNewPosition(house.transform.position, replicaData.Camera.Zoom);
                }
                else
                {
                    _cutSceneCamera.SetNewPosition(replicaData.Camera.Position, replicaData.Camera.Zoom);
                }
            }
        }

        private void OnDialogueEndEvent(CutSceneData cutSceneData)
        {
            if (cutSceneData.CameraIsBackStartPos)
            {
                _cutSceneCamera.SetStartPositionWithAnimation(
                    OnCanceled: () => UnblockCutsceneComponent(cutSceneData));
            }
            else
            {
                UnblockCutsceneComponent(cutSceneData);
            }
        }

        private void UnblockCutsceneComponent(CutSceneData cutSceneData)
        {
            if (cutSceneData.IsUnblockCharacters)
            {
                foreach (var characterData in cutSceneData.CharacterData)
                {
                    var character = _charactersStorage.GetCharacter(characterData.Character);
                    if (character == null)
                    {
                        continue;
                    }

                    character.Move.UnblockMove();
                }
            }

            if (!cutSceneData.IsIgnoreCutSceneList)
            {
                _cutSceneStartController.SetWatched(cutSceneData.ID);
            }

            EndCutSceneEvent?.Invoke(cutSceneData);
        }

        private bool IsHasAllCharacter(CutSceneData cutSceneData)
        {
            foreach (var character in cutSceneData.CharacterData)
            {
                if (!_charactersStorage.Contains(character.Character))
                {
                    Debugging.Log($"Cut scene controller: character {character.Character} is not existing",
                        LogStyle.Error);
                    return false;
                }
            }

            return true;
        }
    }
}