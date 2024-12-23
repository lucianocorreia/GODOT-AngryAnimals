using Godot;
using System;

public partial class SignalManager : Node
{
    [Signal] public delegate void OnAnimalDiedEventHandler();
    [Signal] public delegate void OnCupDestroyedEventHandler();
    [Signal] public delegate void OnLevelCompletedEventHandler();
    [Signal] public delegate void OnAttemptMadeEventHandler();
    [Signal] public delegate void OnAttemptsUpdatedEventHandler(int score);

    public static SignalManager Instance { get; private set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Instance = this;
    }

    public void EmitOnAnimalDied()
    {
        EmitSignal(SignalName.OnAnimalDied);
    }

    public void EmitOnCupDestroyed()
    {
        EmitSignal(SignalName.OnCupDestroyed);
    }

    public void EmitOnLevelCompleted()
    {
        EmitSignal(SignalName.OnLevelCompleted);
    }

    public void EmitOnAttemptMade()
    {
        EmitSignal(SignalName.OnAttemptMade);
    }

    public void EmitOnAttemptsUpdated(int score)
    {
        EmitSignal(SignalName.OnAttemptsUpdated, score);
    }

}
