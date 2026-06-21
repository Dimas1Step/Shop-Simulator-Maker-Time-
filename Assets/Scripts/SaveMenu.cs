using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SaveMenu : MonoBehaviour
{
    public TMP_Text slot1Text;
    public TMP_Text slot2Text;
    public TMP_Text slot3Text;


    void Start()
    {
        UpdateSlotTexts();
    }


    void UpdateSlotTexts()
    {
        UpdateSlotText(1, slot1Text);
        UpdateSlotText(2, slot2Text);
        UpdateSlotText(3, slot3Text);
    }


    void UpdateSlotText(int slot, TMP_Text text)
    {
        if (PlayerPrefs.HasKey($"HP_{slot}"))
        {
            int hp = PlayerPrefs.GetInt($"HP_{slot}");
            int crystals = PlayerPrefs.GetInt($"Crystals_{slot}");

            text.text = $"HP: {hp}\nКристали: {crystals}";
        }
        else
        {
            text.text = $"Слот {slot}\nПорожній слот";
        }
    }


    public void OnSaveSlotClicked(int slot)
    {
        PlayerPrefs.SetInt("CurrentSlot", slot);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Home");
    }
}