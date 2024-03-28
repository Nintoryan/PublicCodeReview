using CoreGame;
using UnityEngine;
using UnityEngine.EventSystems;

public class HungryPerson : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _body;
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _objectSpriteRenderer;
    [SerializeField] private GridController _gridController;
    [SerializeField] private GameObject _bubble;

    public void ActivatePerson()
    {
        _body.SetActive(true);
    }

    public void DeactivatePerson()
    {
        _body.SetActive(false);
    }

    public void SetRequiredObjectSprite(Sprite sprite)
    {
        _objectSpriteRenderer.sprite = sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _gridController.IsHoverOverPerson = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _gridController.IsHoverOverPerson = false;
    }

    public void SetEating()
    {
        _animator.SetTrigger("EatingTrigger");
    }

    public void SetDisappearing()
    {
        _bubble.SetActive(false);
        _animator.SetTrigger("DisappearingTrigger");
    }
}
