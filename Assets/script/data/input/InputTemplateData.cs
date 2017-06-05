using System.Collections.Generic;
using UnityEngine;

public class InputTemplateData {

    public int id = 0;
    public List<Vector2> offposList = new List<Vector2>();

    public InputTemplateData(int id, Vector2[] poss)
    {
        this.id = id;
        for (int i = 0; i < poss.Length; i++)
        {
            offposList.Add(poss[i]);
        }
    }
}
