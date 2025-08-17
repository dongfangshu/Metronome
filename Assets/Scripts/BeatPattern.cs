[System.Serializable]
public struct BeatPattern
{
    public int beatsPerMeasure;   // 小节的拍数（如4/4中的4）
    public int beatNote;          // 每拍的音符（如4/4中的4）
    public string name;           // 节拍名字（如"4/4"）

    public BeatPattern(int beatsPerMeasure, int beatNote, string name)
    {
        this.beatsPerMeasure = beatsPerMeasure;
        this.beatNote = beatNote;
        this.name = name;
    }
}