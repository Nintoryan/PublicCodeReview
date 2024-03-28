using TMPro;
using UnityEngine;

namespace VillageGame.UI.Panels
{
    public class DailyTasksPanel : UIPanel
    {
        [SerializeField] private Transform _tasksRoot;
        [SerializeField] private TextMeshProUGUI _timerText;
        public Transform ContentTransform => _tasksRoot;

        public void SetTimerValue(string value)
        {
            _timerText.SetText(value);
        }
    }
}