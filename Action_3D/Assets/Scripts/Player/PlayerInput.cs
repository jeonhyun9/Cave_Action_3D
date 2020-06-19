using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    //플레이어 캐릭터 조작 위한 사용자 입력 감지
    //감지된 입력값을 다른 컴포넌트가 사용할 수 있도록 제공
    public string moveAxisName = "Vertical";
    public string sideMoveAxisName = "Horizontal";
    public string attackButtonName = "Attack";
    public string guardButtonName = "Guard";
    public string turnButtonName = "Turn";

    private PlayerMovement playerMovement;

    //값 할당은 내부에서만 가능
    public float move { get; private set; }
    public float sideMove { get; private set; }
    public float mouseX { get; private set; }
    public float mouseY { get; private set; }
    public bool sideMoveTrigger { get; private set; }
    public bool attack { get; private set; }
    public bool guard { get; private set; }
    public bool turn { get; private set; }

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }
    // Update is called once per frame
    void Update()
    {
        //입력 감지
        move = Input.GetAxis(moveAxisName);
        sideMove = Input.GetAxis(sideMoveAxisName);
        attack = Input.GetButton(attackButtonName);
        guard = Input.GetButton(guardButtonName);
        turn = Input.GetButton(turnButtonName);
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        //옆으로 이동 중인지 판별한다.
        if (sideMove != 0)
        {
            sideMoveTrigger = true;
        }
        else sideMoveTrigger = false;

    }
}
