using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LudoLoadingPanel : MonoBehaviour
{
    public float splashDuration = 2f;
    public Slider loadingSlider;
    public GameObject SelectScene;
    public GameObject CurrentPanel;

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
        SelectScene.SetActive(true);
        CurrentPanel.SetActive(false);
    }
}
