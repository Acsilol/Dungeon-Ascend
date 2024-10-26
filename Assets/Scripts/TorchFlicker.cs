using UnityEngine;

public class TorchFlicker : MonoBehaviour
{
    public Light torchLight;
    public float minIntensity = 0.8f;
    public float maxIntensity = 1.2f;
    public float flickerSpeed = 0.1f;

    void Update()
    {
        float randomIntensity = Random.Range(minIntensity, maxIntensity);
        torchLight.intensity = Mathf.Lerp(torchLight.intensity, randomIntensity, flickerSpeed);
    }
}
