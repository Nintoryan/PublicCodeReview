using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Services
{
    public class ButtonPressObserver: MonoBehaviour,IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
    {
        private bool isButtonDown;
        private float buttonDownTime;
        
        public Action OnDown;
        public Action<float> OnUp;
        public Action OnEnter;

        public bool IsInteractable { get; private set; } = true;

        public void SetInteractable(bool isInteractable)
        {
            IsInteractable = isInteractable;
            if (!isInteractable)
            {
                isButtonDown = false;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsInteractable)
            {
                return;
            }
            isButtonDown = true;
            buttonDownTime = Time.time;
            OnDown?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!IsInteractable)
            {
                return;
            }
            
            if (isButtonDown)
            {
                float buttonUpTime = Time.time;
                float buttonHoldTime = buttonUpTime - buttonDownTime;
                OnUp?.Invoke(buttonHoldTime);
                //Debug.Log("Button hold time: " + buttonHoldTime + " seconds");
            }

            isButtonDown = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsInteractable)
            {
                return;
            }
            OnEnter?.Invoke();
        }
    }
}