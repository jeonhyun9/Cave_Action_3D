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

    #region "좀비 스탯/상태 + 컴포넌트 변수"
    //애니메이션을 제어하기 위한 애니메이터 컴포넌트
    Animator anim;

    public float findRange = 15f;   //플레이어를 찾는 범위
    public float attackRange = 2f;  //공격 가능 범위
    public float moveRange = 30f;   //최대 이동 범위 (현재 사용 안함)

    Vector3 startPoint;             //몬스터 시작 위치
    //Quaternion startRotation;       //몬스터 시작 회전 값
    Transform player;               //플레이어 트랜스폼
    NavMeshAgent agent;             

    //몬스터 일반 변수
    public int hp = 100;

    public EnemyState state { get; private set; }//몬스터 상태 변수
    #endregion

    #region "자살 좀비일 경우의 변수"
    //자살 좀비일 경우
    public bool isTypeSuicide = false;
    public GameObject explosionPrefab;
    public GameObject bloodPrefab;
    public GameObject waringLight;
    public GameObject moveSound;
    public AudioClip suicideSound;
    #endregion

    #region "중간 보스일 경우의 변수"
    public bool isTypeMiniBoss = false;
    #endregion
 
    #region "피격 & 부활관련 변수들"
    private float reviveTimer = 0f;
    public float reviveTime = 5.0f;
    //불에 맞았는지 판별
    public bool hitByFire = false;
    //출혈효과를 생성할 좌표
    public GameObject hitPoint;
    //사망 효과음
    public GameObject dieSound;
    #endregion

    #region "공격 관련 변수들"

    //적 공격시의 히트박스
    public EnemyHitBox HitBox;
    //히트 박스가 플레이어에게 대미지를 줄 수 있는 시점 판별
    bool canAttack = true;

    //공격 사운드
    public GameObject randomAttackSound;

    //공격 딜레이
    float attTime = 2f; //2초에 한번 공격
    float timer = 0f;   //타이머

    //공격 애니메이션 끝났는지 판별
    private bool attackAnimEnd = true;
    #endregion

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
        anim = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        if(isTypeSuicide)
        {
            state = EnemyState.IDLE;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (state == EnemyState.MOVE && Time.timeScale == 1)
        {
            moveSound.SetActive(true);
        }
        else moveSound.SetActive(false);

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
        if (isTypeMiniBoss)
        {
            agent.SetDestination(player.transform.position);
        }
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
                int rand = Random.Range(0, 2);
                if(isTypeMiniBoss)
                {
                    anim.SetInteger("ATTACKNUM", rand);
                }
                anim.SetTrigger("Attack");
                //플레이어의 필요한 스크립트 컴포넌트를 가져와서 대미지를 주면 된다.
                //player.GetComponent<PlayerMove>().hitDamage(power);
                //타이머 초기화
                timer = 0f;
            }
        }
    }

    public void HitDamage(Vector3 pos , Vector3 normal , int value)
    {
        //예외처리
        //피격 상태 이거나, 죽은 상태 일 때는 대미지 중첩으로 주지 않는다.
        if (state != EnemyState.DAMAGED && state != EnemyState.DIE && state != EnemyState.DIE_FIRE && state != EnemyState.REVIVE)
        {
            EnemyState tempState = state;
            if (!isTypeMiniBoss) state = EnemyState.DAMAGED;
            //체력깎기
            print("좀비 공격당함");
            hp -= value;
            timer = 0f;
            attackAnimEnd = true;

            //몬스터의 체력이 1 이상이면 피격 상태
            if (hp > 0)
            {
                //중간 보스가 아닐 경우 애니메이션 트리거
                if (!isTypeMiniBoss)
                {
                    anim.SetTrigger("Damaged");
                }
                else StartCoroutine(Wait(tempState));
                //StartCoroutine(Wait(tempState));
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
            //if (PlayerInput.Instance.isEnchanted)
            //{
            //    //시꺼멓게 타는 마테리얼로 바꾸는 로직인데 작동을 안한다. 추후 적용
            //    //bodyMaterial.GetComponent<SkinnedMeshRenderer>().materials[0] = body;
            //    //bodyMaterial.GetComponent<SkinnedMeshRenderer>().materials[1] = body1;
            //}
        }
    }

    public void HitDamage(int value,bool isFire)
    {
        if (state != EnemyState.DAMAGED && state != EnemyState.DIE && state != EnemyState.DIE_FIRE && state != EnemyState.REVIVE)
        {
            EnemyState tempState = state;
            if(!isTypeMiniBoss)state = EnemyState.DAMAGED;
            if (isTypeSuicide)
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
                //중간 보스가 아닐 경우 애니메이션 트리거
                if (!isTypeMiniBoss)
                {
                    anim.SetTrigger("Damaged");
                }
                else StartCoroutine(Wait(tempState));
                //StartCoroutine(Wait(tempState));
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

    //피격상태 변경 - 애니메이션 이벤트에서 사용
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
        if(dieSound.activeSelf==false)
        {
            dieSound.SetActive(true);
        }
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
            int rand = Random.Range(0, 10);
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
        dieSound.SetActive(false);
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
        if(isTypeMiniBoss)
        {
            HitBox.isTypeMiniBoss = true;
        }
        HitBox.canAttack = true;
        randomAttackSound.SetActive(true);
    }

    public void CannotAttack()
    {
        HitBox.canAttack = false;
        attackAnimEnd = true;
        randomAttackSound.SetActive(false);
    }

    public void Suicide()
    {
        var _bloodExplosion = PoolingManager.Instance.GetBloodExplosion();
        if (_bloodExplosion != null)
        {
            _bloodExplosion.transform.position = this.transform.position;
            _bloodExplosion.transform.rotation = Quaternion.identity;
            _bloodExplosion.SetActive(true);
        }
        if (Vector3.Distance(this.transform.position, PlayerInput.Instance.transform.position) < 2f)
        {
            //충돌 시 방향 벡터 회전값
            //Vector3 normal = Vector3.Cross(this.transform.position, PlayerInput.Instance.transform.position);
            //Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, normal);
            PlayerInput.Instance.PlayerDamage(50, "KNOCKBACK");
        }
        PlayerInput.Instance.killCount++;
        this.gameObject.SetActive(false);
    }

    public void HitStateEnd()
    {
        if(timer == 0)
        {
            timer = 0f;
            randomAttackSound.SetActive(false);
            state = EnemyState.MOVE;
            print("상태 전환 : Attack -> Move");
            if (!isTypeMiniBoss) anim.SetTrigger("Move");
        }
        else
        {
            timer = 2f;
            randomAttackSound.SetActive(false);
            state = EnemyState.ATTACK;
            print("상태 전환 : Move -> Attack");
        }
    }

    IEnumerator Wait(EnemyState _state)
    {
        state = EnemyState.DAMAGED;
        yield return new WaitForSeconds(0.4f);
        if (_state == EnemyState.ATTACK)
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
            if (!isTypeMiniBoss) anim.SetTrigger("Move");
        }
    }

}
