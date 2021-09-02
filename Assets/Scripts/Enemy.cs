using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B, C };
    public Type enemyType;

    public float curHealth;
    public float maxHealth;

    private GameManager gameManager;


    private Player player_data;
    [SerializeField]
    private Transform Target;
    [SerializeField]
    private CapsuleCollider bodyCol;
    [SerializeField]
    private BoxCollider attackArea;
    [SerializeField]
    private TrailRenderer attackEffect;
    [SerializeField]
    private GameObject bullet;

    [SerializeField]
    private GameObject DropWeapon1;
    [SerializeField]
    private GameObject DropWeapon2;

    [SerializeField] private float viewAngle; // 시야각 (120도);
    [SerializeField] private float viewDistance; // 시야거리 (10미터);
    [SerializeField] private LayerMask targetMask; // 타겟 마스크 (플레이어)



    MeshRenderer[] head_mesh;
    SkinnedMeshRenderer body_mesh;

    Rigidbody rgbd;
    NavMeshAgent nav;
    Animator anim;

    bool isAttack;
    
    public bool isChase;
    bool isWalk;
    bool isDamage;
    bool isDeath;

    public Vector3 startPos;

    //오디오
    [SerializeField]
    private AudioSource hitted_melee;
    [SerializeField]
    private AudioSource swing_weapon;
    [SerializeField]
    private AudioSource shot_gun;
    //

    public float atk;
    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        rgbd = GetComponent<Rigidbody>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        head_mesh = GetComponentsInChildren<MeshRenderer>();
        body_mesh = GetComponentInChildren<SkinnedMeshRenderer>();

        startPos = transform.position;
        Target = GameObject.Find("Player").transform;
        player_data = Target.GetComponent<Player>();
        //Invoke("ChaseStart", 1f);
        anim.SetFloat("walkSpd", Random.Range(1.5f, 1.7f));
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!isDeath)
        {
            if (nav.enabled)
            {
                nav.SetDestination(Target.position);
                nav.isStopped = !isChase;
            }


        }
        else
        {
            bodyCol.enabled = false;
            Target = null;
            rgbd.velocity = Vector3.zero;
            rgbd.angularVelocity = Vector3.zero;
            rgbd.isKinematic = true;
            isChase = false;
            attackArea.enabled = false;
            attackEffect.enabled = false;
            nav.enabled = false;
        }

    }
    void Update()
    {
        AnimationSet();
        if (!isDeath)
        {
            View();
            Targeting();
            if (isChase)
            {
                isWalk = true;
            }
            else
            {
                isWalk = false;
            }
        }
        if (gameManager.count >= 2)
        {
            switch (enemyType)
            {
                case Type.A:
                    atk = 40;
                    nav.speed = 4.5f;
                    break;
                case Type.B:
                    nav.speed = 3.5f;
                    break;
                case Type.C:
                    atk = 35;
                    nav.speed = 4.5f;
                    break;
            }
            
            
        }
        if (gameManager.count >= 4)
        {
            switch (enemyType)
            {
                case Type.A:
                    atk = 50;
                    nav.speed = 5;
                    break;
                case Type.B:
                    nav.speed = 5;
                    break;
                case Type.C:
                    atk = 45;
                    nav.speed = 5;
                    break;
            }
        }
        if (gameManager.count >= 6)
        {
            switch (enemyType)
            {
                case Type.A:
                    nav.speed = 5.5f;
                    atk = 60;
                    break;
                case Type.B:
                    nav.speed = 5;
                    break;
                case Type.C:
                    atk = 55;
                    nav.speed = 5.5f;
                    break;
            }
        }
        if (gameManager.count >= 7)
        {
            switch (enemyType)
            {
                case Type.A:
                    nav.speed = 7f;
                    atk = 100;
                    break;
                case Type.B:
                    nav.speed = 6f;
                    break;
                case Type.C:
                    nav.speed = 7f;
                    atk = 105;
                    break;
            }
        }


    }
    void ChaseStart()
    {
        isChase = true;
    }
    private Vector3 BoundaryAngle(float _angle)
    {
        _angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0f, Mathf.Cos(_angle * Mathf.Deg2Rad));
    }
    private void View()
    {
        Collider[] _target = Physics.OverlapSphere(transform.position, viewDistance, targetMask);
        // 탐지할 target의 Layer값과 범위를 지정하고 탐지된 Collider들을 _taget Collider 배열에 저장.

        for (int i = 0; i < _target.Length; i++)
        {//탐지된 Collider 수만큼 반복
            Transform _targetTf = _target[i].transform;
            if (_targetTf.name == "Player")
            {// 탐지된 Collder의 이름이 Player일때,..

                Vector3 _direction = (_targetTf.position - transform.position).normalized;
                float _angle = Vector3.Angle(_direction, transform.forward);

                // 탐지된 플레이어와의 Angle을 구하고, viewAngle 값보다 적을 때에,
                if (_angle < viewAngle * 0.5f)
                {
                    RaycastHit _hit;
                    // 플레이어 추적을 시작함.
                    if (isAttack == false)
                        Invoke("ChaseStart", 0.1f); // isChase값을 True로 바꿔주는 함수.
                    else
                        isChase = false;

                }

            }
        }
    }
    void Targeting()
    {
        float targetRadius = 0f; // 감지 범위 (구체 반지름)
        float targetRange = 0f; // 감지 거리 

        // 적 타입별로 감지 거리 및 구체 반지름 값 설정 
        switch (enemyType)
        {
            case Type.A:
                targetRadius = 0.5f;
                targetRange = 1f;
                break;
            case Type.B:
                targetRadius = 0.1f;
                targetRange = 14f;
                break;
            case Type.C:
                targetRadius = 0.5f;
                targetRange = 1f;
                break;
        }
        RaycastHit[] rayHits =
            Physics.SphereCastAll(transform.position,  // 바라보는 방향으로
            targetRadius,                              // SphereCast를 쏴서
            transform.forward, targetRange,            // Layer가 Player인 것들만 
            LayerMask.GetMask("Player"));              // rayHits에 저장

        if (rayHits.Length > 0 && !isAttack)           // 설정된 거리내 감지 범위에 있으면
        {                                              // 공격 코루틴 호출
            StartCoroutine(Attack());
        }

    }
    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetTrigger("doAttack");
        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                swing_weapon.Play();
                attackArea.enabled = true;
                attackEffect.enabled = true;
                yield return new WaitForSeconds(0.1f);

                attackArea.enabled = false;
                attackEffect.enabled = false;
                
                yield return new WaitForSeconds(0.4f);

                break;
            case Type.B:
                yield return new WaitForSeconds(0.1f);
                shot_gun.Play();
                GameObject instantiateBullet = Instantiate(bullet, transform.position + new Vector3(0f, 0.5f, 0f), transform.rotation);
                Rigidbody bullet_rgbd = instantiateBullet.GetComponent<Rigidbody>();
                bullet_rgbd.velocity = transform.forward * 15f;
                yield return new WaitForSeconds(0.5f);
                break;
            case Type.C:
                yield return new WaitForSeconds(0.1f);
                swing_weapon.Play();
                attackArea.enabled = true;
                attackEffect.enabled = true;
                yield return new WaitForSeconds(0.3f);

                attackArea.enabled = false;
                attackEffect.enabled = false;
                
                yield return new WaitForSeconds(0.3f);
                break;
        }
        isChase = true;
        isAttack = false;
    }
    void AnimationSet()
    {
        if (isWalk)
        {
            anim.SetBool("isWalk", true);
        }
        else
        {
            anim.SetBool("isWalk", false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerBullet"))
        {
            StartCoroutine(OnDamage_enemy(player_data.atk));
        }
        if (other.gameObject.CompareTag("MyBullet"))
        {
            Destroy(other.gameObject);
            StartCoroutine(OnDamage_enemy(100f));
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Poison"))
        {
            StartCoroutine(OnDamage_enemy(25f));
        }
    }
    IEnumerator OnDamage_enemy(float damage)
    {
        if (!isDamage && !isDeath)
        {
            hitted_melee.Play();
            curHealth -= damage;
            isDamage = true;

            foreach (MeshRenderer mesh in head_mesh)
            {
                mesh.material.color = Color.red;
            }
            body_mesh.material.color = Color.red;

            if (curHealth <= 0 && !isDeath)
            {
                Invoke("KillNumIncrease", 0f);
                isDeath = true;
                this.gameObject.layer = 11;
                anim.SetTrigger("doDeath");
                StartCoroutine(DropItemAndInsertQ(enemyType));
                
                //Destroy(this.gameObject, 2f);




            }
            yield return new WaitForSeconds(0.2f);


            foreach (MeshRenderer mesh in head_mesh)
            {
                mesh.material.color = Color.white;
            }
            body_mesh.material.color = Color.white;
            isDamage = false;
        }
    }
    IEnumerator DropItemAndInsertQ(Type type)
    {
        yield return new WaitForSeconds(1.9f); // 적의 죽음 애니매이션 시간만큼의 딜레이 
        this.gameObject.SetActive(false); // 적을 비활성화 처리함.
        
        // 타입에 맞게 아이템을 랜덤으로 드랍하게 하고, Spawner에 넣기 전 값 초기화를 진행함. 
        if (type == Type.A)
        {
            int randNum = Random.Range(0, 100);
            if (randNum >= 0 && randNum <= 10)
            {
                Instantiate(DropWeapon1, transform.position + new Vector3(0f, 0.5f, 0f),transform.rotation);
            }
            curHealth = 100;
            EnemySpawner.instance.InsertQueue(this.gameObject, Type.A);
        }
        else if (type == Type.B)
        {
            int randNum = Random.Range(0, 100);
            if (randNum >= 0 && randNum <= 10)
            {
                Instantiate(DropWeapon1, transform.position + new Vector3(0f, 0.5f, 0f), transform.rotation);
            }
            else if (randNum >= 11 && randNum <= 30)
            {
                Instantiate(DropWeapon2, transform.position + new Vector3(0f, 0.5f, 0f), transform.rotation);
            }
            curHealth = 50;
            EnemySpawner.instance.InsertQueue(this.gameObject, Type.B);
        }
        else if (type == Type.C)
        {
            int randNum = Random.Range(0, 100);
            if (randNum >= 0 && randNum <= 10)
            {
                Instantiate(DropWeapon1, transform.position + new Vector3(0f, 0.5f, 0f), transform.rotation);
            }
            curHealth = 80;
            EnemySpawner.instance.InsertQueue(this.gameObject, Type.C);
        }
        isDeath = false;
        Target = GameObject.Find("Player").transform;
        nav.enabled = true;
        bodyCol.enabled = true;
        rgbd.isKinematic = false;
    }
    void KillNumIncrease()
    {
        gameManager.killNum += 1;
        player_data.curHealth += player_data.hpGenAmount;
        player_data.curZombiePower += 0.2f;
    }
}
