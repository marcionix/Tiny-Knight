using Godot;
using System;
using System.Linq;

public partial class MobSpawner : Node2D
{
    private PathFollow2D pathFollow2D;
    [Export] PackedScene[] enemies;
    public float mobsPerMinute = 6f;//por minuto
    private float waitTime;
    private float cooldown = 0f;
     

    public override void _Ready() {
        base._Ready();

        pathFollow2D = GetNode("%PathFollow2D") as PathFollow2D;
        //waitTime = frequency/
    }

    public override void _Process(double delta) {
        base._Process(delta);
        if(GameManager.isGameOver)
            return;

        if(cooldown > 0f) {
            cooldown -= (float)delta;
            return;
        } else { 
            cooldown = 60/mobsPerMinute;
            //checar se o ponto eh valido
            var point = GetPoint();
            PhysicsPointQueryParameters2D parameters = new PhysicsPointQueryParameters2D();
            parameters.Position = point;
            parameters.CollisionMask = 0b1001;
            if((GetWorld2D().DirectSpaceState.IntersectPoint(parameters, 1)).Count() > 0) {
                return;
            } else {
                Node2D enemy = enemies[GD.RandRange(0, enemies.Length - 1)].Instantiate<Node2D>();
                enemy.GlobalPosition = point;
                GetParent().AddChild(enemy);
            }
        }
    }

    private Vector2 GetPoint() {
        pathFollow2D.ProgressRatio = GD.Randf();
        return pathFollow2D.GlobalPosition;
    }
}
