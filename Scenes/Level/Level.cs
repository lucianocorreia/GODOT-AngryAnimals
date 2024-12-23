using Godot;
using System;
using System.Runtime.Versioning;

public partial class Level : Node2D
{
    [Export]
    private Marker2D _animalStart;

    [Export]
    private PackedScene _animalScene;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SignalManager.Instance.OnAnimalDied += OnAnimalDied;
        OnAnimalDied();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // if Q is presssed go back to the main menu
        if (Input.IsKeyPressed(Key.Q))
        {
            GameManager.Instance.LoadMain();
        }
    }

    public override void _ExitTree()
    {
        SignalManager.Instance.OnAnimalDied -= OnAnimalDied;
    }

    private void OnAnimalDied()
    {
        Animal animal = _animalScene.Instantiate<Animal>();
        animal.Position = _animalStart.Position;
        AddChild(animal);
    }
}
