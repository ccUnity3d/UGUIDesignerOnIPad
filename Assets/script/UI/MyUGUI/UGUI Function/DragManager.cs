using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DragManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public ScrollRect myscroll;
    public static DragManager Instance;
    public bool IsScroll = true; 
    
    void Awake()
    {
        Instance = this;
    }
}
