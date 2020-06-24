using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Sets the game manager as a static instance that cannot be edited.
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }
    public GameObject player; //Allows the designer to assign the player object in inspector.
    public GameObject titleCanvas; //Allows the designer to assign the title canvas.
    public GameObject uiCanvas; //Allows the designer to assign the ui canvas.

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
    }

    //Called on button press to begin game by hiding the title screen and showing UI and player.
    public void StartGame()
    {
        Debug.Log("Game Has Started");
        titleCanvas.gameObject.SetActive(false);
        player.gameObject.SetActive(true);
        uiCanvas.gameObject.SetActive(true);
    }

}
