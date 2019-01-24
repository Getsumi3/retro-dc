using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ResetSaves
{
	#if UNITY_EDITOR
	[MenuItem("Utilities/Reset Save")]
	public static void DeleteEsave()
	{
		PlayerPrefs.DeleteAll();
		Debug.Log("Save Reseted!");
	}
	#endif
}
