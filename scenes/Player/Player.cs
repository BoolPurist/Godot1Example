using Godot;
using System;
using System.Drawing.Imaging;

public class Player : Area2D
{
    [Signal]
    public delegate void Hit();
    
    [Export]
    public int Speed { get; set; } = 400;

    private Vector2 _screenSize;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _screenSize = GetViewport().Size;
        this.Connect("body_entered", this, nameof(OnBodyEntered));
    }

    public override void _Process(float delta)
    {
        var velocity = new Vector2();

        if (Input.IsActionPressed("ui_right"))
        {
            velocity.x += 1;
        }
        
        if (Input.IsActionPressed("ui_left"))
        {
            velocity.x -= 1;
        }
        
        if (Input.IsActionPressed("ui_up"))
        {
            velocity.y -= 1;
        }
        
        if (Input.IsActionPressed("ui_down"))
        {
            velocity.y += 1;
        }

        var animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");

        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * Speed;
            animatedSprite.Play();
        }
        else
        {
            animatedSprite.Stop();
        }
        
        // Flip sprite and choose the animation

        if (velocity.x != 0)
        {
            animatedSprite.Animation = "walk";
            animatedSprite.FlipV = false;
            animatedSprite.FlipH = velocity.x < 0;
        }
        else if (velocity.y != 0)
        {
            animatedSprite.Animation = "up";
            animatedSprite.FlipV = velocity.y > 0;
        }

        // Updating the position because of the speed.
        Position += velocity * delta;
        Position = new Vector2(
            Mathf.Clamp(Position.x, 0, _screenSize.x), 
            Mathf.Clamp(Position.y, 0 , _screenSize.y))
            ;
    }

    public void Start(Vector2 pos)
    {
        Position = pos;
        Show();
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
    }

    public void OnBodyEntered(RigidBody2D body)
    {
        GD.Print("Hit fired from player");
        Hide();
        EmitSignal("Hit");
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
    }
}
