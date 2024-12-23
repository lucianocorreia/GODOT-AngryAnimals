using Godot;
using System;
using System.Net.NetworkInformation;
using System.Xml.Serialization;

public partial class Animal : RigidBody2D
{
    public enum AnimalState { READY, DRAG, RELEASE }

    private static readonly Vector2 DRAG_LIM_MAX = new Vector2(0, 60);
    private static readonly Vector2 DRAG_LIM_MIN = new Vector2(-60, 0);

    private const float INPULSE_MULTIPLIER = 20.0f;
    private const float INPULSE_MAX = 1200.0f;

    #region Exported Variables

    [Export]
    private Label _debugLabel;
    [Export]
    private AudioStreamPlayer2D _strechSound;
    [Export]
    private AudioStreamPlayer2D _launchSound;
    [Export]
    private AudioStreamPlayer2D _kickSound;
    [Export]
    private Sprite2D _arrow;
    [Export]
    private VisibleOnScreenNotifier2D _visibleOnScreenNotifier;

    #endregion

    private AnimalState _state = AnimalState.READY;
    private float _arrowSacaleX = 0.0f;
    private Vector2 _start = Vector2.Zero;
    private Vector2 _dragStart = Vector2.Zero;
    private Vector2 _draggedVector = Vector2.Zero;
    private Vector2 _lastDraggedVector = Vector2.Zero;
    private int _lastCollisionCount = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ConnectSignals();
        InitializeVariables();
    }

    public override void _PhysicsProcess(double delta)
    {
        UpdateState();
        UpdateDebugLabel();
    }

    private void UpdateDebugLabel()
    {
        _debugLabel.Text = $"St: {_state}, Sl: {Sleeping}\n";
        _debugLabel.Text += $"X: {_dragStart.X:F1}, Y: {_dragStart.Y:F1}\n";
        _debugLabel.Text += $"X: {_draggedVector.X:F1}, Y: {_draggedVector.Y:F1}\n";
    }

    private void InitializeVariables()
    {
        _start = Position;
        _arrowSacaleX = _arrow.Scale.X;
        _arrow.Hide();
    }

    private void ConnectSignals()
    {
        _visibleOnScreenNotifier.ScreenExited += OnScreenExited;
        SleepingStateChanged += OnSleepingStateChanged;
        InputEvent += OnInputEvent;
    }

    private void StartDrag()
    {
        _dragStart = GetGlobalMousePosition();
        _arrow.Show();
    }

    public Vector2 CalculateImpulse()
    {
        return _draggedVector * -INPULSE_MULTIPLIER;
    }

    private void StartRelease()
    {
        _arrow.Hide();
        _launchSound.Play();
        Freeze = false;
        ApplyImpulse(CalculateImpulse());
        SignalManager.Instance.EmitOnAttemptMade();
    }

    private void ConstrainDragWithinLimits()
    {
        _lastDraggedVector = _draggedVector;
        _draggedVector = _draggedVector.Clamp(DRAG_LIM_MIN, DRAG_LIM_MAX);
        Position = _start + _draggedVector;
    }

    private void PlayStretchSound()
    {
        Vector2 diff = _draggedVector - _lastDraggedVector;
        if (diff.Length() > 0 && !_strechSound.Playing)
        {
            _strechSound.Play();
        }
    }

    private void UpdateDraggedVector()
    {
        _draggedVector = GetGlobalMousePosition() - _dragStart;

    }

    private bool DetectRelease()
    {
        if (_state == AnimalState.DRAG && Input.IsActionJustReleased("drag"))
        {
            ChangeState(AnimalState.RELEASE);
            return true;
        }

        return false;
    }

    private void UpdateArrowScale()
    {
        float inpulseLength = CalculateImpulse().Length();
        float scalePercentage = inpulseLength / INPULSE_MAX;
        _arrow.Scale = new Vector2(
            (_arrowSacaleX * scalePercentage) + _arrowSacaleX,
            _arrow.Scale.Y
        );
        _arrow.Rotation = (_start - Position).Angle();
    }

    private void HandleDragging()
    {
        if (DetectRelease()) return;

        UpdateDraggedVector();
        PlayStretchSound();
        ConstrainDragWithinLimits();
        UpdateArrowScale();
    }

    private void PlayKickSoundOnCollision()
    {
        if (_lastCollisionCount == 0 && GetContactCount() > 0 && !_kickSound.Playing)
        {
            _kickSound.Play();
        }

        _lastCollisionCount = GetContactCount();
    }

    private void HandleFlight()
    {
        PlayKickSoundOnCollision();
    }

    private void UpdateState()
    {
        switch (_state)
        {
            case AnimalState.DRAG:
                HandleDragging();
                break;
            case AnimalState.RELEASE:
                HandleFlight();
                break;
        }
    }

    private void ChangeState(AnimalState state)
    {
        _state = state;
        switch (_state)
        {
            case AnimalState.READY:
                break;
            case AnimalState.DRAG:
                StartDrag();
                break;
            case AnimalState.RELEASE:
                StartRelease();
                break;
        }
    }

    private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
        // GD.Print($"OnInputEvent: {@event}");
        if (_state == AnimalState.READY && @event.IsActionPressed("drag"))
        {
            ChangeState(AnimalState.DRAG);
        }
    }

    private void OnSleepingStateChanged()
    {
        if (Sleeping)
        {
            foreach (Node2D body in GetCollidingBodies())
            {
                if (body is Cup cup)
                {
                    cup.Die();
                }
            }
            CallDeferred("Die");
        }
    }

    private void Die()
    {
        SignalManager.Instance.EmitOnAnimalDied();
        QueueFree();
    }

    private void OnScreenExited()
    {
        GD.Print("OnScreenExited");
        Die();
    }
}
