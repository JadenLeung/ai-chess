using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Line : MonoBehaviour
{
    public LineRenderer lineRenderer;

    List <Vector3> points;
    

    public void UpdateLine(Vector3 position, bool isBig = false){
        if(points == null){
            points = new List<Vector3>();
            SetPoint(position, isBig);
            return;
        }

        if(Vector3.Distance(points.Last(), position) > 0.01f){
            SetPoint(position, isBig);
        }
    }
    void SetPoint(Vector3 point, bool isBig = false){
        points.Add(point);
        lineRenderer.positionCount = points.Count;
        if(isBig){
            lineRenderer.startWidth = 0.2f;
            lineRenderer.endWidth = 0.2f;
        }
        lineRenderer.SetPosition(points.Count - 1, point);
    }


}
