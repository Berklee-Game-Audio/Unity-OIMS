using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OIMSPhraseGroup {
	
	public string phraseGroup = "New Phrase";
	//private enum PlaybackModel {Sequential, Random}
	//public PlaybackModel playbackModel = 0;
	public OIMSPhrase[] musicPhrases;
	public int whichPhrase = 0;

	private OIMSCue parentCue;
	
	public bool randomize = false;
	public int numberOfRepeats = 1;
	public int repeatCount = 0;
	private int lastPhrasePlayed = -1;
	private bool skipToNextGroupFlag = false;

	public OIMSPhraseGroup(){
		//Debug.Log("init oimscue");
		//when we create a new cue, make sure to have one phrase group, one phrase, and one layer if there isn't one already
		//musicPhraseGroups.create();
		if(musicPhrases == null){
			musicPhrases = new OIMSPhrase[1];
		}


	}
	
	public void CreatePhraseGroup()
	{

	
	}
	
	public void StartAudio(OIMSCue whichCue = default(OIMSCue), float when = 0f)
	{
		//foreach(OIMSPhrase musicPhrase in musicPhrases)  //not for each one - just the first, unless it's random
		//{
		//	musicPhrase.StartAudio();
		//}	

		//linear version first

		repeatCount = 0;
		skipToNextGroupFlag = false;

		if(randomize){
			whichPhrase = Random.Range(0, musicPhrases.Length);
			lastPhrasePlayed = whichPhrase;
			repeatCount++;
		} else {
			whichPhrase = 0;
		}
		
		musicPhrases[whichPhrase].StartAudio(this, when);
		parentCue = whichCue;

	}

	public void StartAndFadeInAudio(OIMSCue whichCue = default(OIMSCue))
	{
		//foreach(OIMSPhrase musicPhrase in musicPhrases)  //not for each one - just the first, unless it's random
		//{
		//	musicPhrase.StartAudio();
		//}	

		//linear version first
		whichPhrase = 0;
		musicPhrases[whichPhrase].StartAndFadeInAudio(this, 0.0f);
		parentCue = whichCue;

	}

	public void StopAudio()
	{
		foreach(OIMSPhrase musicPhrase in musicPhrases)
		{
			musicPhrase.StopAudio();
		}
	
	}

	public void FadeOutAudio()
	{
		foreach(OIMSPhrase musicPhrase in musicPhrases)
		{
			musicPhrase.FadeOutAudio();
		}
	
	}

	public void DoNext(float when, float currentVolume = 1.0f)
	{
		Debug.Log("PhraseGroup DoNext: " + when + "  numberOfRepeats: " + numberOfRepeats + "  currentVolume:" + currentVolume );
		if(skipToNextGroupFlag){
			parentCue.DoNext(when);
			return;
		}

		if(repeatCount >= numberOfRepeats){
			parentCue.DoNext(when);
			return;
		}
		
		if(randomize && (musicPhrases.Length > 1) ){
			whichPhrase = Random.Range(0, musicPhrases.Length);
			while(whichPhrase == lastPhrasePlayed){
				whichPhrase = Random.Range(0, musicPhrases.Length);
			}
			repeatCount++;
		} else {
			whichPhrase++;
		}

		if(whichPhrase < musicPhrases.Length){
			//Debug.Log("musicPhrases.Length = " + musicPhrases.Length);
			//Debug.Log("whichPhrase = " + whichPhrase);
			musicPhrases[whichPhrase].StartAudio(this, when, currentVolume);
			lastPhrasePlayed = whichPhrase;
		} else {
			parentCue.DoNext(when);
		}
		
	}

	public void SkipToNextGroup(){
		skipToNextGroupFlag = true;
		musicPhrases[whichPhrase].SkipToNextGroup();
	}

}
