using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    [SerializeField] Sprite _xSprite, _oSprite, _cellSprite;
    [SerializeField] Image _image;

    public int Row;
    public int Column;

    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
    }

    public void ChangeSprite(string target)
    {
        _image.sprite = target == "x" ? _xSprite : _oSprite;
    }

    public void OnClick()
    {
        if (Board.Instance.State[Row, Column] == ""
            && !GameManager.Instance.IsGameOver
            && GameManager.Instance.CurrentPlayer == "Human"
            && !(GameModeControl.Instance.GameMode == "Online" && GameManager.Instance.CurrentTurn != ClientControl.Instance.Symbol))
        {
            Board.Instance.State[Row, Column] = GameManager.Instance.CurrentTurn;

            Minimax.Instance.AdjustSpace(Row, Column, ref Minimax.Instance.TopEdge, ref Minimax.Instance.BottomEdge, ref Minimax.Instance.LeftEdge, ref Minimax.Instance.RightEdge);
            AudioManager.Instance.ClickSFX();
            ChangeSprite(GameManager.Instance.CurrentTurn);

            Board.Instance.ValidMoves--;

            if (GameModeControl.Instance.GameMode == "PvC")
            {
                if (Board.Instance.CheckWin(Row, Column, GameManager.Instance.CurrentTurn) == "x")
                {
                    GameManager.Instance.Result = "Win";
                    GameManager.Instance.GameOver();

                    return;
                }
                if (Board.Instance.ValidMoves == 0)
                {
                    GameManager.Instance.Result = "Draw";
                    GameManager.Instance.GameOver();

                    return;
                }
            }
            else if (GameModeControl.Instance.GameMode == "PvP")
            {
                if (Board.Instance.CheckWin(Row, Column, GameManager.Instance.CurrentTurn) == "x" ||
                    Board.Instance.CheckWin(Row, Column, GameManager.Instance.CurrentTurn) == "o")
                {
                    GameManager.Instance.Result = "PvP";
                    GameManager.Instance.GameOver();

                    return;
                }

                if (Board.Instance.ValidMoves == 0)
                {
                    GameManager.Instance.Result = "Draw";
                    GameManager.Instance.GameOver();

                    return;
                }
            }
            else if (GameModeControl.Instance.GameMode == "Online")
            {
                ClientControl.Instance.Spawn(Row, Column);

                if (Board.Instance.CheckWin(Row, Column, GameManager.Instance.CurrentTurn) == "x" ||
                    Board.Instance.CheckWin(Row, Column, GameManager.Instance.CurrentTurn) == "o")
                {
                    GameManager.Instance.Result = "Win";
                    GameManager.Instance.GameOver();

                    return;
                }

                if (Board.Instance.ValidMoves == 0)
                {
                    GameManager.Instance.Result = "Draw";
                    GameManager.Instance.GameOver();

                    return;
                }
            }

            GameManager.Instance.PlayerChange();
        }
    }

    public void Reset()
    {
        _image.sprite = _cellSprite;
    }
}
