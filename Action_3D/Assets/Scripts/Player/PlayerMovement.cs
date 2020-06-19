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
    
    private PlayerInput playerInput;
    private Rigidbody playerRigidbody;
    private Animator playerAnimator;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        //턴로직 사용 보류
        //if(playerInput.turn)
        //{
        //    Turn();
        //}
        MoveSide();
        MoveForward();
        
        //입력값에 따라 애니메이터의 Move 파라미터값 변경
        playerAnimator.SetFloat("Move", playerInput.move);
        playerAnimator.SetFloat("SideMove", playerInput.sideMove);
        playerAnimator.SetBool("SideMoveTrigger", playerInput.sideMoveTrigger);
        playerAnimator.SetBool("TurnTrigger", playerInput.turn);
    }

    private void Turn()
    {
        StartCoroutine(TurnLogic());
    }

    private void MoveSide()
    {
        //상대적 이동 거리 계산
        Vector3 moveDistance = playerInput.sideMove * transform.right * moveSpeed * Time.deltaTime;
        playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);
    }

    // Update is called once per frame
    private void MoveForward()
    {
        //상대적 이동 거리 계산
        Vector3 moveDistance = playerInput.move * transform.forward * moveSpeed * Time.deltaTime;
        //리지드 바디로 이동
        //이렇게 할 경우 벽을 뚫거나 하는 것을 방지할 수 있다.(물리효과를 적용하기 때문에)
        playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);
    }

    IEnumerator TurnLogic()
    {
        yield return new WaitForSeconds(0.1f);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, -transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
