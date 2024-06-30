using Godot;
using System;

public partial class DifficultySystem : Node
{
    [Export] MobSpawner mobSpawner;
    [Export] int initialSpawnRate = 30;
    [Export] int spawnRatePerMinute = 30;
    [Export] float waveDuration = 15f;
    [Export] float breakIntensity = 0.5f;

    float time = 0;
    float valSin = 0f;

    public override void _Ready() {
        base._Ready();
        if(mobSpawner != null) {
            mobSpawner.mobsPerMinute = initialSpawnRate;
        }
    }

    public override void _Process(double delta) {
        base._Process(delta);
        if(GameManager.isGameOver)
            return;

        if(mobSpawner != null) {
            time += (float)delta;
            //funcao seno/ondas
            valSin = Mathf.Sin((time * Mathf.Tau)/ waveDuration);
            valSin = Mathf.Remap(valSin, -1f, 1f, breakIntensity, 1f);

            mobSpawner.mobsPerMinute = initialSpawnRate + spawnRatePerMinute * (time/60) * valSin;
        }
    }
}
