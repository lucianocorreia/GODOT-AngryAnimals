using Godot;
using System;

public partial class Water : Area2D
{
    [Export] private AudioStreamPlayer2D _splashSound;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        _splashSound.Play();
    }
}
