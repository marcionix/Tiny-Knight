using Godot;
using System;

public partial class Follow_Player : Node
{
    [Export] float speed = 1f;
    private Vector2 playerPosition;
    private Enemy me;

    public override void _Ready() {
        base._Ready();
        me = GetParent() as Enemy;
    }

    public override void _PhysicsProcess(double delta) {
        base._PhysicsProcess(delta);
        if(GameManager.isGameOver)
            return;

        if(!me.isAttacking) {
            me.isRunning = true;
            me.inputVector = (GameManager.player_position - me.Position).Normalized();
            me.Velocity = me.inputVector * 100 * speed * (float)delta;
            me.MoveAndSlide();
        }
    }

}
