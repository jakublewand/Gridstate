using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class TitleUIController : MonoBehaviour
{
    private UIDocument _uiDocument;
    private VisualElement _root;
    private VisualElement _optionsOverlay;

    void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
    }

    void OnEnable()
    {
        _root = _uiDocument.rootVisualElement;
        _optionsOverlay = _root.Q<VisualElement>("OptionsOverlay");

        _root.Q<Button>("NewGameBtn").clicked += OnNewGameClicked;
        _root.Q<Button>("LoadGameBtn").clicked += OnLoadGameClicked;
        _root.Q<Button>("OptionsBtn").clicked += OnOptionsClicked;
        _root.Q<Button>("QuitBtn").clicked += OnQuitClicked;
        _root.Q<Button>("CloseOptionsBtn").clicked += OnCloseOptionsClicked;
    }

    void OnNewGameClicked()
    {
        SceneManager.LoadScene("GameScene");
    }

    void OnLoadGameClicked()
    {
        // load functionality?
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
}
