using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamRotate : MonoBehaviour
{
    //카메라를 마우스 움직이는 방향으로 회전하기
    public static float speed = 2;   //회전 속도 (Time.DeltaTime 을 통해 1초에 150도 회전)
    private PlayerInput playerInput;
    //회전 각도를 직접 제어하자
    float angleX, angleY;

    public Slider mouseSpeedSlider;

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
        MouseSpeedAdjustment();
    }

    private void CameraRotate()
    {
        float h = playerInput.mouseX;
        if(h!= 0&& PlayerInput.Instance.state != PlayerInput.PlayerState.HIT)
        {
            //angleX = h * speed * Time.deltaTime;
            ////angleY = v * speed * Time.deltaTime;
            //angleY = Mathf.Clamp(angleY, -60, 60);
            //transform.eulerAngles += new Vector3(0, angleX, 0);
            Vector3 rot = transform.rotation.eulerAngles; // 현재 카메라의 각도를 Vector3로 반환
            rot.y += Input.GetAxis("Mouse X") * speed; // 마우스 X 위치 * 회전 스피드
            Quaternion q = Quaternion.Euler(rot); // Quaternion으로 변환
            q.z = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, q, 2f);
        }
        
        //if(Input.GetMouseButton(1)==false)
        //{
        //    angleX = q * speed * Time.deltaTime;
        //    angleY = Mathf.Clamp(angleY, -60, 60);
        //    transform.eulerAngles += new Vector3(0, angleX, 0);
        //}
    }
    
    private void MouseSpeedAdjustment()
    {
        speed = mouseSpeedSlider.value;
    }
}
