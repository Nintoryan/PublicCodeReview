using UnityEngine;

namespace VillageGame.Logic.Tutorial
{
    public class TutorialMansFacade : MonoBehaviour
    {
        [SerializeField] private TutorialMan[] _fedorTutorialParts;

        public T FindPanel<T>() where T : TutorialMan
        {
            foreach (var panel in _fedorTutorialParts)
            {
                if (panel is T uiPanel)
                {
                    return uiPanel;
                }
            }

            return null;
        }
    }
    
 
}