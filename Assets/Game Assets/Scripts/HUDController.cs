using TMPro;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class HUDController : MonoBehaviour
{
    public TMP_Text rewindTimeText;
    public TMP_Text playTimeText;
    public RectTransform timeTextRect;
    public RectTransform playTimeRect;
    public RectTransform HUDRect;

    void Start()
    {
        GetAllGameObjectsReferences();
        PutTextsInAppriopriatePlaces();
    }

    // function that gets all of the required game objects
    private void GetAllGameObjectsReferences()
    {
        rewindTimeText = GameObject.Find("TimeLeft").GetComponent<TMP_Text>();
        timeTextRect = GameObject.Find("TimeLeft").GetComponent<RectTransform>();
        playTimeText = GameObject.Find("TimePlaying").GetComponent<TMP_Text>();
        playTimeRect = GameObject.Find("TimePlaying").GetComponent<RectTransform>();
        HUDRect = GameObject.Find("HUD").GetComponent<RectTransform>();
    }

    void Update()
    {
        float timeLeft = GameObject.Find("GameController").GetComponent<TimeController>().timeAvailable;
        rewindTimeText.fontSize = 30;
        rewindTimeText.SetText ($"Rewind Available: {FormatMinutesAndSeconds(timeLeft)}");
    }

    private void FixedUpdate()
    {
        float playTime = Time.timeSinceLevelLoad;
        playTimeText.SetText($"Playtime: {FormatMinutesAndSeconds(playTime)}");
    }

    // places the visible texts to appriopriate places on the screen
    private void PutTextsInAppriopriatePlaces()
    {
        timeTextRect.position = new Vector3(timeTextRect.rect.width/2, timeTextRect.rect.height/2, 0);
        playTimeRect.position = new Vector3(HUDRect.position.x, HUDRect.rect.height - playTimeRect.rect.height / 2, 0);
    }

    // formats minutes and seconds nicely
    private string FormatMinutesAndSeconds(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time - minutes * 60);
        string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);
        return niceTime;
    }
}
