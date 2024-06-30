using Godot;
using GodotPlugins.Game;
using System;

public partial class GameManager : Node
{
    [Export]
    public static Vector2 player_position;
    [Export]
    public static Player thePlayer;
    [Export]
    public static bool isGameOver = false;
    public static int monstersDefeated = 0;
    public static string timeElapsed = "00:00";
    public static int totalMeats = 0;
    public static float elapsedTime = 0f;
    private static float sec;
    private static int min;

    [Signal]
    public delegate void GameOverEventHandler();
    public static GameOverEventHandler GameOverEvent;

    public override void _Process(double delta) {
        base._Process(delta);
        if(GameManager.isGameOver)
            return;

        elapsedTime += (float)delta;
        sec = elapsedTime % 60f;
        min = ((int)elapsedTime) / 60;
        //00:00.000
        GameManager.timeElapsed = min.ToString("00") + ":" + sec.ToString("00.000");
    }


    public static void Reset() {
        player_position = Vector2.Zero;
        GameOverEvent = null;
        isGameOver = false;
        monstersDefeated = 0;
        timeElapsed = "00:00";
        totalMeats = 0;
        elapsedTime = 0f;
    }

public static void MeatCollected(int value) {
        totalMeats++;
    }

    public static void EndGame() {
        isGameOver = true;
        GameOverEvent?.Invoke();
    }
}
