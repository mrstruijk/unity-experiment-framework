using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AWS1HourTest : MonoBehaviour
{

    public Transform moveable;

    public UnityEngine.UI.Text fps;
    public UnityEngine.UI.Text uptime;
    public UnityEngine.UI.Text trials;


    void Update()
    {
        moveable.position = new Vector3(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f));
        fps.text = (1.0f / Time.smoothDeltaTime).ToString("0") + "FPS";

        TimeSpan t = TimeSpan.FromSeconds(Time.time);

        uptime.text = t.Hours.ToString("D2") + "h, " + t.Minutes.ToString("D2") + "m, " + t.Seconds.ToString("D2") + "s";
        }

    public void CreateTrials()
    {
        UXF.Session.instance.CreateBlock(500);
        UXF.Session.instance.BeginNextTrial();
    }

    public void BeginTrialTimer()
    {
        trials.text = string.Format("Trial {0}", UXF.Session.instance.currentTrialNum.ToString());
        UXF.Session.instance.Invoke("EndCurrentTrial", 30f);
    }

}
