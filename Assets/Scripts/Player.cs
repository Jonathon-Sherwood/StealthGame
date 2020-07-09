using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Transform tf; //Allows for shorthand throughout code.
    public float turnSpeed = 1f; //Degrees per second.
    public float moveSpeed = 5; //World Space Units per second.
    private bool moving = false;
    public NoiseMaker noiseMaker;
    private Animator anim; //Assigns the variable to this object's animator.

    // Start is called before the first frame update
    void Start()
    {
        //Links this object to GameManager
        GameManager.Instance.player = this.gameObject;
        tf = gameObject.GetComponent<Transform>();
        noiseMaker = GetComponent<NoiseMaker>();
    }

    // Update is called once per frame.
    void Update()
    {
        Movement();
        print(moving);
        if (moving)
        {
            noiseMaker.volumeDistance = 10;
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
            tf.position += tf.up * moveSpeed * Time.deltaTime;
            moving = true;
        } else
        {
            moving = false;
        }
    }


    private void OnDestroy()
    {
        //Removes this gameobject to the Game Manager's list of existing Asteroids on destruction.
        GameManager.Instance.player = null;
    }
}
