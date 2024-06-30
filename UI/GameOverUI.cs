using Godot;
using System;

public partial class GameOverUI : CanvasLayer
{
    public Label timeLabel;
    public Label monstersLabel;

    [Export] float restartDelay = 5f;
    public float restartCooldown = 0f;

    public override void _Ready() {
        base._Ready();
        timeLabel = GetNode("%TimeLabel") as Label;
        monstersLabel = GetNode("%MonstersLabel") as Label;
        restartCooldown = restartDelay;
    }

    public override void _Process(double delta) {
        base._Process(delta);
        if (restartCooldown > 0f) {
            restartCooldown -= (float)delta;
        } else {
            RestartGame();
        }
    }

    private void RestartGame() {
        GameManager.Reset();
        GetTree().ReloadCurrentScene();
    }
}
