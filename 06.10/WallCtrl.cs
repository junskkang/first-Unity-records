using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCtrl : MonoBehaviour
{
    //����ũ ��ƼŬ ������ ������ ����
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

    //�浹�� ������ �� �߻��ϴ� �̺�Ʈ
    void OnCollisionEnter(Collision coll)
    {
        //�浹�� ���ӿ�����Ʈ�� �±װ� ��
        if(coll.collider.tag == "BULLET")
        {
            //����ũ ��ƼŬ�� �������� ����
            GameObject spark = Instantiate(sparkEffect, coll.transform.position, Quaternion.identity);

            //ParticleSystem ������Ʈ�� ����ð�(duration)�� ���� �� ���� ó��
            Destroy(spark, spark.GetComponent<ParticleSystem>().main.duration + 0.2f);

            //�浹�� ���ӿ�����Ʈ ����
            BulletCtrl bulletCtrl = coll.collider.GetComponent<BulletCtrl>();
            //�浹�� �Ѿ� ����
            bulletCtrl.PushObjectPool(); //StartCoroutine(bulletCtrl.PushObjectPool(0));

        }
    }

    public void WallAlphaOnOff(bool isOn = true)
    {
        if (materials == null) return;

        if (isOn) //����ȭ
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
        else //������ȭ
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
