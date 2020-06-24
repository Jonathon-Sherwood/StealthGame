using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(GameManager.Instance.player.transform.position.x, GameManager.Instance.player.transform.position.y, -1);
    }
}
