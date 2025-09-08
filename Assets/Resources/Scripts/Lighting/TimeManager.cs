using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TimeManager : MonoBehaviour
{
    // [SerializeField] private Gradient gradientNightToSunrise;
    // [SerializeField] private Gradient gradientSunriseToDay;
    // [SerializeField] private Gradient gradientDayToSunset;
    // [SerializeField] private Gradient gradientSunsetToNight;

    [SerializeField] private Light globalLight;

    [Header("Sky Preset")]
    [SerializeField] private List<SkyPreset> SkyPresets = new List<SkyPreset>();

    [SerializeField, Range(0, 24)] private int hours;
    public int Hours
    { get { return hours; } set { hours = value; OnHoursChange(value); } }

    [SerializeField] private float dayLengthInMinutes = 5f;
    private float timeOfDay01;

    [SerializeField] private Vector3 originalLightRotation = new();


    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            UpdateSkyboxForHour();
            UpdateLighting();
        }
    }

    void Start()
    {
        timeOfDay01 = hours % 24 / 24f;

        UpdateSkyboxForHour();
        UpdateLighting();
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            float dayLengthSeconds = dayLengthInMinutes * 60f;

            timeOfDay01 += Time.deltaTime / dayLengthSeconds;

            if (timeOfDay01 >= 1f)
            {
                timeOfDay01 -= 1f;
            }

            float currentHour = timeOfDay01 * 24f;
            Hours = Mathf.FloorToInt(currentHour);

            UpdateLighting();
        }
        else
        {
            UpdateSkyboxForHour();
        }

    }

    private void UpdateLighting()
    {
        if (globalLight == null) return;

        float currentHour = timeOfDay01 * 24f;

        float sunAngleX = currentHour / 24f * 360f - 90f;

        globalLight.transform.rotation = Quaternion.Euler(sunAngleX, originalLightRotation.y, originalLightRotation.z);
    }

    private void OnHoursChange(int value)
    {
        if (value == 6)
        {
            StartCoroutine(LerpSkybox(SkyPresets.Find(p => p.name == "Night").skyboxTexture, SkyPresets.Find(p => p.name == "Sunrise").skyboxTexture, 10f));
            StartCoroutine(LerpLight(SkyPresets.Find(p => p.name == "Night").lightColor, SkyPresets.Find(p => p.name == "Sunrise").lightColor, 10f));
        }
        else if (value == 8)
        {
            StartCoroutine(LerpSkybox(SkyPresets.Find(p => p.name == "Sunrise").skyboxTexture, SkyPresets.Find(p => p.name == "Day").skyboxTexture, 10f));
            StartCoroutine(LerpLight(SkyPresets.Find(p => p.name == "Sunrise").lightColor, SkyPresets.Find(p => p.name == "Day").lightColor, 10f));
        }
        else if (value == 18)
        {
            StartCoroutine(LerpSkybox(SkyPresets.Find(p => p.name == "Day").skyboxTexture, SkyPresets.Find(p => p.name == "Sunset").skyboxTexture, 10f));
            StartCoroutine(LerpLight(SkyPresets.Find(p => p.name == "Day").lightColor, SkyPresets.Find(p => p.name == "Sunset").lightColor, 10f));
        }
        else if (value == 20)
        {
            StartCoroutine(LerpSkybox(SkyPresets.Find(p => p.name == "Sunset").skyboxTexture, SkyPresets.Find(p => p.name == "Night").skyboxTexture, 10f));
            StartCoroutine(LerpLight(SkyPresets.Find(p => p.name == "Sunset").lightColor, SkyPresets.Find(p => p.name == "Night").lightColor, 10f));
        }
    }

    private void UpdateSkyboxForHour()
    {
        RenderSettings.skybox.SetFloat("_Blend", 0);

        if (hours < 6)
        {
            RenderSettings.skybox.SetTexture("_Texture1", SkyPresets.Find(p => p.name == "Night").skyboxTexture);
            RenderSettings.skybox.SetTexture("_Texture2", SkyPresets.Find(p => p.name == "Sunrise").skyboxTexture);
            globalLight.color = SkyPresets.Find(p => p.name == "Night").lightColor;
        }
        else if (hours < 8)
        {
            RenderSettings.skybox.SetTexture("_Texture1", SkyPresets.Find(p => p.name == "Sunrise").skyboxTexture);
            RenderSettings.skybox.SetTexture("_Texture2", SkyPresets.Find(p => p.name == "Day").skyboxTexture);
            globalLight.color = SkyPresets.Find(p => p.name == "Sunrise").lightColor;
        }
        else if (hours < 18)
        {
            RenderSettings.skybox.SetTexture("_Texture1", SkyPresets.Find(p => p.name == "Day").skyboxTexture);
            RenderSettings.skybox.SetTexture("_Texture2", SkyPresets.Find(p => p.name == "Sunset").skyboxTexture);
            globalLight.color = SkyPresets.Find(p => p.name == "Day").lightColor;
        }
        else if (hours < 20)
        {
            RenderSettings.skybox.SetTexture("_Texture1", SkyPresets.Find(p => p.name == "Sunset").skyboxTexture);
            RenderSettings.skybox.SetTexture("_Texture2", SkyPresets.Find(p => p.name == "Night").skyboxTexture);
            globalLight.color = SkyPresets.Find(p => p.name == "Sunset").lightColor;
        }
        else if (hours < 24)
        {
            RenderSettings.skybox.SetTexture("_Texture1", SkyPresets.Find(p => p.name == "Night").skyboxTexture);
            RenderSettings.skybox.SetTexture("_Texture2", SkyPresets.Find(p => p.name == "Sunrise").skyboxTexture);
            globalLight.color = SkyPresets.Find(p => p.name == "Night").lightColor;
        }
    }

    private IEnumerator LerpSkybox(Texture2D a, Texture2D b, float time)
    {
        RenderSettings.skybox.SetTexture("_Texture1", a);
        RenderSettings.skybox.SetTexture("_Texture2", b);
        RenderSettings.skybox.SetFloat("_Blend", 0);
        for (float i = 0f; i < time; i += Time.deltaTime)
        {
            RenderSettings.skybox.SetFloat("_Blend", i / time);
            yield return null;
        }
        RenderSettings.skybox.SetTexture("_Texture1", b);
    }

    private IEnumerator LerpLight(Color from, Color to, float time)
    {
        for (float i = 0f; i < time; i += Time.deltaTime)
        {
            globalLight.color = Color.Lerp(from, to, i / time);
            // globalLight.color = lightGradient.Evaluate(i / time);
            yield return null;
        }
    }
}
