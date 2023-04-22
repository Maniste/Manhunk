using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int playerHp = 100;
    private Camera pCam = null;

    private bool isDead = false;
    private bool shouldPlayAnim = true;
    private Vector3 camDeathPos = Vector3.zero;
    private void Awake()
    {
        pCam = Camera.main;
    }

    public void ApplyDamage(int incomingDamage)
    {
        //incase enemy tries to attack while they are dead
        if (playerHp < 0)
            return;

        playerHp -= incomingDamage;

        if (playerHp <= 0)
        {
            playerHp = 0;
            KillPlayer();
        }

    }

    private void KillPlayer()
    {
        isDead = true;
        camDeathPos = pCam.transform.position - (Vector3.up * 0.5f);

        gameObject.layer = 0;

        Player pMove = GetComponent<Player>();
        pMove.enabled = false;
        PlayerStealth pStealth = GetComponent<PlayerStealth>();
        pStealth.enabled = false;
        MeleeWeapon pWeapon = transform.GetChild(0).
            transform.GetChild(0).
            transform.GetChild(0).
            transform.GetComponent<MeleeWeapon>();
        pWeapon.enabled = false;
    }

    private void CameraDeathAnim()
    {
        if (pCam.transform.position.y != camDeathPos.y)
            pCam.transform.position = Vector3.Lerp(pCam.transform.position, camDeathPos, Time.deltaTime * 2f);
        else if (pCam.transform.position.y < (camDeathPos.y * 1.2f))
        {
            shouldPlayAnim = true;
            pCam.transform.position = camDeathPos;
        }
    }

    private void Update()
    {
        if (isDead && shouldPlayAnim)
            CameraDeathAnim();
    }

    private void OnGUI()
    {
        GUI.color = Color.red;
        GUI.Label(new Rect(10, 450, 100, 20), playerHp.ToString());
    }

}

