using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    [SerializeField]
    protected GameState gameState;
    [SerializeField]
    protected PlayerController playerController;
    [SerializeField]
    protected Pawn playerPawn;
    [SerializeField]
    protected MusicHolder musicHolder;
    public GameState GameState => gameState;
    public PlayerController PlayerController => playerController;
    public Pawn PlayerPawn => playerPawn;
    public MusicHolder MusicHolder => musicHolder;
}
