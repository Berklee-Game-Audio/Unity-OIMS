using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(OIMSBank))]
[CanEditMultipleObjects]

public class OIMSBankEditor : Editor {

	public static bool overrideInspectorGUI = true;
	private GUIStyle guiStyle = new GUIStyle(); //create a new variable

	//private int currentCueNumber = -1;
 
	public override void OnInspectorGUI()
    {
		if(!overrideInspectorGUI){
			 base.OnInspectorGUI();
			 return;
		}

		guiStyle.fontSize = 12; //change the font size
		guiStyle.fontStyle = FontStyle.Bold;

		OIMSBank myTarget = (OIMSBank)target;

		EditorUtility.SetDirty(myTarget);

		drawBank(ref myTarget);

		return;
	}

	private void drawBank(ref OIMSBank myBank)
	{
		if(myBank.musicCues == null){
			//drawMusicCues(ref myBank.musicCues);
			return;	
		}

		//int i = 1;

		for (int i = 0; i < myBank.musicCues.Length; i++)
		{
    		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Cue " + i.ToString(), GUILayout.Width(60));
			myBank.musicCues[i] = (OIMSCue)EditorGUILayout.ObjectField ("", myBank.musicCues[i], typeof(OIMSCue), true, GUILayout.Width(250));
			
			if(GUILayout.Button("Play", GUILayout.Width(50))){
			//Debug.Log("Play Button pressed");
				if(EditorApplication.isPlaying){
					for (int j = 0; j < myBank.musicCues.Length; j++){
						if(i == j){
							myBank.musicCues[i].StartAudio();
						} else {
							myBank.musicCues[j].FadeOutAudio();

						}
						

					}

				} else {
					//EditorApplication.play
					EditorApplication.isPlaying = true;
					//myCue.StartAudio();
				}
			}

			if(GUILayout.Button("Stop", GUILayout.Width(50))){
				//Debug.Log("Play Button pressed");
					if(EditorApplication.isPlaying){
						myBank.musicCues[i].StopAudio();
					} else {
					//EditorApplication.play
					EditorApplication.isPlaying = true;
					//myCue.StartAudio();
				}
			}

			if(GUILayout.Button("Del", GUILayout.Width(75))){
				//System.Array.Resize(ref myPhrases, myPhrases.Length - 1); 
				System.Array.Resize(ref myBank.musicCues, myBank.musicCues.Length - 1);
			}

			EditorGUILayout.EndHorizontal();	
		}

		EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
		
		if(GUILayout.Button("Add Cue", GUILayout.Width(75))){
			System.Array.Resize(ref myBank.musicCues, myBank.musicCues.Length + 1);
		}
			
			
			
	}
		
		// foreach(OIMSCue item in myBank.musicCues)
		// {
		// 	EditorGUILayout.LabelField("Cue " + i.ToString(), GUILayout.Width(60));
		// 	item = (OIMSCue)EditorGUILayout.ObjectField ("", item, typeof(OIMSCue), true, GUILayout.Width(200));	
		// 	i++;
		// }


}



