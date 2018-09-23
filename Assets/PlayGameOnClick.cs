using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayGameOnClick : MonoBehaviour {

	EventTrigger e;
	void Start ()
	{
		EventTrigger trigger = GetComponent<EventTrigger>();
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerUp;
		entry.callback.AddListener((data) => { SceneManager.LoadScene(1); });
		trigger.triggers.Add(entry);
	}

}
