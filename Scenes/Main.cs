using Godot;
using System;

public partial class Main : Node2D
{
    [Export] CanvasLayer gameUI;
    [Export] PackedScene gameOverUITemplate;

    public override void _Ready() {
        base._Ready();
        GameManager.GameOverEvent += TriggerGameOver;
    }

    public void TriggerGameOver() {
        if(gameOverUITemplate != null) {
            GameOverUI gameOverUI = gameOverUITemplate.Instantiate() as GameOverUI;
            AddChild(gameOverUI);
            gameOverUI.timeLabel.Text = GameManager.timeElapsed;
            gameOverUI.monstersLabel.Text = GameManager.monstersDefeated.ToString("0");
        }

        gameUI.QueueFree();
        gameUI = null;
    }
}
