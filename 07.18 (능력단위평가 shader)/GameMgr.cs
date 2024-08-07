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

    //Text UI 항목 연결을 위한 변수
    public Text txtScore;
    //누적 점수를 기록하기 위한 변수
    private int totScore = 0;

    public Button BackBtn;

    //몬스터가 출현할 위치를 담을 배열
    public Transform[] points;
    //몬스터 프리팹을 할당할 변수
    public GameObject monsterPrefab;
    //몬스터를 미리 생성해 저장할 리스트 자료형
    public List<GameObject> monsterPool = new List<GameObject>();

    //몬스터를 발생시킬 주기
    public float createTime = 2.0f;
    //몬스터의 최대 발생 개수
    public int maxMonster = 10;
    //게임 종료 여부 변수
    public bool isGameOver = false;

    //07.19 단위기간 평가용 변수 추가
    public Button gSkillBtn;
    public Button postProcessBtn;
    public Slider postProcessSlider;
    bool isPostProcess = false;

    float gSkillCool = 0.0f;
    float gSkillDuration = 10.0f;   //스킬 지속시간
    public Image gSkillIcon;
    public Image gSkillCoolUI;
    bool isSkillPossible = true;
    public bool isSkillOn = false;

    public static Shader grayShader;
    public static Shader defaultShader;
    public static Shader greenShader;

    GameObject[] skillTargets;
    SkinnedMeshRenderer[] targetRenderers;

    //--- 싱글톤 패턴
    public static GameMgr Inst = null;

    void Awake()
    {
        Inst = this;   
    }
    //--- 싱글톤 패턴

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

        // Hierachy 뷰의 SpawnPoint를 찾아 하위에 있는 모든 Transform 컴포넌트를 찾아옴
        points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();

        //몬스터를 생성해 오브젝트 풀에 저장
        for(int i = 0; i < maxMonster; i++)
        {
            //몬스터 프리팹을 생성
            GameObject monster = (GameObject)Instantiate(monsterPrefab);
            //생성한 몬스터의 이름 설정
            monster.name = "Monster_" + i.ToString();
            //생성한 몬스터를 비활성화
            monster.SetActive(false);
            //생성한 몬스터를 오브젝트 풀에 추가
            monsterPool.Add(monster);
        }

        if(points.Length > 0)
        {
            //몬스터 생성 코루틴 함수 호출
            StartCoroutine(this.CreateMonster());
        }

        //07.19 단위기간 평가용
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

    //점수 누적 및 화면 표시
    public void DispScore(int score)
    {
        totScore += score;
        txtScore.text = "score <color=#ff0000>" + totScore.ToString() + "</color>";
    }

    //몬스터 생성 코루틴 함수
    IEnumerator CreateMonster()
    {
        //게임 종료 시까지 무한 루프
        while( !isGameOver )
        {
            //몬스터 생성 주기 시간만큼 메인 루프에 양보
            yield return new WaitForSeconds(createTime);

            //플레이어가 사망했을 때 코루틴을 종료해 다음 루틴을 진행하지 않음
            if (GameMgr.s_GameState == GameState.GameEnd) 
                yield break; //코루틴 함수에서 함수를 빠져나가는 명령

            //오브젝트 풀의 처음부터 끝까지 순회
            foreach(GameObject monster in monsterPool)
            {
                //비활성화 여부로 사용 가능한 몬스터를 판단
                if(monster.activeSelf == false)
                {
                    //몬스터를 출현시킬 위치의 인덱스값을 추출
                    int idx = Random.Range(1, points.Length);
                    //몬스터의 출현위치를 설정
                    monster.transform.position = points[idx].position;
                    //몬스터를 활성화함
                    monster.SetActive(true);

                    //오브젝트 풀에서 몬스터 프리팹 하나를 활성화한 후 for 루프를 빠져나감
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
    public static bool IsPointerOverUIObject() //UGUI의 UI들이 먼저 피킹되는지 확인하는 함수
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
