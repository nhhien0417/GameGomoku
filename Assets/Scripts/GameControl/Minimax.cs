using System.Collections;
using UnityEngine;

public class Minimax : Singleton<Minimax>
{
    private int _maxDepth;
    private int _depth;

    public int TopEdge, BottomEdge, LeftEdge, RightEdge;

    public void AdjustSpace(int row, int column, ref int top, ref int bottom, ref int left, ref int right)
    {
        int boardSize = Board.Instance.Size;

        if (Board.Instance.ValidMoves == boardSize * boardSize)
        {
            top = Mathf.Max(row - 1, 0);
            bottom = Mathf.Min(row + 1, boardSize - 1);
            left = Mathf.Max(column - 1, 0);
            right = Mathf.Min(column + 1, boardSize - 1);
        }
        else if (row >= top - 1 && row <= bottom + 1 && column >= left - 1 && column <= right + 1)
        {
            top = Mathf.Max(Mathf.Min(row - 1, top), 0);
            bottom = Mathf.Min(Mathf.Max(row + 1, bottom), boardSize - 1);
            left = Mathf.Max(Mathf.Min(column - 1, left), 0);
            right = Mathf.Min(Mathf.Max(column + 1, right), boardSize - 1);
        }
    }

    private IEnumerator AIThinking(int row, int column)
    {
        float randomTime = Random.Range(0.5f, 2.0f);
        yield return new WaitForSeconds(randomTime);
        Move(row, column);
    }

    public bool ForceMoveAndDefense()
    {
        for (int i = TopEdge; i <= BottomEdge; i++)
        {
            for (int j = LeftEdge; j <= RightEdge; j++)
            {
                if (Board.Instance.State[i, j] == "")
                {
                    if (Board.Instance.CheckWin(i, j, "o") == "o")
                    {
                        StartCoroutine(AIThinking(i, j));

                        return true;
                    }
                }
            }
        }

        for (int i = TopEdge; i <= BottomEdge; i++)
        {
            for (int j = LeftEdge; j <= RightEdge; j++)
            {
                if (Board.Instance.State[i, j] == "")
                {
                    if (Board.Instance.CheckWin(i, j, "x") == "x")
                    {
                        StartCoroutine(AIThinking(i, j));

                        return true;
                    }
                }
            }
        }

        return false;
    }

    public void AIMove()
    {
        StartCoroutine(AIMoveCoroutine());
    }

    private IEnumerator AIMoveCoroutine()
    {
        float bestScore = float.MinValue;
        int validMoves = Board.Instance.ValidMoves;
        _maxDepth = 0;

        int column = 0;
        int row = 0;

        if (ForceMoveAndDefense())
        {
            yield break;
        }

        for (int i = TopEdge; i <= BottomEdge; i++)
        {
            for (int j = LeftEdge; j <= RightEdge; j++)
            {
                if (Board.Instance.State[i, j] == "")
                {
                    yield return null;
                    Board.Instance.State[i, j] = "o";

                    float score = MinimaxAB(DifficultControl.Instance.TargetDepth, false, float.MinValue, float.MaxValue,
                                            i, j, "o", validMoves - 1, TopEdge, BottomEdge, LeftEdge, RightEdge);

                    Board.Instance.State[i, j] = "";

                    if (score == 0 || score == -1)
                    {
                        score = EvaluateMove(i, j, "o", TopEdge, BottomEdge, LeftEdge, RightEdge);
                    }

                    Debug.Log($"Position: ({i},{j}), Score: {score}, Depth: {_depth}");

                    if (score > bestScore)
                    {
                        bestScore = score;
                        column = j;
                        row = i;

                        // if (bestScore == 1)
                        // {
                        //     Board.Instance.Cells[row, column].OnClick();
                        //     yield break;
                        // }
                    }

                    if (score == bestScore && _depth > _maxDepth)
                    {
                        _maxDepth = _depth;
                        column = j;
                        row = i;
                    }

                    yield return null;
                }
            }
        }

        Move(row, column);
    }

    private void Move(int Row, int Column)
    {
        if (Board.Instance.State[Row, Column] == "" && !GameManager.Instance.IsGameOver)
        {
            Board.Instance.State[Row, Column] = GameManager.Instance.CurrentTurn;

            AdjustSpace(Row, Column, ref TopEdge, ref BottomEdge, ref LeftEdge, ref RightEdge);
            AudioManager.Instance.ClickSFX();
            Board.Instance.Cells[Row, Column].ChangeSprite(GameManager.Instance.CurrentTurn);

            Board.Instance.ValidMoves--;

            if (GameModeControl.Instance.GameMode == "PvC")
            {
                if (Board.Instance.CheckWin(Row, Column, GameManager.Instance.CurrentTurn) == "o")
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

    private float MinimaxAB(int depth, bool isMaximizing, float alpha, float beta, int row, int col, string turn, int moves,
                            int top, int bottom, int left, int right)
    {
        if (Board.Instance.CheckWin(row, col, turn) == "o")
        {
            _depth = depth;
            return 1;
        }
        else if (Board.Instance.CheckWin(row, col, turn) == "x")
        {
            return -1;
        }

        if (moves == 0 || depth == 0)
        {
            return 0;
        }

        if (isMaximizing)
        {
            float bestScore = float.MinValue;

            for (int i = top; i <= bottom; i++)
            {
                for (int j = left; j <= right; j++)
                {
                    if (Board.Instance.State[i, j] == "")
                    {
                        int tempTop = top, tempBottom = bottom, tempLeft = left, tempRight = right;
                        AdjustSpace(i, j, ref tempTop, ref tempBottom, ref tempLeft, ref tempRight);

                        Board.Instance.State[i, j] = "o";
                        float score = MinimaxAB(depth - 1, false, alpha, beta, i, j, "o", moves - 1,
                                                tempTop, tempBottom, tempLeft, tempRight);
                        Board.Instance.State[i, j] = "";

                        bestScore = Mathf.Max(score, bestScore);
                        alpha = Mathf.Max(alpha, score);

                        if (alpha >= beta)
                        {
                            return bestScore;
                        }
                    }
                }
            }

            return bestScore;
        }
        else
        {
            float bestScore = float.MaxValue;

            for (int i = top; i <= bottom; i++)
            {
                for (int j = left; j <= right; j++)
                {
                    if (Board.Instance.State[i, j] == "")
                    {
                        int tempTop = top, tempBottom = bottom, tempLeft = left, tempRight = right;
                        AdjustSpace(i, j, ref tempTop, ref tempBottom, ref tempLeft, ref tempRight);

                        Board.Instance.State[i, j] = "x";
                        float score = MinimaxAB(depth - 1, true, alpha, beta, i, j, "x", moves - 1,
                                                tempTop, tempBottom, tempLeft, tempRight);
                        Board.Instance.State[i, j] = "";

                        bestScore = Mathf.Min(score, bestScore);
                        beta = Mathf.Min(beta, score);

                        if (alpha >= beta)
                        {
                            return bestScore;
                        }
                    }
                }
            }

            return bestScore;
        }
    }

    public float EvaluateMove(int row, int column, string turn, int top, int bottom, int left, int right)
    {
        string opponent = "x";
        float score = 0;

        int[] rowDirections = { -1, 0, 1, 1 };
        int[] colDirections = { 1, 1, 1, 0 };

        float CalculateDirectionalScore(int rowDir, int colDir, string player)
        {
            int consecutive = 0;
            int empty = 0;
            int total = 1;

            for (int d = 0; d < 2; d++)
            {
                int r = row, c = column;
                while (true)
                {
                    r += rowDir * (d == 0 ? -1 : 1);
                    c += colDir * (d == 0 ? -1 : 1);

                    if (r < top || r > bottom || c < left || c > right)
                        break;

                    if (Board.Instance.State[r, c] == player)
                    {
                        consecutive++;
                        total++;
                    }
                    else
                    {
                        if (Board.Instance.State[r, c] == "")
                        {
                            empty++;
                        }
                        break;
                    }
                }
            }

            if (total >= 5)
            {
                return 1.0f;
            }
            else if (total == 4)
            {
                if (empty == 2)
                {
                    return 0.9f;
                }
                else if (empty == 1)
                {
                    return 0.5f;
                }
            }
            else if (total == 3)
            {
                if (empty == 2)
                {
                    return 0.3f;
                }
                else if (empty == 1)
                {
                    return 0.1f;
                }
            }
            else if (total == 2)
            {
                if (empty == 2)
                {
                    return 0.1f;
                }
            }

            return 0;
        }

        float CalculateScore(string player)
        {
            float totalScore = 0;
            for (int i = 0; i < 4; i++)
            {
                totalScore += CalculateDirectionalScore(rowDirections[i], colDirections[i], player);
            }
            return totalScore;
        }

        score += CalculateScore(turn);
        float opponentImpact = CalculateScore(opponent);
        score += opponentImpact;

        return Mathf.Clamp(score, 0, 1);
    }
}