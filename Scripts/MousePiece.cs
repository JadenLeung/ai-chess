using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MousePiece : MonoBehaviour
{
    public GameObject controller;
    public Sprite Wqueen, Wking, Wpawn, Wbishop, Wknight, Wrook, Bqueen, Bking, Bpawn, Bbishop, Bknight, Brook;
    public void Activate(){
        controller = GameObject.FindGameObjectWithTag("GameController");
        this.GetComponent<SpriteRenderer>().sortingOrder = 2;
        switch(this.name){
            case "Wqueen": this.GetComponent<SpriteRenderer>().sprite = Wqueen; break;
            case "Wknight": this.GetComponent<SpriteRenderer>().sprite = Wknight; break;
            case "Wbishop": this.GetComponent<SpriteRenderer>().sprite = Wbishop; break;
            case "Wking": this.GetComponent<SpriteRenderer>().sprite = Wking; break;
            case "Wpawn": this.GetComponent<SpriteRenderer>().sprite = Wpawn; break;
            case "Wrook": this.GetComponent<SpriteRenderer>().sprite = Wrook; break;
            case "Bqueen": this.GetComponent<SpriteRenderer>().sprite = Bqueen; break;
            case "Bknight": this.GetComponent<SpriteRenderer>().sprite = Bknight; break;
            case "Bbishop": this.GetComponent<SpriteRenderer>().sprite = Bbishop; break;
            case "Bking": this.GetComponent<SpriteRenderer>().sprite = Bking; break;
            case "Bpawn": this.GetComponent<SpriteRenderer>().sprite = Bpawn; break;
            case "Brook": this.GetComponent<SpriteRenderer>().sprite = Brook; break;
        }
    }
    public void Update(){
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        mousePos.z = -9;
        transform.position = mousePos;
    }
    public void SetCoords(float x, float y){
       // this.transform.position = new Vector3(1, 0, -1.0f);
    }
}
