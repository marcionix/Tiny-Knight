using Godot;
using System;

public partial class GameUI : CanvasLayer
{
    private Label timerLabel;
    private Label goldLabel;
    private Label meatLabel;

    public string TimerLabel { get => timerLabel.Text; }

    public override void _Ready() {
        base._Ready();
        timerLabel = GetNode<Label>("%TimerLabel");
        goldLabel = GetNode<Label>("%GoldLabel");
        meatLabel = GetNode<Label>("%MeatLabel");

        GameManager.thePlayer.meatCollected += GameManager.MeatCollected;
    }

    public override void _Process(double delta) {
        base._Process(delta);
        if(GameManager.isGameOver)
            return;

        //00:00.000
        timerLabel.Text = GameManager.timeElapsed;
        meatLabel.Text = GameManager.totalMeats.ToString();
    }
}
