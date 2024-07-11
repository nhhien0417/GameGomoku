using UnityEngine;
using DG.Tweening;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] GameObject _gameOver;
    [SerializeField] GameObject _settings;
    [SerializeField] GameObject _mainMenu;
    [SerializeField] GameObject _menuNewGame;
    [SerializeField] GameObject _chooseDifficult;
    [SerializeField] GameObject _background;
    [SerializeField] GameObject _loading;
    [SerializeField] GameObject _notify;
    [SerializeField] GameObject _title;
    [SerializeField] GameObject _dot1, _dot2, _dot3;

    public void GameOverScreenActive(bool status)
    {
        _gameOver.SetActive(status);
        GameManager.Instance.IsGameOver = false;
    }

    public void SettingsScreenActive(bool status)
    {
        _settings.SetActive(status);
    }

    public void StartScreenActive(bool status)
    {
        _mainMenu.SetActive(status);

        if (status)
        {
            _title.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.8f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            _title.transform.DOKill();
            _title.transform.localScale = Vector3.one;
        }
    }

    public void NewGameScreenActive(bool status)
    {
        _menuNewGame.SetActive(status);
    }

    public void DifficultScreenActive(bool status)
    {
        _chooseDifficult.SetActive(status);
    }

    public void BackgroundActive(bool status)
    {
        _background.SetActive(status);
    }

    public void NotifyScreenActive(bool status)
    {
        _notify.SetActive(status);
    }

    public void LoadingScreenActive(bool status)
    {
        _loading.SetActive(status);

        if (status)
        {
            StartCoroutine(AnimateDot());
        }
        else
        {
            StopCoroutine(AnimateDot());

            _dot1.transform.DOKill();
            _dot2.transform.DOKill();
            _dot3.transform.DOKill();

            _dot1.transform.localScale = Vector3.one;
            _dot2.transform.localScale = Vector3.one;
            _dot3.transform.localScale = Vector3.one;
        }
    }

    IEnumerator AnimateDot()
    {
        _dot1.transform.DOScale(2f, 1.0f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        yield return new WaitForSeconds(0.25f);
        _dot2.transform.DOScale(2f, 1.0f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        yield return new WaitForSeconds(0.25f);
        _dot3.transform.DOScale(2f, 1.0f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    public void DisableAll()
    {
        GameOverScreenActive(false);
        SettingsScreenActive(false);
        StartScreenActive(false);
        NewGameScreenActive(false);
        DifficultScreenActive(false);
        BackgroundActive(false);
        LoadingScreenActive(false);
    }
}
