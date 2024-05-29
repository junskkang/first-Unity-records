using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //������ �ؽ�Ʈ ���� ����
    public Transform damageCanvas = null;   //����Ƽ �����
    public GameObject damagePrefab = null;  //����Ƽ �����
    GameObject damageClone;                 //���纻
    DamageCtrl damageText;                  //������Ʈ �޾ƿ����

    //���� ���� ����
    public GameObject CoinPrefab;
    public static int gold = 0;
    HeroCtrl refHero = null;

    Vector3 startPos = Vector3.zero;

    //�̱��� ����
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
