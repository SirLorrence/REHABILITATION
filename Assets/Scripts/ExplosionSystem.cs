using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSystem : MonoBehaviour
{
    Vector3 originalScale = new Vector3(0.01f, 0.01f, 0.01f);
    public Vector3 destinationScale = new Vector3(4.0f, 4.0f, 4.0f);
    public float growthSpeed;
    IEnumerator startShockwave = null;
    void Start()
    {
        startShockwave = ScaleOverTime(5);
        StartCoroutine(startShockwave);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator ScaleOverTime(float time)
    {
        float currentTime = 0.0f;
        do
        {
            transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / time);
            currentTime += Time.deltaTime * growthSpeed;
            yield return null;
        }
        while (currentTime <= time);
        Destroy(gameObject, 0.3f);
    }
}
