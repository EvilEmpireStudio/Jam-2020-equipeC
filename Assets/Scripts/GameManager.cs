using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static int levelToLoad = 2;

    public int currentLevelIndex = 0;

    public static void LoadLevel( int indexLevel ) {
        levelToLoad = indexLevel;
        SceneManager.LoadScene(1); // 1 is loading screen
    }

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
        StartCoroutine(LoadLevelAfterDelay(currentLevelIndex, delay));
    }
    
    public void LoadMenuAfter(float delay) {
        StartCoroutine(LoadLevelAfterDelay(0, delay));
    }
 
    IEnumerator LoadLevelAfterDelay(int index, float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadLevel(index);
    }
}
