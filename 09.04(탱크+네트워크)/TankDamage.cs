using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankDamage : MonoBehaviour
{
    //��ũ ���� �� ����ó���� ���� MeshRenderer ������Ʈ �迭
    private MeshRenderer[] _renderers;

    //��ũ ���� ȿ�� �������� ������ ����
    private GameObject expEffect = null;

    //��ũ�� �ʱ� ����ġ
    private int initHp = 200;
    //��ũ�� ���� ����ġ
    [HideInInspector] public int currHp = 0;

    //HUD�� ������ ����
    public Canvas hudCanvas;
    public Image hpBarImg;
    Color startColor = Color.white;


    PhotonView pv = null;

    private void Awake()
    {
        //��ũ ���� ��� MeshRenderer ������Ʈ�� ������ �� �迭�� �Ҵ�
        _renderers = GetComponentsInChildren<MeshRenderer>();
        pv = GetComponent<PhotonView>();

        //���� ����ġ�� �ʱ� ����ġ�� �ʱⰪ ����
        currHp = initHp;
        //��ũ ���߽� ������ų ���� ȿ���� �ε�
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
        //�浹�� Collider�� �±� ��
        if (currHp > 0 && coll.tag.Contains("Cannon"))
        {
            currHp -= 20;

            //���� ����ġ HUD�� ǥ��
            hpBarImg.fillAmount = (float)currHp / (float)initHp;
            //���� ��ġ�� ���� ü�¹� ���� ����
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

    //���� ȿ�� ���� �� ������ �ڷ�ƾ �Լ�
    IEnumerator ExplosionTank()
    {
        //���� ȿ�� ����
        GameObject effect = Instantiate(expEffect, transform.position, Quaternion.identity);

        Destroy(effect, 3.0f);

        //HUD�� ��Ȱ��ȭ
        hudCanvas.enabled = false;

        //��ũ ���� ó��
        SetTankVisible(false);
        //3�ʵ��� ��ٷȴٰ� Ȱ��ȭ�ϴ� ������ ����
        yield return new WaitForSeconds(5.0f);

        //HUD �ʱ�ȭ
        hpBarImg.fillAmount = 1.0f;
        hpBarImg.color = startColor;
        hudCanvas.enabled = true;

        //������ �� ���� �ʱⰪ ����
        currHp = initHp;
        //��ũ�� �ٽ� ���̰� ó��
        SetTankVisible(true);
    }

    //MeshRenderer�� Ȱ��/��Ȱ��ȭ�ϴ� �Լ�
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
