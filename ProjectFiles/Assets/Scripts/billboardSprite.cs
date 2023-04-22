using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class billboardSprite : MonoBehaviour
{
    private GameObject player = null;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    // Update is called once per frame
    void Update()
    {
        Quaternion rot = Quaternion.LookRotation(player.transform.position, Vector3.up);
        transform.rotation = rot;
    }
}
