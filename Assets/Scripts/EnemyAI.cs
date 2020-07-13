using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    Vector3 hearingPosition; //Lets the enemy look towards the last heard position regardless if the player leaves the radius.
    Vector3 lastSeenPosition; //Moves the enemy to towards the last seen position of the player so that they don't stop when LOS is broken.
    public float hearingDistance; //Lets the designer set a distance of how far the enemy can hear the player's noisemaker.
    public float visionDistance = 40f; //Allows the designer to set how far the enemy can detect the player from visually.
    public float fieldOfView = 90; //Allows the designer to set the angle that the enemy's line of sight has.
    public float moveSpeed; //Allows the designer to set the movement speed of the enemy.
    public float rotateSpeed; //Allows the designer to set the rotation speed of the enemy.
    public float pauseTime; //Allows the designer to decide how long the enemy pauses between actions.
    public Transform[] points; //Allows the designer to add and set as many patrol points as needed.
    private Transform currentPoint; //Used to tell the enemy what its next destination is.
    private int pointSelection; //Holds the current value within the list of points. 
    private bool canMove = true; //Allows the coroutine to pause platforms.
    private bool soundOn; //Used to turn on sound for movement only a single time for looping.
    [HideInInspector]public string gameState = "Idle"; //Sets the gamestate based on a list of conditions.
    public LayerMask layerMask; //Stops objects like spawnpoints and patrol points from blocking the enemy's LOS or FOV.

    private void Start()
    {
        //Sets the first point in the patrol list for the enemy.
        currentPoint = points[pointSelection];
    }

    private void Update()
    {
        StateMachine();

        //Sets the enemy back to its patrol path on player spawn to avoid getting stuck.
        if(GameManager.Instance.gameState == "Spawn Player")
        {
            transform.position = currentPoint.transform.position;
        }
    }

    //Used to check for conditions that switch between the states that dictate enemy behavior.
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

    //Used for quicker changing between states through other methods.
    public void ChangeState(string newState)
    {
        gameState = newState;
    }

    //Used for delays between actions.
    IEnumerator PauseTime()
    {
        canMove = false;
        yield return new WaitForSeconds(Random.Range(0,pauseTime));
        canMove = true;
    }

    //Cancels audio and starts the pause time. Used between states.
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
            //Get the direction of the other object from current object.
            Vector3 dir = currentPoint.position - transform.position;
            //Get the angle from current direction facing to desired target.
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            //Set the angle into a quaternion + sprite offset depending on initial sprite facing direction.
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle + 270));
            //Rotate current game object to face the target using a slerp function which adds some smoothing to the move.
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, (rotateSpeed * .75f) * Time.deltaTime);
            //Creates a vector between enemy and target patrol point.
            Vector3 vectorToTarget = currentPoint.position - transform.position;
            //Creates an angle for vision towards the target patrol point.
            float angleToTarget = Vector3.Angle(vectorToTarget, transform.up);


            //Does not allow the enemy to move until they see their target.
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
        
        //Once the enemy reaches its destination, it delays before picking the next target and continuing.
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
        //Get the direction of the other object from current object.
        Vector3 dir = hearingPosition - transform.position;
        //Get the angle from current direction facing to desired target.
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //Set the angle into a quaternion + sprite offset depending on initial sprite facing direction.
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle + 270));
        //Roatate current game object to face the target using a slerp function which adds some smoothing to the move.
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

        Vector3 targetPosition = lastSeenPosition;                                              //Sets the Game Manager's Player instance to a Vector3
        Vector3 directionToLook = targetPosition - transform.position;                          //Creates a variable for a vector between the player and position.
        transform.up = directionToLook;                                                         //Moves the red axis towards the player, which is rotation only.
        transform.position += directionToLook.normalized * moveSpeed * Time.deltaTime;          //Moves the enemy towards the player.
        if (!soundOn) //Only enables the sound to be played once.
        {
            AudioManager.instance.Play("Enemy Moving");
            soundOn = true;
        }

        //Has the enemy continue to move towards the last known location of the player before stopping.
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

        //Stops detecting the target if they are not moving.
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
        //Destroys the player on contact.
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

            //Sets the target position as a vector3.
            Vector3 vectorToTarget = target.transform.position - transform.position;
            //Sets the angle to the target as a float.
            float angleToTarget = Vector3.Angle(vectorToTarget, transform.up);

            //Check if target is within field of view.
            if (angleToTarget < fieldOfView)
            {
                //Using raycast to see if there are obstructions between us and target.
                RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, vectorToTarget, visionDistance, layerMask);

                //Sets the target's last seeable position as the current target.
                if (hitInfo.collider.gameObject == target)
                    {
                     lastSeenPosition = target.transform.position;
                     return true;
                    }
            else { return false; }
            }


        return false;
    }
}
