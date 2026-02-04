using UnityEngine;
using UnityEngine.UIElements;

public class GameUIController : MonoBehaviour
    // Script for the UI
{

    private VisualElement root;
    private Button playBtn;
    private VisualElement progressFill;
    private Label infoLabel;
    private Button zoomInBtn;
    private Button zoomOutBtn;
    private City city;
    private Label CityName;
    private Label DaysLabel;
    private Label PopLabel;
    private Label PayoutLabel;
    private Label BalanceLabel;

    void OnEnable()
    {
        var uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;

        // Query UI Elements
        playBtn = root.Q<Button>("PlayBtn");
        progressFill = root.Q<VisualElement>("ProgressFill");
        infoLabel = root.Q<Label>("InfoLabel");
        zoomInBtn = root.Q<Button>("ZoomInBtn");
        zoomOutBtn = root.Q<Button>("ZoomOutBtn");
        CityName = root.Q<Label>("CityName");
        DaysLabel = root.Q<Label>("DaysLabel");
        PopLabel = root.Q<Label>("PopLabel");
        PayoutLabel = root.Q<Label>("PayoutLabel");
        BalanceLabel = root.Q<Label>("BalanceLabel");

        // Event Listeners
        if (playBtn != null) playBtn.clicked += TogglePlay;
        if (zoomInBtn != null) zoomInBtn.clicked += () => Zoom(-1f);
        if (zoomOutBtn != null) zoomOutBtn.clicked += () => Zoom(1f);
    }

    private void Start()
    {
        city = City.instance;
    }

    void Update()
    {
        if (city == null) return; // avoid null refs

        //update these every frame, basically optimal to do this even though most of them rarely change
        CityName.text = city.getCityName();
        DaysLabel.text = $"Day {city.getDayCount()}";
        PopLabel.text = city.getPopulation().ToString();
        BalanceLabel.text = city.getBalance().ToString();
        PayoutLabel.text = $"{city.getIncome()}/day";
        progressFill.style.width = new Length(Mathf.Clamp(city.getDayProgress(), 0, 100), LengthUnit.Percent);
    }

    private void TogglePlay()
    {
        city.playPauseGame();
        playBtn.text = city.isPaused() ? "▶" : "II";
    }

    //vibe coded zoom function, remove/improve
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
    public void SetInfoMessage(string msg) => infoLabel.text = msg;
}