using Godot;
using System;
using System.Linq;
using System.Text.RegularExpressions;

public partial class Ritual : Node2D
{
    [Export] public int damage = 50;
    public Area2D area2D { get; private set; }
    public Node2D[] bodies = new Node2D[0];

    public override void _Ready() {
        base._Ready();

        area2D = GetNode<Area2D>("Area2D");
    }

    public void DoEffect() {
        bodies = area2D.GetOverlappingBodies().ToArray();
        for(int i = 0; i < bodies.Length; i++) {
            if(bodies[i] is Enemy) {
                ((Enemy)bodies[i]).Damage(damage);
            }
        }
        QueueFree();
    }

}
