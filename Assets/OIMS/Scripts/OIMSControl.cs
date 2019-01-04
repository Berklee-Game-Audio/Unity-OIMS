using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OIMSControl : MonoBehaviour 
{

	public OIMSBank currentBank;

	//private bool isPlaying = false;
	//private int currentCue = -1;
	public bool screenGui = true;
	//private int selGridInt = 0;
	
	private ArrayList cueNames = new ArrayList();

	private GUIStyle hugeGuiStyle = new GUIStyle(); //create a new variable
	
	// Use this for initialization
	void Start () 
	{
		DontDestroyOnLoad(this.transform.gameObject);
		// foreach(OIMSCue item in currentBank ){
		// 	cueNames.Add(item.cueName);
		// }

		hugeGuiStyle.fontSize = 36; //change the font size
		//hugeGuiStyle.fontStyle = FontStyle.Bold;

		for(int i = 0; i < currentBank.musicCues.Length; i++ ){
			if(currentBank.musicCues[i].cueName != "New Cue"){
				cueNames.Add(currentBank.musicCues[i].cueName);
			} else {
				cueNames.Add(currentBank.musicCues[i].name);
			}
			
			
		}

		//OIMSWaveParser myWaveParser = new OIMSWaveParser();
		//float startTime;
		//float endTime;

		//myWaveParser.LoadWaveData(out startTime, out endTime);
		
		//Debug.Log("the startTime is: " + startTime + "     the endTime is: " + endTime);

	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{

		
			
	}

	void OnGUI() 
	{
        if(!screenGui){
			return;
		}

		GUILayout.Label("OIMS v0.5", hugeGuiStyle);

		// if (GUI.Button(new Rect(10, 10, 150, 100), "Play First Cue")){
		// 	currentBank.musicCues[0].StartAudio();

		// }

		if(currentBank.musicCues == null){
			//drawMusicCues(ref myBank.musicCues);
			return;	
		}

		//int i = 1;

		for (int i = 0; i < currentBank.musicCues.Length; i++)
		{
    		GUILayout.BeginHorizontal();
			GUILayout.Label("Cue: " + currentBank.musicCues[i].cueName, GUILayout.Width(150));
			//currentBank.musicCues[i] = (OIMSCue)GUILayout.ObjectField ("", currentBank.musicCues[i], typeof(OIMSCue), true, GUILayout.Width(250));
			
			if(GUILayout.Button("Play", GUILayout.Width(50))){
			//Debug.Log("Play Button pressed");
				
				for (int j = 0; j < currentBank.musicCues.Length; j++){
					if(i == j){
						currentBank.musicCues[i].StartAudio();

					} else {
						currentBank.musicCues[j].FadeOutAudio();
					}
						

				}

			}

			if(GUILayout.Button("Stop", GUILayout.Width(50))){
				//Debug.Log("Play Button pressed");
				currentBank.musicCues[i].StopAudio();
			}

			GUILayout.EndHorizontal();	
		}

		//GUILayout.LabelField("", GUI.skin.horizontalSlider);
		

    }

	void StartMusic(int whichCue = 0){
		
		
	}
	
	
	

}
