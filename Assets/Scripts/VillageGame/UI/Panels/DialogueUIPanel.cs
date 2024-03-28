using System;
using SO.Data;
using SO.Data.Characters;
using UnityEngine;
using Util;
using VillageGame.UI.Elements;

namespace VillageGame.UI.Panels
{
    public class DialogueUIPanel : UIPanel
    {
        [SerializeField] private DialoguePortrait _portraitRight;
        [SerializeField] private DialoguePortrait _portraitLeft;
        [Space] [SerializeField] private TextPanel _nameText;
        [SerializeField] private TextTypewriter _mainText;
        public TextTypewriter MainText => _mainText;
        private DialoguePortrait _currentPortrait;

        private void Awake()
        {
      
        }

        private void HideAllComponent()
        {
            _portraitLeft.Hide(withAnimation: false);
            _portraitRight.Hide(withAnimation: false);
            _nameText.Hide(withAnimation: false);
            _mainText.Reset();
            _currentPortrait = null;
        }

  
        public override void Hide(Action onHidden = null)
        {
            base.Hide(onHidden);
            HideAllComponent();
        }

        public override void HideNoAction()
        {
            base.HideNoAction();
            HideAllComponent();
        }

        public override void HideNoAnimation()
        {
            base.HideNoAnimation();
            HideAllComponent();
        }

        public void SetCharacter(CharacterType type,
            CharacterPresentationModel presentationModel,
            PortraitReaction portraitReaction,
            bool isLeft)
        {
            if (_currentPortrait == null)
            {
                _portraitLeft.Hide(withAnimation: false);
                _portraitRight.Hide(withAnimation: false);
                SetFirstCharacter(type, presentationModel, portraitReaction, isLeft);
            }
            else
            {
                SetNextCharacter(type, presentationModel, portraitReaction, isLeft);
            }
        }

        private void SetFirstCharacter(CharacterType type,
            CharacterPresentationModel presentationModel,
            PortraitReaction portraitReaction,
            bool isLeft)
        {
            SetCurrentPortrait(isLeft);
            _currentPortrait.SetCharacterType(type);
            _currentPortrait.SetPortrait(presentationModel.PortraitBody,
                presentationModel.GetPortraitEmotion(portraitReaction));
            _currentPortrait.Show();

            _nameText.SetText(presentationModel.Name);
            _nameText.SetTextColor(presentationModel.Color);
            _nameText.Show(withAnimation: true);
        }


        private void SetNextCharacter(CharacterType type,
            CharacterPresentationModel presentationModel,
            PortraitReaction portraitReaction,
            bool isLeft)
        {
            if (_currentPortrait.CurrentCharacter != type || _currentPortrait.IsLeft != isLeft)
            {
                _currentPortrait.Hide(withAnimation: true, OnHidden: () =>
                {
                    SetCurrentPortrait(isLeft);
                    _currentPortrait.SetCharacterType(type);
                    if (presentationModel == null)
                    {
                        Debugging.Log($"{type} не имеет презенташион модел в конфиге", ColorType.Blue);
                    }
                    else
                    {
                        _currentPortrait.SetPortrait(presentationModel.PortraitBody,
                            presentationModel.GetPortraitEmotion(portraitReaction));
                        _nameText.ShowAfterHide(onHidden: () =>
                        {
                            _nameText.SetText(presentationModel.Name);
                            _nameText.SetTextColor(presentationModel.Color);
                        });
                        _currentPortrait.Show();
                    }

                });
            }
            else
            {
                if (presentationModel == null)
                {
                    Debugging.Log($"{type} не имеет презенташион модел в конфиге", ColorType.Blue);
                }
                else
                {
                    _currentPortrait.SetPortrait(presentationModel.PortraitBody,
                        presentationModel.GetPortraitEmotion(portraitReaction));
                }
            }
        }


        private void SetCurrentPortrait(bool isLeft)
        {
            _currentPortrait = isLeft ? _portraitLeft : _portraitRight;
        }
    }
}