using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sideMoveSpeed = 4f;
    public float rotateSpeed = 180f;

    private bool isRollMove = false;
    private PlayerInput playerInput;
    private Rigidbody playerRigidbody;
    private CharacterController cc;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        //턴로직 사용 보류
        //if(playerInput.turn)
        //{
        //    Turn();
        //}
       
        //입력값에 따라 애니메이터의 Move 파라미터값 변경
        if(playerInput.state == PlayerInput.PlayerState.IDLE)
        {
            if(playerInput.move != 0 || playerInput.sideMove != 0)
            {
                playerInput.state = PlayerInput.PlayerState.MOVE;
            }
        }

        if(playerInput.state == PlayerInput.PlayerState.MOVE)
        {
            MoveSide();
            MoveForward();
        }

        if(playerInput.guard)
        {
            if(playerInput.state != PlayerInput.PlayerState.ROLL || playerInput.state != PlayerInput.PlayerState.ATTACK)
            {
                playerInput.state = PlayerInput.PlayerState.GUARD;           
            }
        }

        if(playerInput.state == PlayerInput.PlayerState.ROLL && isRollMove)
        {
            RollForward();
        }


        playerInput.playerAnim.SetFloat("Move", playerInput.move);
        playerInput.playerAnim.SetFloat("SideMove", playerInput.sideMove);
        playerInput.playerAnim.SetBool("SideMoveTrigger", playerInput.sideMoveTrigger);
        //playerAnimator.SetBool("TurnTrigger", playerInput.turn);

    }

    private void RollForward()
    {
        //앞구르기
        Vector3 moveDistance = transform.forward * 5f * Time.deltaTime;
        cc.Move(moveDistance);

       // Vector3 moveDistance = transform.forward * 5f * Time.deltaTime;
       // cc.Move(moveDistance);
    }

    private void Turn()
    {
        StartCoroutine(TurnLogic());
    }

    private void MoveSide()
    {
        //상대적 이동 거리 계산
        Vector3 moveDistance = playerInput.sideMove * transform.right * moveSpeed * Time.deltaTime;
        cc.Move(moveDistance);
        //playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);
    }

    // Update is called once per frame
    private void MoveForward()
    {
        //상대적 이동 거리 계산
        Vector3 moveDistance = playerInput.move * transform.forward * moveSpeed * Time.deltaTime;
        cc.Move(moveDistance);
        //리지드 바디로 이동
        //이렇게 할 경우 벽을 뚫거나 하는 것을 방지할 수 있다.(물리효과를 적용하기 때문에)
        //playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);
    }

    public void RollMoveSwitch()
    {
        if (isRollMove)
        {
            isRollMove = false;
        }
        else isRollMove = true;
    }

    IEnumerator TurnLogic()
    {
        yield return new WaitForSeconds(0.1f);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, -transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
