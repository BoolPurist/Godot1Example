using Godot;
using System;

public class Main : Node2D
{
    [Export] 
    public PackedScene Mob;

    private static readonly Random RandomGenerator = new Random();

    private int _score;

    private HUD _hud;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _hud = GetNode<HUD>("HUD");
        
        // Connecting for signals
        GetNode<Player>("Player").Connect("Hit", this, nameof(GameOver));
        GetNode<Timer>("ScoreTimer").Connect("timeout", this, nameof(OnScoreTimerTimeout));
        GetNode<Timer>("StartTimer").Connect("timeout", this, nameof(OnStartTimerTimeout));
        GetNode<Timer>("MobTimer").Connect("timeout", this, nameof(OnMobTimerTimeout));
        _hud.Connect("StartGame",this, nameof(NewGame));
    }

    private static float RandomRange(float min,float max)
    {
        return Convert.ToSingle( RandomGenerator.NextDouble() * ( (max - min) + max ) );
    }

    public void GameOver()
    {
        GetNode<Timer>("MobTimer").Stop();
        GetNode<Timer>("ScoreTimer").Stop();
        GetTree().CallGroup("mobs", "queue_free");
        _hud.ShowGameOver();
    }

    private void NewGame()
    {
        _score = 0;
        _hud.UpdateScore(_score);
        _hud.ShowMessage("Get Ready !");
        var player = GetNode<Player>("Player");
        player.Start(GetNode<Position2D>("StartPosition").Position);
        GetNode<Timer>("StartTimer").Start();
    }

    public void OnStartTimerTimeout()
    {
        GetNode<Timer>("MobTimer").Start();
        GetNode<Timer>("ScoreTimer").Start();
    }
    
    public void OnScoreTimerTimeout()
    {
        // GD.Print("Score timer timed out");
        _score++;
        _hud.UpdateScore(_score);
    }

    public void OnMobTimerTimeout()
    {
        // GD.Print("Mob timer timed out");
        
        // Get random location from the edge of the level.
        var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocations");

        if (mobSpawnLocation == null)
        {
            GD.Print($"{nameof(mobSpawnLocation)} as {nameof(PathFollow2D)} was null. ");
        }
        else
        {
            mobSpawnLocation.Offset = RandomGenerator.Next();

            // Creating mob and spawn it in the level.
            var mobInstance = Mob.Instance() as RigidBody2D;
            AddChild(mobInstance);

            // Set the mob's direction perpendicular to the path direction.
            var direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

            mobInstance.Position = mobSpawnLocation.Position;

            direction += RandomRange(-Mathf.Pi / 4, Mathf.Pi / 4);
            mobInstance.Rotation = direction;

            mobInstance.LinearVelocity = new Vector2(RandomRange(150f, 250f), 0).Rotated(direction);
        }

    }
}
