using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player" && !GameManager.Instance.playerController.carrying)
        {
            transform.parent = collision.transform;
            transform.position = GameObject.Find("Mouth").transform.position;
            GameManager.Instance.playerController.currentSpeed = GameManager.Instance.playerController.carrySpeed;
            GameManager.Instance.playerController.turnSpeed = GameManager.Instance.playerController.turnSpeed * .5f;
            GameManager.Instance.playerController.carrying = true;
        }

        if (collision.transform.tag == "Spawn")
        {
            GameManager.Instance.playerController.carrying = false;
            GameManager.Instance.playerController.currentSpeed = GameManager.Instance.playerController.moveSpeed;
            GameManager.Instance.playerController.turnSpeed = GameManager.Instance.playerController.turnSpeed * 2f;
            Destroy(this.gameObject);
        }
    }
}
