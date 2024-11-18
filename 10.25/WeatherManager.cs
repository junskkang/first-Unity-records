using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.EventSystems;

//공공데이터 요청해서 받은 주소값의 풀이
//https://apis.data.go.kr/1360000/VilageFcstInfoService_2.0/getUltraSrtNcst?serviceKey=                주소
//CHYXXeKC5XplYINKg69ixxMHq8ZJqBrsqADq2V4AvRdg1XS2nFRAfU7ZbAUF2KRRj2pD2Q0RXMQucmN7DFDHYg%3D%3D        인증키
//&pageNo=1                 페이지
//&numOfRows=1000           넘로우
//&dataType=JSON            데이터 형식
//&base_date=20241104       날짜
//&base_time=2100           시간
//&nx=38                    x값
//&ny=127                   y값

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
    //api호출 주소
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

    //실시간 현재 날짜, 시간에 의한 날씨정보 
    IEnumerator GetWeather(float time)
    {
        //현재 시간
        DateTime currentTime = DateTime.Now;
        int currentHour = currentTime.Hour;
        baseDate = currentTime.ToString();

        //2024-11-05 오후 1:47:16
        Debug.Log(baseDate);

        string dateTimeStr = baseDate;

        // 문자열을 DateTime 형식으로 파싱
        DateTime dateTime = DateTime.Parse(dateTimeStr);

        // 날짜를 "yyyyMMdd" 형식으로 변환
        string formattedDate = dateTime.ToString("yyyyMMdd");
        baseDate = formattedDate;
        // 20241105 실시간 날짜만 가져옴
        Debug.Log(baseDate);


        //실시간 시간 구하기
        //현재 이용하고 있는 데이터는 매 시간 30분에 데이터가 업데이트되므로 30분 이전의 시간이라면 한 시간 전의 정보를 받아온다.
        if (currentTime.Minute < 30)
        {
            currentTime = currentTime.AddHours(-1);
            baseTime = currentTime.ToString("HHmm");
            Debug.Log("30분 이전입니다. 1시간 전 값 : " + baseTime);
        }
        else
        {
            baseTime = currentTime.ToString("HHmm");
            Debug.Log("30분 이후입니다. 현재 값 : " + baseTime);
        }

        //요구사항 : 현재 날짜, 현재 시간 원하는 형태로 구함
        StartCoroutine(RequestWeatherData(baseDate, baseTime));
        yield return null;
    }

    IEnumerator RequestWeatherData(string base_date, string base_time)
    {
        string fullURL = url + key + pageNum + numRows + dataType + "&base_date=" + base_date + "&base_time=" + base_time + xValue + yValue;

        using (UnityWebRequest request = UnityWebRequest.Get(fullURL))
        {
            yield return request.SendWebRequest();

            //기타 장애요소로 인해 값을 가져오는데 실패하였을 경우에 다시 물어보기
            if (request.result != UnityWebRequest.Result.Success)
            {
                int requestAgain = 0;
                while (requestAgain <= 3)
                {
                    //요청실패시 어느 시간 간격으로 재요청을 할 지 
                    yield return new WaitForSeconds(5f);

                    StartCoroutine(RequestWeatherData(base_date, base_time));

                    Debug.Log("에러 : " + request.error + " 재요청 횟수 : " + requestAgain);
                    requestAgain++;
                }
                Debug.Log("야레야레~ 어쩔 수 없군요");
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
                        Debug.Log("현재 실시간 날씨 데이터 값 : " + value);

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
        Debug.Log("현재 날씨 : " + weatherEnum);

        rain.gameObject.SetActive(false);
        snow.gameObject.SetActive(false);

        //테스트용 후킹
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
        //string변수를 모두 더한 최종 주소
        string fullURL = url + key + pageNum + numRows + dataType + baseDate + baseTime + xValue + yValue;

        //UnityWebRequest 함수 중 데이터를 가져오는 Get() 함수 사용
        //api서버에서 규약한 내용을 토대로 유저의 정보에 의한 요구사항을 전달함
        using (UnityWebRequest request = UnityWebRequest.Get(fullURL)) 
        {
            //요청 및 응답이 오는 시간을 기다림
            yield return request.SendWebRequest();

            //응답을 정상적으로 받지 못함
            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                //에러 로그를 출력
                Debug.Log(request.error);
            }
            else //응답을 정상적으로 받음
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
