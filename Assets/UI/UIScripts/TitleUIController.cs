using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class TitleUIController : MonoBehaviour
{
    private UIDocument _uiDocument;
    private VisualElement _root;
    private VisualElement _optionsOverlay;
    public Toggle audioToggle;
    public Toggle hideUnaffordableToggle;

    void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
    }

    void OnEnable()
    {
        _root = _uiDocument.rootVisualElement;
        _optionsOverlay = _root.Q<VisualElement>("OptionsOverlay");
        audioToggle = _root.Q<Toggle>("AudioToggle");
        hideUnaffordableToggle = _root.Q<Toggle>("HideUnaffordableToggle");

        audioToggle.RegisterValueChangedCallback(evt => OnAudioToggleChanged(evt.newValue));
        hideUnaffordableToggle.RegisterValueChangedCallback(evt => OnHideUnaffordableChanged(evt.newValue));
        audioToggle.value = PlayerPrefs.GetInt("Audio", 1) == 1;
        hideUnaffordableToggle.value = PlayerPrefs.GetInt("HideUnaffordable", 0) == 1;

        _root.Q<Button>("NewGameBtn").clicked += OnNewGameClicked;
        _root.Q<Button>("OptionsBtn").clicked += OnOptionsClicked;
        _root.Q<Button>("QuitBtn").clicked += OnQuitClicked;
        _root.Q<Button>("CloseOptionsBtn").clicked += OnCloseOptionsClicked;
    }

    void OnNewGameClicked()
    {
        SceneManager.LoadScene("GameScene");
    }

    void OnOptionsClicked()
    {
        _optionsOverlay.RemoveFromClassList("hidden");
    }

    void OnCloseOptionsClicked()
    {
        _optionsOverlay.AddToClassList("hidden");
    }

    void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void OnAudioToggleChanged(bool isOn) { PlayerPrefs.SetInt("Audio", isOn ? 1 : 0); PlayerPrefs.Save(); }
    void OnHideUnaffordableChanged(bool isOn) { PlayerPrefs.SetInt("HideUnaffordable", isOn ? 1 : 0); PlayerPrefs.Save(); }
}
