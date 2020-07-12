using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    private bool attached;

    private void Update()
    {
        if(attached && GameManager.Instance.player != null)
        transform.position = GameObject.Find("Mouth").transform.position;

        if(GameManager.Instance.player == null)
        {
            attached = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Spawn")
        {
            GameManager.Instance.playerController.carrying = false;
            GameManager.Instance.playerController.currentSpeed = GameManager.Instance.playerController.moveSpeed;
            GameManager.Instance.playerController.turnSpeed = GameManager.Instance.playerController.turnSpeed * 2f;
            Destroy(this.gameObject);
        }

        if (collision.transform.tag == "Player" && !GameManager.Instance.playerController.carrying && attached == false)
        {
            attached = true;
            GameManager.Instance.playerController.currentSpeed = GameManager.Instance.playerController.carrySpeed;
            GameManager.Instance.playerController.turnSpeed = GameManager.Instance.playerController.turnSpeed * .5f;
            GameManager.Instance.playerController.carrying = true;
        }
    }
}
