using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    PlayerController playerController;
    public Vector3 offset;

    void Start() 
    {
        GameObject player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    void Update() {
        transform.position = Vector3.Lerp(transform.position, player.transform.position + offset + new Vector3(playerController.xInput * 3, 0, 0), Time.deltaTime * 3f);
    }
}
