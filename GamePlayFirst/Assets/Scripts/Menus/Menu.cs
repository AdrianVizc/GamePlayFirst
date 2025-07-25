using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    protected virtual void CloseMenus()
    {
        foreach (Menu menu in menusToClose)
        {
            menu.gameObject.SetActive(false);
        }
    }

    public void BackButton()
    {
        Scene thisScene = SceneManager.GetSceneByName(PersistentManager.Instance.GetStringPref("PlayScene").Value);

        if (thisScene.IsValid())
        {
            PauseMenu.Instance.background.SetActive(true);
        }
        else
        {
            MainMenu.Instance.gameObject.SetActive(true);
        }

        SettingsMenu.Instance.gameObject.SetActive(false);

        CloseMenus();
    }
}
