using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowCamera : MonoBehaviour {
	public float speed = 0.1f;
	public float overviewSize = 10000;
	public float scrollSpeed = 0.5f;
	[Header("Mobile Pinch Zoom")]
	public float perspectiveZoomSpeed = 0.5f;        // The rate of change of the field of view in perspective mode.
	public float orthoZoomSpeed = 0.5f;        // The rate of change of the orthographic size in orthographic mode.

	public Vector2 offsetInBodyLengths = Vector2.zero;
	public UICelestialBodyInfo CelestialBodyInfoUI;

	private float distanceMin = 10f;
	private float distanceMax = 1000f;
	private float distanceInitial = 100f;
	private float distanceCurrent;
	private Vector3 positionCurrent;
	private bool animate = false;
	private Vector3 offset = Vector3.zero;

	private GameObject _target;
	private Camera cam;
	private Vector3 baseVector = new Vector3(0, 0, -10f);

	void Start () 
	{		
		gameObject.transform.position = baseVector;
		cam = gameObject.GetComponent<Camera>();
		cam.orthographicSize = 1.5f * overviewSize;
		positionCurrent = GetVector(Vector3.zero);
		distanceCurrent = overviewSize;
		animate = true;
	}

	public void SetTarget(CelestialBody target)
	{
		_target = target.GameObject;
		CelestialBodyInfoUI.SetTarget(target, cam);
		gameObject.transform.parent = _target.transform;
		distanceMax = _target.GetComponent<RectTransform>().sizeDelta.x * 4;
		distanceMin = distanceMax * 0.1f;
		distanceInitial = distanceMax * 0.5f;
		offset = new Vector3(offsetInBodyLengths.x * distanceInitial*0.5f, offsetInBodyLengths.y * distanceInitial*0.5f, 0);
		positionCurrent = GetVector(Vector3.zero) + offset; 
		distanceCurrent = distanceInitial;
		animate = true;
	}

	public void ReturnToOverview()
	{
		CelestialBodyInfoUI.HideUI();
		gameObject.transform.parent = null;
		_target = null;
		positionCurrent = GetVector(Vector3.zero);
		distanceCurrent = overviewSize;
		animate = true;
	}

	private bool CheckPosition()
	{
		bool c1 = Mathf.Abs(cam.orthographicSize - distanceCurrent) < 1;
		bool c2 = Vector3.Distance(gameObject.transform.localPosition, positionCurrent) < 1;
		return c1 && c2;
	}

	private Vector3 GetVector(Vector3 p)
	{
		return new Vector3(p.x, p.y, -10);
	}

	// For mac testing with trackpad
	void OnGUI() 
	{
//		if (!animate && Event.current.type == EventType.ScrollWheel)
//		{
//			distanceCurrent -= Event.current.delta.y * scrollSpeed;
//			cam.orthographicSize = Mathf.Clamp(distanceCurrent, distanceMin, distanceMax);
//		}

	}

	void Update()
	{
		if (animate) {
			gameObject.transform.localPosition = Vector3.Lerp(gameObject.transform.localPosition, positionCurrent, speed * Time.deltaTime); 
			cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, distanceCurrent, scrollSpeed * Time.deltaTime);
			if (CheckPosition()) {
				animate = false;
				gameObject.transform.localPosition = positionCurrent;
				cam.orthographicSize = distanceCurrent;
			}
		} else {
			// If there are two touches on the device...
			if (Input.touchCount == 2)
				PinchZoom(Input.GetTouch(0), Input.GetTouch(1));
			if (Input.GetAxis("Mouse ScrollWheel") != 0) {
				distanceCurrent -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed * speed;
				cam.orthographicSize = Mathf.Clamp(distanceCurrent, distanceMin, distanceMax);
			} 
			if (Input.GetKeyDown(KeyCode.Q))
				ReturnToOverview();
		}
	}


	// From unity src: https://unity3d.com/learn/tutorials/topics/mobile-touch/pinch-zoom
	private void PinchZoom(Touch touchZero, Touch touchOne)
	{
		// Find the position in the previous frame of each touch.
		Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
		Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

		// Find the magnitude of the vector (the distance) between the touches in each frame.
		float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
		float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

		// Find the difference in the distances between each frame.
		float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

		// If the camera is orthographic...
		if (cam.orthographic)
		{
			// ... change the orthographic size based on the change in distance between the touches.
			cam.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;

			// Make sure the orthographic size never drops below zero.
			cam.orthographicSize = Mathf.Max(cam.orthographicSize, 0.1f);
		}
		else
		{
			// Otherwise change the field of view based on the change in distance between the touches.
			cam.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

			// Clamp the field of view to make sure it's between 0 and 180.
			cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 0.1f, 179.9f);
		}
	}
}
