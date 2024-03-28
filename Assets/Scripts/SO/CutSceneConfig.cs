using System;
using System.Linq;
using SO.Data;
using UnityEngine;
using VillageGame.Data;
using VillageGame.Data.Types;

namespace SO
{
    [CreateAssetMenu(fileName = "CutSceneConfig", menuName = "SO/CutSceneConfig")]
    public class CutSceneConfig : ScriptableObject
    {
        public bool IsShowCutscene = true;
        public CutSceneData[] CutScenes;

        public bool TryGetCutScenePresentBox(DayData openDay, out PresentBoxData presentBox)
        {
            var cutScene = CutScenes.FirstOrDefault(s => s.PresentBox != null && s.PresentBox.OpenDate.Equals(openDay));
            presentBox = cutScene?.PresentBox;
            return presentBox != null;
        }

        public int[] GetCoreGameLevelsForCutScene()
        {
            var winCoreGameCutScenes = CutScenes.Where(c => c.Condition.Type == ConditionType.CoreGameWinLevel);
            var levels = winCoreGameCutScenes.Select(c => c.Condition.Value);
            return levels.ToArray();
        }

        public bool TryGetCutSceneData(ConditionData condition, int lastWatchedId, out CutSceneData cutSceneData)
        {
            if (condition.Type == ConditionType.ClearSnowdrift && condition.Value == 12)
            {
                return cutSceneData = CutScenes
                    .FirstOrDefault(c => c.Condition.Equals(condition));
            }

            cutSceneData = CutScenes
                .FirstOrDefault(c => c.ID > lastWatchedId
                                     && ((c.Condition.Value == -1 && c.Condition.Type == condition.Type) ||
                                         c.Condition.Equals(condition)));
            return cutSceneData != null;
        }

        public CutSceneData GetCutSceneData(int id)
        {
            return CutScenes[id];
        }

        private void OnValidate()
        {
            for (var index = 0; index < CutScenes.Length; index++)
            {
                var cutSceneData = CutScenes[index];
                cutSceneData.ID = index;
            }
        }
    }
}