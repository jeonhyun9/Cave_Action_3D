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
        BITE,
        TURN,
        SCREAM,
        PUNCH,
        JUMP,
        JUMP_ATTACK,
        BREATH,
        DIE,
    }

    #region "컴포넌트"
    public GameObject bossHitbox;
    public GameObject bloodSpray;
    public GameObject stomp;
    public GameObject bossHpPanel;
    public GameObject hitPoint;
    public GameObject bossMusic;
    public GameObject bossMoveSound;
    public AudioClip[] bossSoundClip;
    public Image hpBar;
    Animator bossAnim;
    NavMeshAgent bossAgent;
    Transform playerTr;
    CharacterController cc;
    AudioSource audioSource;
    AudioSource bossBgmSource;
    #endregion

    BossState state;
    float hp = 3000;
    float speed;
    float attackRange = 1.5f;
    float findRange = 5f;
    bool isCanJump = false;
    bool isCanHit = true;
    bool isStompCanHit = false;
    private float stompRange = 5f;
    private float jumpAttackStompRange = 3f;
   
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
    }

    // Update is called once per frame
    void Update()
    {
        if(bossHpPanel.activeSelf)
        {
            hpBar.fillAmount = hp / 2000;
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
            case BossState.BITE:
                Bite();
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
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 2.5f);
    }

    private void Jump()
    {
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

    private void Idle()
    {
        //플레이어 조우
        if(Vector3.Distance(transform.position, playerTr.position) < findRange
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
        int pattern = UnityEngine.Random.Range(0, 5);
        print("패턴번호" + pattern);
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
                        state = BossState.PUNCH;
                        bossAnim.SetTrigger("PUNCH");
                    }
                    else
                    {
                        Debug.Log("Out");
                        state = BossState.TURN;
                        bossAnim.SetTrigger("TURN");
                    }
                }
                else
                {
                    state = BossState.MOVE;
                    bossAnim.SetTrigger("MOVE");
                }
                break;
            case 1:
                state = BossState.JUMP;
                bossAnim.SetTrigger("JUMP");
                break;
            case 2:
                StateChange("SCREAM");
                break;
            case 3:
                bossAgent.SetDestination(playerTr.position);
                state = BossState.BREATH;
                bossAnim.SetTrigger("BREATH");
                break;
            case 4:
                StateChange("MOVE");
                break;
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

    private void Bite()
    {

    }

    private void Scream()
    {
        bossAgent.SetDestination(playerTr.position);
    }

    private void Punch()
    {
       
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

    private void Breath()
    {
        Debug.DrawRay(bloodSpray.transform.position, bloodSpray.transform.forward * 3f, Color.green);
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

    public void StateChange(string key)
    {
        state = (BossState)Enum.Parse(typeof(BossState), key);
        bossAnim.SetTrigger(key);
    }

    public void JumpAttackEnd()
    {
        Vector3 dir = playerTr.position - this.transform.position;
        transform.forward = dir;
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

    public void CanJumpSwitch()
    {
        if(!isCanJump)
        {
            isCanJump = true;
        }
        else
        {
            isCanJump = false;
        }
    }

    public void BossHitDamage(Vector3 pos , Vector3 normal, int value)
    {
        if (isCanHit)
        {
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

    public void BossHitDamage(int value)
    {
        if (isCanHit)
        {
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
        }
    }

    public static float GetAngle(Vector3 vStart, Vector3 vEnd)
    {
        Vector3 v = vEnd - vStart;

        return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
    }

    IEnumerator HitCoolTime()
    {
        yield return new WaitForSeconds(0.3f);
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
        audioSource.PlayOneShot(bossSoundClip[0], 0.8f);
    }

    public void PlaySpraySound()
    {
        //피 토하는 사운드 출력
        audioSource.PlayOneShot(bossSoundClip[2], 3f);
    }

    public void PlaySwingSound()
    {
        //주먹 휘두르는 사운드 출력
        audioSource.PlayOneShot(bossSoundClip[3], 1f);
    }
}
