#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[InitializeOnLoad] 
public class OIMSEditorUpdate : EditorWindow {

	private EditorApplication.CallbackFunction s_backgroundUpdateCB;
 	int executionCount = 0;
 	public void BackgroundUpdateFunc()
 	{
     	executionCount++;
     	Debug.Log("running background update func: " + EditorApplication.timeSinceStartup.ToString() + " " + (EditorApplication.isCompiling ? "compiling":"compiled") + " " + executionCount + " delegate: " + EditorApplication.update);
 	}
 
 	[ MenuItem( "Window/Background Updater" ) ]
 	public static void Launch()
 	{
    	EditorWindow window = GetWindow( typeof( OIMSEditorUpdate ) );
    	window.Show();
 	} 
 
 	void OnEnable() {
    	Debug.Log(this + " OnEnable");
		s_backgroundUpdateCB = new EditorApplication.CallbackFunction(BackgroundUpdateFunc);
    	EditorApplication.update += s_backgroundUpdateCB;
 	}
 
 	void OnDisable() {
    	Debug.Log(this + " OnDisable");
    	EditorApplication.update -= s_backgroundUpdateCB;
 	}
 
 	void OnProjectChange() {
    	Debug.LogWarning("OnProjectChange");
 	}
 
 	public void OnGUI()
 	{
    	EditorGUILayout.SelectableLabel((EditorApplication.update!=null)?EditorApplication.update.ToString():"NONE");
 	}
}

#endif
