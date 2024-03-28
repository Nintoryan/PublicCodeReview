using System;
using Data.Scripts.Audio;
using DefaultNamespace.VillageGame.Infrastructure;
using DefaultNamespace.VillageGame.Logic;
using SO.Data;
using SO.Data.Characters;
using UnityEngine;
using Util;
using VContainer;
using VillageGame.Services.CutScenes.Entities;
using VillageGame.Services.Storages;

namespace VillageGame.Services.CutScenes
{
    public class CutSceneAnimator : MonoBehaviour
    {
        private enum CutSceneName
        {
            None,
            ArrivalOfGuid,
            HelloPostman
        }

        [SerializeField] private Transform _animationCharacter;
        [SerializeField] private Camera _cutSceneCamera;
        [SerializeField] private Animator _cutSceneAnimator;
        [SerializeField] private GameObject _train;
        [Header("Arrival Of Guid")]
        [SerializeField] private TrainAnimation _trainAnimation;
        [SerializeField] private Suitcase _suitcase;
        
        private CharactersStorage _characterStorage;
        private Camera _mainCamera;
        private CutSceneStartController _cutSceneStartController;
        private Transform _characterPointInTrainStation;
        
        public Suitcase Suitcase => _suitcase;
        public Action EndAnimationEvent;
        public GameObject Train => _train;
        
        [Inject]
        public void Construct(IObjectResolver objectResolver)
        {
            _mainCamera = Camera.main;
            _characterPointInTrainStation = GameObject.FindWithTag(Constance.CHARACTER_INITIAL_POINT_TAG).transform;
            _characterStorage = objectResolver.Resolve<CharactersStorage>();
            _cutSceneStartController = objectResolver.Resolve<CutSceneStartController>();
        }

        private void Awake()
        {
            SubscribeToEvents(true);
        }

        private void OnDestroy()
        {
            SubscribeToEvents(false);
        }
        public void PlayArrivalOfGuidAnimation()
        {
            _cutSceneAnimator.enabled = true;
            _suitcase.EnableSuitcase();
            _suitcase.SetCloseSprite();
            _cutSceneCamera.gameObject.SetActive(true);
            _mainCamera.gameObject.SetActive(false);
            _cutSceneAnimator.SetTrigger(CutSceneName.ArrivalOfGuid.ToString());
            _trainAnimation.SetMoveAnimation();
        }

        public void PlayHelloPostmanAnimation()
        {
            _cutSceneAnimator.enabled = true;
            _cutSceneAnimator.SetTrigger(CutSceneName.HelloPostman.ToString());
        }

        public void StopAnimation()
        {
            _cutSceneAnimator.enabled = false;
        }

        public void DisableAnimationCharacter()
        {
            _animationCharacter.gameObject.SetActive(false);
            _animationCharacter.transform.position = new Vector3(200, 0, 0);
        }


        public void DisableTrain()
        {
            _train.SetActive(false);
        }

        public void EnableTrain()
        {
            _train.SetActive(true);
        }
        
        #region Game events
        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                GlobalEvent.LoadGameEvent += OnLoadVillageScene;
                GlobalEvent.LoadCoreGameSceneEvent += OnLoadCoreGameScene;
                GlobalEvent.LoadVillageSceneEvent += OnLoadVillageScene;
                CutSceneStateObserver.StartCutsceneEvent += OnStartCutScene;
            }
            else
            {
                GlobalEvent.LoadGameEvent -= OnLoadVillageScene;
                GlobalEvent.LoadCoreGameSceneEvent -= OnLoadCoreGameScene;
                GlobalEvent.LoadVillageSceneEvent -= OnLoadVillageScene;
                CutSceneStateObserver.StartCutsceneEvent -= OnStartCutScene;
            }
        }

        private void OnStartCutScene(CutSceneData obj)
        {
            if (obj.ID == 2)
            {
                _suitcase.DisableSuitcase(5);
                CutSceneStateObserver.StartCutsceneEvent -= OnStartCutScene;
            }
            else if(obj.ID > 2)
            {
                CutSceneStateObserver.StartCutsceneEvent -= OnStartCutScene;
            }
        }
        
        private void OnLoadCoreGameScene()
        {
            StopAnimation();
            _animationCharacter.gameObject.SetActive(false);
            _suitcase.DisableSuitcase();
        }

        private void OnLoadVillageScene()
        {
            var lastWatchedSceneId = _cutSceneStartController.GetLastWatchedSceneId();
            Debugging.Log($"Suitcase: OnLoadVillageScene -> {lastWatchedSceneId}",ColorType.Blue);
            if (lastWatchedSceneId == -1)
            {
                _suitcase.EnableSuitcase();
                _suitcase.SetCloseSprite();
                Debugging.Log($"Suitcase: OnLoadVillageScene -> set close",ColorType.Blue);
            }
            else if(lastWatchedSceneId == 0)
            {
                _suitcase.EnableSuitcase();
                _suitcase.SetOpenSprite();
                Debugging.Log($"Suitcase: OnLoadVillageScene -> set open",ColorType.Blue);
            }
            else
            {
                _suitcase.DisableSuitcase();
                Debugging.Log($"Suitcase: OnLoadVillageScene -> unsubscribe",ColorType.Blue);
                GlobalEvent.LoadVillageSceneEvent -= OnLoadVillageScene;
            }
        }
        
        #endregion
        
        #region Animation Events

        private void MoveGuideToTrainStation()
        {
            var guide = _characterStorage.GetCharacter(CharacterType.Guide);
            guide.Move.BlockMove();
            guide.TeleportToPosition(_characterPointInTrainStation.position, isUp: false,isLeft: false);
        }
        private void SetTrainIdleAnimation()
        {
            _trainAnimation.SetIdleAnimation();
        }

        private void PlayTrainAudioEvent()
        {
            AudioManager.Instance.PlayAudioEvent(AudioEventType.Train);
        }
        private void InvokeEndAnimation()
        {
            EndAnimationEvent?.Invoke();
            StopAnimation();
        }
        
        private void SwitchCameraToMain()
        {
            _mainCamera.transform.position = _cutSceneCamera.transform.position;
            _mainCamera.orthographicSize = _cutSceneCamera.orthographicSize;
            _mainCamera.gameObject.SetActive(true);
            _cutSceneCamera.gameObject.SetActive(false);
        }
        
        #endregion
    }
}