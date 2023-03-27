using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class VoicePlayer : MonoBehaviour
{
    private static VoicePlayer inst;
    public static VoicePlayer instance
    {
#if UNITY_EDITOR
        get
        {
            if (inst) return inst;
            // statically load into inst
            GameObject obj = (GameObject) PrefabUtility.InstantiatePrefab(
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/MainAssets/Levels/Voice Lines/Voice Player.prefab"));
            inst = obj.GetComponent<VoicePlayer>();
            DontDestroyOnLoad(obj);
            return inst;
        }
#else
        get => inst;
#endif
    }
    
    private void Awake()
    {
        if (inst != null)
        {
            if(inst != this) Destroy(gameObject);
            return;
        }
        
        inst = this;
        DontDestroyOnLoad(gameObject);
    }
    
    [SerializeField] private VoiceLineScript voiceScript;
    [SerializeField] private AudioSource voicePlayer;
    
    private int cachedClip = -1;
    
    // Start is called before the first frame update
    private void Start()
    {
        if (cachedClip >= 0)
        {
            PlayVoiceLine((VoiceLineId) cachedClip);
        }
    }
    
    public void PlayVoiceLine(VoiceLineId id)
    {
        if (!voicePlayer)
            cachedClip = (int) id;
        else
        {
            voicePlayer.Stop();
            voicePlayer.clip = voiceScript.GetLine(id).clip;
            if(voicePlayer.clip)
                voicePlayer.PlayDelayed(0.75f); // delay to account for fade time, etc
        }
    }
}
