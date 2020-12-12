using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public enum MenuState {
    MainMenu,
    Credits,
    Tutorial,
    Starting,
    Exiting
}

public class MainMenu : MonoBehaviour
{
    public CinemachinePath mainToCredits;
    public CinemachinePath creditsToMain;

    public CinemachinePath mainToTuto;
    public CinemachinePath tutoToMain;

    public CinemachinePath mainToStart;
    // public CinemachinePath mainToQuit;

    public CinemachineDollyCart cameraDollyCart;

    private MenuState currentState;

    private Animator animator;
    private void Start() {
        currentState = MenuState.MainMenu;
        animator = GetComponent<Animator>();
    }

    public void GotoCredits() {
        currentState = MenuState.Credits;
        cameraDollyCart.m_Path = mainToCredits;
        animator.Play("gotoCredits");
    }

    public void GotoTuto() {
        currentState = MenuState.Tutorial;
        cameraDollyCart.m_Path = mainToTuto;
        animator.Play("gotoTuto");
    }

    public void BackToMain() {
        switch(currentState) {
            case MenuState.Credits:
                cameraDollyCart.m_Path = creditsToMain;
                animator.Play("creditsToMain");
                break;

            case MenuState.Tutorial:
                cameraDollyCart.m_Path = tutoToMain;
                animator.Play("tutoToMain");
                break;

            default:
                break;
        }

        currentState = MenuState.MainMenu;
    }

    public void StartGame() {
        currentState = MenuState.Starting;
        cameraDollyCart.m_Path = mainToStart;
        // AnimationDoneCallback();
        animator.Play("gotoStart");
    }

    public void Quit() {
        currentState = MenuState.Exiting;
        cameraDollyCart.m_Position = 0;
        cameraDollyCart.m_Path = mainToStart;
        // AnimationDoneCallback();
        animator.Play("gotoQuit");
    }

    public void AnimationDoneCallback() {
        switch(currentState) {
            case MenuState.Starting:
                GameManager.LoadLevel(2);
                break;
            case MenuState.Exiting:
                Application.Quit();
                break;
            case MenuState.MainMenu:
                animator.Play("default");
                cameraDollyCart.m_Position = 0;
                cameraDollyCart.m_Path = mainToCredits;
                break;
        }
    }
}
