using Godot;
using System;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }

    private PackedScene _mainScene = GD.Load<PackedScene>("res://Scenes/Main/Main.tscn");

    public override void _Ready()
    {
        Instance = this;
    }

    public void LoadMain()
    {
        GetTree().ChangeSceneToPacked(_mainScene);
    }

}
