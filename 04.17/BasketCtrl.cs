using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketCtrl : MonoBehaviour
{
    [Header("Audio Clip")]
    public AudioClip appleSE;
    public AudioClip bombSE;
    AudioSource aud;

    new ParticleSystem particleSystem;

    LayerMask m_StageMask = -1;
    
    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        this.aud = GetComponent<AudioSource>();
        particleSystem = GetComponent<ParticleSystem>();

        //"Stage"��� �̸��� ���� ���̾��� ��ȣ�� ���������� Ŭ���ϱ� ���� �ɼ� ����
        m_StageMask = 1 << LayerMask.NameToLayer("Stage");
    }

    
    void Update()
    {
        if (GameManager.Inst.GameOverParent.activeSelf == false)
        {
            if(Input.GetMouseButtonDown(0)) 
            { 
                //���콺 Ŭ�� �ϴ� �������� ������ ������ ��
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit, Mathf.Infinity, m_StageMask.value))   //������ ���� ���Ѵ�� ��ڴٴ� �ǹ�
                {
                    float x = Mathf.RoundToInt(hit.point.x);
                    float z = Mathf.RoundToInt(hit.point.z);
                    transform.position = new Vector3(x, 0, z);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Contains("Apple"))
        {
            GameManager.Inst.GetApple();
            this.aud.PlayOneShot(this.appleSE);
            ParticleSystem.MainModule main = particleSystem.main;
            main.startColor = new Color(255, 255, 255, 255);
            particleSystem.Play();
            //���尡 �ߺ��Ǿ �ߵ��Ͽ��� ��
            //Play : ���� ���尡 �� ��� ���߰� �������� �ߵ��� ���� �����
            //PlayOneShot : ���尡 ������ �����. = �ߺ� ��� �ȴٴ� �ǹ�
        }
        else if (other.gameObject.tag.Contains("Bomb"))
        {
            GameManager.Inst.GetBomb();
            this.aud.PlayOneShot(this.bombSE);
            ParticleSystem.MainModule main = particleSystem.main;
            main.startColor = Color.black;
            particleSystem.Play();
        }
        Destroy(other.gameObject);
    }
}
