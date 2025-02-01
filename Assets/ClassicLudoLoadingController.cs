using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class LudoLoadingController : MonoBehaviour
{
   
    public float splashDuration = 2f;
    public Slider loadingSlider;

    void Start()
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

        SceneManager.LoadScene("classicludo"); 
    }
}
