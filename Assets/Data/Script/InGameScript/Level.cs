using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "Level")]
public class Levels : ScriptableObject
{
    [Header("BoardSize")]
    public int width;
    public int height;
    [Header("Starting Tites")]
    public ArrayLayout gemBoardLayout;
    [Header("Score Goals")]
    public int[] ScoreGoals;
    [Header("GameType")]
    public EndGameType Gametype;
    public BlankGoal[] LevelGoals;
    [Header("Special Tiles Layout")]
    public TileMapLayout tileMapLayout;


}
