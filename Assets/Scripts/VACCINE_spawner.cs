using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VACCINE_spawner : MonoBehaviour
{
    public Player player;
    public GameObject[] spawnPoints = new GameObject[10];
    public GameManager gameManager;
    public GameObject[] VACCINE = new GameObject[7];
    bool[] hasVaccine = { false, false, false, false, false, false, false };
    bool[] hasSpawnPos = { false, false, false, false, false, false, false, false, false, false };
    int nextSpawn=10;
    int maxCount = 7;
    int curCount = 0;
    // Update is called once per frame
    void Update()
    {
        if (curCount < maxCount)
        {
            int randNum = Random.Range(0, 9);
            int randNum2 = Random.Range(0, 6);

            if (hasSpawnPos[randNum] == true)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (hasSpawnPos[i] == false)
                    {
                        randNum = i;
                        break;
                    }

                }
            }
            if (hasVaccine[randNum2] == true)
            {
                for (int i = 0; i < 7; i++)
                {
                    if (hasVaccine[i] == false)
                    {
                        randNum2 = i;
                        break;
                    }
                }
            }
            if (gameManager.playTime_i == nextSpawn)
            {
                hasSpawnPos[randNum] = true;
                hasVaccine[randNum2] = true;
                nextSpawn += 10;
                curCount += 1;
                Instantiate(VACCINE[randNum2], spawnPoints[randNum].transform.position, spawnPoints[randNum].transform.rotation);
                
            }

        }
    }
}
