using UnityEngine;
using UnityEngine.UIElements;


/* Created:
  - BuildingCard.uxml - Card template (unused, went programmatic instead)
  - Assets/Resources/UI/img/ - Folder for loading images at runtime

  Modified:
  - GameUI.uxml - Wrapped BuyMenu + StatPanel in RightPanels flex container
  - GameUIStyle.uss - Added .building-card, .card-header, .card-image, .card-maintenance, .card-stats,
   .stat-row styles; reduced margins throughout
  - GameUIController.cs - Added CreateBuildingCard() and AddStatRow() methods; added demo code to
  instantiate 5 cards in OnEnable()

  Moved:
  - house0.png → Assets/Resources/UI/img/house0.png

  Result: 5 demo building cards stacking in buy menu with title + image header, maintenance, stats,
  cost, and buy button. */

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
    private Button closeOptionsBtn;
    private Button showGreetingBtn;
    private Button closeGreetingBtn;
    private ScrollView buildingList;

    private bool uiReady;
    private string lastCityName;
    private int lastDay = int.MinValue;
    private string lastPopText;
    private string lastBalanceText;
    private string lastPayoutText;
    private float lastDayProgress = -1f;
    private float lastJobsValue = -1f;
    private float lastEducationValue = -1f;
    private float lastEnjoymentValue = -1f;
    private float lastSafetyValue = -1f;

    void OnEnable()
    {
        var uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null || uiDoc.rootVisualElement == null)
        {
            return;
        }

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

        closeOptionsBtn = root.Q<Button>("CloseOptionsBtn");
        showGreetingBtn = root.Q<Button>("ShowGreetingBtn");
        closeGreetingBtn = root.Q<Button>("CloseGreetingBtn");
        buildingList = root.Q<ScrollView>("BuildingList");

        RegisterCallbacks();

        if (buildingList != null && buildingManager != null && buildingManager.buildingDefinitions != null)
        {
            buildingList.Clear();
            foreach (BuildingDefinition buildingDefinition in buildingManager.buildingDefinitions)
            {
                if (buildingDefinition == null || buildingDefinition.primaryCategory == PrimaryCategory.TownHall)
                    continue;

                buildingList.Add(CreateBuildingCard(buildingDefinition));
            }
        }

        uiReady = true;
    }

    private void Start()
    {
        city = City.instance;
    }

    void OnDisable()
    {
        UnregisterCallbacks();
        uiReady = false;
    }

    void Update()
    {
        if (!uiReady || city == null || root == null || root.panel == null) return;

        // Update only when values changed to reduce text mesh churn in UITK.
        string cityName = city.getCityName() ?? string.Empty;
        if (CityName != null && cityName != lastCityName)
        {
            CityName.text = cityName;
            lastCityName = cityName;
        }

        int day = city.getDayCount();
        if (DaysLabel != null && day != lastDay)
        {
            DaysLabel.text = $"Day {day}";
            lastDay = day;
        }

        string popText = FormatStat(city.GetStat(City.StatType.Population));
        if (PopLabel != null && popText != lastPopText)
        {
            PopLabel.text = popText;
            lastPopText = popText;
        }

        string balanceText = FormatStat(city.GetStat(City.StatType.Balance));
        if (BalanceLabel != null && balanceText != lastBalanceText)
        {
            BalanceLabel.text = balanceText;
            lastBalanceText = balanceText;
        }

        string payoutText = $"{FormatStat(city.GetStat(City.StatType.Income))}/day";
        if (PayoutLabel != null && payoutText != lastPayoutText)
        {
            PayoutLabel.text = payoutText;
            lastPayoutText = payoutText;
        }

        float dayProgress = Mathf.Clamp(city.getDayProgress(), 0f, 100f);
        if (progressFill != null)
        {
            if (!Mathf.Approximately(dayProgress, lastDayProgress))
            {
                progressFill.style.width = new Length(dayProgress, LengthUnit.Percent);
                lastDayProgress = dayProgress;
            }
        }

        // Update stat bars (values are 0-1, ProgressBar uses 0-100)
        double population = city.GetStat(City.StatType.Population);
        float jobsValue = SafePercent(city.GetStat(City.StatType.Jobs), population);
        float educationValue = SafePercent(city.GetStat(City.StatType.Education), population);
        float enjoymentValue = SafePercent(city.GetStat(City.StatType.Enjoyment), population);
        float safetyValue = SafePercent(city.GetStat(City.StatType.Safety), population);

        if (jobsBar != null && !Mathf.Approximately(jobsValue, lastJobsValue))
        {
            jobsBar.value = jobsValue;
            lastJobsValue = jobsValue;
        }

        if (educationBar != null && !Mathf.Approximately(educationValue, lastEducationValue))
        {
            educationBar.value = educationValue;
            lastEducationValue = educationValue;
        }

        if (enjoymentBar != null && !Mathf.Approximately(enjoymentValue, lastEnjoymentValue))
        {
            enjoymentBar.value = enjoymentValue;
            lastEnjoymentValue = enjoymentValue;
        }

        if (safetyBar != null && !Mathf.Approximately(safetyValue, lastSafetyValue))
        {
            safetyBar.value = safetyValue;
            lastSafetyValue = safetyValue;
        }
    }

    private void TogglePlay()
    {
        if (city == null || playBtn == null) return;
        city.playPauseGame();
        playBtn.text = city.isPaused() ? ">" : "||";
    }


    // Overlay Methods
    private void ShowOverlay(VisualElement overlay)
    {
        if (overlay == null) return;
        overlay.RemoveFromClassList("hidden");
    }

    private void HideOverlay(VisualElement overlay)
    {
        if (overlay == null) return;
        overlay.AddToClassList("hidden");
    }

    private VisualElement CreateBuildingCard(BuildingDefinition buildingDefinition)
    {
        string title = string.IsNullOrEmpty(buildingDefinition.displayName)
            ? buildingDefinition.name
            : buildingDefinition.displayName;
        BuildingEffects effects = buildingDefinition.effects;
        double cost = effects.cost;
        double maintenance = effects.maintenance;

        var card = new VisualElement { name = "BuildingCard" };
        card.AddToClassList("building-card");

        var header = new VisualElement();
        header.AddToClassList("card-header");

        var titleLabel = new Label { text = title ?? string.Empty };
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

        buyBtn.clicked += () => BuyButtonPressed(buildingDefinition);

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
    public void SetInfoMessage(string msg)
    {
        if (infoLabel != null) infoLabel.text = msg ?? string.Empty;
    }

    private void BuyButtonPressed(BuildingDefinition buildingDefinition)
    {
        if (buildingManager == null) return;
        buildingManager.selectedBuilding = buildingDefinition;
    }

    private static string FormatStat(double value)
    {
        return value.ToString("0.##");
    }

    private static float SafePercent(double value, double population)
    {
        if (population <= 0d) return 0f;

        double result = value / population * 100d;
        if (double.IsNaN(result) || double.IsInfinity(result)) return 0f;

        return Mathf.Clamp((float)result, 0f, 100f);
    }

    private void RegisterCallbacks()
    {
        if (playBtn != null) playBtn.clicked += TogglePlay;
        if (zoomInBtn != null) zoomInBtn.clicked += OnZoomInClicked;
        if (zoomOutBtn != null) zoomOutBtn.clicked += OnZoomOutClicked;
        if (optionsBtn != null) optionsBtn.clicked += OnOptionsClicked;
        if (closeOptionsBtn != null) closeOptionsBtn.clicked += OnCloseOptionsClicked;
        if (showGreetingBtn != null) showGreetingBtn.clicked += OnShowGreetingClicked;
        if (closeGreetingBtn != null) closeGreetingBtn.clicked += OnCloseGreetingClicked;
    }

    private void UnregisterCallbacks()
    {
        if (playBtn != null) playBtn.clicked -= TogglePlay;
        if (zoomInBtn != null) zoomInBtn.clicked -= OnZoomInClicked;
        if (zoomOutBtn != null) zoomOutBtn.clicked -= OnZoomOutClicked;
        if (optionsBtn != null) optionsBtn.clicked -= OnOptionsClicked;
        if (closeOptionsBtn != null) closeOptionsBtn.clicked -= OnCloseOptionsClicked;
        if (showGreetingBtn != null) showGreetingBtn.clicked -= OnShowGreetingClicked;
        if (closeGreetingBtn != null) closeGreetingBtn.clicked -= OnCloseGreetingClicked;
    }

    private void OnZoomInClicked()
    {
        if (cameraBehaviour != null) cameraBehaviour.Zoom(-1f);
    }

    private void OnZoomOutClicked()
    {
        if (cameraBehaviour != null) cameraBehaviour.Zoom(1f);
    }

    private void OnOptionsClicked()
    {
        ShowOverlay(optionsOverlay);
    }

    private void OnCloseOptionsClicked()
    {
        HideOverlay(optionsOverlay);
    }

    private void OnShowGreetingClicked()
    {
        HideOverlay(optionsOverlay);
        ShowOverlay(greetingOverlay);
    }

    private void OnCloseGreetingClicked()
    {
        HideOverlay(greetingOverlay);
    }
}
