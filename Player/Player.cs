using Godot;
using System;
using System.Numerics;
using Vector2 = Godot.Vector2;

public partial class Player : CharacterBody2D {
    [ExportCategory("Stats")]
    [Export] public int maxHealth = 100;
    public int health = 100;
    [ExportCategory("Sword")]
    [Export] int swordDamage = 2;
    [ExportCategory("Ritual")]
    [Export] int ritualDamage = 50;
    [Export] float ritualInterval = 30f;
    [Export] PackedScene ritual;
    [ExportCategory("Movements")]
    [Export] float speed = 3f;
    [Export] float deadZone = 0.15f;
    [ExportCategory("Prefabs")]
    [Export] PackedScene deathPrefab;
    ProgressBar healthBar;
    public AnimationPlayer animationPlayer;
    public Sprite2D sprite2D;
    private Area2D swordArea;
    private bool wasRunning;
    bool isRunning = false;
    bool isAttacking = false;
    float attackCooldown = 0;
    private Vector2 inputVector;
    private Vector2 targetVelocity;
    private RandomNumberGenerator rng;
    private Vector2 lastVector;
    float ritualCooldown = 0f;
    private PackedScene damageDigitPrefab;
    private Marker2D damageDigitMarker;

    [Signal]
    public delegate void MeatCollectedEventHandler(int value);
    public MeatCollectedEventHandler meatCollected;

    public override void _Ready() {
        base._Ready();
        animationPlayer = GetNode("AnimationPlayer") as AnimationPlayer;
        sprite2D = GetNode("Sprite2D") as Sprite2D;
        swordArea = GetNode("Area2D") as Area2D;
        healthBar = GetNode("HealthBar") as ProgressBar;
        rng = new RandomNumberGenerator();
        health = maxHealth;
        healthBar.MaxValue = maxHealth;
        damageDigitPrefab = ResourceLoader.Load<PackedScene>("res://Misc/damageDigit.tscn");
        GameManager.thePlayer = this;
    }

    public override void _Process(double delta) {
        base._Process(delta);

        GameManager.player_position = Position;

        ReadInput(delta);

        UpdateAttackCooldown(delta);
        UpdateRitualCooldown(delta);

        PlayRunIdleAnimation();

        if(Input.IsActionJustPressed("attack")) {
            Attack();
        }

        healthBar.Value = health;
        damageDigitMarker = GetNode<Marker2D>("DamageDigitMarker");
    }

    private void UpdateAttackCooldown(double delta) {
        if(isAttacking && attackCooldown <= 0) {
            isAttacking = false;
            BackToIdle();
        } else if(attackCooldown > 0)
            attackCooldown -= (float)delta;
    }

    private void UpdateRitualCooldown(double delta) {
        if(ritualCooldown > 0) {
            ritualCooldown -= (float)delta;
        } else {
            ritualCooldown = ritualInterval;
            if(ritual != null) {
                var rite = ritual.Instantiate<Node2D>();
                AddChild(rite);
                if(rite is Ritual) {
                    ((Ritual)rite).damage = ritualDamage;
                }
            }
        }
    }

    public void BackToIdle() {
        isRunning = false;
        animationPlayer.Play("Idle");
    }

    public override void _PhysicsProcess(double delta) {
        base._PhysicsProcess(delta);

        targetVelocity = inputVector * speed * 100f * (float)delta * (!isAttacking ? 1 : 0.25f);
        Velocity = Velocity.Lerp(targetVelocity, 0.1f);
        MoveAndSlide();
    }

    private void PlayRunIdleAnimation() {
        if(!isAttacking) {
            if(wasRunning != isRunning) {
                if(isRunning) {
                    animationPlayer.Play("Run");
                } else {
                    animationPlayer.Play("Idle");
                }
            }
            RotateSprite();
        }
    }

    private void RotateSprite() {
        if(inputVector.X > 0) {
            sprite2D.FlipH = false;
        } else if(inputVector.X < 0) {
            sprite2D.FlipH = true;
        }
    }

    private void ReadInput(double delta) {
        inputVector = Input.GetVector("move_left", "move_right", "move_up", "move_down");

        if(Mathf.Abs(inputVector.X) < deadZone)
            inputVector.X = 0;
        if(Mathf.Abs(inputVector.Y) < deadZone)
            inputVector.Y = 0;

        if(!inputVector.IsZeroApprox()) {
            lastVector = inputVector;
        }

        wasRunning = isRunning;
        isRunning = !inputVector.IsZeroApprox();
    }

    public void DealDamage() {
        Enemy enemy;
        var bodies = swordArea.GetOverlappingBodies();
        Vector2 direction;
        float dot = -1;
        foreach(var body in bodies) {
            if(body.IsInGroup("Enemies")) {
                enemy = body as Enemy;
                direction = (enemy.Position - Position).Normalized();
                dot = direction.Dot(lastVector);
                if( dot <= 1f && dot >= 0.25f){
                    enemy.Damage(swordDamage);
                }
            }
        }
    }

    public void Attack() {
        if(isAttacking)
            return;

        isAttacking = true;

        int r = rng.RandiRange(1, 2);

        if(Mathf.Abs(lastVector.X) >= Mathf.Abs(lastVector.Y)) {
            animationPlayer.Play("Attack_Side_" + r.ToString());
        } else {
            if(lastVector.Y > 0)
                animationPlayer.Play("Attack_Down_" + r.ToString());
            if(lastVector.Y < 0)
                animationPlayer.Play("Attack_Up_" + r.ToString());
        }

        attackCooldown = 0.6f;
    }

    public void Damage(int amount) {
        health -= amount;
        DamageDigit(amount);
        Blink();
        GD.Print(amount + " de dano na peia, falta " + health);
        if(health <= 0) {
            Die();
        }
    }

    private void DamageDigit(int amount) {
        damageDigit damageDigit = damageDigitPrefab.Instantiate<damageDigit>();
        damageDigit.value = amount;
        damageDigit.GlobalPosition = damageDigitMarker.GlobalPosition;
        AddSibling(damageDigit);
    }

    public void Die() {
        GameManager.EndGame();
        if(deathPrefab != null) {
            Node2D deathObject = deathPrefab.Instantiate() as Node2D;
            AddSibling(deathObject);
            deathObject.Position = Position;
        }
        QueueFree();
    }

    private void Blink() {
        Modulate = Colors.Red;
        var tween = CreateTween();
        tween.SetEase(Tween.EaseType.InOut);
        tween.SetTrans(Tween.TransitionType.Quint);
        tween.TweenProperty(this, "modulate", Colors.White, 0.3f);
    }

    public void Heal(int regenAmount) {
        health += regenAmount;
        if(health > maxHealth) {
            health = maxHealth;
        }
        GD.Print("Recuperou " + regenAmount+", esta com "+health);
    }
}
