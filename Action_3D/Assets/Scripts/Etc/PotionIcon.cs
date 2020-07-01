using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PotionIcon : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isHp;
    public bool isMp;
    private int value;
    private TextMeshProUGUI itemCapacity;
    private Image[] itemImage = new Image[2];
    void Start()
    {
        itemCapacity = GetComponentInChildren<TextMeshProUGUI>();
        itemImage = GetComponentsInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isHp)
        {
            value = PlayerInput.Instance.hpPotionCap;
            print(value);
        }
        else
        {
            value = PlayerInput.Instance.mpPotionCap;
        }

        //아이템 개수
        if(value == 0)
        {
            //0 -> 포션이 비어있는 이미지
            //1 -> 포션이 가득 찬 이미지
            itemImage[0].gameObject.SetActive(true);
            itemImage[1].gameObject.SetActive(false);
            itemCapacity.text = "";
        }
        else
        {
            itemImage[0].gameObject.SetActive(false);
            itemImage[1].gameObject.SetActive(true);
            itemCapacity.text = value.ToString();
        }

    }
}
