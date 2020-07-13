using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    private bool attached; //Updates depending on if the player has touched this item or not.

    private void Update()
    {
        //Mirrors the position of the player's "mouth" when touched.
        if(attached && GameManager.Instance.player != null)
        transform.position = GameObject.Find("Mouth").transform.position;

        //Detaches this from the player if the player is destroyed.
        if(GameManager.Instance.player == null)
        {
            attached = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //When this touches the spawn point, the item is destroyed to mark it as collected.
        if (collision.transform.tag == "Spawn")
        {
            GameManager.Instance.playerController.carrying = false;
            GameManager.Instance.playerController.currentSpeed = GameManager.Instance.playerController.moveSpeed;
            GameManager.Instance.playerController.turnSpeed = GameManager.Instance.playerController.turnSpeed * 2f;
            Destroy(this.gameObject);
        }

        //Slows the player down and marks this as attached to the players mouth.
        if (collision.transform.tag == "Player" && !GameManager.Instance.playerController.carrying && attached == false)
        {
            attached = true;
            GameManager.Instance.playerController.currentSpeed = GameManager.Instance.playerController.carrySpeed;
            GameManager.Instance.playerController.turnSpeed = GameManager.Instance.playerController.turnSpeed * .5f;
            GameManager.Instance.playerController.carrying = true;
        }
    }
}
