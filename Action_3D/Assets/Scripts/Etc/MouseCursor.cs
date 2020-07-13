using UnityEngine;
using System.Collections;

public class MouseCursor : MonoBehaviour
{
    public Texture2D cursorTexture;
    //텍스처의 중심을 마우스 좌표로 할 것인지
    public bool hotSpotIsCenter = false;
    public Vector2 adjustHotSpot = Vector2.zero;
    private Vector2 hotSpot;
    public void Start()
    {
        StartCoroutine("MyCursor");
    }

    IEnumerator MyCursor()
    {
        //렌더링이 완료될 때까지 대기
        yield return new WaitForEndOfFrame();

        //텍스처의 중심을 마우스의 좌표로 사용하는 경우
        if (hotSpotIsCenter)
        {
            hotSpot.x = cursorTexture.width / 2;
            hotSpot.y = cursorTexture.height / 2;
        }
        else
        {
            hotSpot = adjustHotSpot;
        }
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
    }
}
