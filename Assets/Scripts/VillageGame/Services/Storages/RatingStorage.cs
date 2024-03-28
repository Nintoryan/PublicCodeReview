using System;
using System.Threading.Tasks;
using Data.Scripts.Audio;
using SO;
using VContainer;
using VillageGame.Services.LoadingData;
using Web.RequestStructs;

namespace VillageGame.Services.Storages
{
    public class RatingStorage : ILoading
    {
        private readonly ProgressionConfig _progressionConfig;
        public int RatingRequirement { get; private set; }
        public int Raiting { get; private set; }
        public int Level { get; private set; } = 1;

        public Action<int, int> ChangeRaiting;
        public Action<int> ChangeLevel;

        public Action<int> SetLevel;

        [Inject]
        private RatingStorage(IObjectResolver objectResolver)
        {
            _progressionConfig = objectResolver.Resolve<ProgressionConfig>();
        }

        public void LevelUp()
        {
            if (Level < _progressionConfig.playerLevels.Length)
            {
                Level++;
                ChangeLevel?.Invoke(Level);

                var residualRating = Raiting - RatingRequirement;
                Raiting = residualRating > 0 ? residualRating : 0;
                SetRatingRequirement();
                ChangeRaiting?.Invoke(Raiting, RatingRequirement);
                AudioManager.Instance.PlayAudioEvent(AudioEventType.LevelUp);
            }
        }


        public void AddRating(int value)
        {
            Raiting += value;
            //Temp disabling
            //AudioManager.Instance?.PlayAudioEvent(AudioEventType.AddRating);
            if (Raiting >= RatingRequirement)
            {
                LevelUp();
            }
            else
            {
                ChangeRaiting?.Invoke(Raiting, RatingRequirement);
            }
        }


        private void SetRatingRequirement()
        {
            if (Level == 0)
            {
                Level = 1;
            }

            RatingRequirement = Level >= _progressionConfig.playerLevels.Length 
                ? _progressionConfig.playerLevels[^1].RatingRequirement 
                : _progressionConfig.playerLevels[Level].RatingRequirement;
        }

        public void Load(LoadData request)
        {
            if (request.data.player_progress == null) return;
            Level = request.data.player_progress.level;
            Raiting = request.data.player_progress.rating;
            RatingRequirement = _progressionConfig.playerLevels[Level - 1].RatingRequirement;
            SetRatingRequirement();
        }
    }
}