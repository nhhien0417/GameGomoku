using TMPro;
using UnityEngine;

public class DifficultControl : Singleton<DifficultControl>
{
    [SerializeField] UnityEngine.UI.Image _emotion;
    [SerializeField] TextMeshProUGUI _caption;
    [SerializeField] Sprite _easyEmotion, _mediumEmotion, _hardEmotion;

    private Color32 _easyColor = new(0, 255, 255, 255);
    private Color32 _mediumColor = new(255, 255, 0, 255);
    private Color32 _hardColor = new(235, 28, 36, 255);

    public string Difficult;
    public int TargetDepth;

    public void EasyDifficult()
    {
        _emotion.sprite = _easyEmotion;
        _caption.text = "Easy";
        _caption.color = _easyColor;
    }

    public void MediumDifficult()
    {
        _emotion.sprite = _mediumEmotion;
        _caption.text = "Medium";
        _caption.color = _mediumColor;
    }

    public void HardDifficult()
    {
        _emotion.sprite = _hardEmotion;
        _caption.text = "Hard";
        _caption.color = _hardColor;
    }

    public void ConfirmAndPlay()
    {
        Difficult = _caption.text;

        if (GameModeControl.Instance.GameVariation == 5)
        {
            TargetDepth = Difficult switch
            {
                "Easy" => 1,
                "Medium" => 2,
                "Hard" => 3,
                _ => 1
            };
        }
        else if (GameModeControl.Instance.GameVariation == 4)
        {
            TargetDepth = Difficult switch
            {
                "Easy" => 2,
                "Medium" => 3,
                "Hard" => 4,
                _ => 2
            };
        }
        if (GameModeControl.Instance.GameVariation == 3)
        {
            TargetDepth = Difficult switch
            {
                "Easy" => 3,
                "Medium" => 4,
                "Hard" => 5,
                _ => 3
            };
        }


        Board.Instance.DestroyAll();
        Board.Instance.Initialized();

        GameManager.Instance.ResetPlayer();
        GameManager.Instance.NewGame();
        UIManager.Instance.DisableAll();
    }

    public void Close()
    {
        UIManager.Instance.DifficultScreenActive(false);
    }
}
