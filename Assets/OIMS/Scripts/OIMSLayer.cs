using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class OIMSLayer {
	
	public string layerName = "New Layer";
	public AudioClip theAudioClip;
	public AudioClip tempAudioClip;
	public float pickUpTime = 0.0f;
	public float tailTime = 0.0f;
	public float volumeModifier = 1.0f;

	
	//we need multiple game objects because layers can overlap with themselves
	//oldest ones get destroyed first
	private GameObject theGameObject;
	public AudioSource theAudioSource;	
	private GameObject altGameObject;
	public AudioSource altAudioSource;	
	//private float nextStartTime = 0.0f;

	private bool alternateSource = false;

	private OIMSAudioSourceMonitor theAudioSourceMonitor;
	private OIMSAudioSourceMonitor altAudioSourceMonitor;

	private OIMSPhrase parentPhrase;

	public bool fadingIn = false;
	public bool fadingOut = false;


	public void GetInfo(){
		//Debug.Log("The AssetPath is: " + AssetDatabase.GetAssetPath(theAudioSource));
		Debug.Log(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(theAudioClip.name)[0]));
		//theAudioClip.object.GUIDToAssetPath

	}

	public void CreateLayer(float whenToPlay = 0.0f, float currentVolume = 1.0f)
	{
		Debug.Log("CreateLayer volume = " + currentVolume);

		if(theAudioClip == null){
			return;
		}

		//GetInfo();

		if(!alternateSource){
			//Debug.Log("CreateLayer - ");
			theGameObject = new GameObject("OIMSLayer - " + theAudioClip.name + " " + Time.realtimeSinceStartup);
			theAudioSource = theGameObject.AddComponent<AudioSource>();
			theAudioSource.clip = theAudioClip;
			theAudioSource.loop = false;
			theAudioSource.volume = currentVolume * volumeModifier;

			//OIMSAudioSourceMonitor myAudioSourceMonitor = new OIMSAudioSourceMonitor();
		
			theAudioSourceMonitor = theGameObject.AddComponent(typeof(OIMSAudioSourceMonitor)) as OIMSAudioSourceMonitor;
			theAudioSourceMonitor.Init(this);
			theAudioSourceMonitor.StartAudio(whenToPlay);
			alternateSource = true;


		} else {
			altGameObject = new GameObject("OIMSLayer - " + theAudioClip.name + " " + Time.realtimeSinceStartup);
			altAudioSource = altGameObject.AddComponent<AudioSource>();
			altAudioSource.clip = theAudioClip;
			altAudioSource.loop = false;
			altAudioSource.volume = currentVolume * volumeModifier;;

			//OIMSAudioSourceMonitor myAudioSourceMonitor = new OIMSAudioSourceMonitor();
		
			altAudioSourceMonitor = altGameObject.AddComponent(typeof(OIMSAudioSourceMonitor)) as OIMSAudioSourceMonitor;
			altAudioSourceMonitor.Init(this);
			altAudioSourceMonitor.StartAudio(whenToPlay);
			alternateSource = false;

		}



	}
	
	public void StartAudio(OIMSPhrase whichPhrase = default(OIMSPhrase), float whenToPlay = 0.0f, float currrentVolume = 1.0f )
	{
		//Debug.Log("StartAudioLayer - whenToPlay: " + whenToPlay);
		alternateSource = false;
		parentPhrase = whichPhrase;
		fadingIn = false;
		if(currrentVolume == 1.0f){
			fadingOut = false;
		}


		//if there already is a game object, then destroy it
		// if(theGameObject != null){
		// 	Object.DestroyImmediate(theGameObject);
			
		// 	//Destroy(theGameObject);
		// 	//theGameObject.Destroy();
		// }	

		CreateLayer(whenToPlay, currrentVolume);
		
		// if(alternateSource){
		// 	theAudioSourceMonitor.StartAudio(whenToPlay);
		// } else {
		// 	altAudioSourceMonitor.StartAudio(whenToPlay);
		// }
			//myAudioSourceMonitor.StartAudio(numberOfLoops, 2.0f);
		
		return;

	}

	public void StartAndFadeInAudio(OIMSPhrase whichPhrase = default(OIMSPhrase), float whenToPlay = 0.0f )
	{
		Debug.Log("StartAndFadeInAudioLayer - whenToPlay: " + whenToPlay);
		parentPhrase = whichPhrase;
		
		//if there already is a game object, then destroy it
		// if(theGameObject != null){
		// 	Object.DestroyImmediate(theGameObject);
			
		// 	//Destroy(theGameObject);
		// 	//theGameObject.Destroy();
		// }	
		fadingIn = true;
		fadingOut = false;

		CreateLayer(whenToPlay, 0.0f);
		
		// if(alternateSource){
		// 	theAudioSourceMonitor.StartAudio(whenToPlay);
		// } else {
		// 	altAudioSourceMonitor.StartAudio(whenToPlay);
		// }
			//myAudioSourceMonitor.StartAudio(numberOfLoops, 2.0f);
		
		return;

	}
	
	public void StopAudio()
	{
		//Debug.Log("layer stop audio");

		if(theGameObject != null){
			//theAudioSource.Stop();
			UnityEngine.Object.DestroyImmediate(theGameObject);
		}
		
		if(altGameObject != null){
			//altAudioSource.Stop();
			UnityEngine.Object.DestroyImmediate(altGameObject);
		}

	}

	public void FadeOutAudio()
	{
		Debug.Log("FadeOutAudio()");
		fadingIn = false;
		fadingOut = true;
		//if(theGameObject != null){
			//theAudioSource.Stop();
		//	UnityEngine.Object.DestroyImmediate(theGameObject);
		//}
		
		//if(altGameObject != null){
			//altAudioSource.Stop();
		//	UnityEngine.Object.DestroyImmediate(altGameObject);
		//}

	}

	public void DoNext(float when)
	{
		//Debug.Log("Layer DoNext");
		
		//first check to see whether the layer has stuff to do...

		//wait a minute ... check to see whether this is the first layer in a phrase

		//if not, then call parent
		parentPhrase.DoNext(when, theAudioSource.volume);

	}

	public void RemoveObjects(){
		fadingOut = false;
		UnityEngine.Object.DestroyImmediate(theGameObject);
		UnityEngine.Object.DestroyImmediate(altGameObject);
	}

	
}
		

