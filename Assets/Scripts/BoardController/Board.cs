using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Threading.Tasks;

public class Board : Singleton<Board>
{

    [SerializeField] RenderTexture _boardTexture;
    [SerializeField] Canvas _boardCanvas;
    [SerializeField] Transform _board;
    [SerializeField] GridLayoutGroup _gridLayoutGroup;
    [SerializeField] GameObject _cellPrefab, _btnNewGame;
    [SerializeField] TextMeshProUGUI _xPlayer, _oPlayer, _difficult;
    [SerializeField] Image _xIcon, _oIcon, _icon4, _icon5, _line4, _line5;
    [SerializeField] Camera _boardStateCamera;
    [SerializeField] Camera _mainCamera;

    public int Size, ValidMoves;
    public string[,] State;
    public Cell[,] Cells;
    public RectTransform BoardTransform;

    private int _winCondition;

    private void Start()
    {
        Initialized();
    }

    public void Initialized()
    {
        AdjustBoardUI();
        ChangeVariant(GameModeControl.Instance.GameVariation);

        State = new string[Size, Size];
        Cells = new Cell[Size, Size];

        _gridLayoutGroup.constraintCount = Size;
        _gridLayoutGroup.cellSize = new Vector2(890 / Size, 890 / Size);

        ValidMoves = Size * Size;

        CreateBoard();
    }

    public void DestroyAll()
    {
        foreach (var cell in Cells)
        {
            Destroy(cell.gameObject);
        }
    }

    private void CreateBoard()
    {
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                GameObject celltransform = Instantiate(_cellPrefab, _board);
                Cells[i, j] = celltransform.GetComponent<Cell>();

                Cells[i, j].Column = j;
                Cells[i, j].Row = i;
                State[i, j] = "";
            }
        }
    }

    public void ClearBoard()
    {
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                Cells[i, j].Reset();

                State[i, j] = "";
            }
        }

        ValidMoves = Size * Size;
    }

    public string CheckWin(int row, int column, string CurrentTurn)
    {
        //Check ngang
        GameOverControl.Instance.StartWinLine = Cells[row, column].transform.localPosition;
        GameOverControl.Instance.EndWinLine = Cells[row, column].transform.localPosition;
        int count = 0;

        if (row > 0)
        {
            for (int i = row - 1; i >= 0; i--)
            {
                if (State[i, column] == CurrentTurn)
                {
                    count++;
                    GameOverControl.Instance.StartWinLine = Cells[i, column].transform.localPosition;
                }
                else
                {
                    break;
                }
            }
        }

        if (row < Size - 1)
        {
            for (int i = row + 1; i < Size; i++)
            {
                if (State[i, column] == CurrentTurn)
                {
                    count++;
                    GameOverControl.Instance.EndWinLine = Cells[i, column].transform.localPosition;
                }
                else
                {
                    break;
                }
            }

        }

        if (count + 1 >= _winCondition)
        {
            return CurrentTurn;
        }

        //Check doc
        GameOverControl.Instance.StartWinLine = Cells[row, column].transform.localPosition;
        GameOverControl.Instance.EndWinLine = Cells[row, column].transform.localPosition;
        count = 0;

        if (column > 0)
        {
            for (int i = column - 1; i >= 0; i--)
            {
                if (State[row, i] == CurrentTurn)
                {
                    count++;
                    GameOverControl.Instance.StartWinLine = Cells[row, i].transform.localPosition;
                }
                else
                {
                    break;
                }
            }
        }

        if (column < Size - 1)
        {
            for (int i = column + 1; i < Size; i++)
            {
                if (State[row, i] == CurrentTurn)
                {
                    count++;
                    GameOverControl.Instance.EndWinLine = Cells[row, i].transform.localPosition;
                }
                else
                {
                    break;
                }
            }
        }

        if (count + 1 >= _winCondition)
        {
            return CurrentTurn;
        }

        //Check cheo huyen
        GameOverControl.Instance.StartWinLine = Cells[row, column].transform.localPosition;
        GameOverControl.Instance.EndWinLine = Cells[row, column].transform.localPosition;
        count = 0;

        for (int i = column - 1; i >= 0; i--)
        {
            if (i >= 0 && row + i - column >= 0)
            {
                if (State[row + i - column, i] == CurrentTurn)
                {
                    count++;
                    GameOverControl.Instance.StartWinLine = Cells[row + i - column, i].transform.localPosition;
                }
                else
                {
                    break;
                }
            }
        }

        for (int i = column + 1; i < Size; i++)
        {
            if (i <= Size - 1 && row + i - column <= Size - 1)
            {
                if (State[row + i - column, i] == CurrentTurn)
                {
                    count++;
                    GameOverControl.Instance.EndWinLine = Cells[row + i - column, i].transform.localPosition;
                }
                else
                {
                    break;
                }
            }
        }

        if (count + 1 >= _winCondition)
        {
            return CurrentTurn;
        }

        //Check cheo sac
        GameOverControl.Instance.StartWinLine = Cells[row, column].transform.localPosition;
        GameOverControl.Instance.EndWinLine = Cells[row, column].transform.localPosition;
        count = 0;

        for (int i = column - 1; i >= 0; i--)
        {
            if (i >= 0 && row + column - i <= Size - 1)
            {
                if (State[row + column - i, i] == CurrentTurn)
                {
                    count++;
                    GameOverControl.Instance.StartWinLine = Cells[row + column - i, i].transform.localPosition;
                }
                else
                {
                    break;
                }
            }
        }

        for (int i = column + 1; i < Size; i++)
        {
            if (i <= Size - 1 && row + column - i >= 0)
            {
                if (State[row + column - i, i] == CurrentTurn)
                {
                    count++;
                    GameOverControl.Instance.EndWinLine = Cells[row + column - i, i].transform.localPosition;
                }
                else
                {
                    break;
                }
            }
        }

        if (count + 1 >= _winCondition)
        {
            return CurrentTurn;
        }

        //Continue
        GameOverControl.Instance.StartWinLine = Cells[row, column].transform.localPosition;
        GameOverControl.Instance.EndWinLine = Cells[row, column].transform.localPosition;
        return "";
    }

    public void NewGameButton()
    {
        if (!GameManager.Instance.IsGameOver)
        {
            GameManager.Instance.NewGame();
            AudioManager.Instance.TapSFX();
        }
    }

    public void UndoButton()
    {
        if (!GameManager.Instance.IsGameOver)
        {
            GameManager.Instance.UndoMove();
            AudioManager.Instance.TapSFX();
        }
    }

    public void SettingsButton()
    {
        if (!GameManager.Instance.IsGameOver)
        {
            GameManager.Instance.OpenSettings();
            AudioManager.Instance.TapSFX();
        }
    }

    public void HintButton()
    {
        if (!GameManager.Instance.IsGameOver)
        {
            AudioManager.Instance.TapSFX();
        }
    }

    public void BackButton()
    {
        if (!GameManager.Instance.IsGameOver)
        {
            GameManager.Instance.BackToNewGame();
            AudioManager.Instance.TapSFX();
        }

        if (GameModeControl.Instance.GameMode == "Online")
        {
            ClientControl.Instance.DisconnectToSever();
        }
    }

    public void ChangeVariant(int winCondition)
    {
        _winCondition = winCondition;

        if (winCondition == 3)
        {
            Size = 3;
        }
        else if (winCondition == 4)
        {
            Size = 10;
        }
        else if (winCondition == 5)
        {
            Size = 15;
        }
    }

    public void AdjustBoardUI()
    {
        if (GameModeControl.Instance.GameMode == "PvP")
        {
            _oPlayer.text = "PLAYER 2";
            _xPlayer.text = "PLAYER 1";
            _difficult.text = string.Empty;

            _btnNewGame.SetActive(true);
        }
        else if (GameModeControl.Instance.GameMode == "PvC")
        {
            _oPlayer.text = "AI";
            _xPlayer.text = "YOU";
            _difficult.text = $"Difficult: {DifficultControl.Instance.Difficult}";

            _btnNewGame.SetActive(true);
        }
        else if (GameModeControl.Instance.GameMode == "Online")
        {
            _oPlayer.text = ClientControl.Instance.Symbol.Equals("o") ? "YOU" : "OPPONENT";
            _xPlayer.text = ClientControl.Instance.Symbol.Equals("x") ? "YOU" : "OPPONENT";
            _difficult.text = string.Empty;

            _btnNewGame.SetActive(false);
        }

        if (GameModeControl.Instance.GameVariation == 3)
        {
            _icon4.gameObject.SetActive(false);
            _line4.gameObject.SetActive(false);
            _icon5.gameObject.SetActive(false);
            _line5.gameObject.SetActive(false);
        }
        else if (GameModeControl.Instance.GameVariation == 4)
        {
            _icon4.gameObject.SetActive(true);
            _line4.gameObject.SetActive(true);
            _icon5.gameObject.SetActive(false);
            _line5.gameObject.SetActive(false);
        }
        else if (GameModeControl.Instance.GameVariation == 5)
        {
            _icon4.gameObject.SetActive(true);
            _line4.gameObject.SetActive(true);
            _icon5.gameObject.SetActive(true);
            _line5.gameObject.SetActive(true);
        }
    }

    public void ScaleIcon()
    {
        if (GameManager.Instance.CurrentTurn == "x")
        {
            _oIcon.color = new Color32(255, 255, 255, 100);
            _xIcon.color = new Color32(255, 255, 255, 255);

            _oIcon.transform.DOKill();
            _oIcon.transform.localScale = Vector3.one;
            _xIcon.transform.DOScale(1.5f, 1.0f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            _xIcon.color = new Color32(255, 255, 255, 100);
            _oIcon.color = new Color32(255, 255, 255, 255);

            _xIcon.transform.DOKill();
            _xIcon.transform.localScale = Vector3.one;
            _oIcon.transform.DOScale(1.5f, 1.0f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }
    }

    public void ResetScaleIcon()
    {
        _xIcon.transform.DOKill();
        _oIcon.transform.DOKill();
        _xIcon.transform.localScale = Vector3.one;
        _oIcon.transform.localScale = Vector3.one;

        ScaleIcon();
    }

    public async void ShareBoardState(Rect rect)
    {
        _boardCanvas.worldCamera = _boardStateCamera;
        StartCoroutine(SocialShare.Instance.ShareRenderTextureScreenshot(_boardTexture, rect));
        await Task.Delay(100);
        _boardCanvas.worldCamera = _mainCamera;
    }
}
