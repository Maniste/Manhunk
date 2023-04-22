using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHealth : MonoBehaviour, I_Damagable
{
    [SerializeField] private int healthPoints = 100;
    void Start()
    {
        
    }

    public void TakenDamage(int damage)
    {
        healthPoints -= damage;
        CheckCharacterHealth();
    }

    private void CheckCharacterHealth()
    {
        if (healthPoints <= 0)
            gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
