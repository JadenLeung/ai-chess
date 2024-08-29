using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Chessman : MonoBehaviour
{
    //References
    public GameObject controller;
    public GameObject movePlate;
    public GameObject mousePiece;
    public GameObject textmesh;

    //Positions
    public int xBoard = -1;
    public int yBoard = -1;
    private int zBoard = -1;
    public bool isTapped = false;
    public bool shouldSpawn = true;

    //Variable to keep track of "black" or "white" player
    private string player;

    //References to all sprites chess piece can be
    public Sprite Bqueen, Bking, Bpawn, Bbishop, Bknight, Brook;
    public Sprite Wqueen, Wking, Wpawn, Wbishop, Wknight, Wrook;
    private int tapping = 0;  

    public void Start(){
        DestroyMousePiece(); 
    }
    public void Activate(){
        controller = GameObject.FindGameObjectWithTag("GameController");


        //Takes the instantiated location and adjusts the transform
        SetCoords();

        switch(this.name){
            case "Bqueen": this.GetComponent<SpriteRenderer>().sprite = Bqueen; player = "black"; break;
            case "Bknight": this.GetComponent<SpriteRenderer>().sprite = Bknight; player = "black"; break;
            case "Bbishop": this.GetComponent<SpriteRenderer>().sprite = Bbishop; player = "black"; break;
            case "Bking": this.GetComponent<SpriteRenderer>().sprite = Bking; player = "black"; break;
            case "Bpawn": this.GetComponent<SpriteRenderer>().sprite = Bpawn; player = "black"; break;
            case "Brook": this.GetComponent<SpriteRenderer>().sprite = Brook; player = "black"; break;

            case "Wqueen": this.GetComponent<SpriteRenderer>().sprite = Wqueen; player = "white"; break;
            case "Wknight": this.GetComponent<SpriteRenderer>().sprite = Wknight; player = "white"; break;
            case "Wbishop": this.GetComponent<SpriteRenderer>().sprite = Wbishop; player = "white"; break;
            case "Wking": this.GetComponent<SpriteRenderer>().sprite = Wking; player = "white"; break;
            case "Wpawn": this.GetComponent<SpriteRenderer>().sprite = Wpawn; player = "white"; break;
            case "Wrook": this.GetComponent<SpriteRenderer>().sprite = Wrook; player = "white"; break;
        }
    } 
    public void setSprite(string spritename){
        if(spritename == "Wqueen")
            this.GetComponent<SpriteRenderer>().sprite = Wqueen;
        if(spritename == "Wrook")
            this.GetComponent<SpriteRenderer>().sprite = Wrook;
        if(spritename == "Wbishop")
            this.GetComponent<SpriteRenderer>().sprite = Wbishop;
        if(spritename == "Wknight")
            this.GetComponent<SpriteRenderer>().sprite = Wknight;
        if(spritename == "Wking")
            this.GetComponent<SpriteRenderer>().sprite = Wking;
        if(spritename == "Bqueen")
            this.GetComponent<SpriteRenderer>().sprite = Bqueen;
        if(spritename == "Brook")
            this.GetComponent<SpriteRenderer>().sprite = Brook;
        if(spritename == "Bbishop")
            this.GetComponent<SpriteRenderer>().sprite = Bbishop;
        if(spritename == "Bknight")
            this.GetComponent<SpriteRenderer>().sprite = Bknight;
        if(spritename == "Bking")
            this.GetComponent<SpriteRenderer>().sprite = Bking;
    }
    public void SetCoords(){
        float x = xBoard;
        float y = yBoard;

        float mul = (controller.GetComponent<Game>().blackbrain == "user" && controller.GetComponent<Game>().whitebrain != "user") ? -1 : 1;

        x *= 0.7f * mul;
        y *= 0.7f * mul;

        x += -2.45f * mul;
        y += -2.45f * mul;

        this.transform.position = new Vector3(x, y, zBoard);
    }
    public int GetXBoard(){
        return xBoard;
    }

    public int GetYBoard(){
        return yBoard;
    }

    public void SetXBoard(int x){
        xBoard = x;
    }

    public void SetYBoard(int y){
        yBoard = y;
    }
    public void SetZBoard(int z){
        zBoard = z;
    }
    public void OnMouseDown(){ 
        Debug.Log("Down");
        clicked();
    } 
    public void clicked(){
        Debug.Log("Clicked");
        if (this.name[0] == 'B' && controller.GetComponent<Game>().blackbrain != "user") {
            return;
        }
        if (this.name[0] == 'W' && controller.GetComponent<Game>().whitebrain != "user") {
            return;
        }
        if(controller.GetComponent<Game>().pawnpromote || controller.GetComponent<Game>().mode == "setpiece"){
            return;
        }
        if(!controller.GetComponent<Game>().IsGameOver() && controller.GetComponent<Game>().GetCurrentPlayer() == player && !isTapped){
            DestroyMovePlates();
            shouldSpawn = true;
            string moves = InitiateMovePlates();
            Debug.Log("Moves is " + moves);
            tapping = 1;
            isTapped = true;
        }
        if(player == controller.GetComponent<Game>().currentPlayer){
            MousePieceSpawn();
            zBoard = 4;
            SetCoords();
        }
    }
    public void OnMouseUp(){ 
        DestroyMousePiece();
        zBoard = -1;
        SetCoords();

        if(controller.GetComponent<Game>().pawnpromote || controller.GetComponent<Game>().mode == "setpiece"){
            return;
        }
        if(!controller.GetComponent<Game>().IsGameOver() && controller.GetComponent<Game>().GetCurrentPlayer() == player && tapping == 3){
            DestroyMovePlates();
        }
    } 
    public void DestroyMovePlates(bool allplates = false){
        controller.GetComponent<Game>().platepositions = new int [8, 8];
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate"); 
        for(int i = 0; i < movePlates.Length; i++){
            if(movePlates[i] != null && movePlates[i].GetComponent<MovePlate>().GetReference() != null){
                movePlates[i].GetComponent<MovePlate>().GetReference().GetComponent<Chessman>().isTapped = false;
                movePlates[i].GetComponent<MovePlate>().GetReference().GetComponent<Chessman>().tapping = 0;
            }
            if((!movePlates[i].GetComponent<MovePlate>().pieceplate && !movePlates[i].GetComponent<MovePlate>().levelplate) || allplates)
                Destroy(movePlates[i]); 
        }
        isTapped = false;
        tapping = 0;
        //("Destroying!" + tapping + " " + xBoard + " " + yBoard);
    }

    public void DestroyMousePiece(){
        GameObject[] mousePieces = GameObject.FindGameObjectsWithTag("MousePiece");
        for(int i = 0; i < mousePieces.Length; i++){
            Destroy(mousePieces[i]);
        } 
    }

    public string InitiateMovePlates(){
            controller.GetComponent<Game>().platepositions = new int [8, 8];
        if (shouldSpawn) {
            controller.GetComponent<Game>().platepositions[xBoard, yBoard] = 1;
        }
        string ans = "";
        switch(this.name){
            case "Bqueen": 
            case "Wqueen":
                ans += LineMovePlate(0,1);
                ans += LineMovePlate(1,0);
                ans += LineMovePlate(1,1);
                ans += LineMovePlate(-1,0);
                ans += LineMovePlate(0,-1);
                ans += LineMovePlate(-1,-1);
                ans += LineMovePlate(-1,1);
                ans += LineMovePlate(1,-1);
                break;
            case "Bknight":
            case "Wknight":
                ans += LMovePlate();
                break;
            case "Bbishop":
            case "Wbishop":
                ans += LineMovePlate(1,1);
                ans += LineMovePlate(-1,1);
                ans += LineMovePlate(1,-1);
                ans += LineMovePlate(-1,-1);
                break;
            case "Bking":
            case "Wking":
                ans += SurroundMovePlate();
                break;
            case "Brook":
            case "Wrook":
                ans += LineMovePlate(0,1);
                ans += LineMovePlate(1,0);
                ans += LineMovePlate(-1,0);
                ans += LineMovePlate(0,-1);
                break;
            case "Bpawn":
                if (yBoard == 6)
                {
                    ans += PawnMovePlate(xBoard, yBoard - 1, true);
                    if(controller.GetComponent<Game>().GetPosition(xBoard,yBoard-1) == null)
                        ans += PawnMovePlate(xBoard, yBoard - 2, false);
                }
                else
                {
                    ans += PawnMovePlate(xBoard, yBoard - 1, true);
                }
                Game sc = controller.GetComponent<Game>();
                if(yBoard == 3 && sc.enpassant != -1 && (xBoard == sc.enpassant+1 || xBoard == sc.enpassant-1)){
                    ans += PawnMovePlate(sc.enpassant, yBoard - 1, true, true);
                } 
                break;
            case "Wpawn":
                if (yBoard == 1)
                {
                    ans += PawnMovePlate(xBoard, yBoard + 1, true);
                    if(controller.GetComponent<Game>().GetPosition(xBoard,yBoard+1) == null)
                        ans += PawnMovePlate(xBoard, yBoard + 2, false);
                }
                else
                {
                    ans += PawnMovePlate(xBoard, yBoard + 1, true);
                }

                sc = controller.GetComponent<Game>();
                if(yBoard == 4 && sc.enpassant != -1 && (xBoard == sc.enpassant+1 || xBoard == sc.enpassant-1)){
                    ans += PawnMovePlate(sc.enpassant, yBoard + 1, true, true);
                } 

                break;
                
        }
        return ans;
    }

    public string LineMovePlate(int xIncrement, int yIncrement){
        Game sc = controller.GetComponent<Game>();
        string ans = "";
        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;

        while(sc.PositionOnBoard(x,y) && sc.GetPosition(x,y) == null){ //If valid on board and nothing blocking it
            ans += MovePlateSpawn(x,y);
            x += xIncrement;
            y += yIncrement;
        }

        if(sc.PositionOnBoard(x,y) && sc.GetPosition(x,y).GetComponent<Chessman>().player != player){
            ans += MovePlateAttackSpawn(x, y);
        }
        return ans;
    }
    public string LMovePlate(){
        string ans = "";
        ans += PointMovePlate(xBoard + 1, yBoard + 2);
        ans += PointMovePlate(xBoard - 1, yBoard + 2);
        ans += PointMovePlate(xBoard + 2, yBoard + 1);
        ans += PointMovePlate(xBoard + 2, yBoard  - 1);
        ans += PointMovePlate(xBoard + 1, yBoard - 2);
        ans += PointMovePlate(xBoard - 1, yBoard - 2);
        ans += PointMovePlate(xBoard - 2, yBoard + 1);
        ans += PointMovePlate(xBoard - 2, yBoard - 1);
        return ans;
    } 

    public string SurroundMovePlate(){
        string ans = "";
        Game sc = controller.GetComponent<Game>();
        ans += PointMovePlate(xBoard, yBoard + 1, true);
        ans += PointMovePlate(xBoard, yBoard - 1, true);
        ans += PointMovePlate(xBoard + 1, yBoard + 1, true);
        ans += PointMovePlate(xBoard + 1, yBoard - 1, true);
        ans += PointMovePlate(xBoard + 1, yBoard, true);
        ans += PointMovePlate(xBoard - 1, yBoard + 1, true);
        ans += PointMovePlate(xBoard - 1, yBoard - 1, true);
        ans += PointMovePlate(xBoard - 1, yBoard, true);

        if(xBoard == 4 && yBoard == 0 && this.name[0] == 'W' && sc.validKCastle()){ //castling
            ans += PointMovePlate(xBoard + 2, yBoard, true, 1) == "" ? "" : "O-O ";
        }
        if(xBoard == 4 && yBoard == 0 && this.name[0] == 'W' && sc.validQCastle()){ //castling
            ans += PointMovePlate(xBoard - 2, yBoard, true, 2) == "" ? "" : "O-O-O ";
        }
        if(xBoard == 4 && yBoard == 7 && this.name[0] == 'B' && sc.validKCastle()){ //castling
            ans += PointMovePlate(xBoard + 2, yBoard, true, 3) == "" ? "" : "O-O ";
        }
        if(xBoard == 4 && yBoard == 7 && this.name[0] == 'B' && sc.validQCastle()){ //castling
            ans += PointMovePlate(xBoard - 2, yBoard, true, 4) == "" ? "" : "O-O-O ";
        }
        return ans;
    }

    public string PointMovePlate(int x, int y, bool isking = false, int castle = 0){
        Game sc = controller.GetComponent<Game>();
        string ans = "";
        if (this.name.Contains("king")) {
            Coords bad = sc.secretSquare();
            if (sc.squareAttacked(x, y, sc.currentPlayer) != "" || (x == bad.x && y == bad.y)) return ans; 
        }
        if(sc.PositionOnBoard(x,y)){ 
            GameObject cp = sc.GetPosition(x,y);
            if(cp == null){
                ans += MovePlateSpawn(x,y,castle);
            } else if(cp.GetComponent<Chessman>().player != player){
                ans += MovePlateAttackSpawn(x,y);
            }
        }
        return ans;
    }

    public string PawnMovePlate(int x, int y, bool shouldAttack, bool ispassant = false)
    {
        Game sc = controller.GetComponent<Game>();
        string ans = "";
        if (sc.PositionOnBoard(x, y))
        {
            if(ispassant){
                ans += MovePlateAttackSpawn(x, y, true);
                return ans;
            }
            if (sc.GetPosition(x, y) == null)
            {
                ans += MovePlateSpawn(x, y);
            }

            if (shouldAttack && sc.PositionOnBoard(x + 1, y) && sc.GetPosition(x + 1, y) != null && sc.GetPosition(x + 1, y).GetComponent<Chessman>().player != player)
            {
                ans += MovePlateAttackSpawn(x + 1, y);
            }

            if (shouldAttack && sc.PositionOnBoard(x - 1, y) && sc.GetPosition(x - 1, y) != null && sc.GetPosition(x - 1, y).GetComponent<Chessman>().player != player)
            {
                ans += MovePlateAttackSpawn(x - 1, y);
            }
        }
        return ans;
    }
    public void MousePieceSpawn(){
        //mousePiece = GameObject.FindGameObjectWithTag("MousePiece"); 
        GameObject mp = Instantiate(mousePiece, new Vector3(0, 0, -3.0f), Quaternion.identity);
        mp.GetComponent<MousePiece>().name = this.name;
        mp.GetComponent<MousePiece>().Activate();
        //mp.GetComponent<SpriteRenderer>().sortingOrder = 1;
        //(mp.GetComponent<MousePiece>().name);
        //mp.GetComponent<MousePiece>().setSprite("WQueen");
    }
    public string SpawnPlate(int matrixX, int matrixY, int castle, bool ispassant, bool attack) {
        Coords move = new Coords(matrixX, matrixY);
        Game sc = controller.GetComponent<Game>();
        if (sc.inCheck(sc.currentPlayer) && !sc.stopCheck(move) && !this.name.Contains("king")) {
            return "";
        }
        Coords before = new Coords(xBoard, yBoard);
        Coords after = new Coords(matrixX, matrixY);
        if (!sc.movePin(before, after)) return "";
        Coords newsquare = new Coords(matrixX, matrixY);
        if (!shouldSpawn) {
            return newsquare.spos + " ";
        }
        //Get the board value in order to convert to xy coords
        float x = matrixX;
        float y = matrixY;

        if (shouldSpawn) {
            sc.platepositions[matrixX, matrixY] = 1;
        }

        float mul = (controller.GetComponent<Game>().blackbrain == "user" && controller.GetComponent<Game>().whitebrain != "user") ? -1 : 1;
        Debug.Log("mul is " + mul);

        x *= 0.7f * mul;
        y *= 0.7f * mul;

        x += -2.45f * mul;
        y += -2.45f * mul;

        //Set actual unity values
        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);
        //mp.GetComponent<SpriteRenderer>().sortingOrder = 0;
        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
        mpScript.castleplate = castle;
        mpScript.passantplate = ispassant;
        if (attack) {
            mpScript.attack = true;
        }
        return newsquare.spos + " ";
    }
    
    public string MovePlateSpawn(int matrixX, int matrixY, int castle = 0)
    {
        return SpawnPlate(matrixX, matrixY, castle, false, false);
    }

    public string MovePlateAttackSpawn(int matrixX, int matrixY, bool ispassant = false)
    {
        return SpawnPlate(matrixX, matrixY, 0, ispassant, true);
    }
    void Update()
    {   
        if(!controller.GetComponent<Game>().pawnpromote && !(controller.GetComponent<Game>().mode == "setpiece")){  
            if(Input.GetMouseButtonDown(1) && isTapped) DestroyMovePlates();
            if(Input.GetMouseButtonDown(0) && tapping == 2){
                tapping = 3;
                //("tapping3");
            } 
            if(Input.GetMouseButtonUp(0) && tapping == 1){
                tapping = 2;
               // Debug.Log("tapping2");
            } 
            if(!controller.GetComponent<Game>().mouseOnPlate() && Input.GetMouseButtonUp(0) && isTapped)
                DestroyMovePlates(); 
            switch (this.name){
                case "Bpawn": 
                    if (yBoard == 0 ){

                    GameObject cp = controller.GetComponent<Game>().GetPosition(xBoard, yBoard);
                    cp.name = "Bqueen";
                    this.GetComponent<SpriteRenderer>().sprite = Bqueen; player = "black";

                    }
                    break;
            }
        }
        if(Input.GetKeyDown("space")){ //space
            //DestroyMousePiece(); 
        }
    }
}
