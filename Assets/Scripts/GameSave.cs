using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameSave : MonoBehaviour
{
    public int crystals;
    public int health = 100;

    void Start()
    {
        LoadGame();
    }

    public void SaveGame()
    {
        int slot = PlayerPrefs.GetInt("CurrentSlot");

        PlayerPrefs.SetInt($"Crystals_{slot}", crystals);
        PlayerPrefs.SetInt($"Health_{slot}", health);
        PlayerPrefs.SetString($"Time_{slot}",
            DateTime.Now.ToString("dd.MM.yyyy HH:mm"));

        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        int slot = PlayerPrefs.GetInt("CurrentSlot");

        crystals = PlayerPrefs.GetInt($"Crystals_{slot}", 0);
        health = PlayerPrefs.GetInt($"Health_{slot}", 100);
    }
}