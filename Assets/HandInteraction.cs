using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandInteraction : MonoBehaviour {
	public SteamVR_TrackedObject trackedObj;
	public SteamVR_Controller.Device device;
	public float throwForce = 1.5f;

	// Swipe
	public float swipeSum;
	public float touchLast;
	public float touchCurrent;
	public float distance;
	public bool hasSwipedLeft;
	public bool hasSwipedRight;
	public ObjectMenuManager objectMenuManager;

	// Use this for initialization
	void Start () {
		trackedObj = GetComponent<SteamVR_TrackedObject>();
	}
	
	// Update is called once per frame
	void Update () {
		device = SteamVR_Controller.Input((int)trackedObj.index);
		if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad)) {
			
			touchLast = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
		}
			if (device.GetTouch(SteamVR_Controller.ButtonMask.Touchpad)) {
			touchCurrent = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
			distance = touchCurrent - touchLast;
			touchLast = touchCurrent;
			swipeSum += distance;
			if (!hasSwipedRight) {
				if (swipeSum > 0.5f) {
					swipeSum = 0;
					SwipeRight();
					hasSwipedRight = true;
					hasSwipedLeft = false;
				}
			}
			if (!hasSwipedLeft) {
				if (swipeSum < -0.5f) {
					swipeSum = 0;
					SwipeLeft();
					hasSwipedRight = false;
					hasSwipedLeft = true;
				}
			}

			
		}
		if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad)) {
			swipeSum = 0;
			touchCurrent = 0;
			touchLast = 0;
			hasSwipedRight = false;
			hasSwipedLeft = false;
		}

		if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad)) {
			// Spawn object currently selected by menu
			SpawnObject();
		}
	}

	private void SpawnObject() {
		objectMenuManager.SpawnCurrentObject();
	}

	private void SwipeLeft() {
		objectMenuManager.MenuLeft();
	}

	private void SwipeRight() {
		objectMenuManager.MenuRight();
	}

	private void OnTriggerStay(Collider other) {
		if (other.gameObject.CompareTag("Throwable")) {
			if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger)) {
				ThrowObject(other);
			} else if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
				GrabObject(other);
			}
		}
	}

	private void GrabObject(Collider col) {
		col.transform.SetParent(gameObject.transform);
		col.GetComponent<Rigidbody>().isKinematic = true;
		device.TriggerHapticPulse(2000);
		Debug.Log("You are touching down the trigger an an object");
	}

	private void ThrowObject(Collider col) {
		col.transform.SetParent(null);
		Rigidbody rb = col.GetComponent<Rigidbody>();
		rb.isKinematic = false;
		rb.velocity = device.velocity * throwForce;
		rb.angularVelocity = device.angularVelocity;
		Debug.Log("You have released the trigger");

	}
}
