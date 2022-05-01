using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ClockScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject clockHr, clockMin, clockSec;
    private int hour, min, sec, hour0, min0, sec0;
    void Start()
    {
        DateTime now = DateTime.Now;
        hour = now.Hour; 
        min = now.Minute;
        sec = now.Second;
        setClock();
    }

    // Update is called once per frame
    void Update()
    {
        DateTime now = DateTime.Now;
        hour = now.Hour; 
        min = now.Minute;
        sec = now.Second;
        if (hour != hour0 || min != min0 || sec != sec0)
        {
            setClock();
        }
    }

    private void setClock()
    {
        clockHr.transform.eulerAngles = new Vector3(0.0f, 0.0f, -hour * 30.0f - min * 0.5f);
        clockMin.transform.eulerAngles = new Vector3(0.0f, 0.0f, -min * 6.0f - sec * 0.1f);
        clockSec.transform.eulerAngles = new Vector3(0.0f, 0.0f, -sec * 6.0f);

        hour0 = hour;
        min0 = min;
        sec0 = sec;
    }
}
