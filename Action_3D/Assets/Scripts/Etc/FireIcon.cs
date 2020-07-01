using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireIcon : MonoBehaviour
{
    public int value;
    private Image image;
    private void Start()
    {
        image = GetComponent<Image>();
    }
    private void Update()
    {
        if (PlayerInput.Instance.mana >= value)
        {
            image.fillAmount = 1;
        }
        else image.fillAmount = 0;
    }
}
