using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ItemType
{
    HealPotion,
    Diamond
}

public class ItemCtrl : MonoBehaviour
{
    [HideInInspector] public PhotonView pv = null;
    int m_TakeHeroId = -1;  //이 아이템을 획득한 유저의 고유번호
    ItemType m_ItemType = ItemType.Diamond;
    float showOffTimer = 0.0f;  //아이템 먹기 중계 실패시 다시 획득할 수 있게 하기 위해
    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        //누군가 이미 획득했는데 아직도 이 아이템이 존재한다면
        if (pv != null && pv.IsMine && m_TakeHeroId >= 0)
            PhotonNetwork.Destroy(this.gameObject); //삭제 중계

        if (0.0f < showOffTimer)
        {
            showOffTimer -= Time.deltaTime;
            if (showOffTimer <= 0.0f)
            {
                EnableOnOff(true);  //아이템 먹기 중계 실패시 다시 획득할 수 있게 하기 위하여
            }
        }


        transform.Rotate(0.0f, Time.deltaTime * 100.0f, 0.0f);
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (PhotonNetwork.CurrentRoom == null) return;

        if (pv == null) return;

        if (coll.gameObject.tag == "Player")
        {
            Hero_Ctrl a_RefHero = coll.gameObject.GetComponent<Hero_Ctrl>();
            if (a_RefHero != null)
            {
                pv.RPC("OnTrigItemRPC", RpcTarget.AllViaServer, 
                    (int)a_RefHero.pv.Owner.ActorNumber, (int)m_ItemType);
            }

            EnableOnOff(false);
            showOffTimer = 5.0f;
        }
    }

    [PunRPC]
    public void OnTrigItemRPC(int a_HeroId, int itemType)
    {
        if (pv == null) return;

        if (!pv.IsMine) return;

        if (a_HeroId < 0) return;

        if (0 <= m_TakeHeroId) return;  //이미 누군가 주웠다는 의미

        m_TakeHeroId = a_HeroId;

        Hero_Ctrl a_RefHero = null;
        GameObject[] a_Heros = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject a_Hero in a_Heros)
        {
            a_RefHero = a_Hero.GetComponent<Hero_Ctrl>();
            if (a_RefHero.pv.Owner.ActorNumber == m_TakeHeroId)
            {
                a_RefHero.pv.RPC("TakeItemRPC", RpcTarget.AllViaServer, (int)m_ItemType);
                break;
            }
        }

        PhotonNetwork.Destroy(this.gameObject);
    }

    void EnableOnOff(bool a_IsOn)
    {
        MeshRenderer[] a_MeshList = gameObject.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < a_MeshList.Length; i++)
        {
            if (a_MeshList[i] == null)
                continue;

            a_MeshList[i].enabled = a_IsOn;
        }

        BoxCollider[] a_BoxColls = gameObject.GetComponentsInChildren<BoxCollider>();
        for (int i = 0; i < a_BoxColls.Length; i++)
        {
            if (a_BoxColls[i] == null)
                continue;
            a_BoxColls [i].enabled = a_IsOn;
        }
    }
}
