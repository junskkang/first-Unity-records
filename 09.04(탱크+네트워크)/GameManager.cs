using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Ŭ���̾�Ʈ �����찡 ���콺 ��Ŀ���� ������ �ִ��� Ȯ���ϴ� ����
    public static bool isFocus = true;
    private void Awake()
    {
        CreateTank();
        //���� Ŭ������ ��Ʈ��ũ �޼��� ������ �ٽ� ����
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

        //true : �� â�� ��Ŀ���� �����Դٴ� �ǹ�
        //false : ��Ŀ���� �Ҿ��ٴ� �ǹ� 
    }
}
