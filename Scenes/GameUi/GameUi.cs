using Godot;
using System;

public partial class GameUi : Control
{
    [Export] private Label _levelLabel;
    [Export] private Label _attemptsLabel;
    [Export] private VBoxContainer _vb2;
    [Export] private AudioStreamPlayer _gameSound;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _levelLabel.Text = $"Level {ScoreManager.Instance.GetLevelSelected()}";

        SignalManager.Instance.OnLevelCompleted += OnLevelCompleted;
        SignalManager.Instance.OnAttemptsUpdated += OnAttemptsUpdated;
    }

    override public void _ExitTree()
    {
        SignalManager.Instance.OnLevelCompleted -= OnLevelCompleted;
        SignalManager.Instance.OnAttemptsUpdated -= OnAttemptsUpdated;
    }

    public override void _Process(double delta)
    {
        if (_vb2.Visible && Input.IsKeyPressed(Key.Space))
        {
            GameManager.Instance.LoadMain();
        }
    }

    private void OnAttemptsUpdated(int score)
    {
        _attemptsLabel.Text = $"Attempts: {score}";
    }

    private void OnLevelCompleted()
    {
        _vb2.Show();
        _gameSound.Play();
    }

}
