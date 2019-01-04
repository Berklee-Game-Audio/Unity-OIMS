using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OIMSPhrase {
	
	public enum PhraseTypeDefinition {introduction, ending, part_a, part_b, part_c};
	public PhraseTypeDefinition phraseType;
	public string phraseName = "New Phrase";
	public bool synchronizedLayerLoops = false;
	public int loopHowManyTimes = 0;
	private int currentLoopNumber = 1;
	public OIMSLayer[] musicLayers;
	public float longestPickupTime = 0.0f;
	private bool skipToNextGroupFlag = false;

	private OIMSPhraseGroup parentPhraseGroup;
	
	//maybe create an enum defining the type
	// introduction, ending, part a, part b, part c, part of a group
	
	public OIMSPhrase(){
		//Debug.Log("init oimscue");
		//when we create a new cue, make sure to have one phrase group, one phrase, and one layer if there isn't one already
		//musicPhraseGroups.create();
		if(musicLayers == null){
			musicLayers = new OIMSLayer[1];
		}


	}

	public void CreatePhrase()
	{
		longestPickupTime = 0.0f;
		foreach(OIMSLayer selectedLayer in musicLayers){
			//first gather the longest pickupTime
			if(selectedLayer.pickUpTime > longestPickupTime){
				longestPickupTime = selectedLayer.pickUpTime;
			}

			//then create the layers
			selectedLayer.CreateLayer();
		}
	
	}
	
	public void StartAudio(OIMSPhraseGroup whichPhraseGroup = default(OIMSPhraseGroup), float when = 0.0f, float currentVolume = 1.0f)
	{
		
		parentPhraseGroup = whichPhraseGroup;
		currentLoopNumber = 0;
		skipToNextGroupFlag = false;


		//if the layers are synchronized then they have a single start and end point based on the longest single loop
		//if NOT - then the layers are independent with multiple loop lengths, with overlaps dependent on their pickup notes, and reverb tails

		float longestStartOffset = 0.0f;

		//first we need to figure out the longest pickup time
		foreach(OIMSLayer musicLayer in musicLayers){
			if(musicLayer.pickUpTime > longestStartOffset){
				longestStartOffset = musicLayer.pickUpTime;
			}

		}
		
		foreach(OIMSLayer musicLayer in musicLayers){
			//float timeToStart = longestPickupTime + Time.time - musicLayer.pickUpTime;
						
			musicLayer.StartAudio(this, when, currentVolume);

		}
	}

	public void StartAndFadeInAudio(OIMSPhraseGroup whichPhraseGroup = default(OIMSPhraseGroup), float when = 0.0f)
	{
		
		parentPhraseGroup = whichPhraseGroup;
		currentLoopNumber = 0;

		foreach(OIMSLayer musicLayer in musicLayers){
			//float timeToStart = longestPickupTime + Time.time - musicLayer.pickUpTime;
			musicLayer.StartAndFadeInAudio(this, when);
		}
	}

	public void StopAudio()
	{
		foreach(OIMSLayer musicLayer in musicLayers){
			musicLayer.StopAudio();
		}
	}

	public void FadeOutAudio()
	{
		foreach(OIMSLayer musicLayer in musicLayers){
			musicLayer.FadeOutAudio();
		}
	}

	public void DoNext(float when, float currentVolume = 1.0f)
	{
		Debug.Log("Phrase DoNext - skipToNextGroupFlag = " + skipToNextGroupFlag);
		if(skipToNextGroupFlag){
			parentPhraseGroup.DoNext(when, currentVolume);
			return;
		}

		//first check to see whether the layer has stuff to do...
		currentLoopNumber++;
		if(currentLoopNumber < loopHowManyTimes){
			foreach(OIMSLayer musicLayer in musicLayers){
				//float timeToStart = longestPickupTime + Time.time - musicLayer.pickUpTime;
				musicLayer.StartAudio(this, when);
			}
		} else {
			parentPhraseGroup.DoNext(when, currentVolume);
		}


	}

	public void SkipToNextGroup(){
		//musicPhrases[whichPhrase].SkipToNextGroup();
		Debug.Log("skipToNextGroupFlag = true");
		skipToNextGroupFlag = true;
	}


}
