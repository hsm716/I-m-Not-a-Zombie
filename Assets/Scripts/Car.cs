using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public float curHealth;
    bool isDamage;
    bool isDeath;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator OnDamage_enemy(float damage)
    {
        if (!isDamage && !isDeath)
        {
            curHealth -= damage;
            isDamage = true;

            

            if (curHealth <= 0 && !isDeath)
            {
                Destroy(this.gameObject, 2f);
            }
            yield return new WaitForSeconds(0.2f);

            isDamage = false;
        }
    }
}
