using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerNoises
{ 
    Noise_WalkingNoise,
    Noise_RunningNoise,
    Noise_KnifeSwing,
    Noise_EnemyHit,
    Noise_KnifeWallHit
};

public class PlayerNoiseMaster : MonoBehaviour
{
    [SerializeField] private float walkingStepLoudness = 1f;

    [SerializeField] private float runningStepSoundLoudness = 1f;

    [SerializeField] private float knifeSwingLoudness = 1f;

    [SerializeField] private float knifeHitLoudness = 1f;

    [SerializeField] private float enemyHit = 1f;

    [SerializeField] private LayerMask mask = default;

    private Material noiseRadMaterial = null;
    [SerializeField] private GameObject noiseObj = null;


    private float speedOfTime = 20f;
    private float circleVisibilityTime = 10f;
    private float currentTime = 0f;

    private float currentNoiseRad = 0f;

    private float lastRad = 0f;

    public void MakeNoise(PlayerNoises noise)
    {
        if (noise == PlayerNoises.Noise_KnifeWallHit)
            Noise(knifeHitLoudness);
        else if (noise == PlayerNoises.Noise_WalkingNoise)
            Noise(walkingStepLoudness);
        else if (noise == PlayerNoises.Noise_RunningNoise)
            Noise(runningStepSoundLoudness);
        else if (noise == PlayerNoises.Noise_KnifeSwing)
            Noise(knifeSwingLoudness);
        else if (noise == PlayerNoises.Noise_EnemyHit)
            Noise(enemyHit);
    }

    private void Noise(float noiseRadius)
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, noiseRadius, mask);

        for (int i = 0; i < enemies.Length; i++)
        {
            Debug.Log("enemy heard noise");

            //Vector3 noisePos = transform.position - (enemies[i].GetComponent<EnemyAI>().transform.forward);
            Vector3 noisePos = transform.position;
            //noisePos.y = enemies[i].transform.position.y;

            enemies[i].GetComponent<EnemyAI>().EnemyInsideNoiseRad(noisePos);
        }

        DisplayNoiseRad(noiseRadius);


        lastRad = noiseRadius;
        currentNoiseRad = noiseRadius;
        currentTime = circleVisibilityTime;
    }

    private void DisplayNoiseRad(float radius)
    {
        float newRad = radius + 1.5f;

        if (radius > lastRad)
            if (noiseObj)
                noiseObj.transform.localScale = new Vector3(newRad, 1, newRad);
            else
                Debug.Log("No Noise object");
    }

    private void OnDrawGizmosSelected()
    {
        float[] sounds = { walkingStepLoudness, runningStepSoundLoudness, knifeHitLoudness };
        Color[] colors = { Color.red, Color.yellow, Color.green, Color.blue, Color.magenta, Color.red };
        for(int i = 0; i < sounds.Length; i++)
        {
            if (i >= colors.Length)
                Gizmos.color = colors[colors.Length - 1];
            else
                Gizmos.color = colors[i];

            Gizmos.DrawWireSphere(transform.position, sounds[i]);
        }
    }

    private void DisplayNoiseRadius(float rad)
    {
        if (noiseRadMaterial == null)
            noiseRadMaterial = noiseObj.transform.GetChild(0).transform.GetComponent<Renderer>().material;

        float maxValue = circleVisibilityTime;
        float min = maxValue / 100;
        float value = min * currentTime;

        Color newCol = noiseRadMaterial.color;
        newCol.a = value;

        noiseRadMaterial.SetColor("_Color", newCol);
    }


    private bool ShouldDisplayCircle()
    {
        if (currentTime > 0f)
        {
            currentTime -= Time.deltaTime * speedOfTime;
            return true;
        }
        else
        {
            if (lastRad != 0f)
                lastRad = 0f;

            return false;
        }
    }

    private void Update()
    {
        if (ShouldDisplayCircle())
            DisplayNoiseRadius(currentNoiseRad);
    }

}
