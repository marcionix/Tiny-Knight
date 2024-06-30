using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
    [Export] public int health = 10;
    [Export] PackedScene deathPrefab;
    [Export] bool hasAttack = false;
    [Export] int attackDamage = 2;
    [Export] private float cooldown = 2f;
    [ExportCategory("Drops")]
    [Export] float dropChance = 0.1f;
    [Export] PackedScene[] dropItems = new PackedScene[0];
    [Export] float[] dropChances = new float[0];

    private Area2D attackArea;
    private float dist;
    public bool isAttacking;
    public bool wasRunning;
    public bool isRunning = false;
    private AnimationPlayer animationPlayer;
    private float attackCooldown;
    public Vector2 inputVector;
    private Sprite2D sprite2D;
    private PackedScene damageDigitPrefab;
    private Marker2D damageDigitMarker;

    public override void _Ready() {
        base._Ready();
        animationPlayer = GetNode("AnimationPlayer") as AnimationPlayer;
        try {
            attackArea = GetNode("Area2D") as Area2D;
        }catch {
            GD.Print(Name + " has no attack area");
        }
        sprite2D = GetNode("Sprite2D") as Sprite2D;
        damageDigitMarker = GetNode<Marker2D>("DamageDigitMarker");
        damageDigitPrefab = ResourceLoader.Load<PackedScene>("res://Misc/damageDigit.tscn");
    }

    public override void _Process(double delta) {
        base._Process(delta);
        if(GameManager.isGameOver)
            return;

        dist = (Position - GameManager.player_position).Length();
        if(hasAttack) {
            if(!isAttacking && dist < 70) {
                isAttacking = true;
                attackCooldown = cooldown;
                animationPlayer.Play("Attack_Side");
            }
            UpdateAttackCooldown(delta);
        }
        PlayRunIdleAnimation();
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

    private void UpdateAttackCooldown(double delta) {
        if(isAttacking && attackCooldown <= 0) {
            isAttacking = false;
            BackToIdle();
        } else if(attackCooldown > 0) {
            attackCooldown -= (float)delta;
        }
    }

    public void BackToIdle() {
        isRunning = false;
        animationPlayer.Play("Idle");
    }


    public void DealDamage() {
        Player player;
        var bodies = attackArea.GetOverlappingBodies();
        Vector2 direction;
        float dot = -1;
        foreach(var body in bodies) {
            if(body.IsInGroup("Player")) {
                player = (Player)body;
                direction = (player.Position - Position).Normalized();
                dot = direction.Dot((player.Position - Position).Normalized());
                if(dot <= 1f && dot >= 0f) {
                    player.Damage(attackDamage);
                }
            }
        }
    }

    public void Damage(int amount) {
        health -= amount;
        DamageDigit(amount);
        Blink();
        GD.Print(amount + " de dano na peia, falta "+health);
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

    private void Blink() {
        Modulate = Colors.Red;
        var tween = CreateTween();
        tween.SetEase(Tween.EaseType.InOut);
        tween.SetTrans(Tween.TransitionType.Quint);
        tween.TweenProperty(this, "modulate", Colors.White, 0.3f);
    }

    public void Die() {
        GameManager.monstersDefeated++; 
        if(deathPrefab != null) {
            DropObject(deathPrefab);
        }
        //Drop
        DropItem();
        QueueFree();
    }

    private void DropItem() {
        if(GD.Randf() <= dropChance) {
            if(dropItems.Length == 1) {
                DropObject(dropItems[0]);
                return;
            }

            //chance de drop do item
            float maxChance = 0f;
            foreach(var d in dropChances)
                maxChance += maxChance;
            if(maxChance <= 0f) maxChance = 100f;//caso nao haja chances registradas ou aenas um item

            //jogar o dado
            float randVal = GD.Randf() * maxChance;

            float seta = 0;
            PackedScene item = null;
            float chance = 0;
            for(int i = 0; i < dropItems.Length; i++) {
                item = dropItems[i];
                chance = (i < dropChances.Length ? dropChances[i] : maxChance);
                if(randVal <= seta + chance) {
                    DropObject(item);
                    break;
                }
                seta += chance;
            }
        }
    }

    private void DropObject(PackedScene obj) {
        Node2D dropObject = obj.Instantiate() as Node2D;
        AddSibling(dropObject);
        dropObject.Position = Position;
    }

    private void RotateSprite() {
        if(inputVector.X > 0) {
            sprite2D.FlipH = false;
        } else if(inputVector.X < 0) {
            sprite2D.FlipH = true;
        }
    }
}
