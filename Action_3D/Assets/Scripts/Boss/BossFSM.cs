//using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TreeEditor;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

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
    Animator bossAnim;
    NavMeshAgent bossAgent;
    Transform playerTr;
    CharacterController cc;
    #endregion

    BossState state;
    float hp = 2000;
    float speed;
    float attackRange = 1.5f;
    float findRange = 5f;

    // Start is called before the first frame update
    void Start()
    {
        state = BossState.IDLE;
        playerTr = GameObject.Find("Player").transform;
        bossAnim = GetComponent<Animator>();
        bossAgent = GetComponent<NavMeshAgent>();
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
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



    private void Idle()
    {
        if(Vector3.Distance(transform.position, playerTr.position) < findRange
            && PlayerInput.Instance.state != PlayerInput.PlayerState.DIE)
        {
            state = BossState.PATTERN_CHOICE;
        }
    }

    private void PatternChoice()
    {
        int pattern = Random.Range(2, 3);
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
                state = BossState.JUMP_ATTACK;
                bossAnim.SetTrigger("JUMPATTACK");
                break;
            case 3:
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

    }

    private void Punch()
    {
       
    }

    private void JumpAttack()
    {
        Vector3 dir = playerTr.position - this.transform.position;
        transform.forward = dir;
        Vector3 moveDistance = transform.forward * 0.1f * Time.deltaTime;
        cc.Move(moveDistance);
    }

    private void Breath()
    {

    }

    private void Die()
    {

    }

    private void Turn()
    {
        bossAgent.SetDestination(playerTr.position);
    }

    public void ToNextPattern()
    {
        state = BossState.IDLE;
    }

    public void TurnTrigger()
    {
        state = BossState.TURN;
    }

    public void BossHitBoxOn()
    {
        bossHitbox.SetActive(true);
    }

    public void BossHitBoxOff()
    {
        bossHitbox.SetActive(false);
    }

    public void PunchTrigger()
    {
        state = BossState.PUNCH;
    }

    public static float GetAngle(Vector3 vStart, Vector3 vEnd)
    {
        Vector3 v = vEnd - vStart;

        return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
    }

}
