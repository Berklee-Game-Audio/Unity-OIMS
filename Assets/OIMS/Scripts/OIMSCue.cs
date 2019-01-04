using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[System.Serializable]
[CreateAssetMenu(fileName = "MusicCue_01", menuName = "Music/Cue", order = 2)]

public class OIMSCue: ScriptableObject { //this is similar to a playlist
	
	public string cueName = "New Cue";
	public bool immediateTransition = true;
	public float generalFadeInTime = 3.0f;
	public float generalFadeOutTime = 3.0f;
	public float transitionFadeInTime = 3.0f;
	public float transitionfadeOutTime = 3.0f;
	public OIMSPhraseGroup[] musicPhraseGroups;

	public OIMSEvent[] currentEventList;
	public float generatePlaylistCurrentTime;

	public int whichPhraseGroup = 0;

	public bool setupDone = true;
	//public T musicComponents;
	//public List<Generic T> myTestList;
	//public ArrayList myArrayList = new ArrayList();

	public OIMSCue(){
		//while we're creating the cue - turn off the custom inspector GUI
		

		//Debug.Log("init oimscue");
		//when we create a new cue, make sure to have one phrase group, one phrase, and one layer if there isn't one already
		//musicPhraseGroups.create();
		if(musicPhraseGroups == null){
			musicPhraseGroups = new OIMSPhraseGroup[1];
		}

		//setupDone = true;
	}

	public void OnAwake()
	{
		Debug.Log("Cue is awake.");
		setupDone = true;
	}
	
	public void StartAudio(float timeToStart = default(float))
	{
		
		whichPhraseGroup = 0;
		musicPhraseGroups[whichPhraseGroup].StartAudio(this, 0f);
		
		
		// foreach(OIMSPhraseGroup musicPhraseGroup in musicPhraseGroups)
		// {
		// 	musicPhraseGroup[0].StartAudio(this);

		// }		

	}


	public void StopAudio(float timeToStop = default(float))
	{
		foreach(OIMSPhraseGroup musicPhraseGroup in musicPhraseGroups)
		{
			musicPhraseGroup.StopAudio();

		}
				

	}

	public void FadeOutAudio()
	{
		foreach(OIMSPhraseGroup musicPhraseGroup in musicPhraseGroups)
		{
			musicPhraseGroup.FadeOutAudio();

		}
	}

	public void StartAndFadeInAudio()
	{
		foreach(OIMSPhraseGroup musicPhraseGroup in musicPhraseGroups)
		{
			musicPhraseGroups[whichPhraseGroup].StartAndFadeInAudio();

		}
	}

	public void DoNext(float when)
	{
		//Debug.Log("Cue DoNext: " + when);
		whichPhraseGroup++;
		if(whichPhraseGroup < musicPhraseGroups.Length){
			//Debug.Log("musicPhrases.Length = " + musicPhrases.Length);
			//Debug.Log("whichPhrase = " + whichPhrase);
			musicPhraseGroups[whichPhraseGroup].StartAudio(this, when);
		} else {
			//parentCue.DoNext(when);
			Debug.Log("DoNext Cue - > nothing else to do");
		}
		
	}

	public void SkipToNextGroup(){
		musicPhraseGroups[whichPhraseGroup].SkipToNextGroup();
	}


	public void GenerateEventList()
	{
		//first clear the list
		//currentEventList = clear
		
		generatePlaylistCurrentTime = 0.0f;
		ExaminePhraseGroups();
		
	}


	private void ExaminePhraseGroups()
	{
		foreach(OIMSPhraseGroup musicPhraseGroup in musicPhraseGroups)
		{
			ExaminePhrases(musicPhraseGroup);			
		}



	}

	private void ExaminePhrases(OIMSPhraseGroup musicPhraseGroup)
	{
		if(musicPhraseGroup.randomize)
		{
			for(int x = 0; x <= musicPhraseGroup.numberOfRepeats; x++)
			{
				//pick a random phrase




				//then loop the phrase if needed




			}


		} else {
			foreach(OIMSPhrase musicPhrase in musicPhraseGroup.musicPhrases)
			{
				//phrases can loop
			
				if(musicPhrase.loopHowManyTimes > 1)
				{
					for(int x = 1; x <= musicPhrase.loopHowManyTimes; x++)
					{
						ExamineLayers(musicPhrase);
					}
				} else {
					ExamineLayers(musicPhrase);
				}

			}

		}

	}

	private void ExamineLayers(OIMSPhrase musicPhrase)
	{
		//first get the longest pickup time
		float longestPickupTime = 0.0f;
		foreach(OIMSLayer musicLayer in musicPhrase.musicLayers)
		{
			if(musicLayer.pickUpTime > longestPickupTime){
				longestPickupTime = musicLayer.pickUpTime; 
			}
		}
		
		
		//if the layers are synchronized
		if(musicPhrase.synchronizedLayerLoops)
		{
			float longestLoopDuration = 0.0f;
			foreach(OIMSLayer musicLayer in musicPhrase.musicLayers)
			{
				if((musicLayer.theAudioClip.length - musicLayer.pickUpTime - musicLayer.tailTime) > longestLoopDuration){
					longestLoopDuration = musicLayer.theAudioClip.length - musicLayer.pickUpTime - musicLayer.tailTime; 
				}
			}



		} else {
			foreach(OIMSLayer musicLayer in musicPhrase.musicLayers)
			{
					OIMSEvent tempEvent = new OIMSEvent();
					//tempEvent.startTime = generatePlaylistCurrentTime + 



			}





		}




	}

	public void ReorganizeEventList()
	{






	}

	

}
