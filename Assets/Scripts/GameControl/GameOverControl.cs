using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameOverControl : Singleton<GameOverControl>
{
    [SerializeField] Image _gameOverDialog, _gameOverEmotion;
    [SerializeField] TextMeshProUGUI _gameOverCaption;
    [SerializeField] Sprite _bolderWin, _bolderLose, _bolderDraw;
    [SerializeField] Sprite _emotionWin, _emotionLose, _emotionDraw;

    public Vector2 StartWinLine, EndWinLine;
    public Image WinLineImage;

    public void UpdateGameOverScreen()
    {
        if (GameManager.Instance.Result == "Win")
        {
            _gameOverDialog.sprite = _bolderWin;
            _gameOverEmotion.sprite = _emotionWin;
            _gameOverCaption.text = "YOU WIN";
        }
        else if (GameManager.Instance.Result == "Lose")
        {
            _gameOverDialog.sprite = _bolderLose;
            _gameOverEmotion.sprite = _emotionLose;
            _gameOverCaption.text = "YOU LOSE";
        }
        else if (GameManager.Instance.Result == "Draw")
        {
            _gameOverDialog.sprite = _bolderDraw;
            _gameOverEmotion.sprite = _emotionDraw;
            _gameOverCaption.text = "DRAW";
        }
        else if (GameManager.Instance.Result == "PvP")
        {
            _gameOverDialog.sprite = _bolderWin;
            _gameOverEmotion.sprite = _emotionWin;
            _gameOverCaption.fontStyle = FontStyles.UpperCase;
            _gameOverCaption.fontSize = 100;
            _gameOverCaption.text = $"PLAYER {GameManager.Instance.CurrentTurn} WIN";
        }

        DrawWinLine(StartWinLine, EndWinLine);
    }

    public void DrawWinLine(Vector2 startPos, Vector2 endPos)
    {
        WinLineImage.gameObject.SetActive(true);
        RectTransform rectTransform = WinLineImage.rectTransform;

        rectTransform.anchoredPosition = startPos;

        Vector2 direction = endPos - startPos;
        float distance = direction.magnitude;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        rectTransform.rotation = Quaternion.Euler(0, 0, angle);
        rectTransform.pivot = new Vector2(0, 0.5f);

        if (GameModeControl.Instance.GameVariation == 5)
        {
            WinLineImage.pixelsPerUnitMultiplier = 5;
            rectTransform.sizeDelta = new Vector2(0, 10);

            rectTransform.DOSizeDelta(new Vector2(distance, 10), 1.5f).SetEase(Ease.OutQuad);
        }
        else if (GameModeControl.Instance.GameVariation == 4)
        {
            WinLineImage.pixelsPerUnitMultiplier = 2.5f;
            rectTransform.sizeDelta = new Vector2(0, 20);

            rectTransform.DOSizeDelta(new Vector2(distance, 20), 1.5f).SetEase(Ease.OutQuad);
        }
        else if (GameModeControl.Instance.GameVariation == 3)
        {
            WinLineImage.pixelsPerUnitMultiplier = 1;
            rectTransform.sizeDelta = new Vector2(0, 50);

            rectTransform.DOSizeDelta(new Vector2(distance, 50), 1.5f).SetEase(Ease.OutQuad);
        }
    }
}
