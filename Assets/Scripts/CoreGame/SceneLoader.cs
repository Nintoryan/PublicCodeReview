using SO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CoreGame
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private int _sceneIndex = 1;
        [SerializeField] private PlayerPrefsConfig _playerPrefsConfig;

        public void UnloadScene(bool isVictory)
        {
            if (isVictory)
            {
                PlayerPrefs.SetInt(_playerPrefsConfig.VictoryFlag, 1);
            }
            else
            {
                PlayerPrefs.SetInt(_playerPrefsConfig.VictoryFlag, 0);
            }
            PlayerPrefs.SetInt(_playerPrefsConfig.ReloadFlag, 0);
            SceneManager.UnloadSceneAsync(_sceneIndex);
        }

        public void ReloadScene(bool isVictory)
        {
            if (isVictory)
            {
                PlayerPrefs.SetInt(_playerPrefsConfig.VictoryFlag, 1);
            }
            else
            {
                PlayerPrefs.SetInt(_playerPrefsConfig.VictoryFlag, 0);
            }
            PlayerPrefs.SetInt(_playerPrefsConfig.ReloadFlag, 1);
            SceneManager.UnloadSceneAsync(_sceneIndex);
        }
    }
}
