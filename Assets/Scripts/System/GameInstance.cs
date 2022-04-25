using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

public  class GameInstance : Singleton<GameInstance>
{
    [System.Serializable]
    public struct GlobalStruct
    {
        public string key;
        public int value;
    }
    protected static bool applicationIsQuit;
    protected static bool changeScene;
    public static string language = "rus";
    public static bool ApplicationIsQuit => applicationIsQuit;
    public static bool ChangeScene => changeScene;

    protected SaveSystem saveSystem = new SaveSystem();
    protected PoolManager poolManager;
    protected NavigationQueue navigationQueue;
    protected GameState gameState;
    protected PlayerStart playerStart;
    protected PlayerController playerController;
    protected MusicSystem musicSystem;

    protected string level;
    protected int levelEnter = 0;
    protected int levelLoadState = NONE;
    protected const int NONE = 0, LOAD = 1, NEXT = 2;

    public GameState GameState  => gameState; 
    public PlayerController PlayerController => playerController; 
    public PoolManager PoolManager  => poolManager;
    public NavigationQueue NavigationQueue => navigationQueue;
    public MusicSystem MusicSystem => musicSystem;

    protected override void Awake()
    {
        bool bindOnSceneLoaded = (Instance == null);
        base.Awake();
        if(bindOnSceneLoaded)
        SceneManager.sceneLoaded += OnSceneLoaded;

    }
    protected void Create()
    {
       
        GameObject gogm = GameObject.FindGameObjectWithTag("GameController");
        GameObject gops = GameObject.FindGameObjectWithTag("Respawn");
        if (gogm != null && gops != null)
        {
            GameMode gameMode = gogm.GetComponent<GameMode>();
            gameState = Instantiate(gameMode.GameState, gameMode.transform);
            playerStart = gops.GetComponent<PlayerStart>();
            playerController = Instantiate(gameMode.PlayerController, gameMode.transform);
            playerController.enabled = true;
            Pawn playerPawn = Instantiate(gameMode.PlayerPawn);
            playerController.Possess(playerPawn);

            if (musicSystem == null)
            {
                GameObject goms = new GameObject("MusicSystem");
                musicSystem = goms.AddComponent(typeof(MusicSystem)) as MusicSystem;
                AudioSource audio = goms.AddComponent(typeof(AudioSource)) as AudioSource;
                musicSystem.SetAudioSource(audio, gameMode.MusicHolder.GetMixerGroup());
                musicSystem.SetMusicHolder(gameMode.MusicHolder);
                DontDestroyOnLoad(goms);
                goms.transform.SetParent(transform);
            }
            else
            {
                musicSystem.enabled = true;
                musicSystem.SetMusicHolder(gameMode.MusicHolder);
            }


        }
        else
        {
            if (gogm == null)
                Debug.Log("GameMode Not Found");
            if (gops == null)
                Debug.Log("PlayerStart Not Found");
            if (musicSystem != null)
            {
                musicSystem.enabled = false;
                musicSystem.Stop();
            }
                Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            return;
        }

        GameObject gopm = new GameObject("PoolManager");
        poolManager = gopm.AddComponent(typeof(PoolManager)) as PoolManager;
        gopm.transform.SetParent(gogm.transform);

        GameObject gonr = new GameObject("NavigationQueue");
        navigationQueue = gonr.AddComponent(typeof(NavigationQueue)) as NavigationQueue;
        gonr.transform.SetParent(gogm.transform);

        if (levelLoadState == NONE)
        {
            gameState.PlayerStart(playerStart, playerController);
            //Vector3 position;
            //Quaternion rotation;
            //playerStart.GetPoint(0,out position,out rotation);
            //playerController.ControlledPawn.SetTransform(position, rotation);
        }
    }
   
    [ContextMenu("Save")]
    public void InitiateSaveGame()
    {
        saveSystem.Data.save = level;
        saveSystem.Prepare(false).
                   SaveTo("system", true, "system").
                   SaveTo(level).
                   DataTo("save.sv").
                   Close();
    }
    [ContextMenu("Load")]
    public void InitiateLoadGame()
    {
        saveSystem.DataFrom("save.sv");
        LoadScene(saveSystem.Data.save, 0, LOAD);
        
    }
    public bool GetLoadedObject(string name, ref GameObject go)
    {
        return saveSystem.GetLoadedObject(name, ref go);
    }
    public void LoadScene(int enter, int state = NEXT)
    {
        LoadScene(level, enter, state);
    }
    public void LoadScene(string name,int enter, int state = NEXT)
    {
        changeScene = true;
        levelLoadState = state;
        levelEnter = enter;
       
        switch (levelLoadState)
        {
            case NONE:
                saveSystem.ClearData();
                break;
            case LOAD:
                break;
            case NEXT:
                saveSystem.Prepare(false).
                           SaveTo(level, true, "between").
                           SaveTo("next",true,"next").
                           Close();
                break;
            default:
                break;
        }       
        SceneManager.LoadScene(name.ToLower(), LoadSceneMode.Single);
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        level = scene.name.ToLower();
        changeScene = false;
        Create();
        switch (levelLoadState)
        {
            case NONE:
                saveSystem.ClearData();
                break;
            case LOAD:
                saveSystem.Prepare(true).
                           LoadFrom("system", true, "system").
                           LoadFrom(level).
                           Close();
                break;
            case NEXT:
                saveSystem.Prepare(true).
                           LoadFrom(level, true,"between").
                           LoadFrom("next", true, "next").
                           Close();
                gameState.PlayerStart(playerStart, playerController);
                //Vector3 position;
                //Quaternion rotation;
                //playerStart.GetPoint(0, out position, out rotation);
                //playerController.ControlledPawn.SetTransform(position, rotation);
                break;
            default:
                break;
        }
    }
    protected void OnApplicationQuit()
    {
        applicationIsQuit = true;
    }
}
