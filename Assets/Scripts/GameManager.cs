using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager INSTANCE;

    [SerializeField] protected QuaterbackController playerQuaterback = default;
    [Header("Callbacks")]
    [SerializeField] protected UnityEvent onWin = default;
    [SerializeField] protected UnityEvent onLose = default;
    public QuaterbackController Player => playerQuaterback;

    private void Start() {
        INSTANCE = this;
    }

    public void Win() {
        onWin?.Invoke();
    }

    public void Lose() {
        onLose?.Invoke();
    }

    public void ReloadLevelAfter(float delay) {
        StartCoroutine(LoadSceneAfterDelay(SceneManager.GetActiveScene().name, delay));
    }
 
    IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
