using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    Vector3 hearingPosition;
    Vector3 lastSeenPosition;
    public float hearingDistance;
    public float visionDistance = 40f;
    public float fieldOfView = 90;
    public float moveSpeed;
    public float rotateSpeed;
    public float pauseTime;
    public Transform[] points;
    private Transform currentPoint;
    private int pointSelection;
    private bool canMove = true; //Allows the coroutine to pause platforms.
    private bool soundOn;
    public string gameState = "Idle";
    public LayerMask layerMask;

    private void Start()
    {
        currentPoint = points[pointSelection];
    }

    private void Update()
    {
        print(CanHear(GameManager.Instance.player));
        StateMachine();
        if(GameManager.Instance.gameState == "Spawn Player")
        {
            transform.position = currentPoint.transform.position;
        }
    }

    public void StateMachine()
    {
        if (gameState == "Idle")
        {
            Idle();
            if (CanHear(GameManager.Instance.player))
            {
                ChangeState("Search");
            }
            else if (CanSee(GameManager.Instance.player))
            {
                ChangeState("Chase");
            }
        }
        else if (gameState == "Patrol")
        {
            Patrol();
            if (CanHear(GameManager.Instance.player))
            {
                ChangeState("Search");
            }
            else if (CanSee(GameManager.Instance.player))
            {
                ChangeState("Chase");
            }
        }
        else if (gameState == "Search")
        {
            Search();
            if (CanSee(GameManager.Instance.player))
            {
                ChangeState("Chase");
            }
        }
        else if (gameState == "Chase")
        {
            Chase();
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

    IEnumerator PauseTime()
    {
        canMove = false;
        yield return new WaitForSeconds(Random.Range(0,pauseTime));
        canMove = true;
    }

    private void Idle()
    {
        AudioManager.instance.Stop("Enemy Moving");
        soundOn = false;
        StartCoroutine(PauseTime());
        ChangeState("Patrol");
    }

    private void Patrol()
    {

        if (canMove)
        {
            //get the direction of the other object from current object
            Vector3 dir = currentPoint.position - transform.position;
            //get the angle from current direction facing to desired target
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            //set the angle into a quaternion + sprite offset depending on initial sprite facing direction
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle + 270));
            //Roatate current game object to face the target using a slerp function which adds some smoothing to the move
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, (rotateSpeed * .75f) * Time.deltaTime);

            Vector3 vectorToTarget = currentPoint.position - transform.position;

            float angleToTarget = Vector3.Angle(vectorToTarget, transform.up);


            //Check if target is within field of view.
            if (angleToTarget < fieldOfView)
            {
                if (!soundOn) //Only enables the sound to be played once.
                {
                    AudioManager.instance.Play("Enemy Moving");
                    soundOn = true;
                }
                transform.position = Vector3.MoveTowards(transform.position, currentPoint.position, (moveSpeed * .5f) * Time.deltaTime);
            }
        }
        
        if (transform.position == currentPoint.position)
        {
            AudioManager.instance.Stop("Enemy Moving");
            soundOn = false;
            ChangeState("Idle");
            pointSelection++;

            if (pointSelection == points.Length)
            {
                pointSelection = 0;
            }
            currentPoint = points[pointSelection];
        }
    }
    
    private void Search()
    {
        AudioManager.instance.Stop("Enemy Moving");
        soundOn = false;
        //get the direction of the other object from current object
        Vector3 dir = hearingPosition - transform.position;
        //get the angle from current direction facing to desired target
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //set the angle into a quaternion + sprite offset depending on initial sprite facing direction
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle + 270));
        //Roatate current game object to face the target using a slerp function which adds some smoothing to the move
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotateSpeed * Time.deltaTime);

        //Once the enemy can no longer hear the player, they wait long enough to check the area before patrolling.
        if (!CanHear(GameManager.Instance.player) && transform.rotation == rotation)
        {
            ChangeState("Idle");
        }
    }

    private void Chase()
    {
        //Prevents the game from crashing when the player is destroyed.
        if (GameManager.Instance.player == null)
            return;

        Vector3 targetPosition = lastSeenPosition;                         //Sets the Game Manager's Player instance to a Vector3
        Vector3 directionToLook = targetPosition - transform.position;                          //Creates a variable for a vector between the player and position.
        transform.up = directionToLook;                                                         //Moves the red axis towards the player, which is rotation only.
        transform.position += directionToLook.normalized * moveSpeed * Time.deltaTime;      //Moves the ship towards the player.
        if (!soundOn) //Only enables the sound to be played once.
        {
            AudioManager.instance.Play("Enemy Moving");
            soundOn = true;
        }
        if (!CanSee(GameManager.Instance.player))
        {
            if ((transform.position - lastSeenPosition).magnitude <= 0.5f)
            {
                AudioManager.instance.Stop("Enemy Moving");
                soundOn = false;
                ChangeState("Idle");
            }
        }
    }

    private bool CanHear(GameObject target)
    {
        if(GameManager.Instance.player == null) { return false; }

        //Get the target's noise maker if they have one.
        NoiseMaker targetNoiseMaker = target.GetComponent<NoiseMaker>();
        if(targetNoiseMaker == null) { return false; }

        if (targetNoiseMaker.volumeDistance == 0)
        {
            return false;
        }

        //If the distance from target is less than the noise/hearing distance, we can hear it.
        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        if ((targetNoiseMaker.volumeDistance + hearingDistance) > distanceToTarget) {
            hearingPosition = target.transform.position;
            return true;
        }

        return false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(collision.gameObject);
            AudioManager.instance.Stop("Moving"); //Cancels the player's movement sound on death.
            ChangeState("Idle");
        }
    }

    private bool CanSee(GameObject target)
    {
        if (GameManager.Instance.player == null) { return false; }

            Vector3 vectorToTarget = target.transform.position - transform.position;

            float angleToTarget = Vector3.Angle(vectorToTarget, transform.up);

            //Check if target is within field of view.
            if (angleToTarget < fieldOfView)
            {
                //Using raycast to see if there are obstructions between us and target.
                RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, vectorToTarget, visionDistance, layerMask);
                if (hitInfo.collider.gameObject == target)
                    {
                     lastSeenPosition = target.transform.position;
                     return true;
                    }
            }


        return false;
    }
}
