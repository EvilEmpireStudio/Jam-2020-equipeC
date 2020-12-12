using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Slider progressBar;
    public TextMeshProUGUI text;

    void Start() 
    {
        StartCoroutine(LoadAsync(GameManager.levelToLoad));
    }

    // void Update()
    // {
    // }

    IEnumerator LoadAsync(int index) {
        text.text = "Loading... 0%";
        yield return new WaitForSeconds(0.3f);

        Debug.Log("Start coroutine to load scene " + index + " - Time " + Time.timeSinceLevelLoad);
        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(index);
        loadingOperation.allowSceneActivation = false;
        Debug.Log("loadingOperation started" + " - Time " + Time.timeSinceLevelLoad);
        while(loadingOperation.progress < 0.9f)
        {
            var value = Mathf.Clamp01(loadingOperation.progress / 0.9f);
            var prct = Mathf.RoundToInt(value * 100);
            Debug.Log("Loading... " + prct + "%" + " - Time " + Time.timeSinceLevelLoad);
            text.text = "Loading... " + prct + "%";
            progressBar.value = value;

            yield return null;
        }

        var value2 = Mathf.Clamp01(loadingOperation.progress / 0.9f);
        var prct2 = Mathf.RoundToInt(value2 * 100);
        // Debug.Log("Loading... " + prct + "%" + " - Time " + Time.timeSinceLevelLoad);
        text.text = "Loading... " + prct2 + "%";
        progressBar.value = value2;

        Debug.Log("Before scene Activation" + " - Time " + Time.timeSinceLevelLoad);
        loadingOperation.allowSceneActivation = true;
        Debug.Log("AFTER scene Activation" + " - Time " + Time.timeSinceLevelLoad);
    }
}
