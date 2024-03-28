using UnityEngine;
using Random = UnityEngine.Random;

namespace Data.Scripts.Audio
{
    [CreateAssetMenu(fileName = "AudioEventData_", menuName = "SO/Data/AudioEventData")]
    public class AudioEventData : ScriptableObject
    {
        public AudioEventType Type;
        public AudioClip[] Clips;

        public RangedFloat Volume = new RangedFloat()
        {
            MinValue = 1,
            MaxValue = 1,
        };

        [MinMaxRange(0, 2)] public RangedFloat Pitch = new RangedFloat()
        {
            MinValue = 1,
            MaxValue = 1
        };

        public void Play(AudioSource source)
        {
            if (Clips.Length == 0)
            {
                return;
            }

            var clip = Clips[Random.Range(0, Clips.Length)];
            var volume = Random.Range(Volume.MinValue, Volume.MaxValue);
            var pitch = Random.Range(Pitch.MinValue, Pitch.MaxValue);
            source.pitch = pitch;

            source.PlayOneShot(clip, volume);
        }
        
        public void Play(AudioSource source, int index)
        {
            if (Clips.Length == 0 || Clips.Length  <= index)
            {
                return;
            }

            var clip = Clips[index];
            var volume = Random.Range(Volume.MinValue, Volume.MaxValue);
            var pitch = Random.Range(Pitch.MinValue, Pitch.MaxValue);
            source.pitch = pitch;

            source.PlayOneShot(clip, volume);
        }
        
    }

    public enum AudioEventType
    {
        None,
        AddCurrency,
        AddRating,
        WinCoreGame,
        LoseCoreGame,
        TextType,
        CoreGameRowCollapse,
        CoreGameGrabObject,
        CoreGamePutObject,
        PressButton,
        HousePlacing,
        DecorPlacing,
        MenuOpen,
        ObjectGrab,
        ObjectMove,
        Tap,
        CloudsOpen,
        CloudsClose,
        Train,
        ShowStar,
        TextType_Space,
        CoreGameBird,
        CoreGameDefeat,
        CoreGameFlap,
        CoreGameShuffle,
        CoreGameTimeOut,
        PresentClick,
        PresentOpen,
        LevelUp,
        ClearSnowdrift,
        PineCone,
        TreeGrow,
        ClearBigSnowdrift,
        OpenSuitcase
    }
}