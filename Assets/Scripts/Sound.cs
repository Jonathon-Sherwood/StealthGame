using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]

public class Sound
{
    public AudioClip clip; //Allows the designer to attach audio clips to the game manager.

    public string name; //Allows the designer to name the clip that will be called in program.


    //Creates adjustable variables for the audio clips.
    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(.1f, 3f)]
    public float pitch = 1f;
    [Range(0f, 5f)]
    public float time;

    public bool loop;
    public bool mute;

    [HideInInspector]
    public AudioSource source;

}
