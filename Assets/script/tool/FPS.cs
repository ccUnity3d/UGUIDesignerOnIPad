using UnityEngine;
using System.Collections;

public class FPS : MonoBehaviour {

    float start = 0;
    float fps = 0;
    int times = 0;

    void Start ()
    {
        start = 0;
        fps = 0;
        times = 0;
        if(Application.platform == RuntimePlatform.IPhonePlayer) gameObject.SetActive(false);
    }

    void Update ()
    {
        start += Time.deltaTime;
        times += 1;
        if (start > 1)
        {
            fps = times;
            times = 0;
            start = 0;
        }
    }


    void OnGUI()
    {
        GUI.color = Color.red;
        GUI.Label(new Rect(Screen.width - 200f, Screen.height- 200f, 160, 60), fps.ToString());
    }

}
