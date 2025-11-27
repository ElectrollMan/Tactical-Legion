using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Global;

public class ItemPanel : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public Text countText;
    [TextArea]
    public string infoTextString;
    public Weapon weaponType = Weapon.BAZOOKA;

    private Button button;
    private PlayerController currentPlayer;

    void Start()
    {
        button = GetComponent<Button>();
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.TurnBaseController == null)
            return;

        currentPlayer = GameManager.Instance.TurnBaseController
            .GetCurrentTurnTeam()
            .GetCurrentTurnPlayer()
            .PlayerController;

        int ammo = GetAmmoForWeapon(currentPlayer, weaponType);

        if (countText != null)
            countText.text = ammo.ToString();

        if (button != null)
            button.interactable = ammo > 0;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentPlayer == null)
            return;

        int ammo = GetAmmoForWeapon(currentPlayer, weaponType);
        if (ammo <= 0)
        {
            Debug.Log(weaponType + " has no ammo!");
            return;
        }

        currentPlayer.useWeapon[(int)weaponType] = true;
        UIManager.Instance.SetEndTurnButtonActive(false);

        // ✅ NEW: Automatically close and lock the bag when weapon selected
        if (UIManager.Instance != null)
        {
            UIManager.Instance.CloseBagAndLock();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.Instance.SetInfoTipText(infoTextString);
        UIManager.Instance.SetInfoTipActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.SetInfoTipText("");
        UIManager.Instance.SetInfoTipActive(false);
    }

    private int GetAmmoForWeapon(PlayerController player, Weapon weapon)
    {
        switch (weapon)
        {
            case Weapon.BAZOOKA:
                return player.bazookaAmmo;
            case Weapon.BIG_BAZOOKA:
                return player.bigBazookaAmmo;
            case Weapon.MULTI_BAZOOKA:
                return player.multiBazookaAmmo;
            case Weapon.SMALL_ROCKET:
                return player.smallRocketAmmo;
            default:
                return 0;
        }
    }
}
