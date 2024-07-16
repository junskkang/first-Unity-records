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

    MeshRenderer[] meshRenderers;   //상자의 meshrenderer 목록

    public static Shader cubeDefaultShader;   //기본 쉐이더
    public static Shader myTexAddColorShader; //변경 쉐이더

    //캐릭터 전환을 위한 변수
    bool isCatImg = true;

    //회색 전환
    GameObject hero = null;
    SkinnedMeshRenderer[] skinnedMRs = null;

    public static Shader defaultShader = null;  //hero 기본 쉐이더
    public static Shader grayShader = null;     //변경할 쉐이더
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

        myTexAddColorShader = Shader.Find("Custom/MyTexAddColor"); // Resources 안에 있어야함

        anim = HitTarget.GetComponent<Animator>();

        if (CharacterChangeBtn != null)
            CharacterChangeBtn.onClick.AddListener(ChrImgChangeClick);


        //조조 캐릭터 회색 처리 
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

        //플레이시 생성되는 복사본 메테리얼(instance)에 접근하여 쉐이더 변경 
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

        //메테리얼 원본에 접근하여 해당 매테리얼 값을 직접 변경
        //Project에 있는 원본을 변경하는 것이기 때문에 플레이를 꺼도 변경값이 남아있음
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
                //첫번째 매개변수는 쉐이더에서 사용하는 변수명
                meshRenderers[i].material.SetTexture("_MainTex", tempSprite);
                //meshRenderers[i].material.mainTexture = tempSprite;
            }
        }
    }
    private void ChangeGrayClick()
    {
        isStone = !isStone;

        if (isStone)    //그레이 색 적용
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
