using Godot;
using System;

public class Mob : RigidBody2D
{
    [Export] 
    public int MinSpeed = 150;
    [Export] 
    public int MaxSpeed = 250;

    private static readonly Random _random = new Random();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var animSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        var mobTypes = animSprite.Frames.GetAnimationNames();
        animSprite.Animation = mobTypes[_random.Next(0, mobTypes.Length)];
        GetNode<VisibilityNotifier2D>("VisibilityNotifier2D")
            .Connect("screen_exited", this, nameof(OnScreenExited));
    }

    public void OnScreenExited()
    {
        // GD.Print("Enemy died");
        QueueFree();
    }
}
