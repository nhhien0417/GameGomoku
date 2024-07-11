using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwipeControl : MonoBehaviour, IEndDragHandler
{
    [SerializeField] int _maxVariants;
    [SerializeField] float _dragThreshould;
    [SerializeField] Vector3 _pageStep;
    [SerializeField] RectTransform _levelPageRect;
    [SerializeField] Image[] _barImage;
    [SerializeField] Sprite _barClose, _barOpen;
    [SerializeField] Button _previousButton, _nextButton;

    public static int CurrentVariants;
    
    private Vector3 _targetPos;

    private void Awake()
    {
        GameModeControl.Instance.GameVariation = 3;
        CurrentVariants = 1;
        _targetPos = _levelPageRect.localPosition;

        UpdateBar();
    }

    public void Next()
    {
        if (CurrentVariants < _maxVariants)
        {
            CurrentVariants++;
            _targetPos += _pageStep;

            MovePage();
            UpdateArrow();
        }
    }

    public void Previous()
    {
        if (CurrentVariants > 1)
        {
            CurrentVariants--;
            _targetPos -= _pageStep;

            MovePage();
        }
    }

    private void MovePage()
    {
        _levelPageRect.DOLocalMove(_targetPos, 0.5f);
        AudioManager.Instance.SwipeSFX();

        UpdateBar();
        UpdateArrow();

        GameModeControl.Instance.GameVariation = CurrentVariants + 2;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(eventData.position.x - eventData.pressPosition.x) > _dragThreshould)
        {
            if (eventData.position.x > eventData.pressPosition.x)
            {
                Previous();
            }
            else
            {
                Next();
            }
        }
        else
        {
            MovePage();
        }
    }

    private void UpdateBar()
    {
        foreach (var item in _barImage)
        {
            item.sprite = _barClose;
        }

        _barImage[CurrentVariants - 1].sprite = _barOpen;
    }

    private void UpdateArrow()
    {
        _nextButton.interactable = true;
        _previousButton.interactable = true;

        if (CurrentVariants == 1)
        {
            _previousButton.interactable = false;
        }
        else if (CurrentVariants == _maxVariants)
        {
            _nextButton.interactable = false;
        }
    }
}
