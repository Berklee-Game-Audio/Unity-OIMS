using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(OIMSCue))]
[CanEditMultipleObjects]
public class OIMSCueEditor : Editor {

	private static bool overrideInspectorGUI = true;
	private GUIStyle guiStyle = new GUIStyle(); //create a new variable
 
	public override void OnInspectorGUI()
	//public void temp()
    {
		if(!overrideInspectorGUI){
			 base.OnInspectorGUI();
			 return;
		}
		
		
		guiStyle.fontSize = 12; //change the font size
		guiStyle.fontStyle = FontStyle.Bold;

		OIMSCue myTarget = (OIMSCue)target;

		EditorUtility.SetDirty(myTarget);

		//if the setup of the cue is not done - return
		if(!myTarget.setupDone){
			return;
		}

		//myTarget.cueName = EditorGUILayout.TextField("Cue Name", myTarget.cueName);

		drawCue(ref myTarget);

		return;


    }

	private void drawCue(ref OIMSCue myCue)
	{
		//public string cueName = "New Cue";
        
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Play", GUILayout.Width(50))){
			//Debug.Log("Play Button pressed");
			if(EditorApplication.isPlaying){
				myCue.StartAudio();
			} else {
				//EditorApplication.play
				EditorApplication.isPlaying = true;
				//myCue.StartAudio();
			}
		}

		if(GUILayout.Button("Stop", GUILayout.Width(50))){
			//Debug.Log("Play Button pressed");
			myCue.StopAudio();
		}

		if(GUILayout.Button("Start and Fade In", GUILayout.Width(100))){
			//Debug.Log("Play Button pressed");
			myCue.StartAndFadeInAudio();
		}
		
		if(GUILayout.Button("Fade Out", GUILayout.Width(80))){
			//Debug.Log("Play Button pressed");
			myCue.FadeOutAudio();
		}

		if(myCue.musicPhraseGroups.Length > 1){
			if(GUILayout.Button("Skip To Next Group", GUILayout.Width(120))){
				//Debug.Log("Play Button pressed");
				myCue.SkipToNextGroup();
			}
		}

		EditorGUILayout.EndHorizontal();

		myCue.cueName = EditorGUILayout.TextField("Cue Name", myCue.cueName);
		

		EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

		if(myCue.musicPhraseGroups != null){
			drawPhraseGroups(ref myCue.musicPhraseGroups);
			//return;	
		}

		
		
	}

	private void drawPhraseGroups(ref OIMSPhraseGroup[] myPhraseGroups)
	{
		if(myPhraseGroups == null){
			return;
		}
		//otherwise there are phrase groups
		//so lets draw them

		foreach(OIMSPhraseGroup item in myPhraseGroups)
		{
			// if(item.randomize == null){
			// 	return;
			// }
			
			
			EditorGUILayout.BeginHorizontal();

			if(myPhraseGroups[0].musicPhrases[0].musicLayers[0].theAudioClip != null){
				EditorGUILayout.LabelField("GROUP", guiStyle, GUILayout.Width(120));
				EditorGUILayout.LabelField("Randomize Phrases", GUILayout.Width(110));
				item.randomize = EditorGUILayout.Toggle ("", item.randomize, GUILayout.Width(20) );
				if(item.randomize){
					EditorGUILayout.LabelField("Repeat How Many Times: ", GUILayout.Width(130));
					item.numberOfRepeats = EditorGUILayout.IntField (item.numberOfRepeats, GUILayout.Width(40));
				}
				
			//GUILayout.Height(130)

			}
			
			

			EditorGUILayout.EndHorizontal();
			drawMusicPhrase(ref item.musicPhrases);
		}

		if(myPhraseGroups[0].musicPhrases[0].musicLayers[0].theAudioClip != null){
			if(GUILayout.Button("Add Group", GUILayout.Width(115))){
    			//myScript.BuildObject();
				Debug.Log("Add Phrase Group Button pressed");
				System.Array.Resize(ref myPhraseGroups, myPhraseGroups.Length + 1);    //resize array
        	}

		}
		
		
		


		return;

	}

	private void drawMusicPhrase(ref OIMSPhrase[] myPhrases)
	{
		
		if(myPhrases == null){
			return;
		}

		int i = 1;

		foreach(OIMSPhrase item in myPhrases)
		{	
			//first draw the phrase name
			EditorGUILayout.BeginHorizontal();
			//then a delete button ONLY if there is more than one phrase
			if(myPhrases.Length > 1){
				if(GUILayout.Button("Del", GUILayout.Width(30))){
					//Debug.Log("Delete Button pressed");
					System.Array.Resize(ref myPhrases, myPhrases.Length - 1);    //resize array
				}
			}
			
			if(myPhrases[0].musicLayers[0].theAudioClip != null){
				EditorGUILayout.LabelField("Phrase: " + i.ToString(), GUILayout.Width(60));

				EditorGUILayout.LabelField("Loops: ", GUILayout.Width(40));
				item.loopHowManyTimes = EditorGUILayout.IntField (item.loopHowManyTimes, GUILayout.Width(40));	
			}
			
			EditorGUILayout.EndHorizontal();

			//then draw the layers
			drawMusicLayers(ref item.musicLayers);
			i++;
		}

		if(myPhrases[0].musicLayers[0].theAudioClip != null){
			if(GUILayout.Button("Add Phrase", GUILayout.Width(115))){
    			//myScript.BuildObject();
				Debug.Log("Add Phrase Button pressed");
				System.Array.Resize(ref myPhrases, myPhrases.Length + 1);    //resize array
        	}

		}

		EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

		return;

	}

	private void drawMusicLayers(ref OIMSLayer[] myLayers)
	{
		if(myLayers == null){
			return;
		}

		//EditorGUILayout.Foldout(true, "Music Layers");

		int i = 1;

		foreach(OIMSLayer item in myLayers)
		{
			
			//start horizontal
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("    ", GUILayout.Width(30));
			EditorGUILayout.LabelField("Layer " + i.ToString(), GUILayout.Width(60));

			item.theAudioClip = (AudioClip)EditorGUILayout.ObjectField ("", item.theAudioClip, typeof(AudioClip), true, GUILayout.Width(200));	
			if(item.theAudioClip != item.tempAudioClip){
				Debug.Log("The audioclip has changed");
				OIMSWaveParser myWaveParser = new OIMSWaveParser();

				Debug.Log(Application.dataPath + AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(item.theAudioClip.name)[0]));

				float startTime = -1.0f;
				float endTime = -1.0f;

				string myPath = Path.Combine(Application.dataPath.Remove(Application.dataPath.Length - 6), AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(item.theAudioClip.name)[0]));

				myWaveParser.LoadWaveData(myPath, out startTime, out endTime);
		
				Debug.Log("The startTime is: " + startTime + "     the endTime is: " + endTime);
				item.pickUpTime = startTime / 1000.0f;
				item.tailTime = endTime / 1000.0f;
		
				item.tempAudioClip = item.theAudioClip;

			}

			
			//if there is more than one phrase, then we can display the delete button
			// if(myLayers.Length > 1){
			// 	if(GUILayout.Button("Del", GUILayout.Width(50))){
			// 		//Debug.Log("Delete Button pressed");
			// 		System.Array.Resize(ref myLayers, myLayers.Length - 1);    //resize array
			// 	}
			// }
			
			i++;

			if(i == myLayers.Length + 1){
				
				if(myLayers.Length > 1){
					if(GUILayout.Button("Del", GUILayout.Width(50))){
						//Debug.Log("Delete Button pressed");
						System.Array.Resize(ref myLayers, myLayers.Length - 1);    //resize array
					}
				}
				
				if(myLayers[0].theAudioClip != null){
					if(GUILayout.Button("Add Layer", GUILayout.Width(70))){
    					//myScript.BuildObject();
						Debug.Log("Add Layer Button pressed");
						System.Array.Resize(ref myLayers, myLayers.Length + 1);    //resize array
        			}

					//GUILayout.Label(myLayers[i].pickUpTime.ToString());
					//GUILayout.Label(myLayers[i].tailTime.ToString());


				}

				
			}
			
			EditorGUILayout.EndHorizontal();

			

			
			//EditorGUILayout.Separator("", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});


		}

		//if(item.theAudioClip != null){
			// EditorGUILayout.BeginHorizontal();
			// EditorGUILayout.LabelField("     ", GUILayout.Width(200));
			// if(GUILayout.Button("Add Layer", GUILayout.Width(100))){
    		// 	//myScript.BuildObject();
			// 	Debug.Log("Add Layer Button pressed");
			// 	System.Array.Resize(ref myLayers, myLayers.Length + 1);    //resize array
        	// }
			// EditorGUILayout.EndHorizontal();
		//}


	}





}
