using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Metronome : MonoBehaviour
{
    public TextMeshProUGUI bpmText;
    public Button increaseButton, decreaseButton, startStopButton;
    public TextMeshProUGUI playText;
    public TMP_Dropdown measureDropdown;
    public Slider bpmSlider;
    public AudioSource clickAudio;           // 普通拍声音
    public AudioSource accentAudio;          // 强拍声音
    public GameObject dotPrefab;
    public Transform dotsParent;
    private int bpm = 120;
    private bool isRunning = false;
    private float interval = 0f; // 每拍间隔时间
    // 节拍类型
    private int beatsPerMeasure = 4; // 如4/4中的4
    private int beatNote = 4;        // 如4/4中的4
    private int currentBeat = 1;
    private float timer = 0f; // 计时器
    private List<Image> beatDots = new();
    private List<BeatPattern> beatPatterns = new List<BeatPattern>
    {
        new BeatPattern(4, 4, "4/4"),
        new BeatPattern(3, 4, "3/4"),
        new BeatPattern(2, 4, "2/4"),
        new BeatPattern(3, 8, "3/8"),
        new BeatPattern(6, 8, "6/8"),
        new BeatPattern(7, 8, "7/8"),
    };
    const int MinBPM = 40; // 最小BPM
    const int MaxBPM = 200; // 最大BPM

    private const string BMP_PREF_KEY = "bmp";
    private const string BEAT_PREF_KEY = "beat";
    void Start()
    {
        Application.runInBackground = true;
        bpmText.text = bpm.ToString();
        increaseButton.onClick.AddListener(() => ChangeBPM(1));
        decreaseButton.onClick.AddListener(() => ChangeBPM(-1));
        startStopButton.onClick.AddListener(ToggleMetronome);

        // 配置下拉框
        measureDropdown.ClearOptions();
        measureDropdown.AddOptions(beatPatterns.Select(x=>x.name).ToList());
        measureDropdown.onValueChanged.AddListener(OnMeasureChanged);
        bpmSlider.minValue = MinBPM;
        bpmSlider.maxValue = MaxBPM;
        bpmSlider.value = bpm;
        bpmSlider.onValueChanged.AddListener(OnSliderChanged);
        var beatIndex = PlayerPrefs.GetInt(BEAT_PREF_KEY,0);
        measureDropdown.value = beatIndex;
        if (beatIndex  == 0)
        {
            OnMeasureChanged(beatIndex);
        }
        bpm = PlayerPrefs.GetInt(BMP_PREF_KEY,120);
        bpmText.text = bpm.ToString();
        bpmSlider.value = bpm;
    }
    void OnSliderChanged(float value)
    {
        bpm = Mathf.RoundToInt(value);
        bpmText.text = bpm.ToString();
        UpdateInterval();
        ResetMetronome();
        PlayerPrefs.SetInt(BMP_PREF_KEY, bpm);
        PlayerPrefs.Save();
    }
    void Update()
    {
        if (!isRunning)
        {
            return;
        }
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer -= interval;
            UpdateBeatDotsVisual();
            PlayClick();
            currentBeat++;
            if (currentBeat > beatsPerMeasure)
                currentBeat = 1;
        }
    }

    void PlayClick()
    {
        if (currentBeat == 1 && accentAudio != null)
            accentAudio.Play();
        else if (clickAudio != null)
            clickAudio.Play();
    }

    void ChangeBPM(int delta)
    {
        bpm = Mathf.Clamp(bpm + delta, MinBPM, MaxBPM);
        bpmSlider.value = bpm; // 更新滑动条
    }

    void ToggleMetronome()
    {
        isRunning = !isRunning;
        playText.text = isRunning ? "结束" : "开始";
        ResetMetronome();
        if (!isRunning)
        {
            ClearDots();
        }
    }

    void ResetMetronome() {
        timer = interval;
        currentBeat = 1; // 重置当前拍子
    }

    void OnMeasureChanged(int idx)
    {
        idx = Mathf.Clamp(idx,0, beatPatterns.Count - 1);
        var beat = beatPatterns[idx];
        beatsPerMeasure = beat.beatsPerMeasure;
        beatNote = beat.beatNote;
        currentBeat = 1; // 重置当前拍子
        CreateDots();
        UpdateInterval();
        ResetMetronome();
        PlayerPrefs.SetInt(BEAT_PREF_KEY,idx);
        PlayerPrefs.Save();
    }

    void UpdateInterval() {
        interval = 60f / bpm;
        //输出bmp 和节拍信息
        //Debug.Log($"BPM: {bpm}, Beats Per Measure: {beatsPerMeasure}, Beat Note: {beatNote}, Interval: {interval} seconds");
    }
    void ClearDots()
    {
        for (int i = 0; i < beatDots.Count; ++i)
        {
            beatDots[i].color = Color.white;
            beatDots[i].transform.localScale = Vector3.one;
        }
    }
    void CreateDots() {
        foreach (Transform child in dotsParent)
            Destroy(child.gameObject);
        beatDots.Clear();

        for (int i = 0; i < beatsPerMeasure; ++i)
        {
            var dot = Instantiate(dotPrefab, dotsParent).GetComponent<Image>();
            dot.gameObject.SetActive(true);
            beatDots.Add(dot);
        }
    }
    // 高亮当前拍的小圆点
    void UpdateBeatDotsVisual()
    {
        for (int i = 0; i < beatDots.Count; ++i)
        {
            if (i >= this.beatsPerMeasure)
            {
                break;
            }
            if (i == currentBeat - 1)
            {
                // 当前拍高亮
                beatDots[i].color = Color.blue;
                beatDots[i].transform.localScale = Vector3.one * 1.3f;
            }
            else
            {
                beatDots[i].color = Color.white;
                beatDots[i].transform.localScale = Vector3.one;
            }
        }
    }
}