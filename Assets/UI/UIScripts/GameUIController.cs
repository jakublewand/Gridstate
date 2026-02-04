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
    private ProgressBar jobsBar;
    private ProgressBar educationBar;
    private ProgressBar enjoymentBar;
    private ProgressBar safetyBar;
    private Button optionsBtn;
    private VisualElement optionsOverlay;
    private VisualElement greetingOverlay;

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
        jobsBar = root.Q<ProgressBar>("JobsBar");
        educationBar = root.Q<ProgressBar>("EducationBar");
        enjoymentBar = root.Q<ProgressBar>("EnjoymentBar");
        safetyBar = root.Q<ProgressBar>("SafetyBar");
        optionsBtn = root.Q<Button>("OptionsBtn");
        optionsOverlay = root.Q<VisualElement>("OptionsOverlay");
        greetingOverlay = root.Q<VisualElement>("GreetingOverlay");

        // Event Listeners
        if (playBtn != null) playBtn.clicked += TogglePlay;
        if (zoomInBtn != null) zoomInBtn.clicked += () => Zoom(-1f);
        if (zoomOutBtn != null) zoomOutBtn.clicked += () => Zoom(1f);
        if (optionsBtn != null) optionsBtn.clicked += () => ShowOverlay(optionsOverlay);

        var closeOptionsBtn = root.Q<Button>("CloseOptionsBtn");
        var showGreetingBtn = root.Q<Button>("ShowGreetingBtn");
        var closeGreetingBtn = root.Q<Button>("CloseGreetingBtn");
        if (closeOptionsBtn != null) closeOptionsBtn.clicked += () => HideOverlay(optionsOverlay);
        if (showGreetingBtn != null) showGreetingBtn.clicked += () => { HideOverlay(optionsOverlay); ShowOverlay(greetingOverlay); };
        if (closeGreetingBtn != null) closeGreetingBtn.clicked += () => HideOverlay(greetingOverlay);

        // Demo: Create 5 building cards
        var buildingList = root.Q<ScrollView>("BuildingList");

        string[] buildingNames = { "School", "Factory", "Park", "Hospital", "Market" };
        int[] costs = { 500, 800, 300, 1200, 400 };
        int[] maintenance = { 100, 150, 50, 200, 75 };

        for (int i = 0; i < 5; i++)
        {
            buildingList.Add(CreateBuildingCard(buildingNames[i], costs[i], maintenance[i]));
        }
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

        // Update stat bars (values are 0-1, ProgressBar uses 0-100)
        jobsBar.value = (float)city.getJobs() * 100;
        educationBar.value = (float)city.getEducation() * 100;
        enjoymentBar.value = (float)city.getEnjoyment() * 100;
        safetyBar.value = (float)city.getSafety() * 100;
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

    // Overlay Methods
    private void ShowOverlay(VisualElement overlay)
    {
        overlay.RemoveFromClassList("hidden");
    }

    private void HideOverlay(VisualElement overlay)
    {
        overlay.AddToClassList("hidden");
    }

    private VisualElement CreateBuildingCard(string title, int cost, int maintenance)
    {
        var card = new VisualElement { name = "BuildingCard" };
        card.AddToClassList("building-card");

        var header = new VisualElement();
        header.AddToClassList("card-header");

        var titleLabel = new Label { text = title };
        titleLabel.AddToClassList("card-title");
        header.Add(titleLabel);

        var buildingImage = new VisualElement();
        buildingImage.AddToClassList("card-image");
        var houseImg = Resources.Load<Texture2D>("UI/img/house0");
        if (houseImg) buildingImage.style.backgroundImage = new StyleBackground(houseImg);
        header.Add(buildingImage);

        card.Add(header);

        var maintenanceContainer = new VisualElement();
        maintenanceContainer.AddToClassList("card-maintenance");
        var mainLabel = new Label { text = "Maintenance:" };
        mainLabel.AddToClassList("card-label");
        var mainValue = new Label { text = $"{maintenance} / day" };
        mainValue.AddToClassList("card-value");
        maintenanceContainer.Add(mainLabel);
        maintenanceContainer.Add(mainValue);
        card.Add(maintenanceContainer);

        var statsContainer = new VisualElement();
        statsContainer.AddToClassList("card-stats");
        AddStatRow(statsContainer, "Jobs", "+");
        AddStatRow(statsContainer, "Education", "+++");
        AddStatRow(statsContainer, "Enjoyment", "0");
        AddStatRow(statsContainer, "Safety", "0");
        card.Add(statsContainer);

        var footer = new VisualElement();
        footer.AddToClassList("card-footer");
        var costLabel = new Label { text = $"${cost}" };
        costLabel.AddToClassList("card-cost");
        var buyBtn = new Button { text = "Buy" };
        buyBtn.AddToClassList("card-buy-btn");
        footer.Add(costLabel);
        footer.Add(buyBtn);
        card.Add(footer);

        return card;
    }

    private void AddStatRow(VisualElement parent, string statName, string value)
    {
        var row = new VisualElement();
        row.AddToClassList("stat-row");

        var nameLabel = new Label { text = statName };
        nameLabel.AddToClassList("stat-name");
        row.Add(nameLabel);

        var valueLabel = new Label { text = value };
        valueLabel.AddToClassList("stat-value");
        if (value == "0") valueLabel.AddToClassList("stat-zero");
        row.Add(valueLabel);

        parent.Add(row);
    }

    // Public API Methods
    public void SetInfoMessage(string msg) => infoLabel.text = msg;
}