using UnityEngine.EventSystems;
using UnityEngine;

public class BodyClick : MonoBehaviour, IPointerClickHandler {

	public CelestialBody Body { get; set; }

	public void OnPointerClick(PointerEventData eventData) 
	{
		GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
		cam.GetComponent<FollowCamera>().SetTarget(Body);
	}
}
