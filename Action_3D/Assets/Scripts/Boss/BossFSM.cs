using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BossFSM : MonoBehaviour
{
    enum BossState
    {
        IDLE,
        MOVE,
        PATTERN_CHOICE,
        TURN,
        SCREAM,
        PUNCH,
        JUMP,
        JUMP_ATTACK,
        PRE_BREATH,
        BREATH,
        SUMMON,
        DIE,
    }

    //자살 좀비를 소환할 위치 + 거리값 가진 구조체
    public struct GenPoint
    {
        public Transform transform;
        public float distance;
    }

    #region "보스 컴포넌트"
    public GameObject bossHitbox;
    public GameObject bloodSpray;
    public GameObject stomp;
    public GameObject bossHpPanel;
    public GameObject hitPoint;
    public GameObject bossMusic;
    public GameObject bossMoveSound;
    public AudioClip[] bossSoundClip;
    public GenPoint[] genPoint = new GenPoint[4];
    public GameObject suicideZombiePrefab;
    public Image hpBar;
    Animator bossAnim;
    NavMeshAgent bossAgent;
    Transform playerTr;
    CharacterController cc;
    AudioSource audioSource;
    AudioSource bossBgmSource;
    #endregion

    #region "보스 스탯 + 상태 변수"
    BossState state;
    float hp = 3000;
    float attackRange = 1.5f;
    float findRange = 5f;
    bool isCanJump = false;
    bool isCanHit = true;
    bool isStompCanHit = false;
    bool isFirstPattern = true;
    private float stompRange = 5f;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        state = BossState.IDLE;
        playerTr = GameObject.Find("Player").transform;
        bossAnim = GetComponent<Animator>();
        bossAgent = GetComponent<NavMeshAgent>();
        cc = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        bossBgmSource = bossMusic.transform.GetComponent<AudioSource>();

        //자살 좀비를 소환할 위치들의 트랜스폼을 가져온다.
        for (int i = 0; i < genPoint.Length; i++)
        {
            genPoint[i].transform = GameObject.Find("GenPoint" + i.ToString()).transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(bossHpPanel.activeSelf)
        {
            hpBar.fillAmount = hp / 3000;
        }
        if (state == BossState.MOVE && Time.timeScale == 1)
        {
            bossMoveSound.SetActive(true);
        }
        else bossMoveSound.SetActive(false);
        switch (state)
        {
            case BossState.IDLE:
                Idle();
                break;
            case BossState.PATTERN_CHOICE:
                PatternChoice();
                break;
            case BossState.MOVE:
                Move();
                break;
            case BossState.SCREAM:
                Scream();
                break;
            case BossState.PUNCH:
                Punch();
                break;
            case BossState.JUMP:
                Jump();
                break;
            case BossState.JUMP_ATTACK:
                JumpAttack();
                break;
            case BossState.BREATH:
                Breath();
                break;
            case BossState.DIE:
                Die();
                break;
            case BossState.TURN:
                Turn();
                break;
            case BossState.SUMMON:
                //소환 로직은 애니메이션 이벤트에서 실행
                break;
            case BossState.PRE_BREATH:
                Pre_Breath();
                break;
        }
    }

   

    private void Idle()
    {
        //플레이어 조우
        if (Vector3.Distance(transform.position, playerTr.position) < findRange
            && PlayerInput.Instance.state != PlayerInput.PlayerState.DIE)
        {
            state = BossState.PATTERN_CHOICE;
            bossHpPanel.SetActive(true);
            PlayerInput.Instance.isMeetBoss = true;
            bossMusic.SetActive(true);
        }
    }

    private void PatternChoice()
    {
        int pattern = 0;
        //랜덤 패턴 실행
        if (isFirstPattern)
        {
            //첫패턴은 비명 - > 점프 공격
            isFirstPattern = false;
            pattern = 2;
        }
        else
        {
            pattern = UnityEngine.Random.Range(0, 7);
            print("패턴번호" + pattern);
        }
        switch (pattern)
        {
            case 0:
                if (Vector3.Distance(transform.position, playerTr.position) <= attackRange)//공격 범위 안에 들어옴
                {
                    Vector3 direction = playerTr.position - this.transform.position;
                    float angle = Vector3.Angle(direction, this.transform.forward);
                    Debug.Log(angle);
                    //일정 각도 안에 있을 때만 패턴 실행
                    if (angle < 30)
                    {
                        StateChange("PUNCH");
                    }
                    else
                    {
                        //Debug.Log("플레이어가 보스 각도에서 벗어나 있음.");
                        StateChange("TURN");
                    }
                }
                else
                {
                    StateChange("MOVE");
                }
                break;
            case 1:
                StateChange("JUMP");
                break;
            case 2:
                StateChange("SCREAM");
                break;
            case 3:
                state = BossState.PRE_BREATH;
                bossAnim.SetTrigger("PRE_BREATH");
                break;
            case 4:
                StateChange("MOVE");
                break;
            case 5:
                if(suicideZombiePrefab.activeSelf)
                {
                    StateChange("TURN");
                }
                else StateChange("SUMMON");
                break;
            case 6:
                StateChange("JUMP");
                break;
        }
    }

    //애니메이션 이벤트에서 실행
    public void Summon()
    {
        float maxFarLength = 0f;
        int maxFarIdx = 0;
        for (int i = 0; i < genPoint.Length; i++)
        {
            genPoint[i].distance = (playerTr.position - genPoint[i].transform.position).sqrMagnitude;
            if(genPoint[i].distance >= maxFarLength)
            {
                maxFarLength = genPoint[i].distance;
                maxFarIdx = i;
            }
        }
        suicideZombiePrefab.transform.position = genPoint[maxFarIdx].transform.position;
        suicideZombiePrefab.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 2.5f);
    }

    private void Jump()
    {
        //점프로 땅찍기 회피 가능
        if (PlayerInput.Instance.state == PlayerInput.PlayerState.JUMP) return;
        if (stomp.activeSelf && isStompCanHit)
        {
            float distanceToPlayer = (playerTr.position - this.transform.position).sqrMagnitude;
            if (distanceToPlayer < stompRange)
            {
                playerTr.GetComponent<PlayerInput>().PlayerDamage(30, "KNOCKBACK");
            }
        }
    }

    private void Move()
    {
        if (Vector3.Distance(transform.position, playerTr.position) <= attackRange)//공격 범위 안에 들어옴
        {
            state = BossState.PUNCH;
            bossAnim.SetTrigger("PUNCH");
        }
        else
        {
            if (PlayerInput.Instance.state == PlayerInput.PlayerState.DIE)
            {
                state = BossState.IDLE;
                bossAnim.SetTrigger("IDLE");
            }
            else
            {
                bossAgent.SetDestination(playerTr.position);
            }
        }
    }

    private void Scream()
    {
        bossAgent.SetDestination(playerTr.position);
    }

    private void Punch()
    {
        if (Vector3.Distance(transform.position, playerTr.position) <= attackRange)//공격 범위 안에 들어옴
        {
            Vector3 direction = playerTr.position - this.transform.position;
            float angle = Vector3.Angle(direction, this.transform.forward);
            Debug.Log(angle);
            //일정 각도 안에 있을 때만 패턴 실행
            if (angle < 30)
            {
                StateChange("PUNCH");
            }
            else
            {
                StateChange("TURN");
            }
        }
    }

    private void JumpAttack()
    {
        if (isCanJump)
        {
            cc.SimpleMove(transform.forward);
        }
        if(stomp.activeSelf && isStompCanHit )
        {
            float distanceToPlayer = (playerTr.position - this.transform.position).sqrMagnitude;
            if(distanceToPlayer < stompRange)
            {
                playerTr.GetComponent<PlayerInput>().PlayerDamage(30, "KNOCKBACK");
            }
        }
    }

    private void Pre_Breath()
    {
        bossAgent.SetDestination(playerTr.position);
    }

    private void Breath()
    {
        Debug.DrawRay(bloodSpray.transform.position, bloodSpray.transform.forward * 5f, Color.green);
        RaycastHit hitInfo;
        if (bloodSpray.activeSelf)
        {
            if(Physics.Raycast(bloodSpray.transform.position, bloodSpray.transform.forward, out hitInfo, 3f,1<<13))
            {
                hitInfo.collider.gameObject.transform.GetComponent<PlayerInput>().PlayerDamage(30, "KNOCKBACK");
            }
        }
    }

    private void Die()
    {
        bossHitbox.SetActive(false);
        bossHpPanel.SetActive(false);
        bossAgent.enabled = false;
        if(PlayerInput.Instance.bossClear==false)
        {
            PlayerInput.Instance.bossClear = true;
        }
        //보스 음악 페이드아웃
        if(bossBgmSource.volume >= 0)
        {
            bossBgmSource.volume -= Time.deltaTime * 0.1f;
        }
    }

    private void Turn()
    {
        bossAgent.SetDestination(playerTr.position);
    }

    public void ToNextPattern()
    {
        StateChange("PATTERN_CHOICE");
    }

    public void TurnTrigger()
    {
        state = BossState.TURN;
    }

    //상태와 트리거 변경해주는 함수
    public void StateChange(string key)
    {
        //enum을 string으로 변환해줌
        state = (BossState)Enum.Parse(typeof(BossState), key);
        bossAnim.SetTrigger(key);
    }

    public void JumpAttackEnd()
    {
        Vector3 dir = playerTr.position - this.transform.position;
        transform.forward = dir;
        //3분의 1확률로 비명->점프공격을 한번 더 한다.
        int rand = UnityEngine.Random.Range(0, 3);
        if (rand == 0)
        {
            StateChange("SCREAM");
        }
        else
        {
            print("연속 점프 끝");
            state = BossState.PATTERN_CHOICE;
        }
    }

    public void JumpAttackTrigger()
    {
        state = BossState.JUMP_ATTACK;
    }

    public void PunchTrigger()
    {
        state = BossState.PUNCH;
    }

    public void BloodSpraySwitch()
    {
        if (bloodSpray.activeSelf)
        {
            bloodSpray.SetActive(false);
        }
        else bloodSpray.SetActive(true);
    }

    public void StompSwitch()
    {
        if (stomp.activeSelf)
        {
            stomp.SetActive(false);
        }
        else
        {
            //땅찍는 소리 재생
            audioSource.PlayOneShot(bossSoundClip[1], 3f);
            stomp.SetActive(true);
        }
    }

    public void CanJumpOn()
    {
        isCanJump = true;
    }
    public void CanJumpOff()
    {
        isCanJump = false;
    }

    //출혈 효과 + 대미지
    public void BossHitDamage(Vector3 pos , Vector3 normal, int value)
    {
        if (isCanHit)
        {
            StopAllCoroutines();
            hp -= value;

            if (hp > 0)
            {
                isCanHit = false;
                StartCoroutine(HitCoolTime());
            }
            else
            {
                isCanHit = false;
                state = BossState.DIE;
                bossAnim.SetTrigger("DIE");
                PlayerInput.Instance.killCount++;
                //보스 죽는 소리 출력
                audioSource.PlayOneShot(bossSoundClip[4], 1f);
                //보스 bgm 종료
            }

            pos = hitPoint.transform.position;
            //충돌 시 방향 벡터 회전값
            Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, normal);

            //혈흔 효과 생성 오브젝트 풀링
            if (PlayerInput.Instance.isEnchanted)
            {
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
        }

    }

    //출혈 효과 없이 보스 체력만 깎는 함수
    public void BossHitDamage(int value)
    {
        if (isCanHit)
        {
            hp -= value;

            if (hp > 0)
            {
              
            }
            else
            {
                isCanHit = false;
                state = BossState.DIE;
                bossAnim.SetTrigger("DIE");
                PlayerInput.Instance.killCount++;
                //보스 죽는 소리 출력
                audioSource.PlayOneShot(bossSoundClip[4], 1f);
                //보스 bgm 종료
            }
        }
    }

    public static float GetAngle(Vector3 vStart, Vector3 vEnd)
    {
        Vector3 v = vEnd - vStart;

        return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
    }

    IEnumerator HitCoolTime()
    {
        yield return new WaitForSeconds(0.5f);
        isCanHit = true;
    }
    
    public bool GetCanHit()
    {
        return isCanHit;
    }

    public void HitBoxOn()
    {
       bossHitbox.SetActive(true);
    }

    public void HitBoxOff()
    {
        bossHitbox.SetActive(false);
    }

    public void StompHitSwitch()
    {
        if (isStompCanHit)
        {
            isStompCanHit = false;
        }
        else isStompCanHit = true;
    }

    public void PlayScreamSound()
    {
        //비명 사운드 출력
        audioSource.PlayOneShot(bossSoundClip[0], 0.6f);
    }

    public void PlaySpraySound()
    {
        //피 토하는 사운드 출력
        audioSource.PlayOneShot(bossSoundClip[2], 2f);
    }

    public void PlaySwingSound()
    {
        //주먹 휘두르는 사운드 출력
        audioSource.PlayOneShot(bossSoundClip[3], 1f);
    }
}
