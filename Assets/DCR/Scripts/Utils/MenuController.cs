using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

public class MenuController : MonoBehaviour {

	
	public GameObject mainPanel;

	public GameObject settingsPan;

	public GameObject soundPan;
	public GameObject qualityPan;
    public GameObject controlsPan;

    //public GameObject creditsPan;

    bool isInSettings = false;

	void Update()
	{
		ActiveUIWindow();
	}

	private void ActiveUIWindow()
	{
		if (settingsPan.activeSelf && !isInSettings) {
			if (CrossPlatformInputManager.GetButtonDown ("Cancel")) {
				settingsPan.SetActive (false);
				mainPanel.SetActive (true);
			}
		}

		if (controlsPan.activeSelf == true || soundPan.activeSelf == true || qualityPan.activeSelf == true) {
            isInSettings = true;
			if (CrossPlatformInputManager.GetButtonDown ("Cancel")) {
				controlsPan.SetActive (false);
				soundPan.SetActive (false);
                qualityPan.SetActive(false);
				settingsPan.SetActive (true);
                isInSettings = false;

            }
        }
			
	}
}
