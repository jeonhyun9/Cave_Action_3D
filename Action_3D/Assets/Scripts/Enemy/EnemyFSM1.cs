using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

//몬스터 유한 상태 머신
public class EnemyFSM1 : MonoBehaviour
{
    //몬스터 상태 ENUM
    enum EnemyState
    {
        IDLE,
        MOVE,
        ATTACK,
        RETURN,
        DAMAGED,
        DIE,
    }

    //애니메이션을 제어하기 위한 애니메이터 컴포넌트
    Animator anim;

    EnemyState state; //몬스터 상태 변수

    //유용한 기능
    #region "IDLE 상태에 필요한 변수들"
    #endregion

    #region "MOVE 상태에 필요한 변수들"
    #endregion

    #region "ATTACK 상태에 필요한 변수들"
    #endregion

    #region "RETURN 상태에 필요한 변수들"
    #endregion

    #region "DAMAGED 상태에 필요한 변수들"
    #endregion

    #region "DIE 상태에 필요한 변수들"
    #endregion

    #region "IDLE 상태에 필요한 변수들"
    #endregion

    //필요한 변수들
    public float findRange = 15f;   //플레이어를 찾는 범위
    public float moveRange = 30f;   //시작지점에서 최대 이동 가능한 범위
    public float attackRange = 2f;  //공격 가능 범위
    private bool isAttakEnd = true;
    Vector3 startPoint;             //몬스터 시작 위치
    //Quaternion startRotation;       //몬스터 시작 회전 값
    Transform player;               //플레이어를 찾기 위해
    CharacterController cc;
    NavMeshAgent agent;

    public GameObject HitBox;

    //몬스터 일반 변수
    int hp = 100;
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
        //캐릭터 컨트롤러 컴포넌트
        cc = GetComponent<CharacterController>();
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
            case EnemyState.RETURN:
                Return();
                break;
            case EnemyState.DAMAGED:
                Damaged();
                break;
        }
    }

 
    private void Idle()
    {

        //1. 플레이어와 일정 범위가 되면 이동상태로 변경 (탐지 범위)
        //- 플레이어 찾기 (gameobject.find("player")
        //- 일정거리 (거리비교,distanc, mganidd)
        //- 상태 변경 state = move;
        //- 상태 전환 출력
        //Debug.Log(distanceToPlayer);

        //Vector3 dir = (transform.position - player.position);
        //float distance = dir.magnitude;
        //if(distance < findRange)
        //{
        //
        //}

        if(Vector3.Distance(transform.position , player.position) < findRange)
        {
            state = EnemyState.MOVE;
            print("상태 전환 : Idle -> Move");

            //애니메이션
            anim.SetTrigger("Move");
        }
    }

    private void Move()
    {
        //1.플레이어를 향해 이동 후 공격 범위 안에 들어오면 공격 상태로 전환
        //2.플레이어를 추격하더라도 처음 위치에서 일정 범위를 넘어가면 되돌아가기
        //- 플레이어 처럼 캐릭터 컨트롤러를 이용하기
        //- 공격 범위 1미터
        //- 상태 변경
        //- 상태 전환 출력
        //이동 중 이동할 수 있는 최대 범위에 들어왔을 때
        if(Vector3.Distance(transform.position,startPoint) > moveRange)
        {
            state = EnemyState.RETURN;
            print("상태 전환 : Move - > Return");
        }
        //리턴 상태가 아니면 플레이어를 추격해야 한다.
        else if (Vector3.Distance(transform.position, player.position) > attackRange)
        {
            //플레이어를 추격
            //이동 방향 (벡터의 뺄셈)
            Vector3 dir = (player.position - transform.position).normalized;
            //
            //좀 더 자연스럽게 회전 처리를 하고 싶다.
            ////transform.forward = Vector3.Lerp(transform.forward, dir, 10 * Time.deltaTime);
            ////타겟과 본인이 일직선상일 경우 백덤블링으로 회전한다.
            //
            //최종적으로 자연스런 회전처리를 하려면 결국 쿼터니온을 사용해야 한다.
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
            //
            ////문제점 : 몬스터가 백스텝으로 쫓아온다.
            ////몬스터가 타겟을 바라보도록 하자
            ////transform.forward = dir;
            ////캐릭터 컨트롤러를 이용해서 이동하기
            ////중력이 적용 안되는 문제가 있다.
            ////중력 문제를 해결하기 위해서 심플 무브를 사용한다.
            ////심플 무브는 최소한의 물리가 적용되어 중력문제를 해결할 수 있다.
            ////단 내부적으로 시간처리를 하기 때문에 Time.deltaTime을 사용하지 않는다.
            //cc.SimpleMove(dir * speed);
            agent.SetDestination(player.transform.position);
        }
        else //공격 범위 안에 들어옴
        {

            state = EnemyState.ATTACK;
            print("상태 전환 : Move -> Attack");
            timer = 2f;
        }
    }

    private void Attack()
    {
        if (Vector3.Distance(transform.position, player.position) > attackRange + 0.5f)//현재 상태를 무브로 전환하기 (재추격)
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
                transform.LookAt(player);
                //일정 시간마다 플레이어를 공겨하기
                anim.SetTrigger("Attack");
                print("공격");
                //플레이어의 필요한 스크립트 컴포넌트를 가져와서 대미지를 주면 된다.
                //player.GetComponent<PlayerMove>().hitDamage(power);
                //타이머 초기화
                timer = 0f;
            }
        }
    }

    private void Return()
    {
        //1. 몬스터가 플레이어를 추격하더라도 처음 위치에서 일정 범위를 벗어나면 복귀
        //- 처음위치에서 일정 범위
        //- 상태 변경
        //- 상태 전환 출력
        //시작 위치까지 도달하지 않을 때는 이동
        //도착하면 대기 상태로 변경
        if(Vector3.Distance(transform.position,startPoint) > 0.1f)
        {
            Vector3 dir = (startPoint - transform.position).normalized;
            cc.SimpleMove(dir * speed);
            //최종적으로 자연스런 회전처리를 하려면 결국 쿼터니온을 사용해야 한다.
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
            anim.SetTrigger("Return");
        }
        else
        {
            //위치값을 초기 값으로 
            transform.position = startPoint;
            transform.rotation = Quaternion.identity;
            state = EnemyState.IDLE;
            anim.SetTrigger("Idle");
            print("상태 전환 : Return -> Idle");
        }
    }

    //플레이어 쪽에서 충돌 감지를 할 수 있으니 이 함수는 퍼블릭으로 만들자.
    public void HitDamage(int value)
    {
        //예외처리
        //피격 상태 이거나, 죽은 상태 일 때는 대미지 중첩으로 주지 않는다.
        if (state == EnemyState.DAMAGED || state == EnemyState.DIE) return;
        
        //체력깎기
        hp -= value;

        //몬스터의 체력이 1 이상이면 피격 상태
        if(hp > 0)
        {
            state = EnemyState.DAMAGED;
            print("상태 전환 : AnyState -> Damaged");
            print("HP : " + hp);
            anim.SetTrigger("Damaged");
            Damaged();
        }
        //0 이하이면 죽음 상태
        else
        {
            state = EnemyState.DIE;
            print("상태 전환 : AnyState -> Die");
            anim.SetTrigger("Die");
            Die();
        }
    }

    private void Damaged()
    {
        //피격 상태를 처리하기 위한 코루틴을 실행한다.
        print("상태 전환 : Damaged -> Move");
        state = EnemyState.MOVE;
        //StartCoroutine(DamageProc());
    }

    IEnumerator DamageProc()
    {
        yield return new WaitForSeconds(0.5f);
        
        //현재 상태를 이동으로 변경
    }

    private void Die()
    {
        print("적 사망!");
        //진행 죽인 모든 코루틴은 정지한다
        StopAllCoroutines();
        StartCoroutine(DieProc());
    }

    IEnumerator DieProc()
    {
        //캐릭터 컨트롤러 비활성화
        cc.enabled = false;
        //2초 후에 자기 자신을 제거한다.
        yield return new WaitForSeconds(2.0f);
        print("적 사망!");
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

    public void AttackEnd()
    {
        isAttakEnd = true;
    }

    public void HitBoxSwitch()
    {
        if (HitBox.activeSelf)
        {
            HitBox.SetActive(false);
        }
        else HitBox.SetActive(true);
    }
}
