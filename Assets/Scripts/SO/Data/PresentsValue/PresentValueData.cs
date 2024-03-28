using UnityEngine;

namespace SO.Data.Presents
{
    public abstract class PresentValueData : ScriptableObject
    {
        [TextArea] public string Letter_Greetings;
        [TextArea(10,50)] public string Letter_Text;
        [TextArea] public string Letter_Sign;

        public Sprite Icon;

        public abstract bool Equivalent(PresentValueData other);
    }
}