using Godot;
using System;

public partial class Cup : StaticBody2D
{
    public const string GROUP_NAME = "cup";

    [Export] private AnimationPlayer _animationPlayer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _animationPlayer.AnimationFinished += OnAnimationFinished;

    }

    public override void _EnterTree()
    {
        AddToGroup(GROUP_NAME);
    }

    private void OnAnimationFinished(StringName animName)
    {
        SignalManager.Instance.EmitOnCupDestroyed();
        QueueFree();
    }

    public void Die()
    {
        _animationPlayer.Play("vanish");
    }


}
