using System;
using UnityEngine;

[CreateAssetMenu]
public class VoiceLineScript : ScriptableObject
{
    [Serializable]
    public struct VoiceLine
    {
        public AudioClip clip;
        public string subtitle;
    }
    
    [SerializeField] private AudioClip[] voiceLines;
    [SerializeField] private string[] voiceLineScripts;
    
    public VoiceLine GetLine(VoiceLineId id) => new()
    {
        clip = voiceLines[(int) id],
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