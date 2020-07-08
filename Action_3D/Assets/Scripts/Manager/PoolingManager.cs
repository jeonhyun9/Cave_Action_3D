using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{
    #region "싱글턴"
    private static PoolingManager instance = null;
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        NormalHitPooling();
        FireHitPooling();
        PotionGainPooling();
        FireBallPooling();
        FireExplosionPooling();
        FireWallPooling();
        BloodExplosionPooling();
    }

    public static PoolingManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }
    #endregion

    public int sizeMax = 10;                //기본 생성 크기
    public int sizeMax_Occasional = 4;       //가끔 쓰는 것들

    public GameObject normalHitPrefab;      //출혈
    public GameObject fireHitPrefab;        //출혈+불
    public GameObject potionGainPrefab;     //포션 획득 효과
    public GameObject fireBallPrefab;       //파이어볼
    public GameObject fireExplosionPrefab;  //파이어볼 폭발
    public GameObject fireWallPrefab;       //파이어월
    public GameObject bloodExplosionPrefab;  //피 폭발 

    public List<GameObject> normalHitPool = new List<GameObject>();
    public List<GameObject> fireHitPool = new List<GameObject>();
    public List<GameObject> potionGainPool = new List<GameObject>();
    public List<GameObject> fireBallPool = new List<GameObject>();
    public List<GameObject> fireExplosionPool = new List<GameObject>();
    public List<GameObject> fireWallPool = new List<GameObject>();
    public List<GameObject> bloodExplosionPool = new List<GameObject>();

    // Update is called once per frame
 
    public GameObject GetNormalHit()
    {
        for (int i = 0; i < sizeMax; i++)
        {
            if(normalHitPool[i].activeSelf ==false)
            {
                return normalHitPool[i];
            }
        }
        return null;
    }
    
    public GameObject GetFireHit()
    {
        for (int i = 0; i < sizeMax; i++)
        {
            if(fireHitPool[i].activeSelf == false)
            {
                return fireHitPool[i];
            }
        }
        return null;
    }

    public GameObject GetPotionGain()
    {
        //많이 나오는 파티클이 아니므로 4개만 생성
        for (int i = 0; i < sizeMax_Occasional; i++)
        {
            if(potionGainPool[i].activeSelf==false)
            {
                return potionGainPool[i];
            }
        }
        return null;
    }

    public GameObject GetFireball()
    {
        //4개 생성
        for (int i = 0; i < sizeMax_Occasional; i++)
        {
            if (fireBallPool[i].activeSelf == false)
            {
                return fireBallPool[i];
            }
        }
        return null;
    }

    public GameObject GetFireExplosion()
    {
        //4개 생성
        for (int i = 0; i < sizeMax_Occasional; i++)
        {
            if (fireExplosionPool[i].activeSelf == false)
            {
                return fireExplosionPool[i];
            }
        }
        return null;
    }

    public GameObject GetFireWall()
    {
        //4개 생성
        for (int i = 0; i < sizeMax_Occasional; i++)
        {
            if (fireWallPool[i].activeSelf == false)
            {
                return fireWallPool[i];
            }
        }
        return null;
    }

    public GameObject GetBloodExplosion()
    {
        //4개 생성
        for (int i = 0; i < sizeMax_Occasional; i++)
        {
            if (bloodExplosionPool[i].activeSelf == false)
            {
                return bloodExplosionPool[i];
            }
        }
        return null;

    }

    private void NormalHitPooling()
    {
        //피 프리팹 생성
        GameObject objectPools = new GameObject("normalHitPrefabs");

        //풀링 개수만큼 미리 피 이펙트 생성
        for (int i = 0; i < sizeMax; i++)
        {
            var obj = Instantiate<GameObject>(normalHitPrefab, this.transform);
            obj.name = "normal_" + i.ToString("00");
            //비활성화
            obj.SetActive(false);
            //리스트에 추가
            normalHitPool.Add(obj);
        }
    }

    private void FireHitPooling()
    {
        //피+불 프리팹 생성
        GameObject objectPools = new GameObject("fireHitPrefabs");

        //풀링 개수만큼 미리 피 이펙트 생성
        for (int i = 0; i < sizeMax; i++)
        {
            var obj = Instantiate<GameObject>(fireHitPrefab, this.transform);
            obj.name = "fire_" + i.ToString("00");
            //비활성화
            obj.SetActive(false);
            //리스트에 추가
            fireHitPool.Add(obj);
        }
    }

    private void PotionGainPooling()
    {
        //풀링 개수만큼 미리 포션추가 이펙트 생성
        for (int i = 0; i < sizeMax_Occasional; i++)
        {
            var obj = Instantiate<GameObject>(potionGainPrefab, this.transform);
            obj.name = "potionGain_" + i.ToString("00");
            //비활성화
            obj.SetActive(false);
            //리스트에 추가
            potionGainPool.Add(obj);
        }
    }

    private void FireBallPooling()
    {
        //풀링 개수만큼 미리 파이어 볼 이펙트 생성
        for (int i = 0; i < sizeMax_Occasional; i++)
        {
            var obj = Instantiate<GameObject>(fireBallPrefab, this.transform);
            obj.name = "fireBall_" + i.ToString("00");
            //비활성화
            obj.SetActive(false);
            //리스트에 추가
            fireBallPool.Add(obj);
        }
    }

    private void FireExplosionPooling()
    {
        //풀링 개수만큼 미리 폭발 이펙트 생성
        for (int i = 0; i < sizeMax_Occasional; i++)
        {
            var obj = Instantiate<GameObject>(fireExplosionPrefab, this.transform);
            obj.name = "fireExplosion_" + i.ToString("00");
            //비활성화
            obj.SetActive(false);
            //리스트에 추가
            fireExplosionPool.Add(obj);
        }
    }

    private void FireWallPooling()
    {
        //풀링 개수만큼 미리 파이어 월 이펙트 생성
        for (int i = 0; i < sizeMax_Occasional; i++)
        {
            var obj = Instantiate<GameObject>(fireWallPrefab, this.transform);
            obj.name = "fireWall_" + i.ToString("00");
            //비활성화
            obj.SetActive(false);
            //리스트에 추가
            fireWallPool.Add(obj);
        }
    }

    private void BloodExplosionPooling()
    {
        //피 폭발 개수만큼 미리 피 폭발 이펙트 생성
        for (int i = 0; i < sizeMax_Occasional; i++)
        {
            var obj = Instantiate<GameObject>(bloodExplosionPrefab, this.transform);
            obj.name = "bloodExplosion_" + i.ToString("00");
            //비활성화
            obj.SetActive(false);
            //리스트에 추가
            bloodExplosionPool.Add(obj);
        }
    }


}
