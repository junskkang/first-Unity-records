using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankDamage : MonoBehaviour
{
    //탱크 폭파 후 투명처리를 위한 MeshRenderer 컴포넌트 배열
    private MeshRenderer[] _renderers;

    //탱크 폭발 효과 프리팹을 연결할 변수
    private GameObject expEffect = null;

    //탱크의 초기 생명치
    private int initHp = 200;
    //탱크의 현재 생명치
    [HideInInspector] public int currHp = 0;

    //HUD에 연결할 변수
    public Canvas hudCanvas;
    public Image hpBarImg;
    Color startColor = Color.white;


    PhotonView pv = null;

    private void Awake()
    {
        //탱크 모델의 모든 MeshRenderer 컴포넌트를 추출한 후 배열에 할당
        _renderers = GetComponentsInChildren<MeshRenderer>();
        pv = GetComponent<PhotonView>();

        //현재 생명치를 초기 생명치로 초기값 설정
        currHp = initHp;
        //탱크 폭발시 생성시킬 폭발 효과를 로드
        expEffect = Resources.Load("ExplosionMobile") as GameObject;

        startColor = hpBarImg.color;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider coll)
    {
        //충돌한 Collider의 태그 비교
        if (currHp > 0 && coll.tag.Contains("Cannon"))
        {
            currHp -= 20;

            //현재 생명치 HUD에 표기
            hpBarImg.fillAmount = (float)currHp / (float)initHp;
            //생명 수치에 따른 체력바 색상 변경
            if (hpBarImg.fillAmount <= 0.5f)
                hpBarImg.color = Color.yellow;

            if (hpBarImg.fillAmount <= 0.3f)
                hpBarImg.color = Color.red;

            if (currHp <= 0)
            {
                StartCoroutine(this.ExplosionTank());
            }
        }
    }

    //폭발 효과 생성 및 리스폰 코루틴 함수
    IEnumerator ExplosionTank()
    {
        //폭발 효과 생성
        GameObject effect = Instantiate(expEffect, transform.position, Quaternion.identity);

        Destroy(effect, 3.0f);

        //HUD를 비활성화
        hudCanvas.enabled = false;

        //탱크 투명 처리
        SetTankVisible(false);
        //3초동안 기다렸다가 활성화하는 로직을 수행
        yield return new WaitForSeconds(5.0f);

        //HUD 초기화
        hpBarImg.fillAmount = 1.0f;
        hpBarImg.color = startColor;
        hudCanvas.enabled = true;

        //리스폰 시 생명 초기값 설정
        currHp = initHp;
        //탱크를 다시 보이게 처리
        SetTankVisible(true);
    }

    //MeshRenderer를 활성/비활성화하는 함수
    void SetTankVisible(bool isVisible)
    {
        foreach (MeshRenderer renderer in _renderers)
        {
            renderer.enabled = isVisible;
        }

        Rigidbody[] rigids = GetComponentsInChildren<Rigidbody>(true);
        foreach (Rigidbody rig in rigids)
        {
            rig.isKinematic = isVisible;
        }

        BoxCollider[] boxColliders = GetComponentsInChildren<BoxCollider>(true);
        foreach (BoxCollider boxCollider in boxColliders)
        {
            boxCollider.enabled = isVisible;
        }

        //if (isVisible && pv.IsMine)
        //{
        //    float pos = Random.Range(-100.0f, 100.0f);
        //    transform.position = new Vector3(pos, 20.0f, pos);
        //}
    }
}
