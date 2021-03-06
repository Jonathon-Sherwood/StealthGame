﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public Player playerController; //Detects player controller for editing.
    public GameObject playerPrefab; //Allows the player to be respawned from the prefab list.
    public GameObject titleCanvas; //Allows the designer to assign the title canvas.
    public GameObject uiCanvas; //Allows the designer to assign the ui canvas.
    public GameObject spawnPoint; //Allows the deseigner to designate a spawn point for player.
    public GameObject playerDeathScreen; //Activates on player death to notify player for respawn.
    public GameObject gameOverScreen; //Activates on all player lives lost to notify of replay.
    public GameObject victoryCanvas; //Displays when the player has beaten the game.
    public GameObject collectablePrefab; //Assignable variable for different collectables.
    public Text livesText; //Allows the game manager to adjust the UI lives indicator.
    public Text collectableText; //Allows the game manager to adjust the UI collectables indicator.
    private int collectableIndex; //Used for the collectable list to check specific list spot.
    public List <Transform> collectableSpawnPoint; //Allows the designer to add and set locations for collectables.
    private bool musicOn; //Bool used to play music only once.
    public int playerLives = 3; //Adjustable variable for number of player lives.
    [HideInInspector]public int currentCollectables; //Shows how many items the player has collected.
    [HideInInspector]public int maxCollectables; //Used for declaring when the game is complete.
    private int currentLives; //Placeholder for the lives the player has after dying.

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

        //Displays the current amount of lives to the player in the UI.
        livesText.text = "X " + (currentLives + 1);

        //Displays the current amount of collected items to the player in the UI.
        collectableText.text = currentCollectables + " / " + maxCollectables;

        if (player != null)
        {
            playerController = player.GetComponent<Player>();
        }
    }

    //Used to switch between states based on transitions.
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
        else if(gameState == "Victory")
        {
            Victory();
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

    //Used for quicker changing between states through other methods.
    public void ChangeState(string newState)
    {
        gameState = newState;
    }

    //Called on button press to begin game by hiding the title screen and showing UI and player.
    public void StartGame()
    {
        ChangeState("Initialize Game");
    }

    //Turns off all input except the start screen when on the title screen.
    public void StartScreen()
    {
        Time.timeScale = 0;

        if (!titleCanvas.activeSelf)
        {
            titleCanvas.SetActive(true);
        }

        if (gameOverScreen.activeSelf)
        {
            gameOverScreen.SetActive(false);
        }

        if (victoryCanvas.activeSelf)
        {
            victoryCanvas.SetActive(false);
        }
    }

    //Turns off title screen and turns on the UI when gameplay begins. Spawns collectables.
    public void InitializeGame()
    {
        Time.timeScale = 1f;
        titleCanvas.SetActive(false);
        uiCanvas.SetActive(true);
        currentLives = playerLives;
        maxCollectables = 0;
        currentCollectables = 0;

        foreach (GameObject collectable in GameObject.FindGameObjectsWithTag("Collectable"))
        {
            Destroy(collectable.gameObject);
        }

        foreach (GameObject corpse in GameObject.FindGameObjectsWithTag("Corpse"))
        {
            Destroy(corpse.gameObject);
        }

        foreach (Transform spawnPoint in collectableSpawnPoint)
        {
            maxCollectables++;
            Instantiate(collectablePrefab, spawnPoint);
        }
    }

    //Add the player to the world and removes a life.
    public void SpawnPlayer()
    {
        if(player == null)
        player = Instantiate(playerPrefab, spawnPoint.transform.position, Quaternion.identity);
        currentLives--;

        FindObjectOfType<EnemyAI>().corpseSpawned = false; //Allows the enemy to spawn more player deaths.

        if (playerDeathScreen.activeSelf)
        {
            playerDeathScreen.SetActive(false);
        }
    }

    //State used for actual gameplay.
    public void Gameplay()
    {
        if (!musicOn)
        {
            AudioManager.instance.Play("Music");
            musicOn = true;
        }

        if(currentCollectables == maxCollectables)
        {
            AudioManager.instance.Play("Victory");
            ChangeState("Victory");
        }

    }

    //Turns on the death canvas and cancels music.
    public void PlayerDeath()
    {
        if (!playerDeathScreen.activeSelf)
        {
            AudioManager.instance.Stop("Music");
            musicOn = false;
            playerDeathScreen.SetActive(true);
        }
    }

    //Once player runs out of lives, display game over screen and allow restart.
    public void GameOver()
    {
        AudioManager.instance.Stop("Music");
        AudioManager.instance.Stop("Enemy Moving");
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

    public void Victory()
    {
        AudioManager.instance.Stop("Moving");
        AudioManager.instance.Stop("Enemy Moving");
        AudioManager.instance.Stop("Music");
        musicOn = false;

        if (player != null)
        Destroy(player.gameObject);
        
        if (!victoryCanvas.activeSelf)
        {
            victoryCanvas.SetActive(true);
        }

        if (uiCanvas.activeSelf)
        {
            uiCanvas.SetActive(false);
        }
    }

}
