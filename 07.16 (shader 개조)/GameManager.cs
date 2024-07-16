using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Button TakeDamageBtn;
    public Button CharacterChangeBtn;
    public Button ChangeGrayBtn;
    public GameObject HitTarget;
    Color hitColor = new Color(0.8f, 0.3f, 0.35f, 1);
    float timer = 0.0f;
    Animator anim;

    MeshRenderer[] meshRenderers;   //������ meshrenderer ���

    public static Shader cubeDefaultShader;   //�⺻ ���̴�
    public static Shader myTexAddColorShader; //���� ���̴�

    //ĳ���� ��ȯ�� ���� ����
    bool isCatImg = true;

    //ȸ�� ��ȯ
    GameObject hero = null;
    SkinnedMeshRenderer[] skinnedMRs = null;

    public static Shader defaultShader = null;  //hero �⺻ ���̴�
    public static Shader grayShader = null;     //������ ���̴�
    bool isStone = false;

    public Slider alphaSlider = null;
    float alphaVal = 1.0f;
    

    void Start()
    {
        if (HitTarget != null)
            meshRenderers = HitTarget.GetComponentsInChildren<MeshRenderer>();

        if (TakeDamageBtn != null)
            TakeDamageBtn.onClick.AddListener(TakeDamageClick);

        //cubeDefaultShader = Shader.Find("Unlit/Transparent");
        if (meshRenderers != null && 0 < meshRenderers.Length)
            cubeDefaultShader = meshRenderers[0].material.shader;

        myTexAddColorShader = Shader.Find("Custom/MyTexAddColor"); // Resources �ȿ� �־����

        anim = HitTarget.GetComponent<Animator>();

        if (CharacterChangeBtn != null)
            CharacterChangeBtn.onClick.AddListener(ChrImgChangeClick);


        //���� ĳ���� ȸ�� ó�� 
        if (ChangeGrayBtn != null)
            ChangeGrayBtn.onClick.AddListener(ChangeGrayClick);

        hero = GameObject.Find("Pc_Jojo_Skin_01 (1)");

        if (hero != null)
            skinnedMRs = hero.GetComponentsInChildren<SkinnedMeshRenderer>();

        if (skinnedMRs != null && skinnedMRs.Length > 0)
            defaultShader = skinnedMRs[0].material.shader;

        grayShader = Shader.Find("Custom/MyGrayTransparent");

        if (alphaSlider != null)
        {
            alphaSlider.onValueChanged.AddListener(OnAlphaSlider);
            alphaSlider.value = alphaVal;
        }            
    }



    // Update is called once per frame
    void Update()
    {
        UpdateTakeDmg();
    }

    private void TakeDamageClick()
    {
        timer = 0.1f;

        //�÷��̽� �����Ǵ� ���纻 ���׸���(instance)�� �����Ͽ� ���̴� ���� 
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            if (myTexAddColorShader != null)
            {
                if (meshRenderers[i].material.shader != myTexAddColorShader)
                    meshRenderers[i].material.shader = myTexAddColorShader;

                meshRenderers[i].material.SetColor("_AddColor", hitColor);
                anim.SetTrigger("IsHit");
            }
        }

        //if (HitTarget != null)
        //{

        //    HitTarget.GetComponent<MeshRenderer>().material.SetColor("_AddColor", hitColor);
        //    timer = 0.1f;
        //    anim.SetTrigger("IsHit");
        //}

        //���׸��� ������ �����Ͽ� �ش� ���׸��� ���� ���� ����
        //Project�� �ִ� ������ �����ϴ� ���̱� ������ �÷��̸� ���� ���氪�� ��������
        //Material material = Resources.Load("Materials/MyTexAddColorMtrl") as Material;
        //if (material != null)
        //{
        //    if (material.shader != GameManager.myTexAddColorShader)
        //        material.shader = GameManager.myTexAddColorShader;
        //    material.SetColor("_AddColor", hitColor);
        //}

    }

    void UpdateTakeDmg()
    {
        if (timer > 0.0f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0.0f)
            {
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    if (cubeDefaultShader != null)
                    {
                        if (meshRenderers[i].material.shader != cubeDefaultShader)
                            meshRenderers[i].material.shader = cubeDefaultShader;
                    }
                }
                timer = 0.0f;
            }
        }
    }

    private void ChrImgChangeClick()
    {
        isCatImg = !isCatImg;

        Texture tempSprite = null;
        if (isCatImg)
            tempSprite = Resources.Load("Images/m0423") as Texture;
        else
            tempSprite = Resources.Load("Images/m0367") as Texture;

        if (meshRenderers != null)
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                //ù��° �Ű������� ���̴����� ����ϴ� ������
                meshRenderers[i].material.SetTexture("_MainTex", tempSprite);
                //meshRenderers[i].material.mainTexture = tempSprite;
            }
        }
    }
    private void ChangeGrayClick()
    {
        isStone = !isStone;

        if (isStone)    //�׷��� �� ����
        {
            Material[] mts;
            for (int i = 0; i < skinnedMRs.Length; i++)
            {
                mts = skinnedMRs[i].materials;
                for (int a = 0; a < mts.Length; a++)
                {
                    if (GameManager.grayShader != null && mts[a].shader != GameManager.grayShader)
                        mts[a].shader = GameManager.grayShader;
                }
            }
        }
        else
        {
            Material[] mts;
            for (int i = 0; i < skinnedMRs.Length; i++)
            {
                mts = skinnedMRs[i].materials;
                for (int a = 0; a < mts.Length; a++)
                {
                    if (GameManager.defaultShader != null && mts[a].shader != GameManager.defaultShader)
                        mts[a].shader = GameManager.defaultShader;
                }
            }
        }

    }

    private void OnAlphaSlider(float value)
    {
        alphaVal = value;
        if (isStone)
        {
            Material[] mts;
            for (int i = 0; i < skinnedMRs.Length; i++)
            {
                mts = skinnedMRs[i].materials;
                for (int a = 0; a < mts.Length; a++)
                {
                    mts[a].SetFloat("_AlphaValue", alphaVal);
                }
            }
        }
    }
}
