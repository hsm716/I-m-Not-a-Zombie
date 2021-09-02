using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { Bullet, Vaccine, Heart, Weapon };

    public Type type;
    public int value;


    Rigidbody rgbd;

    private void Awake()
    {
        rgbd = GetComponent<Rigidbody>();

    }

    void Update()
    {
        transform.Rotate(Vector3.up * 25 * Time.deltaTime);
    }
}