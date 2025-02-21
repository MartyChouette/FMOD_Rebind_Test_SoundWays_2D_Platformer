using UnityEngine;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{

    [Header("Menu Objects")]
    [SerializeField] private GameObject _mainMenuCanvasGO;
    [SerializeField] private GameObject _settingsMenuCanvasGO;

    [Header("Player Scripts to Deactivate on Pause")] 
    [SerializeField] private PlayerController _player;


    [Header("First Selected")] 
    [SerializeField] private GameObject _mainMenuFirst;
    [SerializeField] private GameObject _settingsMenuFirst;

    private bool isPaused;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _mainMenuCanvasGO.SetActive(false);
        _settingsMenuCanvasGO.SetActive(false);


    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.instance.MenuOpenCloseInput)
        {
            if (!isPaused)
            {
                Pause();
            }
            else
            {
                Unpause();
            }
        }
        
    }


    public void Pause()
    {
        isPaused=true;
        Time.timeScale = 0f;

        _player.enabled = false;

        OpenMainMenu();

        Debug.Log("TRYING TO PAUSE HERE");
    }

    public void Unpause()
    {
        isPaused = false;
        Time.timeScale = 1f;

        _player.enabled = true;

        CloseAllMenus();

        Debug.Log("TRYING TO UNPAUSE THE GAME HERE");
    }

    private void OpenMainMenu()
    {
        _mainMenuCanvasGO.SetActive(true);
        _settingsMenuCanvasGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject((_mainMenuFirst));
        
    }

    private void OpenSettingsMenuHandle()
    {
        _mainMenuCanvasGO.SetActive(false);
        _settingsMenuCanvasGO.SetActive(true);

        EventSystem.current.SetSelectedGameObject((_settingsMenuFirst));


    }

    private void CloseAllMenus()
    {
        _mainMenuCanvasGO.SetActive(false);
        _settingsMenuCanvasGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
    }

    #region Main Menu Button Actions

    public void OnSettingsPress()
    {
        OpenSettingsMenuHandle();
    }

    public void OnResumePress()
    {
        Unpause();
    }

    #endregion

    #region Settings Menu Button Actions

    public void OnSettingsBack()
    {
        OpenMainMenu();
    }

    #endregion
}
