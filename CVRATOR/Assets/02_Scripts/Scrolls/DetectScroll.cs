using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectScroll : MonoBehaviour {

    public GameObject scroll;
    private UVScroller scroller;
    private float startTime;

	// Use this for initialization
	void Awake () {
        scroller = scroll.GetComponent<UVScroller>();
    }

    private void OnCollisionEnter(Collision collision) {
        startTime = Time.time;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.name == "LeftHandAnchor" || collision.gameObject.name == "RightHandAnchor") 
        {
            if (gameObject.name.Contains("Right"))
            {
                //scroll right
                scroller.Scroll(1, startTime);
            }
            else if (gameObject.name.Contains("Left"))
            {
                //scroll left
                scroller.Scroll(-1, startTime);
            }
        }
    }
}
