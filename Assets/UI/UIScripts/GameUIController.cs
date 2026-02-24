using UnityEngine;
using UnityEngine.UIElements;
using System;

[RequireComponent(typeof(AnnouncementManager))]
public class GameUIController : MonoBehaviour
    // Script for the UI
{
    [SerializeField] CameraBehaviour cameraBehaviour;
    [SerializeField] BulidingManager buildingManager;
    private VisualElement root;
    private Button playBtn;
    private VisualElement progressFill;
    private Label infoLabel;
    private Button zoomInBtn;
    private Button zoomOutBtn;
    private Button CamBtn;
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
    private ScrollView buildingList;
    private Button activeFilterBtn;
    private PrimaryCategory? activeFilter = null;

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
        CamBtn = root.Q<Button>("CamBtn");
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
        if (zoomInBtn != null) zoomInBtn.clicked += () => cameraBehaviour.Zoom(-1f);
        if (zoomOutBtn != null) zoomOutBtn.clicked += () => cameraBehaviour.Zoom(1f);
        if (CamBtn != null) CamBtn.clicked += () => cameraBehaviour.changePerspective();
        if (optionsBtn != null) optionsBtn.clicked += () => ShowOverlay(optionsOverlay);

        var closeOptionsBtn = root.Q<Button>("CloseOptionsBtn");
        var showGreetingBtn = root.Q<Button>("ShowGreetingBtn");
        var closeGreetingBtn = root.Q<Button>("CloseGreetingBtn");
        if (closeOptionsBtn != null) closeOptionsBtn.clicked += () => HideOverlay(optionsOverlay);
        if (showGreetingBtn != null) showGreetingBtn.clicked += () => { HideOverlay(optionsOverlay); ShowOverlay(greetingOverlay); };
        if (closeGreetingBtn != null) closeGreetingBtn.clicked += () => HideOverlay(greetingOverlay);

        buildingList = root.Q<ScrollView>("BuildingList");

        void BindFilter(string btnName, PrimaryCategory cat)
        {
            var btn = root.Q<Button>(btnName);
            if (btn != null) btn.clicked += () => SetCategoryFilter(cat, btn);
        }
        BindFilter("FilterHousing",   PrimaryCategory.Housing);
        BindFilter("FilterJobs",      PrimaryCategory.Jobs);
        BindFilter("FilterEducation", PrimaryCategory.Education);
        BindFilter("FilterSafety",    PrimaryCategory.Safety);
        BindFilter("FilterEnjoyment", PrimaryCategory.Enjoyment);

        PopulateBuildings();
    }

    private void Start()
    {
        city = City.instance;
    }

    void Update()
    {
        if (city == null) return; // avoid null refs

        //update these every frame, basically optimal to do this even though most of them rarely change
        CityName.text = city.getCityName() ?? string.Empty;
        DaysLabel.text = $"Day {city.getDayCount()}";
        PopLabel.text = city.GetStat(City.StatType.Population).ToString();
        BalanceLabel.text = $"{Math.Round(city.GetStat(City.StatType.Balance), 1)}k";
        PayoutLabel.text = $"{Math.Round(city.GetStat(City.StatType.Income), 1)}k/payout";
        progressFill.style.width = new Length(Mathf.Clamp(city.getPayoutProgress(), 0, 100), LengthUnit.Percent);

        // Update stat bars (values are 0-1, ProgressBar uses 0-100)
        jobsBar.value = (float)city.GetStat(City.StatType.Jobs) * 100;
        educationBar.value = (float)city.GetStat(City.StatType.Education) * 100;
        enjoymentBar.value = (float)city.GetStat(City.StatType.Enjoyment) * 100;
        safetyBar.value = (float)city.GetStat(City.StatType.Safety) * 100;
    }

    private void TogglePlay()
    {
        city.playPauseGame();
        playBtn.text = city.isPaused() ? "▶" : "II";
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

    private string calcStatfromValue(float value)
    {
        if (value==0f) return "";
        if (value < 0f)
        {
            if (value >= -10f) return "-";
            if (value >= -50f) return "--";
            return "---";
        }
        if (value <= 10f) return "+";
        if (value <= 50f) return "++";
        return "+++";

    }

    private VisualElement CreateBuildingCard(BuildingDefinition buildingDefinition)
    {
        string title = string.IsNullOrEmpty(buildingDefinition.displayName)
            ? buildingDefinition.name
            : buildingDefinition.displayName;
        BuildingEffects effects = buildingDefinition.effects;
        double cost = effects.cost;
        double maintenance = effects.maintenance;

        var card = new Button { name = "BuildingCard" };
        card.AddToClassList("building-card");

        var header = new VisualElement();
        header.AddToClassList("card-header");

        var titleLabel = new Label { text = title ?? string.Empty };
        titleLabel.AddToClassList("card-title");
        header.Add(titleLabel);

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
        AddStatRow(statsContainer, "Housing", calcStatfromValue(effects.housing));
        AddStatRow(statsContainer, "Jobs", calcStatfromValue(effects.jobs));
        AddStatRow(statsContainer, "Education", calcStatfromValue(effects.education));
        AddStatRow(statsContainer, "Enjoyment", calcStatfromValue(effects.enjoyment));
        AddStatRow(statsContainer, "Safety", calcStatfromValue(effects.safety));
        card.Add(statsContainer);

        var footer = new VisualElement();
        footer.AddToClassList("card-footer");
        var costLabel = new Label { text = $"${cost}k" };
        costLabel.AddToClassList("card-cost");
        footer.Add(costLabel);
        card.Add(footer);

        card.clicked += () => BuyButtonPressed(buildingDefinition);

        return card;
    }

    private void AddStatRow(VisualElement parent, string statName, string value)
    {
        if(string.IsNullOrEmpty(value)) return;
        var row = new VisualElement();
        row.AddToClassList("stat-row");

        var nameLabel = new Label { text = statName };
        nameLabel.AddToClassList("stat-name");
        row.Add(nameLabel);

        var valueLabel = new Label { text = value };
        valueLabel.AddToClassList("stat-value");
        if (value == "0") valueLabel.AddToClassList("stat-zero");
        else if (value.StartsWith("-")) valueLabel.AddToClassList("stat-negative");
        row.Add(valueLabel);

        parent.Add(row);
    }

    private void SetCategoryFilter(PrimaryCategory cat, Button btn)
    {
        activeFilterBtn?.RemoveFromClassList("category-filter-btn--active");
        activeFilter = activeFilter == cat ? (PrimaryCategory?)null : cat;
        activeFilterBtn = activeFilter == null ? null : btn;
        activeFilterBtn?.AddToClassList("category-filter-btn--active");
        PopulateBuildings();
    }

    private void PopulateBuildings()
    {
        buildingList.Clear();
        if (buildingManager?.buildingDefinitions == null) return;
        foreach (var def in buildingManager.buildingDefinitions)
        {
            if (def == null || def.primaryCategory == PrimaryCategory.TownHall) continue;
            if (activeFilter != null && def.primaryCategory != activeFilter) continue;
            buildingList.Add(CreateBuildingCard(def));
        }
    }

    // Public API Methods
    public void SetInfoMessage(string msg) => infoLabel.text = msg ?? string.Empty;

    private void BuyButtonPressed(BuildingDefinition buildingDefinition)
    {
        buildingManager.selectedBuilding = buildingDefinition;
    }
}
