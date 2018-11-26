using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyStickInputTest : MonoBehaviour {

    Vector3 startpos;
    Transform thisTransform;
    Vector3 inputDirection;

    void Start () {
        thisTransform = this.transform;
        startpos = thisTransform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        inputDirection = Vector3.zero;
        inputDirection.x = Input.GetAxis("AimHorizontal");
        inputDirection.y = Input.GetAxis("AimVertical");
        Debug.Log(inputDirection);
        thisTransform.position = inputDirection;
    }
}
