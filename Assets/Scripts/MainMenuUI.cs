using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject continuePanel;
    public AudioSource audio;

    void Start()
    {
        audio = FindObjectOfType<AudioSource>();
    }

    public void OpenContinuePanel()
    {
        mainPanel.SetActive(false);
        continuePanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        continuePanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Audio()
    {
        audio.Play();
    }
}
