using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PowerSlider : MonoBehaviour
{
    public Slider powerslider;
    public float speed;

    [SerializeField]
    private bool canPower;
    [Header("Event")]
    public  Action<float> OnPowerSliderStopped;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      if(canPower)
        {
            Move();
        }
    }

    public void StartMoving()
    {
        canPower = true;
    }

    public void StopMoving()
    {
       if(!canPower)
        {
            return;
        }

        canPower = false;
        OnPowerSliderStopped?.Invoke(powerslider.value);
    }
    private void Move()
    {
        powerslider.value = (Mathf.Sin(Time.time * speed) + 1) / 2;
    }
}
