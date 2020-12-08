using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void Lose() {
        StartCoroutine(LoadSceneAfterDelay(SceneManager.GetActiveScene().name, 3f));
    }

    public void Win() {
        StartCoroutine(LoadSceneAfterDelay(SceneManager.GetActiveScene().name, 3f));
    }
 
    IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
