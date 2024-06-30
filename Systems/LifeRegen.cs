using Godot;
using System;

public partial class LifeRegen : Node2D
{
    [Export] int regenAmount = 10;
    private Area2D area2D;

    public override void _Ready() {
        base._Ready();
        area2D = GetNode("Area2D") as Area2D;
        //area2D.BodyEntered += _OnArea2DBodyEntered;
    }

    private void _OnArea2DBodyEntered(Node2D body) {
        if(body.IsInGroup("Player")) {
            Player player = body as Player;
            player.Heal(regenAmount);
            player.meatCollected?.Invoke(1);
            QueueFree();
        }
    }
}
