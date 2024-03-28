using TMPro;
using UnityEngine;

namespace VillageGame.UI.Panels.PresentInformation
{
    public class LetterUI : MiniPanel
    {
        [SerializeField] private TMP_Text _letterGreetings;
        [SerializeField] private TMP_Text _letterText;
        [SerializeField] private TMP_Text _letterSign;
        
        
        public void SetLetter(string greetings,string text, string sign)
        {
            _letterGreetings.text = greetings;
            _letterText.text = text;
            _letterSign.text = sign;
        }
    }
}