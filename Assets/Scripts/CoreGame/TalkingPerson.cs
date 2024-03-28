using CoreGame.SO;
using TMPro;
using UnityEngine;

namespace CoreGame
{
    public class TalkingPerson : MonoBehaviour
    {
        [SerializeField] private Transform _body_0;
        [SerializeField] private Transform _body_1;

        [SerializeField] private Transform _smallPersonBubble;
        [SerializeField] private TextMeshProUGUI _smallPersonBubbleMessageText;

        [SerializeField] private Transform _mediumPersonBubble;
        [SerializeField] private TextMeshProUGUI _mediumPersonBubbleMessageText;

        [SerializeField] private Transform _largePersonBubble;
        [SerializeField] private TextMeshProUGUI _largePersonBubbleMessageText;

        [SerializeField] private Transform _body_2;
        [SerializeField] private Transform _fullscreenPersonBubble;
        [SerializeField] private TextMeshProUGUI _fullscreenPersonBubbleMessageText;

        [SerializeField] private Transform _leftButtonBubble;
        [SerializeField] private TextMeshProUGUI _leftButtonBubbleMessageText;

        [SerializeField] private Transform _rightButtonBubble;
        [SerializeField] private TextMeshProUGUI _rightButtonBubbleMessageText;

        public void ActivatePerson(HelpState state, BubbleSize bubbleSize, string message)
        {
            switch (state)
            {
                case HelpState.LeftButton:
                    _body_0.gameObject.SetActive(false);
                    _body_1.gameObject.SetActive(false);
                    _body_2.gameObject.SetActive(false);
                    //_smallPersonBubble.gameObject.SetActive(false);
                    SetBubble(BubbleSize.None, null);
                    _leftButtonBubble.gameObject.SetActive(true);
                    _rightButtonBubble.gameObject.SetActive(false);
                    _fullscreenPersonBubble.gameObject.SetActive(false);
                    _leftButtonBubbleMessageText.text = message;
                    break;

                case HelpState.RightButton:
                    _body_0.gameObject.SetActive(false);
                    _body_1.gameObject.SetActive(false);
                    _body_2.gameObject.SetActive(false);
                    //_smallPersonBubble.gameObject.SetActive(false);
                    SetBubble(BubbleSize.None, null);
                    _leftButtonBubble.gameObject.SetActive(false);
                    _rightButtonBubble.gameObject.SetActive(true);
                    _fullscreenPersonBubble.gameObject.SetActive(false);
                    _rightButtonBubbleMessageText.text = message;
                    break;

                case HelpState.FirstPerson:
                    _body_0.gameObject.SetActive(true);
                    _body_1.gameObject.SetActive(false);
                    _body_2.gameObject.SetActive(false);
                    //_smallPersonBubble.gameObject.SetActive(true);
                    SetBubble(bubbleSize, message);
                    _leftButtonBubble.gameObject.SetActive(false);
                    _rightButtonBubble.gameObject.SetActive(false);
                    _fullscreenPersonBubble.gameObject.SetActive(false);
                    //_smallPersonBubbleMessageText.text = message;
                    break;

                case HelpState.SecondPerson:
                    _body_0.gameObject.SetActive(false);
                    _body_1.gameObject.SetActive(true);
                    _body_2.gameObject.SetActive(false);
                    //_smallPersonBubble.gameObject.SetActive(true);
                    SetBubble(bubbleSize, message);
                    _leftButtonBubble.gameObject.SetActive(false);
                    _rightButtonBubble.gameObject.SetActive(false);
                    _fullscreenPersonBubble.gameObject.SetActive(false);
                    //_smallPersonBubbleMessageText.text = message;
                    break;

                case HelpState.FullScreenPerson:
                    _body_0.gameObject.SetActive(false);
                    _body_1.gameObject.SetActive(false);
                    _body_2.gameObject.SetActive(true);
                    //_smallPersonBubble.gameObject.SetActive(false);
                    SetBubble(BubbleSize.None, null);
                    _leftButtonBubble.gameObject.SetActive(false);
                    _rightButtonBubble.gameObject.SetActive(false);
                    _fullscreenPersonBubble.gameObject.SetActive(true);
                    _fullscreenPersonBubbleMessageText.text = message;
                    break;

                default:
                    Debug.LogWarning("Incorrect type");
                    break;
            }
        }

        private void SetBubble(BubbleSize bubbleSize, string message)
        {
            switch (bubbleSize)
            {
                case BubbleSize.None:
                    _smallPersonBubble.gameObject.SetActive(false);
                    _mediumPersonBubble.gameObject.SetActive(false);
                    _largePersonBubble.gameObject.SetActive(false);
                    break;

                case BubbleSize.Small:
                    _smallPersonBubble.gameObject.SetActive(true);
                    _mediumPersonBubble.gameObject.SetActive(false);
                    _largePersonBubble.gameObject.SetActive(false);

                    _smallPersonBubbleMessageText.text = message;
                    break;

                case BubbleSize.Medium:
                    _smallPersonBubble.gameObject.SetActive(false);
                    _mediumPersonBubble.gameObject.SetActive(true);
                    _largePersonBubble.gameObject.SetActive(false);

                    _mediumPersonBubbleMessageText.text = message;
                    break;

                case BubbleSize.Large:
                    _smallPersonBubble.gameObject.SetActive(false);
                    _mediumPersonBubble.gameObject.SetActive(false);
                    _largePersonBubble.gameObject.SetActive(true);

                    _largePersonBubbleMessageText.text = message;
                    break;
            }
        }

        public void DeactivatePerson()
        {
            _body_0.gameObject.SetActive(false);
            _body_1.gameObject.SetActive(false); 
            _body_2.gameObject.SetActive(false);
            //_smallPersonBubble.gameObject.SetActive(false);
            SetBubble(BubbleSize.None, null);
            _leftButtonBubble.gameObject.SetActive(false);
            _rightButtonBubble.gameObject.SetActive(false);
            _fullscreenPersonBubble.gameObject.SetActive(false);
        }
    }
}
