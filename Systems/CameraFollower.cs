using Godot;
using System;

public partial class CameraFollower : Camera2D
{
    public override void _Process(double delta) {
        base._Process(delta);
        Position = GameManager.player_position;
    }
}
