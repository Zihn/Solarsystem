using UnityEngine.EventSystems;
using UnityEngine;

public class ReturnToOverview : MonoBehaviour, IPointerClickHandler {

	public FollowCamera followCam;

	public void OnPointerClick(PointerEventData eventData) 
	{
		GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
		followCam.ReturnToOverview();
	}
}
