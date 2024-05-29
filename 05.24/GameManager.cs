using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //데미지 텍스트 관련 변수
    public Transform damageCanvas = null;   //유니티 연결용
    public GameObject damagePrefab = null;  //유니티 연결용
    GameObject damageClone;                 //복사본
    DamageCtrl damageText;                  //컴포넌트 받아오기용

    //코인 관련 변수
    public GameObject CoinPrefab;
    public static int gold = 0;
    HeroCtrl refHero = null;

    Vector3 startPos = Vector3.zero;

    //싱글턴 패턴
    public static GameManager inst;

    private void Awake()
    {
        inst = this;
    }
    void Start()
    {
        if (CoinPrefab == null)
            CoinPrefab = Resources.Load("Coin") as GameObject;

        refHero = GameObject.FindObjectOfType<HeroCtrl>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DamageText(int Value, Vector3 ownerPos, Color ownerColor)
    {
        damageClone = Instantiate(damagePrefab);
        if (damageClone != null && damageCanvas != null) 
        {
            startPos = new Vector3(ownerPos.x, ownerPos.y + 2.0f, ownerPos.z);
            damageClone.transform.SetParent(damageCanvas);
            damageText = damageClone.GetComponent<DamageCtrl>();
            damageText.DamageSpawn(Value, ownerColor);
            damageClone.transform.position = startPos;
        }

    }

    public void GoldDrop1(Vector3 spawnPos, float value)
    {
        if (CoinPrefab != null)
        {
            GameObject gold = Instantiate(CoinPrefab);
            gold.transform.position = spawnPos;
        }
    }
}
