using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalGenerator : MonoBehaviour
{
    public GameObject CatPrefab;
    public GameObject DuckPrefab;
    public GameObject FlowerPrefab;
    public GameObject MolePrefab;
    public GameObject PenguinPrefab;
    public GameObject SheepPrefab;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 20; i++)
        {
            GameObject animal = Instantiate(CatPrefab);

            int ranX = Random.Range(-240, 241);
            int ranY = Random.Range(5, 21);
            int ranZ = Random.Range(-240, 241);
            animal.transform.position = new Vector3(ranX, transform.position.y + ranY, ranZ);
        }

        for (int i = 0; i < 20; i++)
        {
            GameObject animal = Instantiate(DuckPrefab);
    
            int ranX = Random.Range(-240, 241);
            int ranY = Random.Range(5, 21);
            int ranZ = Random.Range(-240, 241);
            animal.transform.position = new Vector3(ranX, transform.position.y + ranY, ranZ);
        }

        for (int i = 0; i < 20; i++)
        {
            GameObject animal = Instantiate(FlowerPrefab);

            int ranX = Random.Range(-240, 241);
            int ranZ = Random.Range(-240, 241);
            animal.transform.position = new Vector3(ranX, 4, ranZ);
        }

        for (int i = 0; i < 20; i++)
        {
            GameObject animal = Instantiate(MolePrefab);

            int ranX = Random.Range(-240, 241);
            int ranY = Random.Range(5, 21);
            int ranZ = Random.Range(-240, 241);
            animal.transform.position = new Vector3(ranX, transform.position.y + ranY, ranZ);
        }
        
        for (int i = 0; i < 20; i++)
        {
            GameObject animal = Instantiate(PenguinPrefab);

            int ranX = Random.Range(-240, 241);
            int ranY = Random.Range(5, 21);
            int ranZ = Random.Range(-240, 241);
            animal.transform.position = new Vector3(ranX, transform.position.y + ranY, ranZ);
        }

        for (int i = 0; i < 20; i++)
        {
            GameObject animal = Instantiate(SheepPrefab);
            int ranX = Random.Range(-240, 241);
            int ranY = Random.Range(5, 21);
            int ranZ = Random.Range(-240, 241);
            animal.transform.position = new Vector3(ranX, transform.position.y + ranY, ranZ);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("bamsongi") == true)
        {
            Destroy(gameObject);
            
        }
    }
    
}
