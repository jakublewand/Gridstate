using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;

[RequireComponent(typeof(AnnouncementManager))]
public class GameUIController : MonoBehaviour
    // Script for the UI
{
    public static GameUIController instance; // static instance for global reference
    [SerializeField] CameraBehaviour cameraBehaviour;
    [SerializeField] BulidingManager buildingManager;
    [SerializeField] AudioSource uiSounds;
    [SerializeField] AudioScript audioScript;
    [SerializeField] Texture2D[] kingLevelImages;  // king0, king1, king2
    [SerializeField] Texture2D[] lionLevelImages;  // lion0, lion1, lion2
    [SerializeField] Texture2D[] softLevelImages;  // soft0, soft1, soft2
    [SerializeField] long levelOnePopGoal = 10000;
    [SerializeField] long levelTwoPopGoal = 10_000_000_000L;
    private Building focusedBuilding;
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
    private VisualElement detailCard;
    private Label detailTitle;
    private Button optionsBtn;
    private VisualElement optionsOverlay;
    private VisualElement greetingOverlay;
    private Button cityCrestBtn;
    private VisualElement[] levelSteps = new VisualElement[3];
    private VisualElement[] levelBarFills = new VisualElement[2];
    private int lastCrestLevel = -1;
    private VisualElement eventPopupOverlay;
    private Label eventMessageLabel;
    private Label eventTitle;
    private Button eventCloseBtn;
    private ScrollView buildingList;
    private Button activeFilterBtn;
    private PrimaryCategory? activeFilter = PrimaryCategory.Housing;
    private Label categoryTitleLabel;
    private float buildingListTimer;
    private Button selectedCharBtn;
    private TextField cityNameField;
    private Label charQuoteLabel;
    private Button startGameBtn;
    public Toggle audioToggle;
    public Toggle hideUnaffordableToggle;
    public static bool IsEventPopupActive { get; private set; }

    void OnEnable()
    {
        instance = this;
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
        audioToggle = root.Q<Toggle>("AudioToggle");
        hideUnaffordableToggle = root.Q<Toggle>("HideUnaffordableToggle");
        greetingOverlay = root.Q<VisualElement>("GreetingOverlay");
        cityCrestBtn = root.Q<Button>("CityCrestBtn");
        levelSteps[0] = root.Q<VisualElement>("LevelStep0");
        levelSteps[1] = root.Q<VisualElement>("LevelStep1");
        levelSteps[2] = root.Q<VisualElement>("LevelStep2");
        levelBarFills[0] = root.Q<VisualElement>("LevelBarFill0");
        levelBarFills[1] = root.Q<VisualElement>("LevelBarFill1");
        eventPopupOverlay = root.Q<VisualElement>("EventPopupOverlay");
        eventMessageLabel = root.Q<Label>("EventMessageLabel");
        eventTitle = root.Q<Label> ("event-title");
        eventCloseBtn = root.Q<Button>("EventCloseBtn");

        // Attach audio

        root.Query<Button>().ForEach(btn => {btn.clicked += () => audioScript.PlaySound(audioScript.click);});

        // Event Listeners
        if (playBtn != null) playBtn.clicked += TogglePlay;
        if (zoomInBtn != null) zoomInBtn.clicked += () => cameraBehaviour.Zoom(-1f);
        if (zoomOutBtn != null) zoomOutBtn.clicked += () => cameraBehaviour.Zoom(1f);
        if (CamBtn != null) CamBtn.clicked += () =>  cameraBehaviour.changePerspective();
        var resetCamBtn = root.Q<Button>("ResetCamBtn");
        if (resetCamBtn != null) resetCamBtn.clicked += () => cameraBehaviour.resetCamera();
        if (optionsBtn != null) optionsBtn.clicked += () => ShowOverlay(optionsOverlay);

        
        var closeOptionsBtn = root.Q<Button>("CloseOptionsBtn");
        var showGreetingBtn = root.Q<Button>("ShowGreetingBtn");
        if (closeOptionsBtn != null) closeOptionsBtn.clicked += () => HideOverlay(optionsOverlay);
        if (showGreetingBtn != null) showGreetingBtn.clicked += () => { HideOverlay(optionsOverlay); ShowOverlay(greetingOverlay); RefreshLevelBar(); };
        if (cityCrestBtn != null) cityCrestBtn.clicked += () => { ShowOverlay(greetingOverlay); RefreshLevelBar(); };
        if (eventCloseBtn != null) eventCloseBtn.clicked += () => { HideOverlay(eventPopupOverlay); IsEventPopupActive = false; };
        audioToggle.RegisterValueChangedCallback(evt => OnAudioToggleChanged(evt.newValue));
        hideUnaffordableToggle.RegisterValueChangedCallback(evt => OnHideUnaffordableChanged(evt.newValue));
        audioToggle.value = PlayerPrefs.GetInt("Audio", 1) == 1;
        hideUnaffordableToggle.value = PlayerPrefs.GetInt("HideUnaffordable", 0) == 1;
        

        // Character selection
        cityNameField = root.Q<TextField>("CityNameField");
        charQuoteLabel = root.Q<Label>("CharQuote");
        var king = root.Q<Button>("KingOfAmerica");
        var lion = root.Q<Button>("LionOfTheNorth");
        var engineer = root.Q<Button>("SoftwareEngineer");
        if (king != null) king.clicked += () => SelectCharacter(king, City.Characters.king, "\"Make your city great again!\"");
        if (lion != null) lion.clicked += () => SelectCharacter(lion, City.Characters.lion, "\"My city was perfect from day one. The citizens just don't know it yet.\"");
        if (engineer != null) engineer.clicked += () => SelectCharacter(engineer, City.Characters.engineer, "\"Children's playground converted to datacenter.\"");
        startGameBtn = root.Q<Button>("StartGameBtn");
        if (startGameBtn != null)
        {
            startGameBtn.SetEnabled(false);
            startGameBtn.clicked += StartGame;
        }

        detailCard = root.Q<VisualElement>("DetailCard");
        detailTitle = root.Q<Label>("DetailTitle");
        var demolishBtn = root.Q<Button>("DemolishBtn");
        var detailCloseBtn = root.Q<Button>("DetailCloseBtn");
        if (detailCloseBtn != null) detailCloseBtn.clicked += () => HideOverlay(detailCard);
        if (demolishBtn != null) demolishBtn.clicked += () => {HideOverlay(detailCard); buildingManager.Demolish(focusedBuilding);};

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

        // Set population goal labels
        var goal0 = root.Q<Label>("LevelGoal0");
        var goal1 = root.Q<Label>("LevelGoal1");
        if (goal0 != null) goal0.text = $"👤 {SufRaw(levelOnePopGoal)}";
        if (goal1 != null) goal1.text = $"👤 {SufRaw(levelTwoPopGoal)}";

        // Show character selection on game start
        ShowOverlay(greetingOverlay);
        if (!city.isPaused()) city.playPauseGame();
        playBtn.text = "▶";
    }

    void Update()
    {
        if (city == null) return; // avoid null refs

        //update these every frame, basically optimal to do this even though most of them rarely change
        CityName.text = city.getCityName() ?? string.Empty;
        DaysLabel.text = $"Day {city.getDayCount()}";
        PopLabel.text = SufRaw(city.GetStat(City.StatType.Population));
        BalanceLabel.text = Suf(city.GetStat(City.StatType.Balance));
        PayoutLabel.text = $"{Suf(city.GetStat(City.StatType.Income))}/payout";
        progressFill.style.width = new Length(Mathf.Clamp(city.getPayoutProgress(), 0, 100), LengthUnit.Percent);

        // Update stat bars (values are 0-1, ProgressBar uses 0-100)
        jobsBar.value = (float)city.GetStat(City.StatType.Jobs) * 100;
        educationBar.value = (float)city.GetStat(City.StatType.Education) * 100;
        enjoymentBar.value = (float)city.GetStat(City.StatType.Enjoyment) * 100;
        safetyBar.value = (float)city.GetStat(City.StatType.Safety) * 100;

        RefreshCityCrest();
        if (greetingOverlay != null && !greetingOverlay.ClassListContains("hidden"))
            UpdateLevelBarFills();

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

    public void ShowEventPopup(string message, string title)
    {
        if (eventMessageLabel != null)
        {
            eventMessageLabel.text = message;
        }
        if (eventTitle != null)
        {
            eventTitle.text = title;
        }
        ShowOverlay(eventPopupOverlay);
        IsEventPopupActive = true;
    }

    private string Suf(float value) => SufRaw(value * 1000f);

    private string SufRaw(float value)
    {
        float abs = Math.Abs(value);
        if (abs < 1000) return Sig3(value);
        if (abs < 1000000f) return Sig3(value / 1000f) + "K";
        if (abs < 1000000000f) return Sig3(value / 1000000f) + "M";
        if (abs < 1000000000000f) return Sig3(value / 1000000000f) + "B";
        if (abs < 1000000000000000f) return Sig3(value / 1000000000000f) + "T";
        return Sig3(value / 1000000000000000f) + "Q";
    }

    private string Sig3(float v)
    {
        float abs = Math.Abs(v);
        if (abs >= 100f) return ((int)Math.Round(v)).ToString();
        if (abs >= 10f) return v.ToString("F1");
        return v.ToString("F2");
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
        var card = new Button { name = "BuildingCard" };

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

        var statsContainer = new VisualElement();
        statsContainer.AddToClassList("card-stats");
        AddStatRow(statsContainer, "Housing", calcStatfromValue(effects.housing));
        AddStatRow(statsContainer, "Jobs", calcStatfromValue(effects.jobs));
        AddStatRow(statsContainer, "Education", calcStatfromValue(effects.education));
        AddStatRow(statsContainer, "Enjoyment", calcStatfromValue(effects.enjoyment));
        AddStatRow(statsContainer, "Safety", calcStatfromValue(effects.safety));
        card.Add(statsContainer);


        card.RegisterCallback<PointerDownEvent>(evt => //pointerdown event is low level: used here for drag&drop buying
        {
            if (evt.button == 0)
            {
                BuyButtonPressed(buildingDefinition); 
                audioScript.PlaySound(audioScript.click);
            }
        }, TrickleDown.TrickleDown);

        //card.clicked += () => { old click check before drag drop
        //    buildingManager.dragDropMode=false;
        //    BuyButtonPressed(buildingDefinition);
        //};
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
            if(city.gameState.topBalance<def.effects.cost/4) {if(PlayerPrefs.GetInt("HideUnaffordable") == 1) {continue;} buildingList.Add(CreateMysteryCard(def)); continue;}
            if(city.gameState.balance<def.effects.cost) {if(PlayerPrefs.GetInt("HideUnaffordable") == 1) {continue;} buildingList.Add(CreateCantAffordCard(def)); continue;}
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


    public void ShowDetails(GameObject building)
    {
        audioScript.PlaySound(audioScript.info);
        focusedBuilding = building.GetComponent<Building>();
        string title = focusedBuilding != null && focusedBuilding.definition != null
            ? (string.IsNullOrEmpty(focusedBuilding.definition.displayName) ? focusedBuilding.definition.name : focusedBuilding.definition.displayName)
            : building.name;

        detailTitle.text = title;
        detailCard.RemoveFromClassList("hidden");
    }

    public void CloseDetails()
    {
        detailCard.AddToClassList("hidden");
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

    private City.Characters selectedCharacter;

    private void SelectCharacter(Button btn, City.Characters character, string quote)
    {
        selectedCharBtn?.RemoveFromClassList("character-card--selected");
        selectedCharBtn = btn;
        selectedCharacter = character;
        selectedCharBtn.AddToClassList("character-card--selected");
        if (charQuoteLabel != null) charQuoteLabel.text = quote;
        startGameBtn?.SetEnabled(true);
        RefreshLevelBar();
    }

    private void StartGame()
    {
        string name = cityNameField?.value;
        if (!string.IsNullOrWhiteSpace(name))
            city.renameCity(name);
        city.SetCharacter(selectedCharacter);
        buildingManager.PlaceTownHall((int)selectedCharacter);
        HideOverlay(greetingOverlay);
        if (city.isPaused()) city.playPauseGame();
        playBtn.text = "II";
    }

    private int GetCityLevel()
    {
        if (city == null) return 0;
        float pop = city.GetStat(City.StatType.Population);
        if (pop >= levelTwoPopGoal) return 2;
        if (pop >= levelOnePopGoal) return 1;
        return 0;
    }

    private Texture2D[] GetCharImages(City.Characters ch)
    {
        return ch switch
        {
            City.Characters.king     => kingLevelImages,
            City.Characters.lion     => lionLevelImages,
            City.Characters.engineer => softLevelImages,
            _                        => kingLevelImages,
        };
    }

    private void RefreshLevelBar()
    {
        var images = GetCharImages(selectedCharacter);
        int level = GetCityLevel();
        for (int i = 0; i < 3; i++)
        {
            if (levelSteps[i] == null) continue;
            if (images != null && i < images.Length && images[i] != null)
                levelSteps[i].style.backgroundImage = new StyleBackground(images[i]);
            if (i == level)
            {
                levelSteps[i].AddToClassList("level-step--active");
                levelSteps[i].RemoveFromClassList("level-step--inactive");
            }
            else
            {
                levelSteps[i].RemoveFromClassList("level-step--active");
                levelSteps[i].AddToClassList("level-step--inactive");
            }
        }
        UpdateLevelBarFills();
    }

    private void UpdateLevelBarFills()
    {
        float pop = city != null ? city.GetStat(City.StatType.Population) : 0;
        // Bar 0: level 0 → 1
        if (levelBarFills[0] != null)
        {
            float f = Mathf.Clamp01(pop / levelOnePopGoal);
            levelBarFills[0].style.width = new StyleLength(new Length(f * 100f, LengthUnit.Percent));
        }
        // Bar 1: level 1 → 2
        if (levelBarFills[1] != null)
        {
            float f = Mathf.Clamp01((pop - levelOnePopGoal) / (float)(levelTwoPopGoal - levelOnePopGoal));
            levelBarFills[1].style.width = new StyleLength(new Length(f * 100f, LengthUnit.Percent));
        }
    }

    private void RefreshCityCrest()
    {
        if (cityCrestBtn == null || city == null) return;
        var ch = city.GetCharacter();
        if ((int)ch == 0) return;
        int level = GetCityLevel();
        if (level == lastCrestLevel) return;
        lastCrestLevel = level;
        var images = GetCharImages(ch);
        if (images != null && level < images.Length && images[level] != null)
            cityCrestBtn.style.backgroundImage = new StyleBackground(images[level]);
    }

    private void BuyButtonPressed(BuildingDefinition buildingDefinition)
    {
        buildingManager.selectedBuilding = buildingDefinition;
    }

    void OnHideUnaffordableChanged(bool isOn) {PlayerPrefs.SetInt("HideUnaffordable", isOn ? 1 : 0); PlayerPrefs.Save();}
    void OnAudioToggleChanged(bool isOn) {PlayerPrefs.SetInt("Audio", isOn ? 1 : 0); PlayerPrefs.Save();}
}

