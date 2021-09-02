using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float playerSpd;
    [SerializeField]
    private float jumpPower;

    [SerializeField]
    private BoxCollider attackArea;

    [SerializeField]
    private TrailRenderer AttackEffect;
    [SerializeField]
    private TrailRenderer WeaponEffect;


    [SerializeField]
    private ParticleSystem SkillEffect;
    [SerializeField]
    private BoxCollider skillArea;

    [SerializeField]
    private DialogManager manager;
    GameObject scanObject;

    //적 스포너
    public GameObject EnemySpawner_;
    public EnemySpawner EnemySpawner_data;
    //

    //백신 재료
    public bool[] vaccines = { false,false,false,false, false, false, false };//v a c c i n e


    //무기
    public int[] curdurability_degree= { 0, 0 };

    public int weaponState; //0:맨손,1:근접무기,2:원거리무기
    bool[] hasWeapons = { true, false, false }; // 무기 보유 유무

    [SerializeField]
    private GameObject handGun;
    [SerializeField]
    private GameObject bat;

    //투척 스킬 OBJ
    public GameObject throw_skillObj;
    public Transform throw_pos;

    /// 총알 및 발탄위치
    [SerializeField]
    private GameObject shot_pointLifhgt;
    public int bullet_mount;
    

    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private Transform shotStartPos;

    public GameManager gameManager;

    public float curHealth;
    public float maxHealth;
    public float curZombiePower;
    public float maxZombiePower;
    public int score;

    public string curLocation;

    public CameraFollow cam;
    Rigidbody rgbd;
    public Animator anim;
    MeshRenderer[] head_mesh;
    SkinnedMeshRenderer body_mesh;


    //오디오//
    [SerializeField]
    private AudioSource swing_bat;
    [SerializeField]
    private AudioSource swing_hand;
    [SerializeField]
    private AudioSource shot_gun;
    [SerializeField]
    private AudioSource item_get;
    //

    //클리어 조건
    public bool isClear = false;
    //Puase
    public bool isPause = false;

    bool rDown; //달리기 버튼
    bool aDown; //공격 버튼
    bool s1Down;//스킬1 버튼
    bool s2Down;//스킬2 버튼
    bool wDown1; //무기 1버튼
    bool wDown2; //무기 1버튼
    bool wDown3; //무기 1버튼
    bool spaceDown;//상호작용 버튼
    

    bool isAttackReady=true;//공격 준비 상태 유무
    bool isAttack; //공격 유무
    bool isWalk;  //걷기 유무
    bool isRun;  //뛰기 유무
    
    public bool isDeath; //죽음 유무
    bool isDamage;//데미지 받는지의 유무

    bool isSkill; //스킬 유무
    bool isSkillReady=true; //스킬 준비 상태 유무
    bool isSkillReady2=true; //스킬 준비 상태 유무

    public float attackDelay; //공격 딜레이
    public float skillDelay;
    public float skillDelay2;

    Vector3 moveVec;
    float h, v;

    //hp젠//
    float hpGenDelay;
    bool ishpGenReady;
    public float hpGenAmount;
    //좀비레벨
    public int zombie_level = 1;
    public bool compactZombie = false;
    
    //공격력
    public float atk=50f; 

    private void Awake()
    {
        attackDelay = 2f;
        rgbd = GetComponent<Rigidbody>();
        AttackEffect.enabled = false;
        head_mesh = GetComponentsInChildren<MeshRenderer>();
        body_mesh = GetComponentInChildren<SkinnedMeshRenderer>();

    }
    private void FixedUpdate()
    {

        if ( !isDeath /*&& !manager.isAction*/)
        {
            Move(h, v);
            Turn();
            Run(); 
            Attack();
            Skill1();
        }
        if(isDeath)
            Death();
        
        FreezingVelocity();
    }
    void Update()
    {
        
        if (curHealth >= maxHealth)
        {
            curHealth = maxHealth;
        }
        else
        {
            HP_Gen();
        }
        if (curZombiePower >= maxZombiePower)
        {
            curZombiePower = maxZombiePower;
        }
        if (weaponState==1&&curdurability_degree[0] <= 0)
        {
            hasWeapons[1] = false;
            weaponState = 0;
        }
        if (weaponState == 2 && curdurability_degree[1] <= 0)
        {
            hasWeapons[2] = false;
            weaponState = 0;
        }
        if (curdurability_degree[0] <= 0 && curdurability_degree[1] <= 0)
        {
            hasWeapons[1] = false;
            hasWeapons[2] = false;
            weaponState = 0;
        }
        if (curZombiePower >= 20)
        {
            atk = 60;
            zombie_level=2;
        }
        if (curZombiePower >= 30)
        {
            atk = 70;
            zombie_level = 3;
        }
        if (curZombiePower >= 40)
        {
            zombie_level = 4;
        }
        if(curZombiePower >= 50)
        {
            atk = 80;
            zombie_level = 5;
        }
        if(curZombiePower >= 70)
        {
            zombie_level = 6;
        }
        if (curZombiePower>=100)
        {
            atk = 100;
            zombie_level = 7;
            compactZombie = true;
        }
        if(gameManager.killNum >= 700)
        {
            isClear = true;
        }
        if (!isDeath &&!isSkill&&gameManager.isFight)
        {
            InputKey();
        }

        WeaponSwap();
        Interaction();
        AnimationSet();
        switch (weaponState)
        {
            case 0:
                handGun.SetActive(false);
                bat.SetActive(false);
                break;
            case 1:
                bat.SetActive(true);
                handGun.SetActive(false);
                break;
            case 2:
                bat.SetActive(false);
                handGun.SetActive(true);
                break;
        }
        score = (int)gameManager.playTime * 5 + gameManager.killNum * 50;

    }
    void Death()
    {
        FreezingVelocity();
        rgbd.isKinematic = true;
    }
    void InputKey()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
        rDown = Input.GetKey(KeyCode.Z);
        aDown = Input.GetButton("Attack");
        if (zombie_level >= 2)
        {
            s1Down = Input.GetKey(KeyCode.X);
            if (zombie_level >= 4)
            {
                s2Down = Input.GetKey(KeyCode.F);
            }
        }
        spaceDown = Input.GetKeyDown(KeyCode.Space);
        wDown1 = Input.GetKeyDown(KeyCode.Alpha1);
        wDown2 = Input.GetKeyDown(KeyCode.Alpha2);
        wDown3 = Input.GetKeyDown(KeyCode.Alpha3);
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            cam.cameraMode = cam.cameraMode == false ? true : false;
        }
        if (Input.GetKeyDown(KeyCode.Escape)&&isClear==false)
        {
            if (isPause)
            {
                isPause = false;
            }
            else
            {
                isPause = true;
            }
        }
   
    }
    void WeaponSwap()
    {
        if (wDown1 && hasWeapons[0])
        {
            weaponState = 0;
        }
        else if (wDown2 && hasWeapons[1])
        {
            weaponState = 1;
        }
        else if (wDown3 && hasWeapons[2])
        {
            weaponState = 2;
        }
    }
    void Interaction()
    {
        if (spaceDown && scanObject != null)
        {
            manager.Action(scanObject);
        }
    }
    void FreezingVelocity()
    {
        rgbd.angularVelocity = Vector3.zero;
        rgbd.velocity = Vector3.zero;
    }
    void Move(float h, float v) // 방향 정보 h,v 를 매개변수로하는 이동 함수  
    {
        moveVec.Set(h, 0, v);  
        // 이동방향 정보를 매개변수 h,v를 x,z값으로 설정함

        moveVec = moveVec.normalized * playerSpd * Time.deltaTime; 
        // normallized(정규화)를 하여 대각선 이동시 빨라지는 현상을 막아줌.

        
        rgbd.MovePosition(transform.position + moveVec); 
        // MovePosition() 함수 매개변수 현재 위치에 moveVec을 더하여 입력한 이동 값에 맞게 오브젝트를 움직이게함
  

        if (moveVec == Vector3.zero) // 움직임
            isWalk = false;
        else
            isWalk = true;

    }
    void Turn()
    {
        if (h == 0 && v == 0)
            return;
        Quaternion newRotation = Quaternion.LookRotation(moveVec);
        // 입력한 값에 대한 방향값을 저장.

        rgbd.rotation = Quaternion.Slerp(rgbd.rotation, newRotation, 15 * Time.deltaTime);
        // Slerp를 이용하여 값을 변경하여, 부드러운 움직임 표현
    }
    void Run()
    {
        if (!isSkill)
        {
                if (rDown)
                {
                    playerSpd = 7f;
                    isRun = true;

                }
                else
                {
                    playerSpd = 5f;
                    isRun = false;

                }
        }
    }
    void HP_Gen()
    {
        hpGenDelay += Time.deltaTime;
        ishpGenReady = 1f < hpGenDelay;
        if (ishpGenReady)
        {
            curHealth+= hpGenAmount;
            hpGenDelay = 0f;
        }
    }
    void Attack()
    {
        if (!isSkill)
        {
            isAttack = false;
            attackDelay += Time.deltaTime;
            // 공격 딜레이

            switch (weaponState)// 공격방식
            {// 0 : 맨손 // 1 : 방망이 // 2 : 권총 
                case 0:
                    isAttackReady = 0.7f < attackDelay;
                    attackArea.size = new Vector3(1f, 1f, 1f);
                    if (isAttackReady && aDown)
                    {   
                        anim.SetTrigger("doAttack");
                        curZombiePower += 0.1f;
                        isAttack = true;
                        swing_hand.Play();
                        StartCoroutine(Swing());
                        //맨손 공격 코루틴 함수 호출
                        attackDelay = 0f;
                    }
                    break;
                case 1:
                    isAttackReady = 0.7f < attackDelay;
                    attackArea.size = new Vector3(1f, 1f, 2f);
                    if (isAttackReady && aDown)
                    {
                        anim.SetTrigger("doWeaponAttack");
                        isAttack = true;
                        swing_bat.Play();
                        StartCoroutine(WeaponSwing());
                        //방망이 공격 코루틴 함수 호출
                        attackDelay = 0f;
                        curdurability_degree[0]--;
                    }
                    break;
                case 2:
                    isAttackReady = 0.4f < attackDelay;
                    if (isAttackReady && aDown &&bullet_mount>=1)
                    {    
                        anim.SetTrigger("doShot");
  
                        isAttack = true;
                        shot_gun.Play();

                        StartCoroutine(Shot());
                        //권총 공격 코루틴 함수 호출

                        attackDelay = 0f;
                        bullet_mount--;
                    }
                    break;
            }
        }
    }
    void FindNearEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        // 태그로 적들을 탐색하여 배열에 저장.
        float dist = Mathf.Infinity;
        Vector3 nearEnemy=Vector3.zero;
        foreach(var enemy in enemies)
        {
            float dist_cost = Vector3.Distance(transform.position, enemy.transform.position);
            if(dist > dist_cost)
            {
                // 적들 중 가장 가까운 거리인 적의 위치 정보를 갱신함.
                dist = dist_cost;
                nearEnemy = enemy.transform.position;
                
            }
        }
        anim.transform.forward = nearEnemy - transform.position;
        // animator의 transform의 값을 변경하여 앞방향을 적을 향해 가르키게함.
    }
    void Skill1()
    {
        if (!isSkill) // 스킬 중이 아닐 때..
        {
            // 각 스킬의 딜레이 값을 증가시킴.
            skillDelay += Time.deltaTime;
            skillDelay2 += Time.deltaTime;

            // 각 스킬의 딜레이 값이 쿨타임 값보다 커졌을 떄, 
            // 스킬을 사용할 수 있는 상태로 변경.
            isSkillReady = 5f < skillDelay;
            isSkillReady2 = 10f < skillDelay2;

            // 스킬1은 질주 스킬이므로 걷고있거나 
            // 달리고 있는 상태일 경우에 작동하도록 제약을 걺.
            if ((isRun||isWalk)&&isSkillReady&&s1Down)
            {
                isSkill = true;
                StartCoroutine(Skill_1()); // 스킬 1 코루틴 호출
                skillDelay=0f;
            }
            if (isSkillReady2 && s2Down)
            {
                StartCoroutine(Skill_2()); // 스킬 2 코루틴 호출
                skillDelay2 = 0f;
            }
            

        }
    }
    IEnumerator Skill_1()
    {
        curZombiePower += 2f; // 좀비스킬 사용 시, 좀비화 진행
        isRun = true;         
        anim.SetFloat("RunSpd", 3.0f); // Run Animation 속도를 증가시켜, 
                                       // 질주 스킬에 어울리게 만듦 
        
        playerSpd = 14f;               // 플레이어 속도를 증가시킴
        SkillEffect.Play();            // 질주 이펙트(파티클)을 보여줌.
        skillArea.enabled = true;      // 스킬 공격 판정 Colldier를 True로 변경.
        yield return new WaitForSeconds(1.5f);  // 1.5초의 스킬지속시간을 지정
        anim.SetFloat("RunSpd", 1.8f); // 스킬 후유증 부분으로, 속도 감소 및 
        skillArea.enabled = false;     // 애니매이션 속도 감소와 공격판정 false처리
        SkillEffect.Stop();            // 이펙트 비활성화
        playerSpd = 8f;                
        yield return new WaitForSeconds(0.9f); // 후유증 0.9초의 지속시간 지정
        isSkill = false;               // 스킬 종료 및 원래 상태로 초기화
        playerSpd = 5f;
        isRun = false;
        anim.SetFloat("RunSpd", 1.5f);
    }
    IEnumerator Skill_2()
    {
        curZombiePower += 3f; // 좀비화 진행
        // 좀비스킬 투척물 생성
        GameObject instantBullet = Instantiate(throw_skillObj, throw_pos.position, throw_pos.rotation); 
        // Rigidybody Velocity 값을 변경하여 앞으로 나아가게 표현함.
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = throw_pos.forward * 25;
        yield return new WaitForSeconds(1f);
    }
    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);
        attackArea.enabled = true;
        // 공격 판정 범위가 되는 Collider를 활성화.
        AttackEffect.enabled = true;
        // 공격 이펙트가 되는 TrailRenderer를 활성화.

        yield return new WaitForSeconds(0.2f);
        attackArea.enabled = false;
        // 공격 판정 범위가 되는 Collider를 비활성화.
        AttackEffect.enabled = false;
        // 공격 이펙트가 되는 TrailRenderer를 비활성화.
        
    }
    IEnumerator WeaponSwing()
    {
        yield return new WaitForSeconds(0.1f);
        attackArea.enabled = true;
        WeaponEffect.enabled = true;

        yield return new WaitForSeconds(0.1f);
        attackArea.enabled = false;
        WeaponEffect.enabled = false;
    }
    IEnumerator Shot()
    {
        shot_pointLifhgt.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        FindNearEnemy();
        // 가장 근처에 있는 적을 향해 조준하는 함수 
        GameObject instantiateBullet = Instantiate(bullet, shotStartPos.position, transform.rotation);
        Rigidbody bullet_rgbd = instantiateBullet.GetComponent<Rigidbody>();
        bullet_rgbd.velocity = transform.forward * 20f;
        shot_pointLifhgt.SetActive(false);
    }
    void AnimationSet()
    {
        if (isWalk)
            anim.SetBool("isWalk", true);
        else
            anim.SetBool("isWalk", false);
        if (isRun)
            anim.SetBool("isRun", true);
        else
            anim.SetBool("isRun", false);
        
    }
    private void OnTriggerEnter(Collider other)
    {
        ObjData obj_data = other.gameObject.GetComponent<ObjData>();
        if (other.gameObject.CompareTag("EnemyBullet"))
        {
            Destroy(other.gameObject);
            StartCoroutine(OnDamage(other.gameObject.GetComponent<Bullet>().atk));
        }
        if (other.gameObject.CompareTag("EnemyMelee"))
        {
            StartCoroutine(OnDamage(20));
        }


        if (other.gameObject.CompareTag("GasStation"))
        {
            curLocation = "주유소";
            EnemySpawner_.transform.position = new Vector3(-22, -5, 38);
            EnemySpawner_data.isSpawn = true;

        }
        else if (other.gameObject.CompareTag("SangGa"))
        {
            curLocation = "상가";
        }
        else if (other.gameObject.CompareTag("Shop"))
        {
            curLocation = "마트";
            EnemySpawner_.transform.position = new Vector3(61, -5, 10);
            EnemySpawner_data.isSpawn = true;

        }
        else if (other.gameObject.CompareTag("ParkingLot"))
        {
            curLocation = "주차장";
        }
        else if (other.gameObject.CompareTag("Park"))
        {
            curLocation = "공원";
            EnemySpawner_.transform.position = new Vector3(-43, -5, -5);
            EnemySpawner_data.isSpawn = true;

        }
        else if (other.gameObject.CompareTag("BuildingNear"))
        {
            curLocation = "주택가";
            EnemySpawner_.transform.position = new Vector3(-29, -5, -69);
            EnemySpawner_data.isSpawn = true;

        }
        else if (other.gameObject.CompareTag("Nothing"))
        {
            curLocation = "";
            EnemySpawner_data.isSpawn = false;
        }


        if (other.gameObject.CompareTag("Item"))
        {
            Item item = other.GetComponent<Item>();
            item_get.Play();
            switch (item.type)
            {
                case Item.Type.Weapon:
                    int weaponIndex = item.value;
                    hasWeapons[weaponIndex] = true;
                    curdurability_degree[weaponIndex - 1] = 100;
                    Destroy(other.gameObject);
                    break;
                case Item.Type.Bullet:
                    bullet_mount += item.value;
                    Destroy(other.gameObject);
                    break;
                case Item.Type.Vaccine:
                    vaccines[item.value] = true;
                    Destroy(other.gameObject);
                    break;
            }
            
        }
        if (other.gameObject.CompareTag("ClearZone"))
        {
            isClear = true;
            Time.timeScale = 0f;
        }

        if (other.gameObject.name == "MESSAGE_note_0100")
        {
            scanObject = other.gameObject;
            if (!obj_data.first_Check)
                manager.Action(scanObject);
          
        }
        if (other.gameObject.name == "NPC_zombie_0101")
        {
            scanObject = other.gameObject;
            if (!obj_data.first_Check)
                manager.Action(scanObject);
        }
        if (other.gameObject.name == "EVENET_note_0102")
        {
            scanObject = other.gameObject;
            if (!obj_data.first_Check)
                manager.Action(scanObject);
        }
        if (other.gameObject.name == "EVENET_note_0103")
        {
            scanObject = other.gameObject;
            if (!obj_data.first_Check)
                manager.Action(scanObject);
        }
        if (other.gameObject.name == "EVENET_note_0104")
        {
            scanObject = other.gameObject;
            if (!obj_data.first_Check)
                manager.Action(scanObject);
            cam.VibrateForTime(2f);
        }

    }
    private void OnTriggerExit(Collider other)
    {
        
        if (other.gameObject.name == "MESSAGE_note_0100")
        {
            scanObject = null;
        }
        if (other.gameObject.name == "NPC_zombie_0101")
        {
            scanObject = null;
        }
        if (other.gameObject.name == "EVENET_note_0102")
        {
            scanObject = null;
        }
        if (other.gameObject.name == "EVENET_note_0103")
        {
            scanObject = null;
        }
        if(other.gameObject.name== "EVENET_note_0104")
        {
            scanObject = null;
        }
    }
    IEnumerator OnDamage(float damage)
    {
        if (!isDamage)
        {
            curHealth -= damage; // 현재체력 감소
            
            for(int i = 0; i < 2; i++)
            {
                curdurability_degree[i]-=5;
                // 무기 내구력 감소
            }

            isDamage = true;

            // 피격 시, 몸을 붉게 표현.
            foreach (MeshRenderer h_mesh in head_mesh)
            {
                h_mesh.material.color = Color.red;
            }
            body_mesh.material.color = Color.red;

            // 현재체력이 0 이하일 때, 죽음 처리
            if (curHealth <= 0 && !isDeath)
            {
                isDeath = true;
                this.gameObject.layer = 12;
                anim.SetTrigger("doDeath");

            }

            // 데미지를 입는 쿨타임
            yield return new WaitForSeconds(0.3f);

            // 다시 원래 색으로 초기화
            foreach (MeshRenderer h_mesh in head_mesh)
            {
                h_mesh.material.color = Color.white;
            }
            body_mesh.material.color = Color.white;
            isDamage = false;
        }
    }

   

}
