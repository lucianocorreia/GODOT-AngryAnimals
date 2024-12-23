using System;
using System.ComponentModel;

public class LevelScore
{
    public int LevelNumber { get; set; }
    public int BestScore { get; set; }
    public DateTime DateSet { get; set; }

    public LevelScore(int levelNumber, int bestScore)
    {
        LevelNumber = levelNumber;
        BestScore = bestScore;
        DateSet = DateTime.UtcNow;
    }

}
