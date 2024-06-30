using Godot;
using System;

public partial class damageDigit : Node2D
{
    public int value = 0;
    public override void _Ready() {
        base._Ready();
        ((Label)GetNode("%ValueLabel")).Text = value.ToString();
    }
}
