using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineGenerator : MonoBehaviour
{
    public GameObject linePrefab;
    public GameObject controller;

    Line activeLine;

    void Start(){
        controller = GameObject.FindGameObjectWithTag("GameController");
    }
    void Update(){
        if(controller.GetComponent<Game>().deleteLines == true){
            deleteLines();
            controller.GetComponent<Game>().deleteLines = false;
        }
        if(Input.GetMouseButtonDown(0)){
            deleteLines();
        }
        if(Input.GetMouseButtonDown(1) && !Input.GetMouseButton(0)){
            GameObject newLine = Instantiate(linePrefab, new Vector3(0, 0, -3.0f), Quaternion.identity);
            newLine.tag = "Line";
            activeLine = newLine.GetComponent<Line>();
        }

        if(Input.GetMouseButtonUp(1)){
            activeLine = null;
            GameObject newLine = Instantiate(linePrefab, new Vector3(0, 0, -3.0f), Quaternion.identity);
            newLine.tag = "Line";
            activeLine = newLine.GetComponent<Line>();
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
            mousePos.y = mousePos.y - .1f;
            mousePos.z = -2;
            activeLine.UpdateLine(mousePos, true);
            for(int i = 0; i < 15; i++){
                mousePos.y = mousePos.y + .011f;
                activeLine.UpdateLine(mousePos, true);
            }
            activeLine = null;
        }

        if(activeLine != null){
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
            mousePos.z = -2;
            activeLine.UpdateLine(mousePos);
        }
        
    }
    public void deleteLines(){
        GameObject[] lines = GameObject.FindGameObjectsWithTag("Line"); 
        for(int i = 0; i < lines.Length; i++){
            Destroy(lines[i]);
        }
    }
}
