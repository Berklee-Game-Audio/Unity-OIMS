using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class OIMSAudioSourceMonitor : MonoBehaviour {

	//private int remainingNumberOfLoops = 0;
	private float nextPlayTime = 0f;
	private new AudioSource audio;
	private OIMSLayer parentLayer;
	private bool playedAlready = false;

	private float nextActionTime = -1f;
	//private bool nextActionSent = false;

	//private bool fadingOut = false;
	
	// Use this for initialization
	
	void OnEnable()
	{
		//EditorApplication.update += Update;
	}

	public void Init(OIMSLayer whichLayer = default(OIMSLayer))
	{
		playedAlready = false;
		parentLayer = whichLayer;
	}
	
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//Debug.Log("Update Updating");
	}

	void EditorUpdate()
	{
		//MonitorAudio.MoveNext();
		Debug.Log("Editor Updating");
	}

	public void StartAudio(float whenToPlay = 0.0f, float nextAudioPickupTime = 0.0f)
	{
		//remainingNumberOfLoops = numberOfLoops;
		audio = GetComponent<AudioSource>();
		//audio.Play();
		//nextPlayTime = Time.realtimeSinceStartup + audio.clip.length;
		//nextPlayTime = Time.realtimeSinceStartup + whenToPlay;
		nextPlayTime = whenToPlay;
		
		
		StartCoroutine("MonitorAudio");
		
		if(parentLayer.fadingIn){
			StartCoroutine("FadeInAudio");
		}

		StartCoroutine("FadeOutAudio");
		

		//if(parentLayer.fadingOut){
		//	StartCoroutine("FadingOutAudio");
		//}
	}

	private IEnumerator MonitorAudio()
	{
		//Debug.Log("MonitorAudio");
		//Debug.Log("remainingNumberOfLoops: " + remainingNumberOfLoops);

		
		while(Time.realtimeSinceStartup < nextPlayTime)
		{
			//then we just wait
			//Debug.Log("we're just waiting");
			yield return null;
		} 
		
		//Debug.Log("we are in");

		//if we haven't played it yet, then play it
		if(!playedAlready){
			audio.Play();
			if(parentLayer.tailTime > 0.0f){
				nextActionTime = Time.realtimeSinceStartup + audio.clip.length - parentLayer.tailTime;
			} else {
				nextActionTime = Time.realtimeSinceStartup + audio.clip.length;
			}			
			
			Debug.Log("OIMSAudioSourceMonitor next action time: " + nextActionTime);

			playedAlready = true;
			//also pass it up to the parent to decide what to do after this
			//parentLayer.DoNext(nextActionTime);
			yield return null;
		} 

		while(Time.realtimeSinceStartup < (nextActionTime - 0.5f)){
			yield return null;
		}
		
		parentLayer.DoNext(nextActionTime);
		//nextActionSent = true;
		
		while(audio.isPlaying){
			//Debug.Log("Waiting to Destroy Object");
			yield return null;
		}

		//Debug.Log("Destroy Object");
		Destroy(this.gameObject);

		yield return null;

	}

	private IEnumerator FadeInAudio()
	{
		if(audio == null){
			yield return null;
		}
			
		while(parentLayer.fadingIn == true)
		{
			
			audio.volume = audio.volume + 0.001f;

			if(audio.volume > 0.98f){
				parentLayer.fadingIn = false;
			}
			yield return null;

		}

		yield return null;

	}

	private IEnumerator FadeOutAudio()
	{
		if(audio == null){
			yield return null;
		}
		
		while(parentLayer.fadingOut == false){
			yield return null;
		}

		while(audio.volume > 0.001f){
			audio.volume = audio.volume - 0.01f;
			yield return null;
		}

		if(audio.volume <= 0.001f){
			//get rid of the objects
			parentLayer.RemoveObjects();
		}

		yield return null;

	}
}
