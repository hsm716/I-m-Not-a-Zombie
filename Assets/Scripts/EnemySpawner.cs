using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject EnemyPrefabs_male;
    public GameObject EnemyPrefabs_police;
    public GameObject EnemyPrefabs_female;
    Queue<GameObject> Enemys1 = new Queue<GameObject>();//male
    Queue<GameObject> Enemys2 = new Queue<GameObject>();//police
    Queue<GameObject> Enemys3 = new Queue<GameObject>();//female
    public static EnemySpawner instance;
    public bool isSpawn;
    private void Awake()
    {
        instance = this;
        for (int i = 0; i < 10; i++)
        {
            var enemy1 = Instantiate(EnemyPrefabs_male, transform.position, transform.rotation);
            enemy1.SetActive(false);
            Enemys1.Enqueue(enemy1);
        }
        for (int i = 0; i < 10; i++)
        {
            var enemy2 = Instantiate(EnemyPrefabs_police, transform.position, transform.rotation);
            enemy2.SetActive(false);
            Enemys2.Enqueue(enemy2);
        }
        for (int i = 0; i < 10; i++)
        {
            var enemy3 = Instantiate(EnemyPrefabs_female, transform.position, transform.rotation);
            enemy3.SetActive(false);
            Enemys3.Enqueue(enemy3);
        }
        StartCoroutine(Spawn(Enemy.Type.A));
        StartCoroutine(Spawn(Enemy.Type.B));
        StartCoroutine(Spawn(Enemy.Type.C));
    }
    IEnumerator Spawn(Enemy.Type type)
    {
        while (true)
        {
            GameObject enemy1=null;
            if (isSpawn) // isSpawn이 true일 경우에만
            {
                if (type == Enemy.Type.A)
                {
                    if (Enemys1.Count != 0) // que가 비어있지 않다면,
                    {

                        enemy1 = GetQueue(type); // 해당 타입 Que에서 꺼내서
                        enemy1.SetActive(true); // 꺼낸 오브젝트를 활성화.
                    }
                }
                else if (type == Enemy.Type.B)
                {
                    if (Enemys2.Count != 0)
                    {

                        enemy1 = GetQueue(type);
                        enemy1.SetActive(true);
                    }
                }
                else if (type == Enemy.Type.C)
                {
                    if (Enemys3.Count != 0)
                    {

                        enemy1 = GetQueue(type);
                        enemy1.SetActive(true);
                    }
                }
            }
            yield return new WaitForSeconds(2f);
        }
        
    }
    public void InsertQueue(GameObject p_object, Enemy.Type mode)
    {
        if (mode == Enemy.Type.A)
        {
            Enemys1.Enqueue(p_object);
        }
        else if (mode == Enemy.Type.B)
        {
            Enemys2.Enqueue(p_object);
        }
        else if (mode == Enemy.Type.C)
        {
            Enemys3.Enqueue(p_object);
        }

        p_object.SetActive(false);
    }
    public GameObject GetQueue(Enemy.Type mode)
    {
        GameObject t_object = null;
        float xPos = Random.Range(-10, 10); // 랜덤 x값
        float zPos = Random.Range(-10, 10); // 랜덤 z값
        float yRot = Random.Range(0, 360);  // 랜덤 회전값
        Vector3 RandomVector = new Vector3(xPos, 0.0f, zPos);
        if (mode == Enemy.Type.A)
        {
            t_object = Enemys1.Dequeue(); // Que에서 꺼냄.
            t_object.transform.position = transform.position+RandomVector; // 위치값에 RandomVector값을 더해줌.

            t_object.transform.localEulerAngles += new Vector3(0, yRot, 0); 
            t_object.layer = 9; // Layer를 Enemy를 가르키는 9로 설정


        }
        else if (mode == Enemy.Type.B)
        {
            t_object = Enemys2.Dequeue();
            t_object.transform.position = transform.position + RandomVector;
            t_object.transform.localEulerAngles += new Vector3(0, yRot, 0);
            t_object.layer = 9;
           
        }
        else if (mode == Enemy.Type.C)
        {
            t_object = Enemys3.Dequeue();
            t_object.transform.position = transform.position + RandomVector;
            t_object.transform.localEulerAngles += new Vector3(0, yRot, 0);
            t_object.layer = 9;
          
        }
        StartCoroutine(ChasePlayer_after_wait(t_object.GetComponent<Enemy>(), 20)); // 모든 적은 초기에 지정한 시간이 지난 후에
        t_object.SetActive(true);                                                   // 플레이어를 추적하게 설정.

        return t_object;
    }
    IEnumerator ChasePlayer_after_wait(Enemy enm,float sec)
    {
        yield return new WaitForSeconds(sec);
        enm.isChase = true;
    }

}
