using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    //UI
    public Button silhouetteBtn;
    public Button redShaderBtn;
    public Button grayShaderBtn;
    public Slider alphaSlider;
    public Button postProcessBtn;
    public Slider intensitySlider;

    //적용 대상
    GameObject silhouetteTarget;
    SkinnedMeshRenderer[] silhouetteRenderers;
    GameObject redShaderTarget;
    SkinnedMeshRenderer[] redShaderRenderers;
    GameObject grayShaderTarget;
    SkinnedMeshRenderer[] grayShaderRenderers;
    MeshRenderer weaponRenderer;

    //쉐이더
    public static Shader silhouetteShaderDefault;
    public static Shader silhouetteShader;
    public static Shader redShaderDefault;
    public static Shader redShader;
    public static Shader grayShaderDefault;
    public static Shader grayShader;
    public static Shader weaponShaderDefault;

    //변수
    bool isRed = false;
    bool isGray = false;
    float alphaValue = 1.0f;
    bool isPostProcess = false;
    bool isSilhouette = false;

    void Start()
    {
        //실루엣 관련 연결
        silhouetteTarget = GameObject.Find("Player");

        if (silhouetteTarget != null)
            silhouetteRenderers = silhouetteTarget.GetComponentsInChildren<SkinnedMeshRenderer>();

        if (silhouetteRenderers != null && 0 < silhouetteRenderers.Length)
            silhouetteShaderDefault = silhouetteRenderers[0].material.shader;

        silhouetteShader = Shader.Find("SilhouetteShader");

        if (silhouetteBtn != null)
            silhouetteBtn.onClick.AddListener(SilhouetteOnOff);

        //레드 쉐이더 관련 연결
        redShaderTarget = GameObject.Find("monster");

        if (redShaderTarget != null)
            redShaderRenderers = redShaderTarget.GetComponentsInChildren<SkinnedMeshRenderer>();

        if (redShaderRenderers != null && redShaderRenderers.Length > 0)
            redShaderDefault = redShaderRenderers[0].material.shader;

        redShader = Shader.Find("Custom/RedShader");

        if (redShaderBtn != null)
            redShaderBtn.onClick.AddListener(RedShaderOnOff);

        //그레이 쉐이더, 알파 관련 연결
        grayShaderTarget = GameObject.Find("GrayPlayer");

        if (grayShaderTarget != null)
            grayShaderRenderers = grayShaderTarget.GetComponentsInChildren<SkinnedMeshRenderer>();

        if (grayShaderTarget != null)
            weaponRenderer = grayShaderTarget.GetComponentInChildren<MeshRenderer>();

        if (grayShaderRenderers != null && grayShaderRenderers.Length > 0)
            grayShaderDefault = grayShaderRenderers[0].material.shader;

        if (weaponRenderer != null)
            weaponShaderDefault = weaponRenderer.material.shader;

        grayShader = Shader.Find("Custom/GrayShader");

        if (grayShaderBtn != null)
            grayShaderBtn.onClick.AddListener(GrayShaderOnOff);

        if (alphaSlider != null)
        {
            alphaSlider.onValueChanged.AddListener(OnAlphaSlider);
            alphaSlider.value = alphaValue;
        }

        //포스트 프로세스 관련 연결
        if (postProcessBtn != null)
            postProcessBtn.onClick.AddListener(PostProcessOnOff);

        if (isPostProcess)
            Camera.main.GetComponent<PostProcessVolume>().enabled = true;
        else
            Camera.main.GetComponent<PostProcessVolume>().enabled = false;

        if (intensitySlider != null)
        {
            Bloom bloomLayer = null;
            PostProcessVolume volume = Camera.main.GetComponent<PostProcessVolume>();
            volume.profile.TryGetSettings(out bloomLayer);
            intensitySlider.onValueChanged.AddListener(OnIntensitySlider);
            intensitySlider.value = bloomLayer.dirtIntensity.value / 30.0f;
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SilhouetteOnOff()
    {
        isSilhouette = !isSilhouette;
        if (isSilhouette)
        {
            Material[] mts;
            for (int i = 0; i < silhouetteRenderers.Length; i++)
            {
                mts = silhouetteRenderers[i].materials;
                for (int a = 0; a < mts.Length; a++)
                {
                    if (GameManager.silhouetteShader != null && mts[a].shader != GameManager.silhouetteShader)
                    {
                        mts[a].shader = GameManager.silhouetteShader;
                    }
                }
            }
        }
        else
        {
            Material[] mts;
            for (int i = 0; i < silhouetteRenderers.Length; i++)
            {
                mts = silhouetteRenderers[i].materials;
                for (int a = 0; a < mts.Length; a++)
                {
                    if (GameManager.silhouetteShaderDefault != null && mts[a].shader != GameManager.silhouetteShaderDefault)
                    {
                        mts[a].shader = GameManager.silhouetteShaderDefault;
                    }
                }
            }
        }
    }

    private void RedShaderOnOff()
    {
        isRed = !isRed;

        if (isRed)
        {
            Material[] mts;
            for (int i = 0; i < redShaderRenderers.Length; i++) 
            {
                mts = redShaderRenderers[i].materials;
                for (int a = 0; a < mts.Length; a++)
                {
                    if (GameManager.redShader != null && mts[a].shader != GameManager.redShader)
                    {
                        mts[a].shader = GameManager.redShader;
                        mts[a].SetColor("_AddColor", new Color(0.8f, 0.3f, 0.35f, 1));
                    }
                        
                }
            }
        }
        else
        {
            Material[] mts;
            for (int i = 0; i < redShaderRenderers.Length; i++)
            {
                mts = redShaderRenderers[i].materials;
                for (int a = 0; a < mts.Length; a++)
                {
                    if (GameManager.redShaderDefault != null && mts[a].shader != GameManager.redShaderDefault)
                        mts[a].shader = GameManager.redShaderDefault;
                }
            }
        }
    }

    private void GrayShaderOnOff()
    {
        isGray = !isGray;

        if (isGray) 
        {
            Material[] mts;
            for (int i = 0; i < grayShaderRenderers.Length; i++)
            {
                mts = grayShaderRenderers[i].materials;
                for (int a = 0; a < mts.Length; a++)
                {
                    if (GameManager.grayShader != null && mts[a].shader != GameManager.grayShader)
                    {
                        mts[a].shader = GameManager.grayShader;
                    }

                }
            }

            weaponRenderer.material.shader = GameManager.grayShader;
        }
        else
        {
            Material[] mts;
            for (int i = 0; i < grayShaderRenderers.Length; i++)
            {
                mts = grayShaderRenderers[i].materials;
                for (int a = 0; a < mts.Length; a++)
                {
                    if (GameManager.grayShaderDefault != null && mts[a].shader != GameManager.grayShaderDefault)
                    {
                        mts[a].shader = GameManager.grayShaderDefault;
                    }

                }
            }
            weaponRenderer.material.shader = GameManager.weaponShaderDefault;
        }

    }

    private void OnAlphaSlider(float value)
    {
        alphaValue = value;
        if (isGray)
        {
            Material[] mts;
            for (int i = 0; i < grayShaderRenderers.Length; i++)
            {
                mts = grayShaderRenderers[i].materials;
                for (int a = 0; a < mts.Length; a++)
                {
                    mts[a].SetFloat("_Alpha", alphaValue);
                }
            }
            weaponRenderer.material.SetFloat("_Alpha", alphaValue);
        }
    }

    private void PostProcessOnOff()
    {
        isPostProcess = !isPostProcess;
        if (isPostProcess)
            Camera.main.GetComponent<PostProcessVolume>().enabled = true;        
        else
            Camera.main.GetComponent<PostProcessVolume>().enabled = false;        
    }

    private void OnIntensitySlider(float value)
    {
        Bloom bloomLayer = null;
        PostProcessVolume volume = Camera.main.GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out bloomLayer);
        bloomLayer.intensity.value = value * 30.0f;
    }
}
