using System;
using System.Collections.Generic;
using UnityEngine;

namespace SO
{
    [CreateAssetMenu(fileName = "NewYearProjectsDatabase", menuName = "SO/NewYearProjectsDatabase")]
    public class NewYearProjectsDatabase : ScriptableObject
    {
        public List<NewYearProjectConfig> NewYearProjects;
    }
}
