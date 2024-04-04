using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameDirector : MonoBehaviour
{
    GameObject car;
    GameObject flag;
    GameObject distance;

    public Text[] Player;
    [HideInInspector] public int PlayerNum;

    public Button Replay_Btn;

    [HideInInspector] public float RecordLength;
    List<float> PlayerRecord = new List<float>();
    List<float> PlayerScore = new List<float>();


    void Start()
    {  
        if (Replay_Btn != null)
            Replay_Btn.onClick.AddListener(() =>
            { 
                
                SceneManager.LoadScene("GameScene");
            });
        
        this.car = GameObject.Find("car");    //유니티 상 오브젝트를 불러옴
        this.flag = GameObject.Find("flag");
        this.distance = GameObject.Find("Distance");  
    }

    // Update is called once per frame
    void Update()
    {
        float length = (this.flag.transform.position.x - this.car.transform.position.x);
        float AbLength = Mathf.Abs(length);

        this.distance.GetComponent<Text>().text = "Distance : " + AbLength.ToString("F2") + " m";
        RecordLength = AbLength;
    }

    public void Record()
    {
        this.Player[PlayerNum].GetComponent<Text>().text += RecordLength.ToString("F2") + "m";
        PlayerNum++;
        PlayerRecord.Add(RecordLength);
        PlayerScore.Add(RecordLength);

        if (PlayerScore.Count == 3)
            Judge();
    }

    void Judge()
    {
        PlayerScore.Sort();

        for(int i=0; i<Player.Length; i++)
        {
            if (PlayerScore[0] == PlayerRecord[i])
            {
                this.Player[i].GetComponent<Text>().text += " 1등";
            }
            else if (PlayerScore[1] == PlayerRecord[i])
            {
                this.Player[i].GetComponent<Text>().text += " 2등";
            }
            else if (PlayerScore[2] == PlayerRecord[i])
            {
                this.Player[i].GetComponent<Text>().text += " 3등";
            }
        }
        Replay_Btn.gameObject.SetActive(true);
    }
}
