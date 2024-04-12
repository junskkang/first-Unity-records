using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ArrowGenerator : MonoBehaviour
{
    public GameObject arrowPrefab;
    float spawn = 2.0f;
    float delta = 0.0f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        delta += Time.deltaTime;
        if (delta > spawn)
        {
            delta = 0.0f;
            GameObject go = Instantiate(arrowPrefab) as GameObject;

            int dropIdx = Random.Range(-2, 3); //È­»ìÀÇ xÁÂÇ¥ ·£´ý°ª
            go.GetComponent<ArrowCtrl>().InitArrow(dropIdx);
        }
    }

    
}
