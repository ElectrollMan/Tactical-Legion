using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Transform hud;
    public Text playerName_Text;
    public Text playerHP_Text;
    public Transform playerArrow;

    public void UpdateColor(Color color)
    {
        playerName_Text.color = color;
        playerHP_Text.color = color;
        playerArrow.GetComponent<Image>().color = color;
    }

    public void UpdateName(string name)
    {
        playerName_Text.text = name;
    }

    public void UpdatePlayerHP(int hp)
    {
        playerHP_Text.text = hp.ToString();
    }

    public void SetArrowActive(bool isActive)
    {
        playerArrow.gameObject.SetActive(isActive);
    }

    public void SetHudActive(bool isActive)
    {
        hud.gameObject.SetActive(isActive);
    }
}
