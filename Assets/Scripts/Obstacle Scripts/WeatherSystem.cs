using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WeatherSystem : MonoBehaviour
{
    public ParticleSystem rainEffect;
    public ParticleSystem stormEffect;
    private Light2D _light;
    public float weatherChangeInterval = 10f;
    private float timer;

    public GameObject backGround;

    enum WeatherType { Sunny, Rain, Storm }
    private WeatherType currentWeather;
    private WeatherType previousWeather = WeatherType.Sunny;

    public Transform cameraTransform;
    public GameObject lucky_Icon_Image;



    private SpriteRenderer bgspriteRenderer;

    void Start()
    {
        bgspriteRenderer = backGround.GetComponent<SpriteRenderer>();
        _light = GetComponent<Light2D>();
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

        // if weather is previous weather, do event lucky
        if (currentWeather == previousWeather)
        {
            // do event lucky
            Debug.Log("Lucky Time");
            GameplayController.instance.IsLucky = true;
            lucky_Icon_Image.SetActive(true);
            SoundManager.instance.PlayCoinSound();
        }
        else
        {
            Debug.Log("Not Lucky");

            GameplayController.instance.IsLucky = false;
            lucky_Icon_Image.SetActive(false);
            SoundManager.instance.PlaySummonSound();

        }
        switch (currentWeather)
        {
            case WeatherType.Sunny:
                Debug.Log("Sunny");
                previousWeather = WeatherType.Sunny;
                _light.intensity = 1f;
                rainEffect.Stop();
                stormEffect.Stop();
                RenderSettings.fog = false; // fog mean sương mù
                GameplayController.instance.moveSpeed += 4f * Time.deltaTime;

                break;
            case WeatherType.Rain:
                Debug.Log("Rain");
                previousWeather = WeatherType.Rain;
                StartCoroutine(DarkenOverTime(3f, 0.5f));
                _light.intensity = 0.3f;
                rainEffect.Play();
                stormEffect.Stop();
                RenderSettings.fog = true;
                GameplayController.instance.moveSpeed -= 1f * Time.deltaTime;
                break;
            case WeatherType.Storm:
                Debug.Log("Storm");
                previousWeather = WeatherType.Storm;
                StartCoroutine(DarkenOverTime(7f, 0.7f));
                _light.intensity = 0.1f;
                rainEffect.Play();
                stormEffect.Play();
                RenderSettings.fog = true;
                GameplayController.instance.moveSpeed -= 3f * Time.deltaTime;

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
