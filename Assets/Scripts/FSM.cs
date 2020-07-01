using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM : MonoBehaviour
{
    public string AIState = "Idle";
    public float aiSenseRadius;
    public float moveSpeed;
    public Vector3 targetPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(AIState == "Idle")
        {
            //Run Idle Behavior
            Idle();
            //Check for transitions.
            if(Vector3.Distance(transform.position, targetPosition) < aiSenseRadius)
            {

            }
        } else if (AIState == "Patrol")
        {
            Patrol();
        } else if (AIState == "Hear")
        {
            Hear();
        } else if (AIState == "Chase")
        {
            Chase();
        }
        else
        {
            Debug.LogWarning("AIState not found: " + AIState);
        }
    }

    void Idle()
    {
        //Do Nothing.
    }

    void Patrol()
    {
        //TODO: Write Patrol state.
    } 

    void Hear()
    {
        //TODO: Write Hear State.
    }

    void Chase()
    {
        Vector3 vectorToTarget = targetPosition - transform.position;
        transform.position += vectorToTarget.normalized * moveSpeed * Time.deltaTime;
    }

    public void ChangeState(string newState)
    {
        AIState = newState;
    }

}
