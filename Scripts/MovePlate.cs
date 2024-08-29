using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MovePlate : MonoBehaviour
{
    public GameObject controller;

    GameObject reference = null;

    //Board Positions, not world positions
    int matrixX;
    int matrixY;

    //false = movement, ture = attacking
    public bool attack = false;

    public bool pieceplate = false;
    public bool mypromoteplate = false;
    public bool levelplate = false;
    public bool ispressed = false;
    public bool checkplate = false;
    public int castleplate = 0;
    public bool passantplate = false;
    public TextMeshPro score;
    public void Start(){
        controller = GameObject.FindGameObjectWithTag("GameController");
        this.GetComponent<SpriteRenderer>().sortingOrder = 1;
        score.fontSize = 90;
        score.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        if(levelplate){
            int level = (7-matrixY)*7 + matrixX;
            if(Game.medals[level] == "gray")
                gameObject.GetComponent<SpriteRenderer>().color = new Color(0.0f, 0.3f, 0.0f, 1.0f); 
            else if(Game.medals[level] == "gold")
                gameObject.GetComponent<SpriteRenderer>().color = Color.yellow; 
            else if(Game.medals[level] == "bronze")
                gameObject.GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.4f, 0.2f, 1.0f);
            else if(Game.medals[level] == "silver")
                gameObject.GetComponent<SpriteRenderer>().color = new Color32(217,  217 , 217 , 255);
                

               

            score.text = (level < 10 ? "0" : "") + level.ToString(); 
        }
        else
            score.text = "";
        if(attack) gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        if(pieceplate) gameObject.GetComponent<SpriteRenderer>().color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
        if(castleplate > 0) gameObject.GetComponent<SpriteRenderer>().color = new Color32(51, 255, 224, 255);
        if(checkplate) gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 0.0f, 1.0f);;

    }
    public void Update(){
        if(GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>().actuallypromote && mypromoteplate){
            mypromoteplate = false;
            GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>().actuallypromote = false;
            //Debug.Log("promoting");
            MovePiece();  
        }
        controller = GameObject.FindGameObjectWithTag("GameController");
        if(levelplate){
            this.GetComponent<BoxCollider2D>().enabled = (controller.GetComponent<Game>().platepositions[matrixX, matrixY] != 1 && controller.GetComponent<Game>().GetPosition(matrixX, matrixY) == null);
        }
        if(Input.GetKeyDown("space")){ //space
            //("(" + matrixX + "," + matrixY + ")"); 
        }
        if(pieceplate) {
            if(ispressed)
                gameObject.GetComponent<SpriteRenderer>().color = new Color(0.0f, 0.0f, 1.0f, 1.0f);
            else
                gameObject.GetComponent<SpriteRenderer>().color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
        }
    }
    
    public void OnMouseOver()
    {
        if (checkplate) return;
        controller = GameObject.FindGameObjectWithTag("GameController");
        if(levelplate){
            if(((7-matrixY)*7 + matrixX) >= controller.GetComponent<Game>().levels.GetLength(0)) return;
            GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = "Level " + ((7-matrixY)*7 + matrixX) + ": " + controller.GetComponent<Game>().levels[(7-matrixY)*7 + matrixX, 5];
        }
        if(pieceplate || levelplate) return;
        if(Input.GetMouseButtonUp(0)){
            activateMovePlate(); 
        }
        else if(Input.GetMouseButtonDown(0)){
            activateMovePlate();
        }
    }
    public void OnMouseDown(){
        controller = GameObject.FindGameObjectWithTag("GameController");
        if (checkplate) return;
        if(levelplate){
            Debug.Log((7-matrixY)*7 + matrixX);
            controller.GetComponent<Game>().makeLevel((7-matrixY)*7 + matrixX);
        }
        if(pieceplate){
            controller.GetComponent<Game>().removePiece(matrixX, matrixY);
        }
        if(pieceplate && controller.GetComponent<Game>().spent < controller.GetComponent<Game>().budget){
            if(ispressed){
                controller.GetComponent<Game>().cancelPiecePlate();
                return;
            }
            if(controller.GetComponent<Game>().setpiece){
                GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate"); 
                for(int i = 0; i < movePlates.Length; i++){
                if(movePlates[i].GetComponent<MovePlate>().ispressed == true)
                    movePlates[i].GetComponent<MovePlate>().ispressed = false;
                }
            }
            activateMovePlate(); 
        }
    }
    public void activateMovePlate(){
        bool nopromote = false;
        controller = GameObject.FindGameObjectWithTag("GameController");
        if(controller.GetComponent<Game>().pawnpromote) return;
        if(attack){
            GameObject cp;
            if(passantplate && reference.GetComponent<Chessman>().name[0] == 'W') 
                cp = controller.GetComponent<Game>().GetPosition(matrixX, matrixY-1);
            else if(passantplate && reference.GetComponent<Chessman>().name[0] == 'B') 
                cp = controller.GetComponent<Game>().GetPosition(matrixX, matrixY+1);
            else
                cp = controller.GetComponent<Game>().GetPosition(matrixX, matrixY);

            if(cp.name == "Wking"){
                 controller.GetComponent<Game>().Winner("black");
                 nopromote = true;
            }
            if(cp.name == "Bking"){ 
                if(controller.GetComponent<Game>().numPiece("Bking") == 1)
                    controller.GetComponent<Game>().Winner("white");
                nopromote = true;
            }
            Destroy(cp);
            controller.GetComponent<Game>().recentmove = "capture";
        }
        else{
            if(castleplate > 0)
                controller.GetComponent<Game>().recentmove = "castle";
            else
                controller.GetComponent<Game>().recentmove = "move";
        }
        if(pieceplate){
            controller.GetComponent<Game>().hideMainButtons();
            controller.GetComponent<Game>().setpiece = true;
            controller.GetComponent<Game>().setpiecex = matrixX;
            controller.GetComponent<Game>().setpiecey = matrixY;
            ispressed = true;
        }
        if((reference.GetComponent<Chessman>().name == "Wpawn" && matrixY == 7
        || reference.GetComponent<Chessman>().name == "Bpawn" && matrixY == 0) && !nopromote){
            //reference.GetComponent<Chessman>().DestroyMovePlates(); 
            controller.GetComponent<Game>().hideMainButtons();
            mypromoteplate = true;
            controller.GetComponent<Game>().pawnpromote = true;

            //Debug.Log("pawn");
            return;
        }
        MovePiece();
    }
    public void MovePiece(){
        if(pieceplate) return;
        controller = GameObject.FindGameObjectWithTag("GameController");
        Game sc = controller.GetComponent<Game>();
        if(reference.GetComponent<Chessman>().name == "Wpawn" && matrixY == 7 || 
        reference.GetComponent<Chessman>().name == "Bpawn" && matrixY == 0){
            controller.GetComponent<Game>().recentmove = "promote";
            string pie = controller.GetComponent<Game>().piecepromote;
            Debug.Log("Promoting piecei s " + pie);
            reference.GetComponent<Chessman>().name = pie;
            reference.GetComponent<Chessman>().setSprite(pie);
        } 

        controller.GetComponent<Game>().SetPositionEmpty(reference.GetComponent<Chessman>().GetXBoard(),
        reference.GetComponent<Chessman>().GetYBoard());

        int oldx = reference.GetComponent<Chessman>().GetXBoard();
        int oldy = reference.GetComponent<Chessman>().GetYBoard();

        if(reference.GetComponent<Chessman>().name[1] == 'p' && reference.GetComponent<Chessman>().GetYBoard() == 1 && matrixY == 3){
            controller.GetComponent<Game>().enpassant = matrixX;
            Debug.Log("23234");
        }
        else if(reference.GetComponent<Chessman>().name[1] == 'p' && reference.GetComponent<Chessman>().GetYBoard() == 6 && matrixY == 4)
            controller.GetComponent<Game>().enpassant = matrixX;
        else controller.GetComponent<Game>().enpassant = -1;

        reference.GetComponent<Chessman>().SetXBoard(matrixX);
        reference.GetComponent<Chessman>().SetYBoard(matrixY);
        reference.GetComponent<Chessman>().SetCoords();
        controller.GetComponent<Game>().SetPosition(reference);

        string str = "";
        if(reference.GetComponent<Chessman>().name[2] == 'n')
        str += reference.GetComponent<Chessman>().name[2].ToString().ToUpper();
        else
        str += reference.GetComponent<Chessman>().name[1].ToString().ToUpper();

        if(reference.GetComponent<Chessman>().name[1] == 'p') str = "";

        string sattack = "";
        if (attack && str == "") {
            sattack += (char)(oldx + 'a') + "";
        }
        sattack += str + (attack? "x" : "") + (char)('a' + matrixX) + "" + (matrixY+1) + (sc.enpassant != -1 ? "!!" : "");
        if (castleplate > 0) { 
            if (matrixX == 6) sattack = "O-O";
            else sattack = "O-O-O"; 
        }
        Coords kloc = sc.findKing(sc.currentPlayer == "white" ? "black" : "white");
        Debug.Log("King is in " + kloc.spos);
        Debug.Log("squareAttacked? " + sc.squareAttacked(kloc.x, kloc.y, sc.currentPlayer == "white" ? "black" : "white"));
        if (sc.squareAttacked(kloc.x, kloc.y, sc.currentPlayer == "white" ? "black" : "white") != "") {
            sattack += "+";
            sc.GetComponent<Game>().recentmove = "check";
        }
        sc.allmoves.Add(sattack);
        sc.redomoves.Clear();

        int rookx = -1;
        int rooky = -1;
        int rookx2 = -1;
        int rooky2 = -1;

        if(castleplate == 1){
            rookx = 7; rooky = 0; rookx2 = 5; rooky2 = 0;
        }
        if(castleplate == 2){
            rookx = 0; rooky = 0; rookx2 = 3; rooky2 = 0; 
        }
        if(castleplate == 3){
            rookx = 7; rooky = 7; rookx2 = 5; rooky2 = 7;
        }
        if(castleplate == 4){
            rookx = 0; rooky = 7; rookx2 = 3; rooky2 = 7; 
        }
        
 
        if(castleplate > 0){
            GameObject reference2 = controller.GetComponent<Game>().GetPosition(rookx, rooky);
            controller.GetComponent<Game>().SetPositionEmpty(rookx,rooky);
            reference2.GetComponent<Chessman>().SetXBoard(rookx2);
            reference2.GetComponent<Chessman>().SetYBoard(rooky2);
            reference2.GetComponent<Chessman>().SetCoords(); 
            controller.GetComponent<Game>().SetPosition(reference2);
        }
        controller.GetComponent<Game>().NextTurn();
        reference.GetComponent<Chessman>().DestroyMovePlates();

    }
    public void SetCoords(int x, int y){
        matrixX = x;
        matrixY = y;
    }
    public void SetReference(GameObject obj){
        reference = obj;
    }
    public GameObject GetReference(){
        return reference;
    }
}
