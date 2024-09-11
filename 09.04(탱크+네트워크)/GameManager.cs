using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //클라이언트 윈도우가 마우스 포커스를 가지고 있는지 확인하는 변수
    public static bool isFocus = true;
    private void Awake()
    {
        CreateTank();
        //포톤 클라우드의 네트워크 메세지 수신을 다시 연결
        PhotonNetwork.IsMessageQueueRunning = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateTank()
    {
        float pos = Random.Range(-100.0f, 100.0f);
        PhotonNetwork.Instantiate("Tank", new Vector3(pos, 20.0f, pos), Quaternion.identity);
    }

    private void OnApplicationFocus(bool focus)
    {
        isFocus = focus;

        //true : 이 창에 포커스를 가져왔다는 의미
        //false : 포커스를 잃었다는 의미 
    }
}
