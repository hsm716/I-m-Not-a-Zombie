using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float atk;
    private void Awake()
    {
        Destroy(this.gameObject, 3f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer==14)
        {
            Destroy(this.gameObject);
        }
    }
}
