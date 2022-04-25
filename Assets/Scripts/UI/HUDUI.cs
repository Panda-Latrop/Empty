using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
public class HUDUI : SimpleUI
{
    [SerializeField]
    protected Image health, stamina,pain;
    [SerializeField]
    protected Text timer;
    [SerializeField]
    protected float painStartAlpha = 0.25f;

    public void ShowTimer()
    {
        timer.enabled = true;
    }
    public void HideTimer()
    {
        timer.enabled = false;
    }
    public void SetTimer(float time)
    {
        if (time < 0)
            return;
        StringBuilder strb = new StringBuilder();
        var t = System.TimeSpan.FromSeconds(time);
        int m = t.Minutes;
        int s = t.Seconds;
        if (m < 10)
            strb.Append(0);
        strb.Append(m);
        strb.Append(":");
        if (s < 10)
            strb.Append(0);
        strb.Append(s);
        timer.text = strb.ToString();
        if (time < 30.0f)
            timer.color = Color.red;
        else
            timer.color = Color.white;

        //timer.text
    }
    public void SetPain()
    {
        pain.enabled = true;
        Color color = pain.color;
        color.a = painStartAlpha;
        pain.color = color;
        enabled = false;
    }
    public void StopPain()
    {
        pain.enabled = true;
    }
    public void StartPain()
    {
        enabled = true;
        pain.enabled = true;
        Color color = pain.color;
        color.a = painStartAlpha;
        pain.color = color;
    }
    public void SetHealth(float precent)
    {
        //Debug.Log("Call");
        health.fillAmount = precent;
    }
    public void SetStamina(float precent)
    {
       // Debug.Log("Call");
        stamina.fillAmount = precent;
    }
    protected void LateUpdate()
    {
        if(pain.color.a > 0)
        {
            Color color = pain.color;
            color.a = color.a - Time.deltaTime;
            pain.color = color;
        }
        else
        {
            pain.enabled = false;
            enabled = false;
        }
    }
}
