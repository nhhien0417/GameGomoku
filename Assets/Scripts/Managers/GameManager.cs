using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    private string _startPlayer = "Human";
    private string _startTurn = "x";

    public string Result;
    public string CurrentPlayer;
    public string CurrentTurn;
    public bool IsGameOver;

    private void Start()
    {
        Initialized();
    }

    private void Initialized()
    {
        SettingsManager.Instance.Initialized();
        AudioManager.Instance.Initialized();
        FirebaseManager.Instance.Initialized();

        UIManager.Instance.StartScreenActive(true);
        SceneManager.LoadScene("Gomoku");
    }

    public void NewGame()
    {
        CurrentPlayer = _startPlayer;
        CurrentTurn = _startTurn;
        Result = string.Empty;

        Board.Instance.ResetScaleIcon();
        Board.Instance.ClearBoard();

        UIManager.Instance.DisableAll();
        GameOverControl.Instance.WinLineImage.gameObject.SetActive(false);

        if (_startPlayer == "AI")
        {
            Minimax.Instance.TopEdge = Minimax.Instance.BottomEdge = Minimax.Instance.LeftEdge = Minimax.Instance.RightEdge = Board.Instance.Size / 2;
            Minimax.Instance.AIMove();
        }
    }

    public void NewOnlineGame()
    {
        NetworkManager.Instance.ConnectToServer();
    }

    public void Replay()
    {
        if (GameModeControl.Instance.GameMode == "Online")
        {
            if (!NetworkManager.Instance.IsExit)
            {
                ClientControl.Instance.ReplayRequest();
            }
            else
            {
                NetworkManager.Instance.IsExit = false;
                NetworkManager.Instance.ConnectToServer();
            }
        }

        ResetGame();
        NewGame();
    }

    public void ResetGame()
    {
        if (GameModeControl.Instance.GameMode == "PvC")
        {
            _startPlayer = _startPlayer == "Human" ? "AI" : "Human";
        }

        _startTurn = _startTurn == "x" ? "o" : "x";
    }

    public void ResetPlayer()
    {
        _startPlayer = "Human";
        _startTurn = "x";
    }

    public void UndoMove()
    {
        //
    }

    public void OpenSettings()
    {
        UIManager.Instance.SettingsScreenActive(true);
    }

    public void CloseSettings()
    {
        UIManager.Instance.SettingsScreenActive(false);
    }

    public void GameOver()
    {
        IsGameOver = true;
        GameOverControl.Instance.UpdateGameOverScreen();

        StartCoroutine(WaitGameOver());
    }

    private IEnumerator WaitGameOver()
    {
        if (Result != "Draw")
            yield return new WaitForSeconds(1.8f);

        AudioManager.Instance.Vibration();
        AudioManager.Instance.GameOverSFX();
        UIManager.Instance.GameOverScreenActive(true);
    }

    public void PlayerChange()
    {
        CurrentTurn = CurrentTurn == "x" ? "o" : "x";
        Board.Instance.ScaleIcon();

        if (GameModeControl.Instance.GameMode == "PvC")
        {
            CurrentPlayer = CurrentPlayer == "Human" ? "AI" : "Human";
        }
        else
        {
            CurrentPlayer = "Human";
        }

        if (CurrentPlayer == "AI")
        {
            Minimax.Instance.AIMove();
        }
    }

    public void BackToMainMenu()
    {
        UIManager.Instance.DisableAll();
        UIManager.Instance.NotifyScreenActive(false);
        UIManager.Instance.BackgroundActive(true);
        UIManager.Instance.StartScreenActive(true);

        GameOverControl.Instance.WinLineImage.gameObject.SetActive(false);
    }

    public void BackToNewGame()
    {
        UIManager.Instance.DisableAll();
        UIManager.Instance.BackgroundActive(true);
        UIManager.Instance.NewGameScreenActive(true);

        GameOverControl.Instance.WinLineImage.gameObject.SetActive(false);
    }

    public void NewGameScreen()
    {
        UIManager.Instance.NewGameScreenActive(true);
        UIManager.Instance.StartScreenActive(false);
    }

    public void StartGame()
    {
        NewGame();

        UIManager.Instance.DisableAll();
    }

    public void ShareBoard()
    {
        UIManager.Instance.GameOverScreenActive(false);

        Vector2 size = Board.Instance.BoardTransform.rect.size;
        Rect rect = new(25, 475, size.x + 100, size.y + 100);

        StartCoroutine(WaitToShareBoard(rect));
    }

    IEnumerator WaitToShareBoard(Rect rect)
    {
        // StartCoroutine(SocialShare.Instance.ShareRegionOfScreenshot(rect));
        Board.Instance.ShareBoardState(rect);

        yield return new WaitForSeconds(0.2f);
        UIManager.Instance.GameOverScreenActive(true);
    }
    public void CloseGame()
    {
        Application.Quit();
    }
}
