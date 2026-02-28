using UnityEngine;
using UnityEngine.UIElements;

public enum AnnounceColor
{
    White,
    Green,
    Yellow,
    Red,
    Blue
}

public class AnnouncementManager : MonoBehaviour
{
    public static AnnouncementManager instance; // static instance var for global reference

    [SerializeField] float announceDuration = 5.0f; // in seconds
    [SerializeField] AudioScript audioScript;
    [SerializeField] AudioSource uiSounds;

    private Label infoLabel; // actual text
    private VisualElement infoPill; // container (change color etc)
    private float timer;
    private bool announcing;
    private string colorClass;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        var uiDoc = GetComponent<UIDocument>();
        var root = uiDoc.rootVisualElement;
        infoLabel = root.Q<Label>("InfoLabel");
        infoPill = infoLabel.parent; // InfoLabel -> pill

        ShowRandomMessage();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0.0f)
        {
            if (announcing)
                StopAnnounce();
            else
                ShowRandomMessage();
        }
    }

    private void ShowRandomMessage()
    {
        infoLabel.text = AnnouncementMessages.GetRandom();
        timer = announceDuration;
        announcing = false;
    }

    private void StopAnnounce()
    {
        if (colorClass != null)
        {
            infoPill.RemoveFromClassList(colorClass);
            colorClass = null;
        }
        ShowRandomMessage();
    }

    public void msgAnnounce(AnnounceColor color, string text)
    {
        announcing = true;
        timer = announceDuration;

        colorClass = "pill-announce-" + color.ToString().ToLower(); 
        infoPill.AddToClassList(colorClass);

        infoLabel.text = text;
        switch (color) {
            case AnnounceColor.White: {uiSounds.PlayOneShot(audioScript.info); break;};
            case AnnounceColor.Yellow: {uiSounds.PlayOneShot(audioScript.info); break;};
            case AnnounceColor.Red: {uiSounds.PlayOneShot(audioScript.error); break;};
            default: {uiSounds.PlayOneShot(audioScript.info); break;};
        };

    }
}
