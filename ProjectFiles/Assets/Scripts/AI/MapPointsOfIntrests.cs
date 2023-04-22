using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PatrolPath 
{
    public List<Transform> Points = new List<Transform>();
    public Transform transform;

    public Vector3 GetPoint(int index)
    {
        if (Points.Count > index)
            return Points[index].position;
        else
        {
            Debug.Log("index of:" + index + " not found");
            return Vector3.zero;
        }    
    }
}


public class MapPointsOfIntrests : MonoBehaviour
{
    public List<PatrolPath> Paths = new List<PatrolPath>();
}
