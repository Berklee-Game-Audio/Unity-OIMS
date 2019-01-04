using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "MusicBank_Level_1", menuName = "Music/Bank", order = 1)]
//[System.Serializable]
public class OIMSBank : ScriptableObject {
    public string bankName = "New Bank";

	public float genericSourceFadeOutTime = 3.0f;
	public float genericDestinationFadeInTime = 2.0f;
	public AudioClip genericTransition;
	
    public OIMSCue[] musicCues;
    public OIMSTransition[] musicTransisitions;
    //public TransitionMa
    
    void Awake()
    {
    	Debug.Log("Music System Started");
    
    
    }
    
}
