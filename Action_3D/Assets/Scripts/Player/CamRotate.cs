using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRotate : MonoBehaviour
{
    //카메라를 마우스 움직이는 방향으로 회전하기
    public float speed = 50;   //회전 속도 (Time.DeltaTime 을 통해 1초에 150도 회전)
    private PlayerInput playerInput;
    //회전 각도를 직접 제어하자
    float angleX, angleY;


    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        //transform.eulerAngles = new Vector3 (28,0,0);
    }

    // Update is called once per frame
    void Update()
    {
        CameraRotate();
    }

    private void CameraRotate()
    {
        float h = playerInput.mouseX;
        float v = playerInput.mouseY;

        //마우스 입력이 있을때만 카메라 회전
        if(h!= 0 && v!=0)
        {
            angleX = h * speed * Time.deltaTime;
            angleY = v * speed * Time.deltaTime;
            angleY = Mathf.Clamp(angleY, -60, 60);
            transform.eulerAngles += new Vector3(0, angleX, 0);
        }
    }
}
