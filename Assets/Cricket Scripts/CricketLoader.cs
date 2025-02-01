using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CricketLoader : MonoBehaviour
{
    public float splashDuration = 2f;
    public Slider loadingSlider;

    private void Start()
    {
        StartCoroutine(LoadNextSceneAfterDelay());
    }
    IEnumerator LoadNextSceneAfterDelay()
    {
        float elapsedTime = 0f;

        while (elapsedTime < splashDuration)
        {
            float value = elapsedTime / splashDuration;
            loadingSlider.value = value;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        loadingSlider.value = 1f;
        SceneManager.LoadScene("Menu");
    }
}
