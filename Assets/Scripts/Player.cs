using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Transform tf; //Allows for shorthand throughout code.
    public float turnSpeed = 1f; //Degrees per second.
    public float moveSpeed = 5; //World Space Units per second.
    public float carrySpeed; //Different adjustable speed for when the player is carrying a collectable.
    [HideInInspector] public float currentSpeed; //Used to change the player's speed through different variables.
    private bool moving = false; //Bool used for causing noise or not.
    private NoiseMaker noiseMaker; //Detects attached noisemaker for editing.
    private Animator anim; //Assigns the variable to this object's animator.
    private bool soundOn; //Used for only turning on the looping sound once.
    [HideInInspector] public bool carrying; //Bool used for speed changes when carrying a collectable.

    // Start is called before the first frame update
    void Start()
    {
        //Links this object to its components.
        GameManager.Instance.player = this.gameObject;
        anim = GetComponent<Animator>();
        tf = gameObject.GetComponent<Transform>();
        noiseMaker = GetComponent<NoiseMaker>();
        noiseMaker.maxVolume = noiseMaker.volumeDistance;
        currentSpeed = moveSpeed;
    }

    // Update is called once per frame.
    void Update()
    {
        Movement();

        //Sets noisemaker's volume to on or off based on movement.
        if (moving)
        {
            noiseMaker.volumeDistance = noiseMaker.maxVolume;
        } else
        {
            noiseMaker.volumeDistance = 0;
        }
    }

    void Movement()
    {
        //Rotate player to the left.
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            tf.Rotate(0, 0, turnSpeed * Time.deltaTime);
        }
        //Rotate player to the right.
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            tf.Rotate(0, 0, -turnSpeed * Time.deltaTime);
        }
        //Move player forward relative to direction facing.
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            tf.position += tf.up * currentSpeed * Time.deltaTime;
            moving = true;
            anim.SetBool("Moving", true);
            if (!soundOn)
            {
                AudioManager.instance.Play("Moving");
                soundOn = true;
            }
        } else
        {
            anim.SetBool("Moving", false);
            AudioManager.instance.Stop("Moving");
            soundOn = false;
            moving = false;
        }
    }


    private void OnDestroy()
    {
        //Removes this gameobject from the Game Manager's player on destruction.
        GameManager.Instance.player = null;
    }
}
