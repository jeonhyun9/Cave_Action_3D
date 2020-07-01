using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{
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
    }


    public static PoolingManager Instance
    {
        get
        {
            if(instance == null)
            {
                return null;
            }return instance;
        }
    }
    public int sizeMax = 10;
    public int sizeMaxPotionGain = 4;

    public GameObject normalHitPrefab;
    public GameObject fireHitPrefab;
    public GameObject potionGainPrefab;

    public List<GameObject> normalHitPool = new List<GameObject>();
    public List<GameObject> fireHitPool = new List<GameObject>();
    public List<GameObject> potionGainPool = new List<GameObject>();

    // Update is called once per frame
    void Update()
    {

    }

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
        for (int i = 0; i < sizeMaxPotionGain; i++)
        {
            if(potionGainPool[i].activeSelf==false)
            {
                return potionGainPool[i];
            }
        }
        return null;
    }

    private void NormalHitPooling()
    {
        //블러드 프리팹 생성
        GameObject objectPools = new GameObject("normalHitPrefabs");

        //풀링 개수만큼 미리 피 이펙트 생성
        for (int i = 0; i < sizeMax; i++)
        {
            var obj = Instantiate<GameObject>(normalHitPrefab, objectPools.transform);
            obj.name = "normal_" + i.ToString("00");
            //비활성화
            obj.SetActive(false);
            //리스트에 추가
            normalHitPool.Add(obj);
        }
    }

    private void FireHitPooling()
    {
        //블러드 프리팹 생성
        GameObject objectPools = new GameObject("fireHitPrefabs");

        //풀링 개수만큼 미리 피 이펙트 생성
        for (int i = 0; i < sizeMax; i++)
        {
            var obj = Instantiate<GameObject>(fireHitPrefab, objectPools.transform);
            obj.name = "fire_" + i.ToString("00");
            //비활성화
            obj.SetActive(false);
            //리스트에 추가
            fireHitPool.Add(obj);
        }
    }

    private void PotionGainPooling()
    {
        //포션 추가 이펙트 생성
        GameObject objectPools = new GameObject("potionGainPrefabs");

        //풀링 개수만큼 미리 피 이펙트 생성
        for (int i = 0; i < sizeMaxPotionGain; i++)
        {
            var obj = Instantiate<GameObject>(potionGainPrefab, objectPools.transform);
            obj.name = "fire_" + i.ToString("00");
            //비활성화
            obj.SetActive(false);
            //리스트에 추가
            potionGainPool.Add(obj);
        }
    }



}
