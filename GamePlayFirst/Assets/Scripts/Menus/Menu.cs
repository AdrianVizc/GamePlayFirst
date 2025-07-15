using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] protected Menu[] menusToOpen;
    [SerializeField] protected Menu[] menusToClose;

    public void ShowMenu(int val)
    {
        CloseMenus();

        if (menusToOpen[val] == null)
        {
            Debug.Log("There is nothing at element " + val);
            return;
        }
        menusToOpen[val].gameObject.SetActive(true);        
    }

    private void CloseMenus()
    {
        foreach (Menu menu in menusToClose)
        {
            menu.gameObject.SetActive(false);
        }
    }

    public void BackButton()
    {
        if (PersistentManager.Instance.GetBoolPref("isGamePaused").Value) //temp for testing
        {
            PauseMenu.Instance.gameObject.SetActive(true);
        }
        else
        {
            MainMenu.Instance.gameObject.SetActive(true);
        }

        SettingsMenu.Instance.gameObject.SetActive(false);

        CloseMenus();
    }
}
