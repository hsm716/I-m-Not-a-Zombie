using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow: MonoBehaviour
{
    public GameObject Target;
    public Vector3 offset;

    [SerializeField]private bool cameraType;

    //카메라 shake about
    [SerializeField]
    private float ShakeAmount;
    private float ShakeTime;

    public bool cameraMode;
    public void VibrateForTime(float time)
    {
        ShakeTime = time;
    }
    private void Update()
    {
        if (cameraType == false)
        {
            if (cameraMode == false)
            {

                transform.rotation = Quaternion.Euler(new Vector3(55f, 0f, 0f));
                offset = new Vector3(0f, 13f, -8f);
            }
            else if (cameraMode == true)
            {
                transform.rotation = Quaternion.Euler(new Vector3(20f, 0f, 0f));
                offset = new Vector3(0f, 5f, -11f);
            }
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, Target.transform.position + 
            offset, 5f * Time.deltaTime);

        if (ShakeTime > 0)
        {
            transform.position = Random.insideUnitSphere * ShakeAmount + transform.position;
            ShakeTime -= Time.deltaTime;
        }
        else
        {
            ShakeTime = 0.0f;
        }
    }
}
