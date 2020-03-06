using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVScroller : MonoBehaviour {

    float scrollSpeed = .0005f;
    float offset = 0;
    Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        offset = GetComponent<Renderer>().material.GetTextureOffset("_MainTex").x;
    }


    //direction = -1 for scroll left; direction = 1 for scroll right
    public void Scroll(float direction, float time)
    {
        offset += scrollSpeed * direction;//(time  * scrollSpeed) * direction;
        Debug.Log("Scroll: "+ (Time.time - time));
        rend.material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
    }
    
}
