using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;

[RequireComponent(typeof(AnnouncementManager))]
public class GameUIController : MonoBehaviour
    // Script for the UI
{
    [SerializeField] CameraBehaviour cameraBehaviour;
    [SerializeField] BulidingManager buildingManager;
    [SerializeField] AudioSource uiSounds;
    [SerializeField] AudioScript audioScript;
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
    private PrimaryCategory? activeFilter = PrimaryCategory.Housing;
    private Label categoryTitleLabel;
    private float buildingListTimer;

    void OnEnable()
    {
        var uiDoc = GetComponent<UIDocument>();
        //audioScript = uiSounds.GetComponent<AudioScript>();
        root = uiDoc.rootVisualElement;

        // Query UI Elements
        playBtn = root.Q<Button>("PlayBtn");
        progressFill = root.Q<VisualElement>("ProgressFill");
        infoLabel = root.Q<Label>("InfoLabel");
        zoomInBtn = root.Q<Button>("ZoomInBtn");
        zoomOutBtn = root.Q<Button>("ZoomOutBtn");
        CamBtn = root.Q<Button>("CamBtn");
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

        // Attach audio

        root.Query<Button>().ForEach(btn => {btn.clicked += () => uiSounds.PlayOneShot(audioScript.click);});

        // Event Listeners
        if (playBtn != null) playBtn.clicked += TogglePlay;
        if (zoomInBtn != null) zoomInBtn.clicked += () => cameraBehaviour.Zoom(-1f);
        if (zoomOutBtn != null) zoomOutBtn.clicked += () => cameraBehaviour.Zoom(1f);
        if (CamBtn != null) CamBtn.clicked += () =>  cameraBehaviour.changePerspective();
        if (optionsBtn != null) optionsBtn.clicked += () => ShowOverlay(optionsOverlay);

        var closeOptionsBtn = root.Q<Button>("CloseOptionsBtn");
        var showGreetingBtn = root.Q<Button>("ShowGreetingBtn");
        var closeGreetingBtn = root.Q<Button>("CloseGreetingBtn");
        if (closeOptionsBtn != null) closeOptionsBtn.clicked += () => HideOverlay(optionsOverlay);
        if (showGreetingBtn != null) showGreetingBtn.clicked += () => {HideOverlay(optionsOverlay); ShowOverlay(greetingOverlay); };
        if (closeGreetingBtn != null) closeGreetingBtn.clicked += () => HideOverlay(greetingOverlay);

        buildingList = root.Q<ScrollView>("BuildingList");
        categoryTitleLabel = root.Q<Label>("CategoryTitle");
        activeFilterBtn = root.Q<Button>("FilterHousing");

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

    }
    
    private void Start()
    {
        city = City.instance;
        PopulateBuildings();
    }

    void Update()
    {
        if (city == null) return; // avoid null refs

        //update these every frame, basically optimal to do this even though most of them rarely change
        CityName.text = city.getCityName() ?? string.Empty;
        DaysLabel.text = $"Day {city.getDayCount()}";
        PopLabel.text = city.GetStat(City.StatType.Population).ToString();
        BalanceLabel.text = Suf(city.GetStat(City.StatType.Balance));
        PayoutLabel.text = $"{Suf(city.GetStat(City.StatType.Income))}/payout";
        progressFill.style.width = new Length(Mathf.Clamp(city.getPayoutProgress(), 0, 100), LengthUnit.Percent);

        // Update stat bars (values are 0-1, ProgressBar uses 0-100)
        jobsBar.value = (float)city.GetStat(City.StatType.Jobs) * 100;
        educationBar.value = (float)city.GetStat(City.StatType.Education) * 100;
        enjoymentBar.value = (float)city.GetStat(City.StatType.Enjoyment) * 100;
        safetyBar.value = (float)city.GetStat(City.StatType.Safety) * 100;

        buildingListTimer += Time.deltaTime;
        if (buildingListTimer >= 1/4f)
        {
            buildingListTimer = 0f;
            PopulateBuildings();
        }
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

    private string Suf(float value)
    {
        value *= 1000;
        if (value < 1000) return Math.Round(value, 1).ToString();
        if (value < 1000000) return Math.Round(value / 1000, 1) + "K";
        if (value < 1000000000) return Math.Round(value / 1000000, 1) + "M";
        if (value < 1000000000000) return Math.Round(value / 1000000000, 1) + "B";
        if (value < 1000000000000000) return Math.Round(value / 1000000000000, 1) + "T";
        return Math.Round(value / 1000000000000000, 1) + "Q";
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
        float cost = effects.cost;
        float maintenance = effects.maintenance;

        var card = new Button { name = "BuildingCard" };
        card.clicked += () => uiSounds.PlayOneShot(audioScript.click);

        card.AddToClassList("building-card");

        var header = new VisualElement();
        header.AddToClassList("card-header");

        var titleLabel = new Label { text = title ?? string.Empty };
        titleLabel.AddToClassList("card-title");
        header.Add(titleLabel);

        var costLabel = new Label { text = $"${Suf(cost)}" };
        costLabel.AddToClassList("card-cost");
        header.Add(costLabel);

        card.Add(header);

        var maintenanceContainer = new VisualElement();
        maintenanceContainer.AddToClassList("card-maintenance");
        var mainLabel = new Label { text = "Maintenance:" };
        mainLabel.AddToClassList("card-label");
        var mainValue = new Label { text = $"{Suf(maintenance)} / day" };
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

    private void SetCategoryFilter(PrimaryCategory? cat, Button btn)
    {
        activeFilterBtn?.RemoveFromClassList("category-filter-btn--active");
        activeFilter = cat;
        activeFilterBtn = btn;
        activeFilterBtn?.AddToClassList("category-filter-btn--active");
        PopulateBuildings();
        if (categoryTitleLabel != null)
            categoryTitleLabel.text = activeFilter?.ToString() ?? "";
    }

    private void PopulateBuildings()
    {
        buildingList.Clear();
        if (buildingManager?.buildingDefinitions == null) return;
        // sort definitions by cost (cheap to expensive)
        var defs = buildingManager.buildingDefinitions
            .Where(def => def != null && def.primaryCategory != PrimaryCategory.TownHall)
            .Where(def => activeFilter == null || def.primaryCategory == activeFilter)
            .OrderBy(def => def.effects.cost);
        foreach (var def in defs)
        {
            if(city.gameState.topBalance<def.effects.cost/4) {buildingList.Add(CreateMysteryCard(def)); continue;}
            if(city.gameState.balance<def.effects.cost) {buildingList.Add(CreateCantAffordCard(def)); continue;}
            buildingList.Add(CreateBuildingCard(def));
        }
    }

    // Public API Methods
    public void SetInfoMessage(string msg) => infoLabel.text = msg ?? string.Empty;

    private VisualElement CreateCantAffordCard(BuildingDefinition buildingDefinition)
    {
        var card = CreateBuildingCard(buildingDefinition);
        card.RemoveFromClassList("building-card");
        card.AddToClassList("building-card");
        card.AddToClassList("building-card--red");
        return card;
    }

    private VisualElement CreateMysteryCard(BuildingDefinition buildingDefinition)
    {
        float cost = buildingDefinition.effects.cost;

        var card = new VisualElement();
        card.AddToClassList("building-card");
        card.AddToClassList("building-card--mystery");

        var header = new VisualElement();
        header.AddToClassList("card-header");

        var titleLabel = new Label { text = "???" };
        titleLabel.AddToClassList("card-title");
        header.Add(titleLabel);

        var costLabel = new Label { text = $"${Suf(cost)}" };
        costLabel.AddToClassList("card-cost");
        header.Add(costLabel);

        card.Add(header);
        return card;
    }

    private void BuyButtonPressed(BuildingDefinition buildingDefinition)
    {
        buildingManager.selectedBuilding = buildingDefinition;
    }
}
