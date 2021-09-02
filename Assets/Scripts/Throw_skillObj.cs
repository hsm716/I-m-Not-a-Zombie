using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw_skillObj : MonoBehaviour
{
    public TrailRenderer throw_trail;
    public GameObject poison;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {

            StartCoroutine(poison_attack());
        }
    }
    IEnumerator poison_attack()
    {
        yield return new WaitForSeconds(1f);
        throw_trail.enabled = false;
        poison.SetActive(true);
        yield return new WaitForSeconds(5f);
        poison.SetActive(false);
        Destroy(this.gameObject);
    }
}
