using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DetectionRing : MonoBehaviour
{
    public delegate void IsBeingDetected(EnemyAI enemy);
    public static IsBeingDetected OnIsBeingDetected;

    private List<Transform> enemyPos = new List<Transform>();
    private List<Canvas> uiRings = new List<Canvas>();

    private Transform playerTransform = null;

    private bool isBeingSee = false;

    private void Awake()
    {
        playerTransform = Camera.main.transform;
        uiRings.Add(transform.GetChild(0).transform.GetComponent<Canvas>());
        Debug.Log(uiRings[0]);
    }

    private void OnEnable()
    {
        OnIsBeingDetected += GetDetectionInfo;
    }

    private void OnDisable()
    {
        OnIsBeingDetected -= GetDetectionInfo;
    }

    private void GetDetectionInfo(EnemyAI enemy)
    {
        //Incase list is empty
        if(enemyPos.Count < 1)
        {
            enemyPos.Add(enemy.transform);
        }

        for (int i = 0; i < enemyPos.Count; i++)
        {
            if(enemyPos[i].GetComponent<EnemyAI>() == enemy)
            {
                enemyPos[i] = enemy.transform;
                UpdateRotationOfRings(i);
                return;
            }
        }

        enemyPos.Add(enemy.transform);
        UpdateRotationOfRings(enemyPos.Count);
    }

    private void UpdateRotationOfRings(int index)
    {
        RawImage image = uiRings[index].transform.GetChild(0).transform.GetComponent<RawImage>();
        Color newCol = image.color;

        float detectionTime = enemyPos[index].GetComponent<EnemyAI>().DetectionTime;
        float currentValue = enemyPos[index].GetComponent<EnemyAI>().CurrentDetectionValue;
        float minimunValue = (detectionTime / 255f);
        float finalValue = minimunValue * (currentValue * detectionTime);
        float finalLerpedValue = Mathf.InverseLerp(0, detectionTime, currentValue);

        newCol.a = finalLerpedValue;
        image.color = newCol;

    
        Vector3 newDir = new Vector3(-enemyPos[index].forward.x,0f, -enemyPos[index].forward.z);
        Vector3 projection = Vector3.ProjectOnPlane(playerTransform.forward, Vector3.up);

        float angel = Vector3.SignedAngle(projection, newDir, Vector3.up);

        uiRings[index].GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, -angel);
    }

}
