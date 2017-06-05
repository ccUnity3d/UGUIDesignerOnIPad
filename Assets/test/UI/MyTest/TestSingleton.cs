using UnityEngine;
using System.Collections;

public class TestSingleton : MonoBehaviour {

    // Use this for initialization
    void Awake()
    {
        Debug.Log("Awake");
        //  UIManager.Instance.Main();
    }
	void Start () {

        Debug.Log("Start");
    }
    void Main()
    {
        Debug.Log("Main");
    }
	// Update is called once per frame
	void Update () {
	
	}
}
