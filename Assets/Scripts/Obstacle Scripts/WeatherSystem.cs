using System.Collections;
using UnityEngine;

public class WeatherSystem : MonoBehaviour
{
    public ParticleSystem rainEffect;
    public ParticleSystem stormEffect;
    private Light sunLight;
    public float weatherChangeInterval = 10f;
    private float timer;

    public GameObject backGround;

    enum WeatherType { Sunny, Rain, Storm }
    private WeatherType currentWeather;

    public Transform cameraTransform;

   

    private SpriteRenderer bgspriteRenderer;

    void Start()
    {
        bgspriteRenderer = backGround.GetComponent<SpriteRenderer>();
        // Tạo Light
        sunLight = new GameObject("SunLight").AddComponent<Light>();
        sunLight.type = LightType.Directional;
        sunLight.intensity = 1f;

        ChangeWeather();
    }

    void Update()
    {
        if (!PlayerController.instance.player_Died)
        {
            timer += Time.deltaTime;
            if (timer >= weatherChangeInterval)
            {
                ChangeWeather();
                timer = 0;
            }

        }
        else
        {
            //stop all effects
            rainEffect.Stop();
            stormEffect.Stop();

        };
    }

    void ChangeWeather()
    {
        currentWeather = (WeatherType)Random.Range(0, 3);
        switch (currentWeather)
        {
            case WeatherType.Sunny:
                Debug.Log("Sunny");
                sunLight.intensity = 1f;
                rainEffect.Stop();
                stormEffect.Stop();
                RenderSettings.fog = false; // fog mean sương mù
                break;
            case WeatherType.Rain:
                Debug.Log("Rain");
                StartCoroutine(DarkenOverTime(3f,0.5f));
                //sunLight.intensity = 0.3f;
                rainEffect.Play();
                stormEffect.Stop();
                RenderSettings.fog = true;
                break;
            case WeatherType.Storm:
                Debug.Log("Storm");
                StartCoroutine(DarkenOverTime(7f,0.7f));
                //sunLight.intensity = 0.1f;
                rainEffect.Play();
                stormEffect.Play();
                RenderSettings.fog = true;
                break;
        }
    }

   
    IEnumerator DarkenOverTime(float duration, float darkFactor)
    {
        Color initialColor = bgspriteRenderer.color;  // Save the initial color of the sprite
        Color targetColor = initialColor * darkFactor;   // Darken the color by 50%

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;  // Update elapsed time
            float t = Mathf.Clamp01(elapsedTime / duration);  // Normalize the time between 0 and 1

            // Gradually interpolate the color from initial to target using Lerp
            bgspriteRenderer.color = Color.Lerp(initialColor, targetColor, t);

            yield return null;  // Wait until the next frame
        }

        // Ensure the final color is the target color
        bgspriteRenderer.color = targetColor;

        // Wait for a moment (e.g., 1 second) before restoring the brightness
        yield return new WaitForSeconds(1f);

        // Step 2: Restore the sprite to its original brightness
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);  // Normalize the time between 0 and 1

            // Gradually interpolate the color from darkened to original
            bgspriteRenderer.color = Color.Lerp(targetColor, initialColor, t);
            yield return null;  // Wait until the next frame
        }

        // Ensure the final color is the original color
        bgspriteRenderer.color = initialColor;
    }
}
