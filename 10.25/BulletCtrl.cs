using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    Vector3 dirVec = Vector3.right;
    float moveSpeed = 15.0f;        //날아가는 속도
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 3.0f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += dirVec * moveSpeed * Time.deltaTime;

        //화면 밖을 벗어난 총알 바로 삭제
        if (CameraResolution.maxVtW.x + 1.0f < transform.position.x ||
            CameraResolution.minVtW.x - 1.0f > transform.position.x ||
            CameraResolution.maxVtW.y + 1.0f < transform.position.y ||
            CameraResolution.minVtW.y - 1.0f > transform.position.y)
            Destroy(gameObject);
    }

    public void BulletSpawn(Vector3 startPos, Vector3 dirVec, float mvSpeed = 15.0f)
    {
        this.dirVec = dirVec;
        this.moveSpeed = mvSpeed;
        transform.position = new Vector3 (startPos.x, startPos.y, 0.0f);
    }
}
