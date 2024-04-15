using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalGenerator : MonoBehaviour
{
    public GameObject Cat;
    public GameObject LOVEDUCK;
    public GameObject FLOWER;
    public GameObject mole_attack;
    public GameObject penguin;
    public GameObject sheep;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 20; i++)
        {
            GameObject animal = Instantiate(Cat);

            int ranX = Random.Range(-240, 241);
            int ranY = Random.Range(5, 31);
            int ranZ = Random.Range(-240, 241);
            animal.transform.position = new Vector3(ranX, transform.position.y + ranY, ranZ);
        }

        for (int i = 0; i < 20; i++)
        {
            GameObject animal = Instantiate(LOVEDUCK);
    
            int ranX = Random.Range(-240, 241);
            int ranY = Random.Range(5, 31);
            int ranZ = Random.Range(-240, 241);
            animal.transform.position = new Vector3(ranX, transform.position.y + ranY, ranZ);
        }

        for (int i = 0; i < 20; i++)
        {
            GameObject animal = Instantiate(FLOWER);

            int ranX = Random.Range(-240, 241);
            int ranZ = Random.Range(-240, 241);
            animal.transform.position = new Vector3(ranX, 5, ranZ);
        }

        for (int i = 0; i < 20; i++)
        {
            GameObject animal = Instantiate(mole_attack);

            int ranX = Random.Range(-240, 241);
            int ranY = Random.Range(5, 31);
            int ranZ = Random.Range(-240, 241);
            animal.transform.position = new Vector3(ranX, transform.position.y + ranY, ranZ);
        }
        
        for (int i = 0; i < 20; i++)
        {
            GameObject animal = Instantiate(penguin);

            int ranX = Random.Range(-240, 241);
            int ranY = Random.Range(5, 31);
            int ranZ = Random.Range(-240, 241);
            animal.transform.position = new Vector3(ranX, transform.position.y + ranY, ranZ);
        }

        for (int i = 0; i < 20; i++)
        {
            GameObject animal = Instantiate(sheep);
            int ranX = Random.Range(-240, 241);
            int ranY = Random.Range(5, 31);
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
