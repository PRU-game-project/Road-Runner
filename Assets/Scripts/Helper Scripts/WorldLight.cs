using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WorldLight : MonoBehaviour
{
    public float duration = 5f;
    
    [SerializeField]
    private Gradient gradient;
    private Light2D _light;
    private float _startTime;
    // Start is called before the first frame update
    void Start()
    {
        _light = GetComponent<Light2D>();
        _startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the time elapsed since the start time
        float timeElapsed = Time.time - _startTime;
        // Calculate the percentage based on the sine of the time elapsed
        float percentage = Mathf.Sin( timeElapsed / duration * Mathf.PI * 2) * 0.5f + 0.5f;
        // Clamp the percentage to be between & and 1
        percentage = Mathf.Clamp01(percentage);
        _light.color = gradient.Evaluate(percentage);
    }
}
