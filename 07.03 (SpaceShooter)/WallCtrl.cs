using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCtrl : MonoBehaviour
{
    //스파크 파티클 프리팹 연결할 변수
    public GameObject sparkEffect;

    public Material materials;

    FollowCam refCam;

    // Start is called before the first frame update
    void Start()
    {
        refCam = GameObject.FindObjectOfType<FollowCam>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!refCam.isBorder)
            gameObject.GetComponent<MeshRenderer>().material = materials;
    }

    //충돌이 시작할 때 발생하는 이벤트
    void OnCollisionEnter(Collision coll)
    {
        //충돌한 게임오브젝트의 태그값 비교
        if(coll.collider.tag == "BULLET")
        {
            //스파크 파티클을 동적으로 생성
            GameObject spark = Instantiate(sparkEffect, coll.transform.position, Quaternion.identity);

            //ParticleSystem 컴포넌트의 수행시간(duration)이 지난 후 삭제 처리
            Destroy(spark, spark.GetComponent<ParticleSystem>().main.duration + 0.2f);

            //충돌한 게임오브젝트 삭제
            BulletCtrl bulletCtrl = coll.collider.GetComponent<BulletCtrl>();
            //충돌한 총알 제거
            bulletCtrl.PushObjectPool(); //StartCoroutine(bulletCtrl.PushObjectPool(0));

        }
    }

    public void WallAlphaOnOff(bool isOn = true)
    {
        if (materials == null) return;

        if (isOn) //투명화
        {
            materials.SetFloat("_Mode", 3); //Transparent
            materials.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            materials.SetInt("_DstBlend", (int)(UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha));
            materials.SetInt("_ZWrite", 0);
            materials.DisableKeyword("_ALPHATEXT_ON");
            materials.DisableKeyword("_ALPHABLEND_ON");
            materials.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            materials.renderQueue = 3000;
            materials.color = new Color(1, 1, 1, 0.3f);
        }
        else //불투명화
        {
            materials.SetFloat("_Mode", 0); //Opaque
            materials.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            materials.SetInt("_DstBlend", (int)(UnityEngine.Rendering.BlendMode.Zero));
            materials.SetInt("_ZWrite", 1);
            materials.DisableKeyword("_ALPHATEXT_ON");
            materials.DisableKeyword("_ALPHABLEND_ON");
            materials.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            materials.renderQueue = -1;
            materials.color = new Color(1, 1, 1, 1);
        }
    }
}
