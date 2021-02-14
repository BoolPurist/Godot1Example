using Godot;
using System;

public class HUD : CanvasLayer
{
    [Signal]
    public delegate void StartGame();

    private Label _messageLabel;
    private Timer _messageTimer;
    private Button _startButton;
    private Label _scoreLabel;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _messageLabel = GetNode<Label>("Message");
        _messageTimer = GetNode<Timer>("MessageTimer");
        _startButton = GetNode<Button>("StartButton");
        _scoreLabel = GetNode<Label>("ScoreLabel");

        _startButton.Connect("pressed", this, nameof(OnStartButtonPressed));
        _messageTimer.Connect("timeout", this, nameof(OnMessageTimerTimeout));
    }

    public void ShowMessage(string text)
    {
        _messageLabel.Text = text;
        _messageLabel.Show();
        _messageTimer.Start();
    }

    public void UpdateScore(int score)
    {
        _scoreLabel.Text = score.ToString();
    }

    public async void ShowGameOver()
    {
      ShowMessage("Game Over !");
      
      await ToSignal(_messageTimer, "timeout");

      _messageLabel.Text = "Dodge the\nCreeps !";

      await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
      _startButton.Show();
    }

    public void OnStartButtonPressed()
    {
        _startButton.Hide();
        EmitSignal("StartGame");
    }

    public void OnMessageTimerTimeout()
    {
        _messageLabel.Hide();
    }
}
