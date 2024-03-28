using System;
using System.Collections.Generic;
using UnityEngine;
using VillageGame.Data;

namespace SO
{
    [CreateAssetMenu(fileName = "PresentBoxesConfig", menuName = "SO/PresentBoxesConfig", order = 0)]
    public class PresentBoxesConfig : ScriptableObject
    {
        public List<PresentBoxData> PresentsBoxesData;

        private void OnValidate()
        {
            for (int i = 0; i < PresentsBoxesData.Count; i++)
            {
                PresentsBoxesData[i].ID = i;
            }
        }
    }
}