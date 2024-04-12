using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CloudWaveCtrl : MonoBehaviour
{
    GameObject player;
    float destroyDistance = 10.0f;   //주인공 아래로 10m

    public GameObject[] Clouds;      //연결할 구름 게임오브젝트 배열 선언
    public GameObject Fish; 
    void Start()
    {
        player = GameObject.Find("cat");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = player.transform.position;

        //구름이 주인공으로부터 10m 아래 거리이면 파괴
        if (transform.position.y < playerPos.y - destroyDistance)
        {
            Destroy(gameObject);
        }
    }

    public void SetHideCloud(int a_Count)
    {//a_Count 몇 개의 구름을 감출 것인지 
        List<int> active = new List<int>();     //상자 생성  
        for (int i =0; i < Clouds.Length; i++)  //상자에 구름갯수만큼 구슬 추가
        {
            active.Add(i);
        }

        for(int i = 0; i < a_Count; i++)             //제거할 갯수만큼 구슬 선택
        {
            int ran = Random.Range(0, active.Count); //구슬 갯수를 최댓값으로 하여 랜덤 추첨
            Clouds[active[ran]].SetActive(false);    //구슬 번호에 적힌 숫자에 해당하는 구름 제거

            active.RemoveAt(ran);                    //뽑은 구슬 제거
        }

        active.Clear();                              //구슬 비우기

        
        //활성화가 되어있는 구름 위에만 물고기 스폰시키기
        int range = 10;
        SpriteRenderer[] a_CloudObj = GetComponentsInChildren<SpriteRenderer>();
        //GetComponentsInChildren함수의 매개변수 기본값은 false 
        //false의 의미 : Active가 켜져있는 객체만 찾아온다.
        //true의 의미 : Active상태에 상관없이 해당 <T>값을 갖고 있으면 다 찾아옴
        
        //우리는 구름을 랜덤으로 Active상태로 만들어줬기 때문에 
        //그 위에 물고기를 스폰시키려면 false옵션을 쓰면 됨.
        for (int ii = 0; ii < a_CloudObj.Length; ii++)
        {
            if (Random.Range(0, range) == 0)    //10% 확률로
                SpawnFish(a_CloudObj[ii].transform.position);
        }
    }

    void SpawnFish(Vector3 a_Pos)
    {
        GameObject go = Instantiate(Fish);
        go.transform.position = a_Pos + Vector3.up*0.8f;
    }
}
