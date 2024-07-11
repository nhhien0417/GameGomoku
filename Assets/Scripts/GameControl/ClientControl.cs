using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientControl : Singleton<ClientControl>
{
    public string Symbol;

    public void Move(int Row, int Column)
    {
        if (Board.Instance.State[Row, Column] == "" && !GameManager.Instance.IsGameOver)
        {
            Board.Instance.State[Row, Column] = GameManager.Instance.CurrentTurn;

            AudioManager.Instance.ClickSFX();
            Board.Instance.Cells[Row, Column].ChangeSprite(GameManager.Instance.CurrentTurn);

            Board.Instance.ValidMoves--;

            if (GameModeControl.Instance.GameMode == "Online")
            {
                if (Board.Instance.CheckWin(Row, Column, GameManager.Instance.CurrentTurn) == "x" ||
                    Board.Instance.CheckWin(Row, Column, GameManager.Instance.CurrentTurn) == "o")
                {
                    GameManager.Instance.Result = "Lose";
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

    public void DisconnectToSever()
    {
        if (GameModeControl.Instance.GameMode == "Online")
        {
            NetworkManager.Instance.SendMessageToServer("[DISCONNECT]");
        }
    }

    public void Spawn(int row, int column)
    {
        if (GameModeControl.Instance.GameMode == "Online")
        {
            NetworkManager.Instance.SendMessageToServer($"[SPAWN ({row},{column})]");
        }
    }

    public void ReplayRequest()
    {
        if (GameModeControl.Instance.GameMode == "Online")
        {
            NetworkManager.Instance.SendMessageToServer("[REPLAY REQUEST]");
        }
    }

    public void ExitMatch()
    {
        if (GameModeControl.Instance.GameMode == "Online")
        {
            NetworkManager.Instance.SendMessageToServer("[EXIT]");
        }
    }

    public void CancelWaiting()
    {
        if (GameModeControl.Instance.GameMode == "Online")
        {
            NetworkManager.Instance.SendMessageToServer("[EXIT]");
            NetworkManager.Instance.DisconnectFromSever();
        }
    }

    public void CloseApp()
    {
        if (GameModeControl.Instance.GameMode == "Online")
        {
            NetworkManager.Instance.SendMessageToServer("[CLOSEAPP]");
        }
    }
}
