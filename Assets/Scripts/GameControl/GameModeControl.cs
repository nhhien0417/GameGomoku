public class GameModeControl : Singleton<GameModeControl>
{
    public string GameMode;
    public int GameVariation;

    public void PlayerVsPlayer()
    {
        GameMode = "PvP";
        GameVariation = SwipeControl.CurrentVariants + 2;

        Board.Instance.DestroyAll();
        Board.Instance.Initialized();

        GameManager.Instance.ResetPlayer();
        GameManager.Instance.NewGame();
        UIManager.Instance.DisableAll();
    }

    public void PlayerVsComputer()
    {
        GameMode = "PvC";
        GameVariation = SwipeControl.CurrentVariants + 2;

        UIManager.Instance.DifficultScreenActive(true);
    }

    public void PlayOnline()
    {
        GameMode = "Online";
        GameVariation = 5;

        Board.Instance.DestroyAll();
        Board.Instance.Initialized();

        GameManager.Instance.NewGame();
        UIManager.Instance.DisableAll();
    }
}
