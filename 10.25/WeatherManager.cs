using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.EventSystems;

//���������� ��û�ؼ� ���� �ּҰ��� Ǯ��
//https://apis.data.go.kr/1360000/VilageFcstInfoService_2.0/getUltraSrtNcst?serviceKey=                �ּ�
//CHYXXeKC5XplYINKg69ixxMHq8ZJqBrsqADq2V4AvRdg1XS2nFRAfU7ZbAUF2KRRj2pD2Q0RXMQucmN7DFDHYg%3D%3D        ����Ű
//&pageNo=1                 ������
//&numOfRows=1000           �ѷο�
//&dataType=JSON            ������ ����
//&base_date=20241104       ��¥
//&base_time=2100           �ð�
//&nx=38                    x��
//&ny=127                   y��

public enum Weather
{
    Sunny,
    Rainy,
    RainySnowy,
    Snowy,
    Raindrop = 5,
    RaindropSnowyblow,
    Snowblow
}

public class WeatherManager : MonoBehaviour
{
    //apiȣ�� �ּ�
    string url = "http://apis.data.go.kr/1360000/VilageFcstInfoService_2.0/getUltraSrtNcst?serviceKey=";
    string key = "CHYXXeKC5XplYINKg69ixxMHq8ZJqBrsqADq2V4AvRdg1XS2nFRAfU7ZbAUF2KRRj2pD2Q0RXMQucmN7DFDHYg%3D%3D";
    string pageNum = "&pageNo=1";
    string numRows = "&numOfRows=1000";
    string dataType = "&dataType=JSON";
    string baseDate = "&base_date=20241104";
    string baseTime = "&base_time=2100";
    string xValue = "&nx=38";
    string yValue = "&ny=127";

    string resultValue = "";
    GameObject rain;
    ParticleSystem rainParticle;
    GameObject snow;
    ParticleSystem snowParticle;

    // Start is called before the first frame update
    void Start()
    {
        rain = GameObject.Find("RainController");
        rainParticle = rain.GetComponent<ParticleSystem>();
        snow = GameObject.Find("SnowController");
        snowParticle = snow.GetComponent<ParticleSystem>();

        //StartCoroutine(GetWeatherData());

        StartCoroutine(GetWeather(1));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //�ǽð� ���� ��¥, �ð��� ���� �������� 
    IEnumerator GetWeather(float time)
    {
        //���� �ð�
        DateTime currentTime = DateTime.Now;
        int currentHour = currentTime.Hour;
        baseDate = currentTime.ToString();

        //2024-11-05 ���� 1:47:16
        Debug.Log(baseDate);

        string dateTimeStr = baseDate;

        // ���ڿ��� DateTime �������� �Ľ�
        DateTime dateTime = DateTime.Parse(dateTimeStr);

        // ��¥�� "yyyyMMdd" �������� ��ȯ
        string formattedDate = dateTime.ToString("yyyyMMdd");
        baseDate = formattedDate;
        // 20241105 �ǽð� ��¥�� ������
        Debug.Log(baseDate);


        //�ǽð� �ð� ���ϱ�
        //���� �̿��ϰ� �ִ� �����ʹ� �� �ð� 30�п� �����Ͱ� ������Ʈ�ǹǷ� 30�� ������ �ð��̶�� �� �ð� ���� ������ �޾ƿ´�.
        if (currentTime.Minute < 30)
        {
            currentTime = currentTime.AddHours(-1);
            baseTime = currentTime.ToString("HHmm");
            Debug.Log("30�� �����Դϴ�. 1�ð� �� �� : " + baseTime);
        }
        else
        {
            baseTime = currentTime.ToString("HHmm");
            Debug.Log("30�� �����Դϴ�. ���� �� : " + baseTime);
        }

        //�䱸���� : ���� ��¥, ���� �ð� ���ϴ� ���·� ����
        StartCoroutine(RequestWeatherData(baseDate, baseTime));
        yield return null;
    }

    IEnumerator RequestWeatherData(string base_date, string base_time)
    {
        string fullURL = url + key + pageNum + numRows + dataType + "&base_date=" + base_date + "&base_time=" + base_time + xValue + yValue;

        using (UnityWebRequest request = UnityWebRequest.Get(fullURL))
        {
            yield return request.SendWebRequest();

            //��Ÿ ��ֿ�ҷ� ���� ���� �������µ� �����Ͽ��� ��쿡 �ٽ� �����
            if (request.result != UnityWebRequest.Result.Success)
            {
                int requestAgain = 0;
                while (requestAgain <= 3)
                {
                    //��û���н� ��� �ð� �������� ���û�� �� �� 
                    yield return new WaitForSeconds(5f);

                    StartCoroutine(RequestWeatherData(base_date, base_time));

                    Debug.Log("���� : " + request.error + " ���û Ƚ�� : " + requestAgain);
                    requestAgain++;
                }
                Debug.Log("�߷��߷�~ ��¿ �� ������");
            }
            else
            {
                JObject json = JObject.Parse(request.downloadHandler.text);

                var items = json["response"]["body"]["items"]["item"];
                foreach (var item in items)
                {
                    if (item["category"].ToString() == "PTY")
                    {
                        string value = item["obsrValue"].ToString();
                        Debug.Log("���� �ǽð� ���� ������ �� : " + value);

                        //WeatherObjectOnOff(value);

                        int parse = int.Parse(value);

                        SetWeather(parse);
                    }

                }
            }
        }
    }

    void SetWeather(int weather)
    {
        Weather weatherEnum = (Weather)weather;
        Debug.Log("���� ���� : " + weatherEnum);

        rain.gameObject.SetActive(false);
        snow.gameObject.SetActive(false);

        //�׽�Ʈ�� ��ŷ
        //weather = 6;


        if (weather.Equals((int)Weather.Sunny)) 
        {
            return;
        }

        if (weather == 1 || weather == 2 || weather == 5 || weather == 6)
        {
            rain.gameObject.SetActive(true);
            rainParticle.Play();
        }

        if (weather == 2 || weather == 3 || weather == 6 || weather == 7)
        {
            snow.gameObject.SetActive(true);
            snowParticle.Play();
        }

        if (weather == 0)
        {
            rain.gameObject.SetActive(false);
            snow.gameObject.SetActive(false);
        }


    }
    void WeatherObjectOnOff(string value)
    {
        if (value != "")
        {


            if (value == "1" || value == "2" || value == "5" || value == "6")
            {
                rain.gameObject.SetActive(true);
            }

            if (value == "2" || value == "3" || value == "6" || value == "7")
            {
                snow.gameObject.SetActive(true);
            }

            if (value == "0")
            {
                rain.gameObject.SetActive(false);
                snow.gameObject.SetActive(false);
            }


        }
    }

    IEnumerator GetWeatherData()
    {
        //string������ ��� ���� ���� �ּ�
        string fullURL = url + key + pageNum + numRows + dataType + baseDate + baseTime + xValue + yValue;

        //UnityWebRequest �Լ� �� �����͸� �������� Get() �Լ� ���
        //api�������� �Ծ��� ������ ���� ������ ������ ���� �䱸������ ������
        using (UnityWebRequest request = UnityWebRequest.Get(fullURL)) 
        {
            //��û �� ������ ���� �ð��� ��ٸ�
            yield return request.SendWebRequest();

            //������ ���������� ���� ����
            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                //���� �α׸� ���
                Debug.Log(request.error);
            }
            else //������ ���������� ����
            {
                Debug.Log(request.downloadHandler.text);

                JObject json = JObject.Parse(request.downloadHandler.text);

                var items = json["response"]["body"]["items"]["item"];

                foreach (var item in items) 
                {
                    if (item["category"].ToString() == "PTY")
                    {
                        string value = item["obsrValue"].ToString();
                        Debug.Log("obsrValue : " + value);
                    }                   

                }
            }
        }
    }
}
