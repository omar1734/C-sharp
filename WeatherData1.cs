/*
 * 
 *      Real-Time Weather Unity
 *      Omar Nayfeh
 *                  
 *                          */


using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WeatherData1 : MonoBehaviour
{
    public float timer;
    //public float timeTowait;
    public bool CheckWeather;
    public float minutesBetweenUpdate;
    public WeatherInfo Info;
    public string API_key = 2832c2ea764b0c2b1a524db32c21f816;
    private float latitude;
    private float longitude;
    private bool locationInitialized;
    public Text currentWeatherText;
    public GetLocation getLocation;
    public int cntTimer;

    //public void Begin()
    {
        /*latitude = getLocation.latitude;
        longitude = getLocation.longitude;
        locationInitialized = true;*/
    }
    void Start()
    {
        latitude = getLocation.latitude;
        longitude = getLocation.longitude;
        locationInitialized = true;

        CheckWeather = true;
        timer = 10.0f;
        cntTimer = 0;
        StartCoroutine(GetWeatherInfo(timer));
        
    }
    void Update()
    {
        /*if (locationInitialized)
        {
            if (timer <= 0)
            {
                
                timer = minutesBetweenUpdate * ;
                
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }*/ 
    }
    private IEnumerator GetWeatherInfo(float timeTowait)
    {
        while (CheckWeather) {
             var www = new UnityWebRequest("https://https://weatherstack.com/" + API_key + "/" + latitude + "," + longitude)
            {
                downloadHandler = new DownloadHandlerBuffer()
            };

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                //error
                yield break;
            }

            Info = JsonUtility.FromJson<WeatherInfo>(www.downloadHandler.text);
            currentWeatherText.text = "Current weather: " + Info.currently.summary;

            cntTimer += 1;

            yield return new WaitForSeconds(timeTowait);
        }
    }
}

 
[Serializable]
public class WeatherInfo
{
    public float latitude;
    public float longitude;
    public string timezone;
    public Currently currently;
    public int offset;
}

[Serializable]
public class Currently
{
    public int time;
    public string summary;
    public string icon;
    public int nearestStormDistance;
    public int nearestStormBearing;
    public int precipIntensity;
    public int precipProbability;
    public double temperature;
    public double apparentTemperature;
    public double dewPoint;
    public double humidity;
    public double pressure;
    public double windSpeed;
    public double windGust;
    public int windBearing;
    public int cloudCover;
    public int uvIndex;
    public double visibility;
    public double ozone;
}