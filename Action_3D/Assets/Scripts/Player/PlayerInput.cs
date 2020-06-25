﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour
{
    private static PlayerInput instance = null;

    private void Awake()
    {
        if(null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    
    public static PlayerInput Instance
    {
        get
        {
            if(null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    //플레이어 캐릭터 조작 위한 사용자 입력 감지
    //감지된 입력값을 다른 컴포넌트가 사용할 수 있도록 제공
    public string moveAxisName = "Vertical";
    public string sideMoveAxisName = "Horizontal";
    public string attackButtonName = "Attack";
    public string guardButtonName = "Guard";
    public string turnButtonName = "Turn";
    public string rollButtonName = "Roll";

    private PlayerMovement playerMovement;
    public GameObject sword;
    public GameObject trail;
    public float Hp = 100f;
    public float Stamina = 100f;
    public bool isAttackEnd;
    public bool isRollEnd;
    private bool isBackButtonInput = false;
    public Vector3 dir;
    public enum PlayerState
    {
        IDLE,
        MOVE,
        ATTACK,
        GUARD,
        ROLL,
        HIT,
        DIE,
    }

    //값 할당은 내부에서만 가능
    public Animator playerAnim { get; set; }
    public PlayerState state { get; set;}
    public float move { get; private set; }
    public float sideMove { get; private set; }
    public float mouseX { get; private set; }
    public float mouseY { get; private set; }
    public bool sideMoveTrigger { get; private set; }
    public bool attack { get; private set; }
    public bool guard { get; private set; }
    public bool turn { get; private set; }
    public bool roll { get; private set; }

    private void Start()
    {
        playerAnim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        state = PlayerState.IDLE;
        isAttackEnd = true;
        isRollEnd = true;
    }
    // Update is called once per frame
    void Update()
    {
        //입력 감지
        move = Input.GetAxis(moveAxisName);
        sideMove = Input.GetAxis(sideMoveAxisName);
        attack = Input.GetButton(attackButtonName);
        guard = Input.GetButtonDown(guardButtonName);
        turn = Input.GetButton(turnButtonName);
        roll = Input.GetButton(rollButtonName);
     
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
       
        //if ()

        if (state != PlayerState.HIT && state != PlayerState.DIE)
        {
            //안맞을 때 실행
            if (move == 0 && sideMove == 0 && isAttackEnd == true && isRollEnd == true)
            {
                //if(isBackButtonInput) transform.forward *= -1;
                isBackButtonInput = false;
                state = PlayerState.IDLE;
            }

            //공격 애니메이션이 끝난 상태일 때, 왼클릭 시 공격
            if (attack && isAttackEnd)
            {
                Attack();
            }

            //구르기 애니메이션이 끝난 상태일 때 , 스페이스 클릭시 구르기
            if (roll && isRollEnd)
            {
                Roll();
            }
        }

        //옆으로 이동 중인지 판별한다.
        if (sideMove != 0 && move == 0)
        {
            sideMoveTrigger = true;
        }
        else sideMoveTrigger = false;

    }

    private void Roll()
    {
        #region "구르기 로직"
        state = PlayerState.ROLL;
        isRollEnd = false;
        if(sideMove > 0)
        {
            transform.forward = transform.right;
        }
        if(sideMove > 0 && move > 0)
        {
            transform.forward = transform.forward - transform.right;
        }
        if(sideMove < 0)
        {
            transform.forward = -transform.right;
        }
        if(sideMove < 0 && move > 0)
        {
            transform.forward = transform.forward - (-transform.right);
        }
        if(move < 0)
        {
            isBackButtonInput = true;
            transform.forward *= -1;
        }
        if (sideMove > 0 && move < 0)
        {
            transform.forward = (-transform.forward) - transform.right;
        }
        if (sideMove < 0 && move < 0)
        {
            transform.forward = (-transform.forward) - (-transform.right);
        }
        if (isBackButtonInput) playerAnim.SetBool("ISBACKTURN", true);
        playerAnim.SetTrigger("ROLL");
        #endregion
    }

    public void RollEnd()
    {
        if(!isBackButtonInput)
        {
            isRollEnd = true;
        }
    }

    public void TurnEnd()
    {
        //transform.forward *= -1;
        isRollEnd = true;
        playerAnim.SetBool("ISBACKTURN", false);
        state = PlayerState.IDLE;
    }

    public void AttackEnd()
    {
        state = PlayerState.IDLE;
        isAttackEnd = true;
        sword.SetActive(false);
    }

    public void SwordColliderOn()
    {
        sword.SetActive(true);
    }

    private void Attack()
    {
        state = PlayerState.ATTACK;
        int rand = UnityEngine.Random.Range(1, 3);
        isAttackEnd = false;
        playerAnim.SetTrigger("ATTACK");
        playerAnim.SetInteger("ATTACKNUM", rand);
    }


    public void SwordTrailOn()
    {
        trail.SetActive(true);
    }

    public void SwordTrailOff()
    {
        trail.SetActive(false);
    }

    public void PlayerDamage(float value)
    {
        Hp -= value;
        playerAnim.SetTrigger("HIT");
        print("플레이어의 현재 HP" + Hp);
    }

}
