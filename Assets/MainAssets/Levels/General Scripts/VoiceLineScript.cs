using System;
using UnityEngine;

[CreateAssetMenu]
public class VoiceLineScript : ScriptableObject
{
    [Serializable]
    public struct VoiceLine
    {
        public AudioClip clip;
        public float volume;
        public string subtitle;
    }
    
    [SerializeField] public float wizardVolume = 1;
    [SerializeField] public float demonVolume = 1;
    [SerializeField] private AudioClip[] voiceLines;
    [SerializeField] private string[] voiceLineScripts;
    
    public VoiceLine GetLine(VoiceLineId id) => new()
    {
        clip = voiceLines[(int) id],
        volume = voiceLines[(int) id]?.name.Contains("Wizard") ?? true ? wizardVolume : demonVolume,
        subtitle = voiceLineScripts[(int) id]
    };
}

public enum VoiceLineId
{
    PotionIntroL = 0, PotionIntroP,
    PotionFailure1A, PotionFailure2A,
    PotionCompleteL, PotionCompleteP,
    
    MazeFirstL, MazeFirstP,
    MazeIntroP, MazeIntroL = 16,
    MazeFailure1P = 9, MazeFailure2P, MazeFailure3A,
    MazeCompleteA,
    
    MazeEnemyCloseP, MazeLastLeverP, MazeLibLeverL = 15
}