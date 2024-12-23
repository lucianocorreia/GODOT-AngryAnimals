using Godot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ScoreManager : Node
{
    const int DEFAULT_SCORE = 1000;
    const string SCORE_FILE = "user://animals.json";

    public static ScoreManager Instance { get; private set; }

    private int _levelSelected;
    private List<LevelScore> _levelScores = new();

    public override void _Ready()
    {
        Instance = this;
        LoadScore();
    }

    public override void _ExitTree()
    {
        SaveScore();
    }

    public int GetLevelSelected()
    {
        return _levelSelected;
    }

    public int SetLevelSelected(int level)
    {
        return _levelSelected = level;
    }

    private LevelScore GetLevelScore(int level)
    {
        return _levelScores.FirstOrDefault(x => x.LevelNumber == level);
    }

    public int GetLevelBestScore(int level)
    {
        LevelScore levelScore = GetLevelScore(level);
        if (levelScore != null)
        {
            return levelScore.BestScore;
        }

        return DEFAULT_SCORE;
    }

    public void SetLevelBestScore(int level, int score)
    {
        LevelScore levelScore = GetLevelScore(level);
        if (levelScore != null)
        {
            if (score < levelScore.BestScore)
            {
                levelScore.BestScore = score;
                levelScore.DateSet = DateTime.UtcNow;
            }
        }
        else
        {
            _levelScores.Add(new LevelScore(level, score));
        }

        SaveScore();
    }

    private void SaveScore()
    {
        using var file = FileAccess.Open(SCORE_FILE, FileAccess.ModeFlags.Write);
        if (file != null)
        {
            string jsonStr = JsonConvert.SerializeObject(_levelScores);
            file.StoreString(jsonStr);
        }
    }

    private void LoadScore()
    {
        using var file = FileAccess.Open(SCORE_FILE, FileAccess.ModeFlags.Read);
        if (file != null)
        {
            string jsonStr = file.GetAsText();
            if (!string.IsNullOrEmpty(jsonStr))
            {
                _levelScores = JsonConvert.DeserializeObject<List<LevelScore>>(jsonStr);
            }
        }
    }

}
