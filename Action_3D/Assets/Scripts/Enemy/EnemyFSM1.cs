//using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

//몬스터 유한 상태 머신
public class EnemyFSM1 : MonoBehaviour
{
    //몬스터 상태 ENUM
    public enum EnemyState
    {
        IDLE,
        MOVE,
        ATTACK,
        RETURN,
        DAMAGED,
        DIE,
        REVIVE,
        DIE_FIRE,
    }

  
    //애니메이션을 제어하기 위한 애니메이터 컴포넌트
    Animator anim;

    public EnemyState state { get; private set; }//몬스터 상태 변수
    #region "자살 좀비"
    //자살 좀비일 경우
    public bool isTypeSuicide = false;
    public GameObject explosionPrefab;
    public GameObject bloodPrefab;
    public GameObject waringLight;
    public GameObject moveSound;
    public AudioClip suicideSound;
    #endregion

    #region "중간 보스"
    //자살 좀비일 경우
    public bool isTypeMiniBoss = false;
    #endregion

    //유용한 기능
    #region "IDLE 상태에 필요한 변수들"
    #endregion

    #region "MOVE 상태에 필요한 변수들"
    #endregion

    #region "ATTACK 상태에 필요한 변수들"
    //공격 애니메이션 끝났는지 판별
    private bool attackAnimEnd = true;
    #endregion

    #region "RETURN 상태에 필요한 변수들"
    #endregion

    #region "DAMAGED 상태에 필요한 변수들"
    #endregion

    #region "DIE 상태에 필요한 변수들"
    private float reviveTimer = 0f;
    public float reviveTime = 5.0f;
    public bool hitByFire = false;
    public GameObject hitPoint;
    #endregion

    #region "IDLE 상태에 필요한 변수들"
    #endregion

    public float findRange = 15f;   //플레이어를 찾는 범위
    public float attackRange = 2f;  //공격 가능 범위
    public float moveRange = 30f;
    bool canAttack = true;
    Vector3 startPoint;             //몬스터 시작 위치
    //Quaternion startRotation;       //몬스터 시작 회전 값
    Transform player;               //플레이어를 찾기 위해
    NavMeshAgent agent;

    public EnemyHitBox HitBox;

    //몬스터 일반 변수
    public int hp = 100;
    int att = 5;
    float speed = 0.5f;
    float offset;
    //공격 딜레이
    float attTime = 2f; //2초에 한번 공격
    float timer = 0f;   //타이머

    // Start is called before the first frame update
    void Start()
    {
        //몬스터 상태 초기화
        state = EnemyState.IDLE;
        //시작지점 저장
        startPoint = transform.position;
        //startRotation = transform.rotation;
        //플레이어 트랜스폼
        player = GameObject.Find("Player").transform;
        //애니메이터 컴포넌트
        anim = GetComponentInChildren<Animator>();
        //애니메이션 오프셋 랜덤 지정
        anim.SetFloat("Offset", UnityEngine.Random.Range(0.0f, 1.0f));
        //랜덤 값 지정
        transform.eulerAngles = new Vector3(0, UnityEngine.Random.Range(0, 360), 0);
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == EnemyState.MOVE && Time.timeScale == 1)
        {
            moveSound.SetActive(true);
        }
        else moveSound.SetActive(false);
        //OnDrawGizmos();
        //상태에 따른 행동 처리
        switch (state)
        {
            case EnemyState.IDLE:
                Idle();
                break;
            case EnemyState.MOVE:
                Move();
                break;
            case EnemyState.ATTACK:
                Attack();
                break;
            case EnemyState.DIE:
                Die();
                break;
            case EnemyState.DIE_FIRE:
                break;
        }
    }
    
    public void StartHit()
    {
        CannotAttack();
        state = EnemyState.DAMAGED;
        print(state);
    }

    public void EndHit()
    {
        CanAttack();
        state = EnemyState.MOVE;
        print("좀비상태:이동");
    }

    private void Idle()
    {
        if(Vector3.Distance(transform.position , player.position) < findRange)
        {
            state = EnemyState.MOVE;
            if (isTypeSuicide) waringLight.SetActive(true);
            print("상태 전환 : Idle -> Move");

            //애니메이션
            anim.SetTrigger("Move");
        }
    }

    private void Move()
    {
        if (Vector3.Distance(transform.position, player.position) <= attackRange)//공격 범위 안에 들어옴
        {
            state = EnemyState.ATTACK;
            print("상태 전환 : Move -> Attack");
            timer = 2f;
        }
        else
        {
            if(PlayerInput.Instance.state == PlayerInput.PlayerState.DIE)
            {
                state = EnemyState.IDLE;
                anim.SetTrigger("Idle");
            }
            agent.SetDestination(player.transform.position);
        }
    }

    private void Attack()
    {
        if (PlayerInput.Instance.state == PlayerInput.PlayerState.DIE) return;
        if (Vector3.Distance(transform.position, player.position) > attackRange && isTypeSuicide==false && attackAnimEnd)//현재 상태를 무브로 전환하기 (재추격)
        {
            state = EnemyState.MOVE;
            print("상태 전환 : Attack -> Move");
            anim.SetTrigger("Move");
            //타이머 초기화
            timer = 0f;
        }
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            timer += Time.deltaTime;
            if (timer > attTime)
            {
                //일정 시간마다 플레이어를 공겨하기
                attackAnimEnd = false;
                agent.SetDestination(player.transform.position);
                int rand = Random.Range(0, 2);
                anim.SetInteger("ATTACKNUM", rand);
                print("어택넘" + rand);
                anim.SetTrigger("Attack");
                print("공격");
                //플레이어의 필요한 스크립트 컴포넌트를 가져와서 대미지를 주면 된다.
                //player.GetComponent<PlayerMove>().hitDamage(power);
                //타이머 초기화
                timer = 0f;
            }
        }
    }

    //플레이어 쪽에서 충돌 감지를 할 수 있으니 이 함수는 퍼블릭으로 만들자.
    public void HitDamage(Vector3 pos , Vector3 normal , int value)
    {
        //예외처리
        //피격 상태 이거나, 죽은 상태 일 때는 대미지 중첩으로 주지 않는다.
        if (state != EnemyState.DAMAGED && state != EnemyState.DIE && state != EnemyState.DIE_FIRE)
        {
            //체력깎기
            print("좀비 공격당함");
            hp -= value;
            timer = 0f;
            attackAnimEnd = true;

            //몬스터의 체력이 1 이상이면 피격 상태
            if (hp > 0)
            {
                //중간 보스일 경우 피격모션이 없기 때문에 0.5초간 무적 판정만 준다.
                if (!isTypeMiniBoss)
                {
                    anim.SetTrigger("Damaged");
                }
                StartCoroutine(Wait(state));
            }
            //0 이하이면 죽음 상태
            else
            {
                state = EnemyState.DIE;
                print("상태 전환 : AnyState -> Die");
                anim.SetTrigger("Die");
            }

            pos = hitPoint.transform.position;
            Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, normal);
            //혈흔 효과 생성 오브젝트 풀링
            if (PlayerInput.Instance.isEnchanted)
            {
                hitByFire = true;
                var _hit = PoolingManager.Instance.GetFireHit();
                if (_hit != null)
                {
                    _hit.transform.position = pos;
                    _hit.transform.rotation = rot;
                    _hit.SetActive(true);
                }
            }
            else
            {
                var _hit = PoolingManager.Instance.GetNormalHit();
                if (_hit != null)
                {
                    _hit.transform.position = pos;
                    _hit.transform.rotation = rot;
                    _hit.SetActive(true);
                }
            }

            if (PlayerInput.Instance.isEnchanted)
            {
                //시꺼멓게 타는 마테리얼로 바꾸는 로직인데 작동을 안한다.
                //bodyMaterial.GetComponent<SkinnedMeshRenderer>().materials[0] = body;
                //bodyMaterial.GetComponent<SkinnedMeshRenderer>().materials[1] = body1;
            }

           
        }
    }

    public void HitDamage(int value,bool isFire)
    {
        if (state != EnemyState.DAMAGED && state != EnemyState.DIE && state != EnemyState.DIE_FIRE)
        {
            if(isTypeSuicide)
            {
                Suicide();
                return;
            }
            //체력깎기
            print("좀비 공격당함");
            hp -= value;
            timer = 0f;
            attackAnimEnd = true;
            hitByFire = isFire;

            //몬스터의 체력이 1 이상이면 피격 상태
            if (hp > 0)
            {
                //중간 보스일 경우 피격모션이 없기 때문에 0.5초간 무적 판정만 준다.
                if (!isTypeMiniBoss)
                {
                    anim.SetTrigger("Damaged");
                }
                StartCoroutine(Wait(state));
            }
            //0 이하이면 죽음 상태
            else
            {
                state = EnemyState.DIE;
                print("상태 전환 : AnyState -> Die");
                anim.SetTrigger("Die");
            }
        }
    }

    public void HitStateSwitch()
    {
        if (state != EnemyState.DAMAGED)
        {
            state = EnemyState.DAMAGED;
        }
        else state = EnemyState.MOVE;
    }

    private void Die()
    {
        HitBox.gameObject.SetActive(false);
        agent.enabled = false;
        if (!hitByFire)
        {
            reviveTimer += Time.deltaTime;
            if (reviveTimer > reviveTime)
            {
                state = EnemyState.REVIVE;
                anim.SetTrigger("Revive");
            }
        }
        else
        {
            //불에 타죽었을 때, 일정확률로 포션 획득
            state = EnemyState.DIE_FIRE;
            PlayerInput.Instance.killCount++;
            //체력 흡수 
            int rand = Random.Range(0, 7);
            if (rand == 7)
            {
                PlayerInput.Instance.hpPotionCap++;
                var _gain = PoolingManager.Instance.GetPotionGain();
                if (_gain != null)
                {
                    _gain.transform.position = hitPoint.transform.position;
                    //hp소량회복
                    if(PlayerInput.Instance.hp <= 95)
                    {
                        PlayerInput.Instance.hp += 5;
                    }
                    //_gain.transform.rotation = Quaternion.identity;
                    _gain.SetActive(true);
                }
            }

        }
        //진행 죽인 모든 코루틴은 정지한다
        //StopAllCoroutines();
        //StartCoroutine(DieProc());
    }

    IEnumerator DieProc()
    {
        //캐릭터 컨트롤러 비활성화

        agent.enabled = false;
        //2초 후에 자기 자신을 제거한다.
        yield return new WaitForSeconds(2.0f);
        print("적 사망!");
    }

    public void Revive()
    {
        agent.enabled = true;
        canAttack = true;
        timer = 0f;
        reviveTimer = 0f;
        hp = 100;
        anim.SetTrigger("Idle");
        state = EnemyState.IDLE;
    }

    private void OnDrawGizmos()
    {
        //공격 가능 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        //플레이어 찾을 수 있는 범위
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, findRange);

        //이동 가능 최대 범위
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startPoint, moveRange);
    }

    public void CanAttack()
    {
        HitBox.canAttack = true;
    }

    public void CannotAttack()
    {
        HitBox.canAttack = false;
        attackAnimEnd = true;
    }

    public void Suicide()
    {
        GameObject explosion = Instantiate(explosionPrefab);
        explosion.transform.position = this.transform.position;
        explosion.transform.position += new Vector3(0, 0.2f, 0);
        GameObject blood = Instantiate(bloodPrefab);
        blood.transform.position = this.transform.position;
        if(Vector3.Distance(this.transform.position,PlayerInput.Instance.transform.position) < 2f)
        {
            //충돌 시 방향 벡터 회전값
            //Vector3 normal = Vector3.Cross(this.transform.position, PlayerInput.Instance.transform.position);
            //Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, normal);
            PlayerInput.Instance.PlayerDamage(50, "KNOCKBACK");
        }
        Destroy(explosion, 3f);
        PlayerInput.Instance.killCount++;
        this.gameObject.SetActive(false);
    }

    IEnumerator Wait(EnemyState _state)
    {
        state = EnemyState.DAMAGED;
        yield return new WaitForSeconds(0.2f);
        if(_state == EnemyState.ATTACK)
        {
            timer = 2f;
            state = EnemyState.ATTACK;
            print("상태 전환 : Move -> Attack");
           
        }
        else if (_state == EnemyState.MOVE)
        {
            timer = 0f;
            state = EnemyState.MOVE;
            print("상태 전환 : Attack -> Move");
            if (!isTypeMiniBoss)anim.SetTrigger("Move");           
        }
    }

}
