using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }
    public GameObject player; //Allows the designer to assign the player object in inspector.
    public Canvas canvas;

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
    }

    public void StartGame()
    {
        Debug.Log("Game Has Started");
        canvas.gameObject.SetActive(false);
        player.gameObject.SetActive(true);
    }

}
