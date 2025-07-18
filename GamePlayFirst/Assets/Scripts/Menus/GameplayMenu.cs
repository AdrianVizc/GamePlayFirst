using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameplayMenu : Menu
{
    /*public static SettingsGameplay instance;

    [Space]
    [SerializeField] TMP_Dropdown cameraViewDropdown;

    [SerializeField] private GameObject moveSpeedLabelObject;
    [SerializeField] private GameObject moveSpeedSliderObject;
    [SerializeField] private GameObject moveSpeedInputObject;

    [Space]

    [SerializeField] private Slider moveSpeedSlider;
    [SerializeField] TMP_InputField moveSpeedInputField;
    [SerializeField] private Slider inputLatencySlider;
    [SerializeField] TMP_InputField inputLatencySliderValueText;

    public override void ReturnToPrevious()
    {
        UIManager.instance.SettingsMenu.ClearArrows();
        base.ReturnToPrevious();
    }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);

            return;
        }

        LoadSliders(); //In awake so Player script can get the value
    }

    private void OnEnable()
    {
        DisableIfInGame();

        LoadSliders();
        SetMoveSpeedValueText();
        SetInputLatencyValueText();
        LoadCameraDropdown();
    }

    public void LoadCameraDropdown()
    {
        cameraViewDropdown.value = PersistentManager.instance.GetIntPref("cameraView").Value;
    }


    public void SetCameraView(int val)
    {
        var camView = PersistentManager.instance.GetIntPref("cameraView");
        camView.SetValue(val);
        camView.Save();

        if (GameLoop.instance != null)
        {
            GameLoop.instance.player.SwitchCamera();
        }
    }


    private void LoadSliders() //Loads and sets the sliders to previous setting
    {
        var savedMoveValue = PersistentManager.instance.GetFloatPref("relativeMoveSpeed").Value;
        moveSpeedSlider.value = savedMoveValue;

        var savedLatencyValue = PersistentManager.instance.GetFloatPref("inputLatency").Value;
        inputLatencySlider.value = savedLatencyValue;
    }

    #region MovementSpeedSlider

    private void SetMoveSpeedValueText() //Constantly update the text to = the slider value
    {
        var val = PersistentManager.instance.GetFloatPref("relativeMoveSpeed").Value;

        if (val % 1 == 0) //if value is whole, will display as int
        {
            moveSpeedInputField.text = val.ToString("F0");
        }
        else
        {
            moveSpeedInputField.text = val.ToString("F2");
        }
    }

    private void DisableIfInGame() //Disables the move speed slider if in game
    {
        if (GameLoop.instance == null)
        {
            moveSpeedSlider.interactable = true;
            //moveSpeedLabelObject.SetActive(true);
            //moveSpeedSliderObject.SetActive(true);
            //moveSpeedInputObject.SetActive(true);
        }
        else
        {
            moveSpeedSlider.interactable = false;
            //moveSpeedLabelObject.SetActive(false);
            //moveSpeedSliderObject.SetActive(false);
            //moveSpeedInputObject.SetActive(false);
        }
    }

    public void ManualEditSpeedValue(string val)
    {
        float value = float.Parse(val);

        value = Mathf.Clamp(value, 1f, 3f);
        var temp = PersistentManager.instance.GetFloatPref("relativeMoveSpeed");
        temp.SetValue(value);
        temp.Save();

        LoadSliders();
        SetMoveSpeedValueText();
    }

    public void SaveMoveSliderValue(float val)
    {
        float roundedValue = Mathf.Round(val * 100f) / 100f; //Value at 2 decimal

        var temp = PersistentManager.instance.GetFloatPref("relativeMoveSpeed");

        temp.SetValue(roundedValue);
        temp.Save();

        SetMoveSpeedValueText();
    }

    #endregion

    #region InputLatencySlider

    private void SetInputLatencyValueText()
    {
        var val = PersistentManager.instance.GetFloatPref("inputLatency").Value;

        if (val % 1 == 0)
        {
            inputLatencySliderValueText.text = val.ToString("F0") + " ms";
        }
        else
        {
            inputLatencySliderValueText.text = val.ToString("F2") + " ms";
        }
    }

    public void ManualEditLatencyValue(string val)
    {
        float value = float.Parse(val);

        value = Mathf.Clamp(value, 0f, 10);
        var temp = PersistentManager.instance.GetFloatPref("inputLatency");
        temp.SetValue(value);
        temp.Save();

        LoadSliders();
        SetInputLatencyValueText();
    }

    public void SaveInputLatencySliderValue(float val) //Changes slider value when slide moves + saves the value
    {
        float roundedValue = Mathf.Round(val * 100f) / 100f; //Value at 2 decimal

        var temp = PersistentManager.instance.GetFloatPref("inputLatency");

        temp.SetValue(roundedValue);
        temp.Save();

        SetInputLatencyValueText();
    }

    #endregion
    */

}
