using UnityEngine;
using DataInfo;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;
    
    private DataManager dataManager;
    public GameData gameData;

    private void Awake()
    {
        if (instance==null)
        {
            instance = this;
        }
        //instance에 할당된 클래스의 인스턴스가 다를 경우 새로 생성된 클래스를 의미
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }

        dataManager = GetComponent<DataManager>();
        dataManager.Initialize();

        LoadGameData();
    }

    void LoadGameData()
    {
        GameData data = dataManager.Load();

        gameData.TimeCount = data.TimeCount;
        gameData.killCount = data.killCount;
        gameData.TotalScore = data.TotalScore;

        //print("로드 실행됨 " + gameData.TimeCount + gameData.killCount + gameData.TotalScore);
    }

    public void KillCountPlus()
    {
        ++gameData.killCount;
    }

    public void SetTotalScore()
    {
        gameData.TotalScore = (gameData.killCount * 20000) / (gameData.TimeCount / 10);
    }

    public void SaveGameData()
    {
        dataManager.Save(gameData);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
