using UnityEngine;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;

public class SelectedUIController : MonoBehaviour {

	private EventSystem eventSystem;
	//first ui element to select after keyboard press
	public GameObject selectedObject;

	private bool btnSelected;

	// Use this for initialization
	void Start () {
		eventSystem = GameObject.FindObjectOfType<EventSystem> ();	
	}
	
	// Update is called once per frame
	void Update () {
		if (CrossPlatformInputManager.GetAxisRaw ("Vertical") != 0 && btnSelected == false) {
			eventSystem.SetSelectedGameObject (selectedObject);
			btnSelected = true;
		}

	}

	private void OnDisable()
	{
		btnSelected = false;
	}
}
