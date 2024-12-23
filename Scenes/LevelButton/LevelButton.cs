using Godot;
using System;

public partial class LevelButton : TextureButton
{
    static readonly Vector2 HOVER_SCALE = new Vector2(1.1f, 1.1f);
    static readonly Vector2 NORMAL_SCALE = new Vector2(1.0f, 1.0f);


    [Export] private int _levelNumber { get; set; }
    [Export] private Label _levelLabel;
    [Export] private Label _scoreLabel;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _levelLabel.Text = _levelNumber.ToString();
        _scoreLabel.Text = ScoreManager.Instance.GetLevelBestScore(_levelNumber).ToString("D4");

        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
        Pressed += OnPressed;
    }

    private void OnPressed()
    {
        GD.Print("Level selected: " + _levelNumber);
        ScoreManager.Instance.SetLevelSelected(_levelNumber);
        GetTree().ChangeSceneToFile($"res://Scenes/Level/Level{_levelNumber}.tscn");
    }

    private void OnMouseExited()
    {
        Scale = NORMAL_SCALE;
    }

    private void OnMouseEntered()
    {
        Scale = HOVER_SCALE;
    }

}
