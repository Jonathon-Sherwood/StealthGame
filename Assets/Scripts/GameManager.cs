using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public string gameState = "Start Screen";

    //Sets the game manager as a static instance that cannot be edited.
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }
    [HideInInspector] public GameObject player; //Allows the designer to assign the player object in inspector.
    public Player playerController;
    public GameObject playerPrefab; //Allows the player to be respawned from the prefab list.
    public GameObject titleCanvas; //Allows the designer to assign the title canvas.
    public GameObject uiCanvas; //Allows the designer to assign the ui canvas.
    public GameObject spawnPoint; //Allows the deseigner to designate a spawn point for player.
    public GameObject playerDeathScreen; //Activates on player death to notify player for respawn.
    public GameObject gameOverScreen; //Activates on all player lives lost to notify of replay.
    public GameObject collectablePrefab;
    private int collectableIndex;
    public List <Transform> collectableSpawnPoint;
    private bool musicOn;

    public int playerLives = 3;
    private int currentLives;

    //Sets the game manager to a singleton.
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Debug.LogWarning("Attempted to make a second game manager.");
            Destroy(this.gameObject);
        }
        currentLives = playerLives;
    }

    public void Update()
    {
        StateMachine();

        if(player != null)
        {
            playerController = player.GetComponent<Player>();
        }
    }

    public void StateMachine()
    {
        if (gameState == "Start Screen")
        {
            StartScreen();
            //Transitions from Button Click in Start Game function.
        }
        else if (gameState == "Initialize Game")
        {
            InitializeGame();
            ChangeState("Spawn Player");
        }
        else if (gameState == "Spawn Player")
        {
            SpawnPlayer();
            ChangeState("Gameplay");
        }
        else if (gameState == "Gameplay")
        {
            Gameplay();
            if(player == null && currentLives > 0)
            {
                ChangeState("Player Death");
            } else if (player == null && currentLives <= 0)
            {
                ChangeState("Game Over");
            }
        }
        else if (gameState == "Player Death")
        {
            PlayerDeath();
            if (Input.anyKeyDown)
            {
                ChangeState("Spawn Player");
            }
        }
        else if (gameState == "Game Over")
        {
            GameOver();
            if (Input.anyKeyDown)
            {
                ChangeState("Start Screen");
            }
        }
        else
        {
            Debug.LogWarning("Game Manager tried to change to non existent state: " + gameState);
        }
    }

    public void ChangeState(string newState)
    {
        gameState = newState;
    }

    //Called on button press to begin game by hiding the title screen and showing UI and player.
    public void StartGame()
    {
        ChangeState("Initialize Game");
    }

    public void StartScreen()
    {
        if (!titleCanvas.activeSelf)
        {
            titleCanvas.SetActive(true);
        }

        if (gameOverScreen.activeSelf)
        {
            gameOverScreen.SetActive(false);
        }
    }

    public void InitializeGame()
    {
        //TODO: Reset variables in Initialize Game
        titleCanvas.SetActive(false);
        uiCanvas.SetActive(true);
        currentLives = playerLives;
        foreach(Transform spawnPoint in collectableSpawnPoint)
        Instantiate(collectablePrefab, spawnPoint);
    }

    public void SpawnPlayer()
    {
        //Add the player to the world and removes a life.
        if(player == null)
        player = Instantiate(playerPrefab, spawnPoint.transform.position, Quaternion.identity);
        currentLives--;

        if (playerDeathScreen.activeSelf)
        {
            playerDeathScreen.SetActive(false);
        }
    }

    public void Gameplay()
    {
        if (!musicOn)
        {
            AudioManager.instance.Play("Music");
            musicOn = true;
        }
        //TODO: Gameplay
    }

    public void PlayerDeath()
    {
        if (!playerDeathScreen.activeSelf)
        {
            AudioManager.instance.Stop("Music");
            musicOn = false;
            playerDeathScreen.SetActive(true);
        }
    }

    public void GameOver()
    {
        AudioManager.instance.Stop("Music");
        musicOn = false;

        if (!gameOverScreen.activeSelf)
        {
            gameOverScreen.SetActive(true);
        }

        if (uiCanvas.activeSelf)
        {
            uiCanvas.SetActive(false);
        }
    }

}
