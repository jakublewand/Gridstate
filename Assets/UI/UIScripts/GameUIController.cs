using UnityEngine;
using UnityEngine.UIElements;

public class GameUIController : MonoBehaviour
{
    private VisualElement _root;
    private Button _playBtn;
    private VisualElement _progressFill;
    private Label _infoLabel;

    private Button _zoomInBtn;
    private Button _zoomOutBtn;

    private bool _isPaused = false;
    private float _payoutTimer = 0f;
    private const float SECONDS_PER_DAY = 10f;

    void OnEnable()
    {
        var uiDoc = GetComponent<UIDocument>();
        _root = uiDoc.rootVisualElement;

        // Query UI Elements
        _playBtn = _root.Q<Button>("PlayBtn");
        _progressFill = _root.Q<VisualElement>("ProgressFill");
        _infoLabel = _root.Q<Label>("InfoLabel");

        _zoomInBtn = _root.Q<Button>("ZoomInBtn");
        _zoomOutBtn = _root.Q<Button>("ZoomOutBtn");

        // Event Listeners
        if (_playBtn != null) _playBtn.clicked += TogglePlay;
        if (_zoomInBtn != null) _zoomInBtn.clicked += () => Zoom(-1f);
        if (_zoomOutBtn != null) _zoomOutBtn.clicked += () => Zoom(1f);
    }

    void Update()
    {
        if (!_isPaused && _progressFill != null)
        {
            // Progress Bar Demo
            _payoutTimer += Time.deltaTime;
            float progress = (_payoutTimer / SECONDS_PER_DAY) * 100f;
            _progressFill.style.width = new Length(Mathf.Clamp(progress, 0, 100), LengthUnit.Percent);

            if (_payoutTimer >= SECONDS_PER_DAY) _payoutTimer = 0f;
        }
    }

    private void TogglePlay()
    {
        _isPaused = !_isPaused;

        // Change Icon
        _playBtn.text = _isPaused ? "II" : "▶";

        // API style feedback
        SetInfoMessage(_isPaused ? "Game Paused" : "Simulation Running...");
    }

    private void Zoom(float direction)
    {
        var cam = Camera.main;
        if (cam == null) return;

        float step = 2f;
        float minY = 5f;
        float maxY = 30f;

        Vector3 pos = cam.transform.position;
        pos.y = Mathf.Clamp(pos.y + direction * step, minY, maxY);
        cam.transform.position = pos;
    }

    // Public API Methods
    public void SetInfoMessage(string msg) => _infoLabel.text = msg;
    public void SetBalance(string amount) => _root.Q<Label>("BalanceLabel").text = amount;
}