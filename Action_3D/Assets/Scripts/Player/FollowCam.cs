using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    //카메라가 플레이어를 따라다니기
    //플레이어한테 바로 카메라를 붙여서 이동해도 상관없다
    //하지만 게임에 따라서 드라마틱한 연출이 필요한 경우에
    //타겟을 따라다니도록 하는게 1인칭에서 3인칭으로 또는 그 반대로 변경이 쉽다.
    //또한 순간이동이 아닌 슈팅게임에서 꼬랑지가 따라다니는 것 같은 효과도 연출이 가능하다
    //지금은 우리 눈 역할을 할거라서 그냥 순간이동 시킨다.

    //private Transform target;    //카메라가 따라다닐 타겟
    public Transform target1st;
    public Transform target3rd;
    bool isFps = true;

    public float followSpeed = 10.0f;
 
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        //카메라 위치를 강제로 타겟 위치에 고정해둔다.
        //transform.position = target.position;

        ChangeView();

        //FollowCamera();
        
    }

    private void ChangeView()
    {
        if(Input.GetKeyDown("1"))
        {
            isFps = true;
        }
        if(Input.GetKeyDown("3"))
        {
            isFps = false;
        }

        if(isFps)
        {
            //카메라 위치 타겟위치에 고정
            transform.position = target1st.position;
        }
        else
        {
            //카메라 위치 타겟위치에 고정
            transform.position = target3rd.position;
            //transform.LookAt(target1st.position);
        }
    }


    //타겟을 따라다니기
    //private void FollowCamera()
    //{
    //    //타겟 방향 구하기 (벡터의 뺄셈)
    //    //방향 = 타겟 - 자기 자신
    //    Vector3 dir = target.position - transform.position;
    //    //방향으로 쓸 때는 노말라이즈 해야함.
    //    dir.Normalize();
    //    transform.Translate(dir * followSpeed * Time.deltaTime);
    //    if (first == false) transform.LookAt(player);
    //
    //    //문제점 : 타겟에 도착하면 덜덜덜 거림
    //    if (Vector3.Distance(transform.position, target.position) < 1.0f)
    //    {
    //
    //        transform.position = target.position;
    //    }
    //}
}
