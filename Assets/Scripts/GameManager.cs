using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void Lose() {
        StartCoroutine(LoadSceneAfterDelay("mainScene", 3f));
    }

    public void Win() {
        StartCoroutine(LoadSceneAfterDelay("mainScene", 3f));
    }
 
    IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
