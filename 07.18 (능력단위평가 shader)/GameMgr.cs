using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    GameIng,
    GameEnd
}

public class GameMgr : MonoBehaviour
{
    public static GameState s_GameState = GameState.GameIng;

    //Text UI �׸� ������ ���� ����
    public Text txtScore;
    //���� ������ ����ϱ� ���� ����
    private int totScore = 0;

    public Button BackBtn;

    //���Ͱ� ������ ��ġ�� ���� �迭
    public Transform[] points;
    //���� �������� �Ҵ��� ����
    public GameObject monsterPrefab;
    //���͸� �̸� ������ ������ ����Ʈ �ڷ���
    public List<GameObject> monsterPool = new List<GameObject>();

    //���͸� �߻���ų �ֱ�
    public float createTime = 2.0f;
    //������ �ִ� �߻� ����
    public int maxMonster = 10;
    //���� ���� ���� ����
    public bool isGameOver = false;

    //07.19 �����Ⱓ �򰡿� ���� �߰�
    public Button gSkillBtn;
    public Button postProcessBtn;
    public Slider postProcessSlider;
    bool isPostProcess = false;

    float gSkillCool = 0.0f;
    float gSkillDuration = 10.0f;   //��ų ���ӽð�
    public Image gSkillIcon;
    public Image gSkillCoolUI;
    bool isSkillPossible = true;
    public bool isSkillOn = false;

    public static Shader grayShader;
    public static Shader defaultShader;
    public static Shader greenShader;

    GameObject[] skillTargets;
    SkinnedMeshRenderer[] targetRenderers;

    //--- �̱��� ����
    public static GameMgr Inst = null;

    void Awake()
    {
        Inst = this;   
    }
    //--- �̱��� ����

    // Start is called before the first frame update
    void Start()
    {
        s_GameState = GameState.GameIng;

        DispScore(0);

        if(BackBtn != null)
            BackBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Lobby");
            });

        // Hierachy ���� SpawnPoint�� ã�� ������ �ִ� ��� Transform ������Ʈ�� ã�ƿ�
        points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();

        //���͸� ������ ������Ʈ Ǯ�� ����
        for(int i = 0; i < maxMonster; i++)
        {
            //���� �������� ����
            GameObject monster = (GameObject)Instantiate(monsterPrefab);
            //������ ������ �̸� ����
            monster.name = "Monster_" + i.ToString();
            //������ ���͸� ��Ȱ��ȭ
            monster.SetActive(false);
            //������ ���͸� ������Ʈ Ǯ�� �߰�
            monsterPool.Add(monster);
        }

        if(points.Length > 0)
        {
            //���� ���� �ڷ�ƾ �Լ� ȣ��
            StartCoroutine(this.CreateMonster());
        }

        //07.19 �����Ⱓ �򰡿�
        if (gSkillBtn != null)
            gSkillBtn.onClick.AddListener(SkillClick);

        if (isPostProcess)
        {
            Camera.main.GetComponent<PostProcessVolume>().enabled = true;
            postProcessSlider.gameObject.SetActive(true);
        }
        else
        {
            Camera.main.GetComponent<PostProcessVolume>().enabled = false;
            postProcessSlider.gameObject.SetActive(false);
        }

        if (postProcessBtn != null)
            postProcessBtn.onClick.AddListener(PostProcessClick);

        if (postProcessSlider != null)
        {
            Bloom bloomLayer = null;
            PostProcessVolume postProcessVolume = Camera.main.GetComponent<PostProcessVolume>();
            postProcessVolume.profile.TryGetSettings(out bloomLayer);

            postProcessSlider.onValueChanged.AddListener(OnPostProcess);
            postProcessSlider.value = bloomLayer.dirtIntensity.value / 20.0f;
        }

        grayShader = Shader.Find("Custom/GrayShader");
        greenShader = Shader.Find("Custom/GreenShader");

        if (monsterPrefab != null)
        {
            targetRenderers = monsterPool[0].GetComponentsInChildren<SkinnedMeshRenderer>();
            defaultShader = targetRenderers[0].material.shader;
        }
            
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            SkillClick();

        if (!isSkillPossible && s_GameState != GameState.GameEnd)
        {
            if (gSkillCool <= gSkillDuration)
            {
                isSkillOn = true;
                gSkillCoolUI.color = Color.yellow;
                gSkillCoolUI.transform.localScale = new Vector2(1.2f, 1.2f);
            }
            else
            {
                isSkillOn = false;

                if (!isSkillOn)
                    gSkillIcon.color = new Color(0.5f, 0.5f, 0.5f);

                gSkillCoolUI.color = Color.white;
                gSkillCoolUI.transform.localScale = Vector2.one;

                skillTargets = GameObject.FindGameObjectsWithTag("MONSTER");

                
                if (skillTargets != null && skillTargets.Length > 0)
                {
                    for (int i = 0; i < skillTargets.Length; i++)
                    {
                        targetRenderers = skillTargets[i].GetComponentsInChildren<SkinnedMeshRenderer>();
                        Material[] mts;
                        for (int a = 0; a < targetRenderers.Length; a++)
                        {
                            mts = targetRenderers[a].materials;
                            for (int x = 0; x < mts.Length; x++)
                            {
                                if (GameMgr.defaultShader != null && mts[x].shader != GameMgr.defaultShader)
                                {
                                    mts[x].shader = GameMgr.defaultShader;
                                    skillTargets[i].GetComponent<MonsterCtrl>().monsterState = MonsterCtrl.MonsterState.idle;
                                }                                    
                            }
                        }                       
                    }
                }
            }

            if (gSkillCool >= 15.0f)
            {
                gSkillCool = 15.0f;

                isSkillPossible = true;

                if (isSkillPossible)
                    gSkillIcon.color = new Color(1.0f, 1.0f, 1.0f);
            }
            else
            {
                gSkillCool += Time.deltaTime;

                if (isSkillPossible)
                    gSkillCoolUI.fillAmount = 1.0f;
                else
                    gSkillCoolUI.fillAmount = gSkillCool / 15.0f;
            }
        }
    }

    //���� ���� �� ȭ�� ǥ��
    public void DispScore(int score)
    {
        totScore += score;
        txtScore.text = "score <color=#ff0000>" + totScore.ToString() + "</color>";
    }

    //���� ���� �ڷ�ƾ �Լ�
    IEnumerator CreateMonster()
    {
        //���� ���� �ñ��� ���� ����
        while( !isGameOver )
        {
            //���� ���� �ֱ� �ð���ŭ ���� ������ �纸
            yield return new WaitForSeconds(createTime);

            //�÷��̾ ������� �� �ڷ�ƾ�� ������ ���� ��ƾ�� �������� ����
            if (GameMgr.s_GameState == GameState.GameEnd) 
                yield break; //�ڷ�ƾ �Լ����� �Լ��� ���������� ���

            //������Ʈ Ǯ�� ó������ ������ ��ȸ
            foreach(GameObject monster in monsterPool)
            {
                //��Ȱ��ȭ ���η� ��� ������ ���͸� �Ǵ�
                if(monster.activeSelf == false)
                {
                    //���͸� ������ų ��ġ�� �ε������� ����
                    int idx = Random.Range(1, points.Length);
                    //������ ������ġ�� ����
                    monster.transform.position = points[idx].position;
                    //���͸� Ȱ��ȭ��
                    monster.SetActive(true);

                    //������Ʈ Ǯ���� ���� ������ �ϳ��� Ȱ��ȭ�� �� for ������ ��������
                    break;
                }//if(monster.activeSelf == false)
            }//foreach(GameObject monster in monsterPool)
        }// while( !isGameOver )

    }//IEnumerator CreateMonster()
    private void SkillClick()
    {
        if (isSkillPossible)
        {           
            gSkillCoolUI.fillAmount = 0.0f;

            gSkillCool = 0.0f;

            isSkillPossible = false;

            skillTargets = GameObject.FindGameObjectsWithTag("MONSTER");

            if (skillTargets != null && skillTargets.Length > 0)
            {
                for (int i = 0; i < skillTargets.Length; i++) 
                {
                    targetRenderers = skillTargets[i].GetComponentsInChildren<SkinnedMeshRenderer>();
                    Material[] mts;
                    for (int a = 0; a < targetRenderers.Length; a++)
                    {
                        mts = targetRenderers[a].materials;
                        for (int x = 0; x < mts.Length; x++)
                        {
                            if (GameMgr.grayShader != null && mts[x].shader != GameMgr.grayShader)
                                mts[x].shader = GameMgr.grayShader;
                        }
                        skillTargets[i].GetComponent<MonsterCtrl>().monsterState = MonsterCtrl.MonsterState.petrified;
                        //Debug.Log(skillTargets[i].GetComponent<MonsterCtrl>().monsterState);
                    }
                }
            }          
        }
    }

    private void PostProcessClick()
    {
        isPostProcess = !isPostProcess;

        if (isPostProcess)
        {
            Camera.main.GetComponent<PostProcessVolume>().enabled = true;
            postProcessSlider.gameObject.SetActive(true);
        }
        else
        {
            Camera.main.GetComponent<PostProcessVolume>().enabled = false;
            postProcessSlider.gameObject.SetActive(false);
        }
            
    }

    private void OnPostProcess(float value)
    {
        Bloom bloomLayer = null;
        PostProcessVolume volume = Camera.main.GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out bloomLayer);
        bloomLayer.intensity.value = value * 20.0f;
    }
    public static bool IsPointerOverUIObject() //UGUI�� UI���� ���� ��ŷ�Ǵ��� Ȯ���ϴ� �Լ�
    {
        PointerEventData a_EDCurPos = new PointerEventData(EventSystem.current);

#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)

			List<RaycastResult> results = new List<RaycastResult>();
			for (int i = 0; i < Input.touchCount; ++i)
			{
				a_EDCurPos.position = Input.GetTouch(i).position;  
				results.Clear();
				EventSystem.current.RaycastAll(a_EDCurPos, results);
                if (0 < results.Count)
                    return true;
			}

			return false;
#else
        a_EDCurPos.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(a_EDCurPos, results);
        return (0 < results.Count);
#endif
    }//public bool IsPointerOverUIObject()
     
}//public class GameMgr : MonoBehaviour
