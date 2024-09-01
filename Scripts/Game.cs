using System.Collections;
using System.Collections.Generic; 
using System.Threading;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;  


public class Game : MonoBehaviour
{

    public bool dev;

    public bool canrequest;
    public bool sendingreq;
    public GameObject chesspiece;

    public GameObject movePlate;

    //Positions and team for each chesspiece
    private GameObject[,] positions = new GameObject[8, 8];
    public bool[,] pieceplatepositions = new bool[8, 8];
    private GameObject[] playerBlack = new GameObject[16];
    private GameObject[] playerWhite = new GameObject[16];

    public GameObject restartbutton;
    public GameObject undobutton;
    public GameObject redobutton;
    public GameObject homebutton;

    public AudioSource source;
    public AudioClip moveclip;
    public AudioClip captureclip;
    public AudioClip movecaptureclip;
    public AudioClip capturecaptureclip;
    public AudioClip promoteclip;
    public AudioClip promotecaptureclip;
    public AudioClip castleclip;
    public AudioClip castlecaptureclip;
    public AudioClip castlepromoteclip;
    public AudioClip winclip, checkclip, gameendclip;

    public AudioClip movepromoteclip;
    public AudioClip capturepromoteclip;
    public AudioClip promotepromoteclip;

    public string recentmove = "move";
    public string whitebrain = "", blackbrain = "";
    public GameObject readybutton;
    public GameObject menubutton;
    public GameObject [] promotes = new GameObject[6];
    public string currentPlayer = "white";
    public int [,] platepositions = new int [8, 8];

    public bool gameOver = false;
    public int enpassant = -1;
    public bool botpassant = false;
    public int actualpassantx = -1;
    public int actualpassanty = -1;
    public int fishhelp = 0;
    public int preyx = -1;
    public int preyy = -1;
    public int predatorx = -1;
    public int predatory = -1;
    public string curposition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
    public string metacurposition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
    public int savepositionw = 16;
    public int savepositionb = 16;
    public int moves = 0;
    public string goal = "moves";
    public string title = "Level Select";
    public List<string> undo = new List<string>();
    public List<string> redo = new List<string>();
    public List<string> allmoves = new List<string>();
    public List<string> redomoves = new List<string>();

    public List<int> undospent = new List<int>();
    public List<int> redospent = new List<int>();
    public int whitekings = 1;
    public bool pawnpromote = false;
    public bool setpiece = false;
    public int setpiecex = -1;
    public int setpiecey = -1;
    public int budget = 16;
    public int spent = 0;
    public bool actuallypromote = false;
    public string piecepromote = "Wqueen";
    public int levelnum = 0;
    public bool deleteLines = false;

    public string mode = "ingame"; 
    public bool pieceremoved = false;
    public string savepieces = "";
    public int savespent = 0;
    public int tutorialnum = 0;

    public string[,] levels  = {{"`", "1yyyyyyy/1yyyyyyy/1yyyyyyy/1yyyyyyy/1yyyyyyy/1yyyyyyy/1yyyyyyy/7K", "user", "user", "levels", "Level Select", "moves",""}, // w - - 0 1
    {"f", "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR", "user", "user", "ingame", "Sandbox Mode", "none",""},
    {"f", "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR", "user", "GPT", "ingame", "User vs GPT", "moves",""},
    {"f", "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR", "GPT", "user", "ingame", "GPT vs User", "none",""},
    {"f", "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR", "user", "fishGPT", "ingame", "user vs fishGPT", "moves",""},
    {"f", "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR", "user", "random", "ingame", "user vs Random", "moves",""},
    {"f", "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR", "fish", "fish", "ingame", "Stockfish vs Stockfish", "moves",""},
    {"f", "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR", "GPT", "GPT", "ingame", "GPT vs GPT", "moves",""},
    {"f", "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR", "fishGPT", "fishGPT", "ingame", "fishGPT vs fishGPT", "moves",""},
    {"f", "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR", "GPT", "gemini", "ingame", "GPT vs Gemini", "moves",""},
    {"f", "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR", "fishGPT", "fishgemini", "ingame", "fishGPT vs fishGemini", "moves",""},
    {"f", "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR", "fish", "random", "ingame", "Stockfish vs random", "moves",""},
    {"f", "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR", "fish", "fishGPT", "ingame", "Stockfish vs fishGPT", "moves",""},
    {"f", "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR", "random", "random", "ingame", "Random vs Random", "moves",""},
    {"f", "k7/5P2/8/8/8/8/4K3/8", "user", "user", "ingame", "Promote test", "none",""},
    {"f", "r1bq1b1r/pp1nkppp/4pn2/2ppP3/3P4/5N2/PPPN1PPP/R1BQKB1R", "user", "user", "ingame", "Weird pawn capture", "none",""},
    {"f", "7k/8/8/8/3p4/8/4P3/7K", "user", "user", "ingame", "White En passant", "none",""},
    {"f", "k6B/8/8/8/8/8/8/B6K", "user", "user", "ingame", "Weird bishops", "none",""},
    }; 

    public string charlist = "`1234567890-=qwetyuiop[]asdfghjkl;";

    public static int cool = 5;
    public static string [] medals;
    // Start is called before the first frame update
    void Start()
    {
        dev = true;
        if(Menu.Mmedals == null){
            medals =  new string[levels.Length]; 
            for(int i = 0; i < medals.Length; i++){
                medals[i] = "gray";
            }
            Menu.setMedals();
        }
        else{
            medals = Menu.Mmedals;
        }
        mode = "levels";
        whitebrain = Menu.whitebrain;
        blackbrain = Menu.blackbrain;
        fishhelp = Menu.fishhelp;
        //restartbutton = GameObject.FindGameObjectWithTag("RestartButton");
        readybutton = GameObject.FindGameObjectWithTag("ReadyButton");
        undobutton = GameObject.FindGameObjectWithTag("UndoButton");
        redobutton = GameObject.FindGameObjectWithTag("RedoButton");
        promotes[0] = GameObject.FindGameObjectWithTag("PromoteK");
        promotes[1] = GameObject.FindGameObjectWithTag("PromoteQ"); 
        promotes[2] = GameObject.FindGameObjectWithTag("PromoteR");
        promotes[3] = GameObject.FindGameObjectWithTag("PromoteB");
        promotes[4] = GameObject.FindGameObjectWithTag("PromoteN");
        promotes[5] = GameObject.FindGameObjectWithTag("PromoteP");
        restartbutton.GetComponent<Button>().onClick.AddListener(() => restart());
        undobutton.GetComponent<Button>().onClick.AddListener(Undo);
        redobutton.GetComponent<Button>().onClick.AddListener(Redo);
        readybutton.GetComponent<Button>().onClick.AddListener(ready);
        homebutton.GetComponent<Button>().onClick.AddListener(home);
        promotes[0].GetComponent<Button>().onClick.AddListener(() => setPromoteButton("Wking"));
        promotes[1].GetComponent<Button>().onClick.AddListener(() => setPromoteButton("Wqueen"));
        promotes[2].GetComponent<Button>().onClick.AddListener(() => setPromoteButton("Wrook"));
        promotes[3].GetComponent<Button>().onClick.AddListener(() => setPromoteButton("Wbishop"));
        promotes[4].GetComponent<Button>().onClick.AddListener(() => setPromoteButton("Wknight"));
        promotes[5].GetComponent<Button>().onClick.AddListener(() => setPromoteButton("Wpawn"));
        
        title = "Level Select";
        promoteVisibility(false);
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = true;
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = "No level selected";
        metacurposition = levels[0,1];
        setStringPosition(metacurposition);
        curposition = metacurposition;
        undo.Add(getStringPosition());
        hideMainButtons();

        for(int i = 0; i < levels.GetLength(0); i++){
            levels[i,0] = charlist[i] + "";
        }
        makeLevel(1);
    }

    public GameObject Create(string name, int x, int y){
        GameObject obj = Instantiate(chesspiece, new Vector3(0, 0, -1), Quaternion.identity);
        Chessman cm = obj.GetComponent<Chessman>(); //We have access to the GameObject, we need the script
        cm.name = name; //This is a built in variable that Unity has, so we did not have to declare it before
        cm.SetXBoard(x);
        cm.SetYBoard(y);
        cm.Activate(); //It has everything set up so it can now Activate()
        return obj;
    }
    public void setPromoteButton(string name){
        if(setpiece){
            piecepromote = name;
            setpiece = false;
            GameObject[] playerWhite2 = new GameObject[playerWhite.Length + 1];
            if(name == "Wpawn") spent += 1;
            if(name == "Wknight") spent += 3;
            if(name == "Wbishop") spent += 3;
            if(name == "Wrook") spent += 5;
            if(name == "Wqueen") spent += 9;
            undospent.Add(spent);
            redospent.Clear();
            playerWhite2[playerWhite.Length] = Create(name, setpiecex, setpiecey);
            for(int i = 0; i < playerWhite.Length; i++){
                playerWhite2[i] = playerWhite[i];
            }
            playerWhite = playerWhite2;
            SetPosition(playerWhite[playerWhite.Length - 1]);
            GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate"); 
            for(int i = 0; i < movePlates.Length; i++){
                if(movePlates[i].GetComponent<MovePlate>().ispressed == true)
                    movePlates[i].GetComponent<MovePlate>().ispressed = false;
            }
            pieceremoved = false;
            restartbutton.SetActive(true);
            readybutton.SetActive(true);
            //undobutton.SetActive(true);
            //redobutton.SetActive(true);
            playSound("promote");

            //pieceplatepositions[setpiecex, setpiecey] = false;
            undo.Add(getStringPosition());
            redo.Clear();
        }
        else{
            if(currentPlayer == "white")
                piecepromote =  "W" + name.Substring(1);
            else
                piecepromote =  "B" + name.Substring(1);
            pawnpromote = false;
            actuallypromote = true;
        }
    }
    public void SetPosition(GameObject obj){
        Chessman cm = obj.GetComponent<Chessman>();

        //Overwrites either empty space or whatever was there
        positions[cm.GetXBoard(), cm.GetYBoard()] = obj;
    }
    public void SetPositionEmpty(int x, int y){
        positions[x,y] = null;
    }
    public GameObject GetPosition(int x, int y){
        return positions[x,y];
    }
    public bool PositionOnBoard(int x, int y){
        if(x < 0 || y < 0 || x >= positions.GetLength(0) || y >= positions.GetLength(1)) return false;
        return true; 
    }

    public string GetCurrentPlayer(){
        return currentPlayer;
    }

    public bool IsGameOver(){
        return gameOver;
    }

    public void NextTurn(){
        if(mode != "levels"){
            restartbutton.SetActive(true);
            undobutton.SetActive(true);
            redobutton.SetActive(true);
        }

        playSound(recentmove);
        moves++;  
        if(currentPlayer == "white"){
            currentPlayer = "black";
        } else{
            currentPlayer = "white";
        }
        if(mode == "sandbox" || mode == "ingame"){
            if(numPiece("Wking") == 0)
                Winner("black");

            if (nomoves(currentPlayer)) {
                if (inCheck(currentPlayer)) {
                    Winner(currentPlayer == "white" ? "black": "white");
                } else {
                    Winner("stalemate");
                }
            }
            if (insufficientMaterial()) {
                Winner("insufficient material");
            }   
            if (halfMoves() >= 100) {
                Winner("the 50 move rule");
            }   
        }
        canrequest = !gameOver;
        undo.Add(getStringPosition());
        redo.Clear();

    }
    public string removeX(string s){
        string s2 = "";
        for(int i = 0; i < s.Length; i++){
            if(s[i] == 'x')
                s2 += "1";
            else
                s2 += s[i];
        }
        return renderStringPosition(s2);
    }
    public int pawns(string name){
        if(name[1] == 'p') return 1;
        if(name[2] == 'n') return 3;
        if(name[1] == 'b') return 3;
        if(name[1] == 'r') return 5;
        if(name[1] == 'q') return 9;
        return 0;
    }
    public bool breakHalfMove(string move) {
        return (move.Contains("x") || char.IsLower(move[0]));
    }
    public int halfMoves() {
        int cnt = 0;
        for (int i = allmoves.Count - 1; i >= 0; i--) {
            if (breakHalfMove(allmoves[i])) {
                break;
            } else {
                cnt++;
            }
        }
        return cnt;
    }
    public Dictionary<string, int> allPieces() {
        Dictionary<string, int> allpieces = new Dictionary<string, int>();
        allpieces[""] = 0; 
        allpieces["K"] = 0; 
        allpieces["N"] = 0; 
        allpieces["R"] = 0; 
        allpieces["Q"] = 0; 
        allpieces["B"] = 0;
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                string s = getLetterFromPiece(getPiece(i, j)); 
                if (s == "Null") continue;
                allpieces[s]++;
            }
        }
        return allpieces;
    }
    public bool insufficientMaterial() {
        Dictionary<string, int> allpieces = allPieces();
        if (allpieces["R"] == 0 && allpieces["Q"] == 0 && allpieces[""] == 0) {
            return (allpieces["B"] < 2 && allpieces["N"] == 0) || (allpieces["N"] < 3 && allpieces["B"] == 0);
        }
        return false;
    }
    public bool threeFold() {
        return undo.GroupBy(s => s).Any(g => g.Count() >= 3);
    }
    public string getPieceFromLetter(string name) {
        if('a' <= name[0] && name[0] <= 'z') return "pawn";
        if(name[0] == 'P') return "pawn";
        if(name[0] == 'N') return "knight";
        if(name[0] == 'B') return "bishop";
        if(name[0] == 'R') return "rook";
        if(name[0] == 'Q') return "queen";
        return "king";
    }
    public string getLetterFromPiece(string name) {
        if(name.Contains("pawn")) return "";
        if(name.Contains("knight")) return "N";
        if(name.Contains("bishop")) return "B";
        if(name.Contains("rook")) return "R";
        if(name.Contains("queen")) return "Q";
        return "K";
    }
    public char oppositeColorL(char c) {
        return c == 'W' ? 'B' : 'W';
    }
    public string oppositeColor(string s) {
        return s == "white" ? "black" : "white";
    }
    public char curCL() { // spits out 'W' or 'B'
        return char.ToUpper(currentPlayer[0]);
    }
    public string getOriginalPiece(Coords square, string curmove) {
        curmove = curmove.Replace("x", "");
        int mul = currentPlayer == "white" ? -1 : 1;
        int y = currentPlayer == "white" ? 3 : 4;
        Debug.Log("weegfsregs " + (square.x - 1) + " " + (7 - y) + " " + getPiece(square.x - 1, 7 - y) + " " + enpassant);
        string anyattack = squareAttacked(square.x, square.y, oppositeColor(currentPlayer));
        if (curmove[0] == 'P') {
            if (getPiece(square.x, square.y + 1 * mul) == (curCL() + "pawn") && positions[square.x, square.y] == null) {
                anyattack = ("P" + (char)(square.x + 'a') + (square.y + 1 + mul) + " ");
            } else if (getPiece(square.x, square.y + 2 * mul) == (curCL() + "pawn") && square.y == y && positions[square.x, square.y] == null) {
                anyattack = ("P" + (char)(square.x + 'a') + (square.y + 1 + mul * 2) + " ");
            } else if (enpassant == square.x && (getPiece(square.x + 1, 7 - y) == curCL() + "pawn")) {
                anyattack = ("P" + (char)(square.x + 'a' + 1)+ "" + (8 - y) + " ");
                botpassant = true;
            } else if (enpassant == square.x && getPiece(square.x - 1, 7 - y) == curCL() + "pawn") {
                anyattack = ("P" + (char)(square.x + 'a' - 1) + "" + (8 - y) + " ");
                botpassant = true;
                Debug.Log("Here and " + anyattack);
            } else if (getPiece(square.x, square.y)[0] != oppositeColorL(char.ToUpper(currentPlayer[0]))) {
                return "";
            }
        }


        string shouldhave = curmove.Substring(0, curmove.IndexOf(square.spos));
        string[] allattacks = anyattack.Split(' ');
        foreach (string move in allattacks) {
            bool good = true;
            foreach (char c in shouldhave) {
                if (!move.Contains(shouldhave)) {
                    good = false;
                } 
            }
            if (good) {
                return move;
            }
        }
        return "";
    }
    public void removePiece(int x, int y){
        if(positions[x,y] != null){
            if(mode == "setpiece"){
                spent -= pawns(positions[x,y].GetComponent<Chessman>().name);
            }
            Destroy(positions[x,y]);
            pieceremoved = true;
        }
    }
    public void spawnPiece(string name, int x, int y) {
        GameObject[] playerWhite2 = new GameObject[playerWhite.Length + 1];
        playerWhite2[playerWhite.Length] = Create(name, x, y);
        for(int i = 0; i < playerWhite.Length; i++){
            playerWhite2[i] = playerWhite[i];
        }
        playerWhite = playerWhite2;
        SetPosition(playerWhite[playerWhite.Length - 1]);
    }

    public bool validKCastle() {
        int y = currentPlayer == "white" ? 0 : 7;
        char k = currentPlayer == "white" ? 'K' : 'k';
        char r = currentPlayer == "white" ? 'R' : 'r';
        return (squareAttacked(4, y, currentPlayer) + squareAttacked(5, y, currentPlayer) + squareAttacked(6, y, currentPlayer) == "")
            && Nevermoved(k, 4, y) && Nevermoved(r, 7, y) && positions[5,y] == null && positions[6,y] == null;
    }

    public bool validQCastle() {
        int y = currentPlayer == "white" ? 0 : 7;
        char k = currentPlayer == "white" ? 'K' : 'k';
        char r = currentPlayer == "white" ? 'R' : 'r';
        return (squareAttacked(2, y, currentPlayer) + squareAttacked(3, y, currentPlayer) + squareAttacked(4, y, currentPlayer) == "")
            && Nevermoved(k, 4, y) && Nevermoved(r, 0, y) && positions[1,7] == null && positions[2,y] == null && positions[3,y] == null;
    }

    public string blockSquares(Coords before, Coords after) {
        string block = "";
        int dx = after.x - before.x;
        dx = dx < 0 ? -1 : (dx == 0 ? 0 : 1);
        int dy = after.y - before.y;
        dy = dy < 0 ? -1 : (dy == 0 ? 0 : 1);
        int startx = before.x + dx;
        int starty = before.y + dy;
        while (startx != after.x || starty != after.y) { 
            Coords temp = new Coords(startx, starty);
            block += temp.spos + " ";
            startx += dx;
            starty += dy;
        }
        return block; 
    }

    public bool inCheck(string color) { // color of the king in check
        return squareAttacked(findKing(color).x, findKing(color).y, color) != "";
    }
    public bool stopCheck(Coords move) { // piece is uppercase, this func is for pieces that aren't kings
        string[] allattacks = squareAttacked(findKing(currentPlayer).x, findKing(currentPlayer).y, currentPlayer).Split(' ');
        if (allattacks.Length > 2) return false;
        string predator = allattacks[0];
        Coords before = new Coords(predator.Substring(predator.Length - 2));
        string blocks = before.spos + " ";
        if (predator[0] != 'N' && predator[0] != 'P') {
            blocks += blockSquares(before, findKing(currentPlayer));
        }
        return blocks.Contains(move.spos);
    }
    public Coords secretSquare() {
        Coords kingpos = findKing(currentPlayer);
        string[] allattacks = squareAttacked(kingpos.x, kingpos.y, currentPlayer).Split(' ');
        string validAttack = "";
        Coords before = new Coords(-1, -1);
        Coords after = new Coords(kingpos.x, kingpos.y);
        for (int i = 0; i < allattacks.Length; i++) {
            if (allattacks[i].Contains('Q') || allattacks[i].Contains('B') || allattacks[i].Contains('R')) {
                validAttack = allattacks[i];
            }
        }
        if (validAttack == "") return before;
        before.setCoords(validAttack.Substring(validAttack.Length - 2));
        int dx = after.x - before.x;
        dx = dx < 0 ? -1 : (dx == 0 ? 0 : 1);
        int dy = after.y - before.y;
        dy = dy < 0 ? -1 : (dy == 0 ? 0 : 1);
        return new Coords(after.x + dx, after.y + dy);
    }

    public string randomMove(string color, bool specific) {
        GameObject[] pieces = playerWhite;
        if (color == "black") pieces = playerBlack;

        int r = Random.Range(0, pieces.Length);
        if (pieces[r] == null) {
            return randomMove(color, specific);
        }
        Chessman m = pieces[r].GetComponent<Chessman>();
        m.shouldSpawn = false;
        string moves = m.InitiateMovePlates();
        if (moves == "") {
            return randomMove(color, specific);
        }
        string[] movesarr = moves.Split(' ');
        r = Random.Range(0, movesarr.Length - 1);
        if (movesarr[r].Contains("O")) {
            return movesarr[r];
        }
        Coords after = new Coords(movesarr[r]);
        Coords before = new Coords(m.xBoard, m.yBoard);
        if (specific)
            return getLetterFromPiece(m.name) + before.spos + movesarr[r];
        if (getLetterFromPiece(m.name) == "" && before.x != after.x) {
            return before.spos[0] + "x" + after.spos;
        }
        return getLetterFromPiece(m.name) + movesarr[r];
    }
 
    public void moveBot(string[] moves, int index, bool stockfish = false) {
        if (gameOver) return;
        if (index > 110 || (index > 1 && stockfish)) {
            if (stockfish) fishReq();
            return;
        }
        if (nomoves(currentPlayer)) {
            if (inCheck(currentPlayer)) {
                Winner(currentPlayer == "white" ? "black": "white");
            } else {
                Winner("stalemate");
            }
            return;
        }
        botpassant = false;
        Debug.Log((stockfish ? "Stockfish" : "GPT") + " is attempting move #" + (index + 1) + ", which is " + (index < moves.Length ? moves[index] : "idk"));
        string curmove = "";
        int rookx = -1;
        int rookx2 = -1;
        int rooky = currentPlayer == "white" ? 0 : 7;
        string piece = "";
        Coords before = new Coords(-1, -1);
        Coords after = new Coords(-1, -1);
        if (stockfish) {
            string temp = moves[0];
            before.setCoords(temp.Substring(0, 2));
            after.setCoords(temp.Substring(2, 2));
            piece = getPiece(before.x, before.y);
            curmove = getLetterFromPiece(piece) + after.spos;
            if (index == 1)
                curmove = getLetterFromPiece(piece) + before.spos + after.spos;
            if (getLetterFromPiece(piece) == "K" && (temp == "e8g8" || temp == "e1g1")) {
                curmove = "O-O";
            } else if (getLetterFromPiece(piece) == "K" && (temp == "e8c8" || temp == "e1c1")) {
                curmove = "O-O-O";
            }

        } else if (index >= moves.Length) {
            Debug.Log("Invalid moves, generating random move");
            curmove = randomMove(currentPlayer, index > 101);
            Debug.Log("random move is " + curmove);
        } else {
            curmove = moves[index];
        }
        if ((curmove == "O-O" && !validKCastle()) || (curmove == "O-O-O" && !validQCastle()) || curmove == "") {
            moveBot(moves, index + 1, stockfish);
            return; 
        }
        Coords otherking = findKing(currentPlayer == "white" ? "black" : "white");
        if (curmove == "O-O") {
            rookx = 7; rookx2 = 5;
            before.setCoords(currentPlayer == "white" ? "e1" : "e8");
            after.setCoords(currentPlayer == "white" ? "g1" : "g8");
        }
        if (curmove == "O-O-O") {
            rookx = 0; rookx2 = 3;
            before.setCoords(currentPlayer == "white" ? "e1" : "e8");
            after.setCoords(currentPlayer == "white" ? "c1" : "c8");
        }


        if (rookx == -1 && squareAttacked(otherking.x, otherking.y, currentPlayer == "white" ? "black" : "white") != "") { // other player in check
            string kingpred = squareAttacked(otherking.x, otherking.y, currentPlayer == "white" ? "black" : "white");
            string firstpred = kingpred.Split("")[0];
            firstpred = firstpred.Replace(" ", "");
            before.setCoords(firstpred.Substring(firstpred.Length - 2));
            Debug.Log(before);
            after.setCoords(otherking.x, otherking.y);
            Debug.Log(after); 
            curmove = firstpred[0] + "x" + firstpred.Substring(firstpred.Length - 2) + "#";
            piece = getPieceFromLetter(curmove);
        } else if (rookx == -1){
            piece = getPieceFromLetter(curmove);
            after.setCoords(curmove.Substring(curmove.Length - 2));
            string olds = getOriginalPiece(after, piece == "pawn" ? 'P' + curmove : curmove);
            if (olds == "" || getPiece(after.x, after.y)[0] == char.ToUpper(currentPlayer[0])) {
                moveBot(moves, index + 1, stockfish);
                return;
            }
            before.setCoords(olds.Substring(olds.Length - 2));
        }
        if (PositionOnBoard(otherking.x, otherking.y) && positions[otherking.x, otherking.y] != null)
            positions[otherking.x, otherking.y].GetComponent<Chessman>().DestroyMovePlates();

        Debug.Log("Before is " + before);
        Chessman m = positions[before.x, before.y].GetComponent<Chessman>();
        m.shouldSpawn = false;
        string[] move = m.InitiateMovePlates().Split(' ');
        Debug.Log("allmoves is " + m.InitiateMovePlates());
        bool valid = false;
        foreach (string s in move) {
            string notation = (getLetterFromPiece(m.name) + s); 
            // Debug.Log(notation + " " + after.spos);
            if (notation.Contains(after.spos) || (curmove == "O-O" && s == "O-O") || (curmove == "O-O-O" && s == "O-O-O")) {
                valid = true;
            }
        }
        if (!valid) {
            moveBot(moves, index + 1, stockfish);
            return;
        }

        if (rookx != -1) { // castle
            piece = "king";
            GameObject reference2 = GetPosition(rookx, rooky);
            SetPositionEmpty(rookx,rooky);
            reference2.GetComponent<Chessman>().SetXBoard(rookx2);
            reference2.GetComponent<Chessman>().SetYBoard(rooky);
            reference2.GetComponent<Chessman>().SetCoords(); 
            SetPosition(reference2);
        }
        

        if (rookx != -1) {
            recentmove = "castle";
        } else if (positions[after.x, after.y] || botpassant) {
            if (botpassant) {
                removePiece(after.x, currentPlayer == "white" ? 4 : 3);
                positions[after.x, currentPlayer == "white" ? 4 : 3] = null;
            }
            else {
                removePiece(after.x, after.y);
                positions[after.x, after.y] = null;
            }
            recentmove = "capture";
            if (!curmove.Contains("x")) {
                if (piece == "pawn" && curmove.Length == 2) {
                    curmove = before.spos[0] + "x" + curmove;
                } else {
                    curmove = curmove.Insert(1, "x");
                }
            }
                
        } else {
            recentmove = "move";
        }

        if (positions[before.x, before.y] && getPiece(before.x, before.y) == char.ToUpper(currentPlayer[0]) + piece.ToLower()) {
            GameObject reference = positions[before.x, before.y];
            reference.GetComponent<Chessman>().SetXBoard(after.x);
            reference.GetComponent<Chessman>().SetYBoard(after.y);
            reference.GetComponent<Chessman>().SetCoords();
            positions[before.x, before.y] = null;
            SetPosition(reference);
        }
        else {
            spawnPiece(char.ToUpper(currentPlayer[0]) + piece.ToLower(), after.x, after.y);
        }
        
        GameObject r = positions[after.x, after.y];
        if (piece == "pawn" && before.y == 1 && after.y == 3) {
            enpassant = after.x;
        } else if(piece == "pawn" && before.y == 6 && after.y == 4) {
            enpassant = after.x;
        } else {
            enpassant = -1;
        }


        string str = "";
        if(r.GetComponent<Chessman>().name[2] == 'n') str += r.GetComponent<Chessman>().name[2].ToString().ToUpper();
        else str += r.GetComponent<Chessman>().name[1].ToString().ToUpper();

        if(r.GetComponent<Chessman>().name[1] == 'p') str = "";
        if (enpassant != -1) allmoves.Add(curmove + "!!");
        else allmoves.Add(curmove);
        redomoves.Clear();

        if (r.GetComponent<Chessman>().name.Contains("pawn") && after.y % 7 == 0){
            recentmove = "promote";
            r.GetComponent<Chessman>().name = char.ToUpper(currentPlayer[0]) + "queen";
            r.GetComponent<Chessman>().setSprite(char.ToUpper(currentPlayer[0]) + "queen");
        } 

        Coords king = findKing(currentPlayer == "white" ? "black" : "white");
        if (squareAttacked(king.x, king.y, currentPlayer == "white" ? "black" : "white") != "") {
            allmoves[allmoves.Count - 1] += "+";
            recentmove = "check";
        }
        NextTurn();
    }
    public void randomReq() {
        moveBot(new string[] {"bruh"}, 100);
    }
    public void sendReq(string model) {
            sendingreq = true;
            Debug.Log("Trying to get request");
            GameObject.FindGameObjectWithTag("desctext").GetComponent<Text>().text = "Attempting to get " + model + " request";
            string position = getStringPosition();
            string prev = renderMoves();
            // Thread.Sleep(2000);
            string check = inCheck(currentPlayer) ? ". Remember, you are in check, and sometimes you can capture the piece checking you." : "";
            string jsonString = $"{{\"fen\":\"{position}\",\"color\":\"{currentPlayer}\",\"prev\":\"{prev}\",\"model\":\"{model}\"}}";
            Debug.Log(jsonString);
            StartCoroutine(PostRequest("https://monkey2.azurewebsites.net/submit", jsonString));
    }

    public void fishReq() {
        sendingreq = true;
        Debug.Log("Trying to get stockfish request");
        GameObject.FindGameObjectWithTag("desctext").GetComponent<Text>().text = "Attempting to get Stockfish request";
        string fen = getFEN();
        int depth = 11;
        StartCoroutine(GetRequest($"https://stockfish.online/api/s/v2.php?fen={fen}&depth={depth}"));
    }

    IEnumerator GetRequest(string url)
    {
        // Create a UnityWebRequest for the GET request
        UnityWebRequest request = UnityWebRequest.Get(url);

        // Send the request and wait for a response
        yield return request.SendWebRequest();

        // Check if the request encountered any errors
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
            fishReq();
            yield return null;
        }
        else
        {
            // Request was successful, process the response
            string jsonString = request.downloadHandler.text;
            Debug.Log("Response: " + jsonString);
            if (jsonString == "") {
                fishReq();
                yield return null;
            }
            JObject jsonObject = JObject.Parse(jsonString);
            string s = jsonObject["bestmove"].ToString();
            string ans = s.Split(' ')[1];
            Debug.Log("Bestmove is " + ans);
            sendingreq = false;
            moveBot(new string[]{ans}, 0, true);
        }
    }
    IEnumerator PostRequest(string url, string jsonData)
    {
        // Create a UnityWebRequest for the POST request
        UnityWebRequest request = new UnityWebRequest(url, "POST");

        // Convert JSON data to a byte array
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Set the content type to application/json
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request and wait for a response
        yield return request.SendWebRequest();

        // Check if the request encountered any errors
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
            yield return null;
        }
        else
        {
            // Request was successful, process the response
            string message = request.downloadHandler.text;
            Debug.Log("Message: " + message);
            string[] processed = processText(message);
            Debug.Log("Response: " + string.Join(" ", processed));
            sendingreq = false;
            moveBot(processed, 0);
        }
    }

    public string getNotation(int x, int y) {
        return (char)('a' + x) + "" + (y + 1);
    }
    public string nameToletter(string name) {
        if (name[2] == 'n') return "N"; 
        return name[1].ToString().ToUpper();
    }
    static string[] ExtractBoldText(string input)
    {

        string pattern = @"\*\*(.*?)\*\*";
        MatchCollection matches = Regex.Matches(input, pattern);
        string[] boldTexts = new string[matches.Count];
        for (int i = 0; i < matches.Count; i++)
        {
            string s = matches[i].Groups[1].Value;
            if (s.Contains("...")) {
                 int index = s.IndexOf("...");
                 s = s.Substring(index + 3);
            }
            s = s.Replace(" ", "");
            s = s.Replace("+", "");
            boldTexts[i] = s;
        }

        return boldTexts;
    }
    static string[] processText(string input) {
        string [] sarr = input.Split('\n');
        for (int i = 0; i < sarr.Length; i++) {
            string s = sarr[i];
            if (s.Contains(".")) {
                 int index = s.LastIndexOf(".");
                 s = s.Substring(index + 1);
            }
            s = s.Replace(" ", "");
            s = s.Replace("+", "");
            s = s.Replace("#", "");
            sarr[i] = s;
        }
        return sarr;
    }

    public string renderMoves() {
        string total = "";
        int a = 0;
        foreach (string s in allmoves) {
            if (a % 2 == 0) {
                total += (a / 2 + 1) + ". ";
            }
            total += s.Replace("!", "") + " ";
            a++;
        }
        return total;
    }

    public Coords findKing(string color) {
        if (color == "white") {
            foreach (GameObject o in playerWhite) {
                if (o == null) continue;
                if (o.GetComponent<Chessman>().name == "Wking") {
                    int x = o.GetComponent<Chessman>().xBoard;
                    int y = o.GetComponent<Chessman>().yBoard;
                    return new Coords (x, y);
                }
            } 
        } else {
            foreach (GameObject o in playerBlack) {
                if (o == null) continue;
                if (o.GetComponent<Chessman>().name == "Bking") {
                    int x = o.GetComponent<Chessman>().xBoard;
                    int y = o.GetComponent<Chessman>().yBoard;
                    return new Coords (x, y);
                }
            } 
        }
        return new Coords(-1, -1);
    }
    public Coords isPinned(Coords piece) { // returns the coordinates of the velocity
        Coords ans = new Coords(-100, -100);
        Coords king = findKing(currentPlayer);
        if (piece.Equals(king)) return ans;
        int dx = piece.x - king.x;
        int dy = piece.y - king.y;
        if (dx == 0) {
            dy /= Mathf.Abs(dy);
        } else if (dy == 0) {
            dx /= Mathf.Abs(dx);
        } else if (Mathf.Abs(dx) == Mathf.Abs(dy)) {
            dx /= Mathf.Abs(dx);
            dy /= Mathf.Abs(dy);
        } else {
            return ans;
        }
        int startx = king.x + dx;
        int starty = king.y + dy;
        bool noblocks = true;
        while ((startx != piece.x || starty != piece.y)) { // sees if piece is inline with king 
            if (positions[startx, starty] != null) {
                noblocks = false;
            }
            startx += dx;
            starty += dy;
        }
        if (!noblocks) return ans;
        bool isPinned = false;
        startx += dx;
        starty += dy;
        while (PositionOnBoard(startx, starty)) {
            if (!positions[startx, starty]) {
                startx += dx;
                starty += dy;
                continue;
            }
            string name = getPiece(startx, starty);
            if (char.ToLower(name[0]) == currentPlayer[0]) {
                break;
            }
            if ((name.Contains("rook") && dx * dy == 0) || name.Contains("queen") || (dx * dy != 0 && name.Contains("bishop"))) {
                isPinned = true; 
                break;
            } else {
                break;
            }
        }
        if (!isPinned) {
            return ans;
        }
        return new Coords(dx, dy);

    }
    public bool movePin(Coords before, Coords after) {
        Coords pinned = isPinned(before);
        bool canmove = true;
        if (pinned.x != -100) {
            int movedx = after.x - before.x;
            int movedy = after.y - before.y;
            if (movedx == 0) {
                movedy /= Mathf.Abs(movedy);
            } else if (movedy == 0) {
                movedx /= Mathf.Abs(movedx);
            } else if (Mathf.Abs(movedx) == Mathf.Abs(movedy)) {
                movedx /= Mathf.Abs(movedx);
                movedy /= Mathf.Abs(movedy);
            } else {
                canmove = false;
            }
            if (canmove) {
                canmove = ((movedx == pinned.x && movedy == pinned.y) || (movedx == -pinned.x && movedy == -pinned.y));
            }
        }
        return canmove;
    }
    public void ready(){
        if(mode == "setpiece"){
            savepieces = getStringPosition();
            pawnpromote = false;
            gameOver = false;
            deleteLines = true;
            mode = "ingame";
            undobutton.SetActive(true);
            redobutton.SetActive(true);
            Debug.Log(undo[undo.Count-1]);
            Debug.Log(removeX(undo[undo.Count-1]));
            curposition = removeX(undo[undo.Count-1]);
            moves = 0;
            undo.Clear();
            redo.Clear();
            currentPlayer = "white";   
            undo.Clear();
            undo.Add(curposition);
            GameObject.Find("ReadyButton").GetComponentInChildren<Text>().text = "Resetup";
            GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate"); 
            for(int i = 0; i < movePlates.Length; i++){
                    Destroy(movePlates[i]);
            }
            pieceplatepositions = new bool[8, 8];
            savespent = spent;
        }
        else{ //reset
            curposition = savepieces;
            spent = savespent;
            GameObject.Find("ReadyButton").GetComponentInChildren<Text>().text = "Ready";
            mode = "setpiece";
            restart(true);   
        }
    }
    public void restart(bool saveit = false){
        canrequest = true;
        sendingreq = false;
        DestroyAllPlates(); 
        undospent.Clear();
        redospent.Clear();
        allmoves.Clear();
        redomoves.Clear();
        pawnpromote = false;
        gameOver = false;
        deleteLines = true;
        setpiece = false;
        promoteVisibility(false);
        pawnpromote = false;
        restartbutton.SetActive(true);
        undo.Clear();
        redo.Clear();
        currentPlayer = "white";   
        if(mode == "levels"){
            hideMainButtons();
        }else{
            showMainButtons();
        }
        
        if(mode == "ingame"){
            undobutton.SetActive(true);
            redobutton.SetActive(true);
        }
        else{
            if(saveit){
                spent = savespent;
                undospent.Add(spent);
            }
            else
                spent = 0;
        }
        moves = 0;
        if(mode != "levels")
            GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = false;
        else{
            GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = true;
            GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = "No level selected";
        }
        GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().enabled = false;
        if(mode == "ingame" || saveit)
            setStringPosition(curposition);
        else{
            curposition = metacurposition;
            setStringPosition(metacurposition);
        }
        undo.Add(curposition);
            
    }


    public void DestroyAllPlates(){
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate"); 
        for(int i = 0; i < movePlates.Length; i++){
                Destroy(movePlates[i]);
        }
    }
    public char pieceFromString(string pos, int px, int py){
        int x = 0;
        int y = 7;
        for(int i = 0; i < pos.Length; i++){
            //Debug.Log(x + " " + y);
            if(x == px && y == py){
                return pos[i];
            }
            if(pos[i] - '0' >= 0 && pos[i] - '0' <= 7){
                int num = pos[i] - '0';
                x+=num;
                continue;
            }
            if(pos[i] == '/'){
                x = 0;
                y--;
                continue;
            }
            x++;
        }
        return 'f';
    }
    public bool Nevermoved(char piece, int px, int py){
        bool nevermoved = true;
        for(int i = 0; i < undo.Count; i++){
            nevermoved = nevermoved && pieceFromString(undo[i], px, py) == piece;
        }
        return nevermoved;
    }
    public void setStringPosition(string position){
        int lenw = 0;
        int lenb = 0;  
        for(int i = 0; i < position.Length; i++){
            if(position[i] <= 'Z' && position[i] >= 'A')
                lenw++;
            if(position[i] <= 'z' && position[i] >= 'a' && position[i] != 'x' && position[i] != 'y') //x means pieceselect, y means level
                lenb++;
        }

        enpassant = -1;
        actualpassantx = -1;
        actualpassanty = -1;

        pieceplatepositions = new bool[8, 8];
        DestroyAllPlates();
        

        for(int i = 0; i < playerWhite.Length; i++){
            Destroy(playerWhite[i]);
        }
        for(int i = 0; i < playerBlack.Length; i++){
            Destroy(playerBlack[i]);
        }
        playerWhite = new GameObject[lenw];
        playerBlack = new GameObject[lenb];

        int x = 0;
        int y = 7;
        int whiteindex = 0;
        int blackindex = 0;

        for(int i = 0; i < position.Length; i++){
            if(position[i] - '0' >= 0 && position[i] - '0' <= 7){
                int num = position[i] - '0';
                x+=num;
                continue;
            }
            if(position[i] == '/'){
                x = 0;
                y--;
                continue;
            }
            if(position[i] == 'k')
                playerBlack[blackindex++] = Create("Bking", x, y);
            else if(position[i] == 'q')
                playerBlack[blackindex++] = Create("Bqueen", x, y);
            else if(position[i] == 'r')
                playerBlack[blackindex++] = Create("Brook", x, y);
            else if(position[i] == 'b')
                playerBlack[blackindex++] = Create("Bbishop", x, y);
            else if(position[i] == 'n')
                playerBlack[blackindex++] = Create("Bknight", x, y);
            else if(position[i] == 'p')
                playerBlack[blackindex++] = Create("Bpawn", x, y);
            else if(position[i] == 'K')
                playerWhite[whiteindex++] = Create("Wking", x, y);
            else if(position[i] == 'Q')
                playerWhite[whiteindex++] = Create("Wqueen", x, y);
            else if(position[i] == 'R')
                playerWhite[whiteindex++] = Create("Wrook", x, y);
            else if(position[i] == 'B')
                playerWhite[whiteindex++] = Create("Wbishop", x, y);
            else if(position[i] == 'N')
                playerWhite[whiteindex++] = Create("Wknight", x, y);
            else if(position[i] == 'P')
                playerWhite[whiteindex++] = Create("Wpawn", x, y);
            x++;
        }
          x = 0;
         y = 7;
         for(int i = 0; i < metacurposition.Length && (mode == "setpiece" || mode == "levels"); i++){
            if(metacurposition[i] - '0' >= 0 && metacurposition[i] - '0' <= 7){
                int num = metacurposition[i] - '0';
                x+=num;
                continue;
            }
            else if(metacurposition[i] == '/'){
                x = 0;
                y--;
                continue;
            }else if((metacurposition[i] != 'x' && metacurposition[i] != 'y') || (metacurposition[i] == 'y' && levels.GetLength(0) <= (7-y)*7 + x)){
                x++;
                continue;
            }
            if (!dev && metacurposition[i] == 'y' && (7-y)*7 + x > completedLevels() * 2 + 1) {
                break;   
            }
            if((metacurposition[i] == 'x' || metacurposition[i] == 'y') && !pieceplatepositions[x,y]){
                if(metacurposition[i] == 'x')
                    setPlate("pieceplate", x, y);
                else
                    setPlate("levelplate", x, y);
                x++;
                continue;
            }

            x++;
        }
        for(int i = 0; i < playerBlack.Length; i++){
            SetPosition(playerBlack[i]);
        } 
        for(int i = 0; i < playerWhite.Length; i++){
            SetPosition(playerWhite[i]);
        }
    }
    public void cancelPiecePlate(){
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate"); 
        for(int i = 0; i < movePlates.Length; i++){
            if(movePlates[i].GetComponent<MovePlate>().ispressed == true)
                movePlates[i].GetComponent<MovePlate>().ispressed = false;
        }
        if(pieceremoved){
            undo.Add(getStringPosition());
            redo.Clear();
            undospent.Add(spent);
            redospent.Clear();
        }
        setpiece = false;
        promoteVisibility(false);
        restartbutton.SetActive(true);
    }
    public void removeListDuplicates(){
        for(int i = 1; i < undo.Count; i++){
            if(undo[i] == undo[i-1]){
                undo.RemoveAt(i);
                i--;
            }
        }
        for(int i = 1; i < redo.Count; i++){
            if(redo[i] == redo[i-1]){
                redo.RemoveAt(i);
                i--;
            }
        }
        for(int i = 1; i < undospent.Count; i++){
            if(undospent[i] == undospent[i-1]){
                undospent.RemoveAt(i);
                i--;
            }
        }
        for(int i = 1; i < redospent.Count; i++){
            if(redospent[i] == redospent[i-1]){
                redospent.RemoveAt(i);
                i--;
            }
        }
    }
    public void setPlate(string name, int x, int y) {
        float x2 = x;
        float y2 = y;
        
        float mul = (blackbrain == "user" && whitebrain != "user") ? -1 : 1;

        x2 *= 0.7f * mul;
        y2 *= 0.7f * mul;

        x2 += -2.45f * mul;
        y2 += -2.45f * mul; 

        GameObject mp = Instantiate(movePlate, new Vector3(x2, y2, 0.0f), Quaternion.identity);
        mp.GetComponent<SpriteRenderer>().sortingOrder = 0;
        MovePlate mpScript = mp.GetComponent<MovePlate>();
        if (name == "checkplate") mpScript.checkplate = true;
        if (name == "pieceplate") mpScript.pieceplate = true;
        if (name == "levelplate") mpScript.levelplate = true;
        mpScript.SetReference(chesspiece);
        mpScript.SetCoords(x, y);
        if (name == "levelplate") {
            pieceplatepositions[x,y] = true;
        } else {
            platepositions[x,y] = 1;  
        }
    }
    public string getStringPosition(){
        string str = "";
        for(int y = 7; y >= 0; y--){
            for(int x = 0; x < 8; x++){
                if(pieceplatepositions[x,y] && positions[x,y] == null)
                    str += 'x';
                else if(positions[x,y] == null)
                    str += "1";
                else{
                    char piece = positions[x,y].GetComponent<Chessman>().name[0];
                    if(piece == 'W'){
                        if(positions[x,y].GetComponent<Chessman>().name[2] == 'n')
                            str += positions[x,y].GetComponent<Chessman>().name[2].ToString().ToUpper();
                        else
                            str += positions[x,y].GetComponent<Chessman>().name[1].ToString().ToUpper();
                    }
                    else {
                        if(positions[x,y].GetComponent<Chessman>().name[2] == 'n')
                            str += positions[x,y].GetComponent<Chessman>().name[2].ToString();
                        else
                            str += positions[x,y].GetComponent<Chessman>().name[1].ToString();
                    }
                }
            }
            str += "/";
        }
        str = str.Substring(0, str.Length - 1);
        return renderStringPosition(str);
    } 
    public string getFEN() {
        string s = getStringPosition();
        s += " " + currentPlayer[0] + " ";
        string castle = castleFEN();
        s += (castle == "" ? "-" : castle) + " ";
        string passant = passantFEN();
        s += (passant == "" ? "-" : passant) + " " + halfMoves() + " ";
        s += allmoves.Count / 2;
        return s;
    }
    public string castleFEN() {
        string s = "";
        if (Nevermoved('K', 4, 0) && Nevermoved('R', 7, 0)) {
            s += "K";
        }
        if (Nevermoved('K', 4, 0) && Nevermoved('R', 0, 0)) {
            s += "Q";
        }
        if (Nevermoved('k', 4, 7) && Nevermoved('r', 7, 7)) {
            s += "k";
        }
        if (Nevermoved('k', 4, 7) && Nevermoved('r', 0, 7)) {
            s += "q";
        }
        return s;
    }
    public string passantFEN() {
        string s = "";
        if (enpassant != -1){
            Coords p = new Coords(enpassant, currentPlayer == "black" ? 2 : 5);
            s = p.spos;
        }
        return s;
    }
    public string renderStringPosition(string s){
        string s2 = "";
        int adder = 0;
        for(int i = 0; i < s.Length; i++){
            if(i < s.Length - 1 && s[i] - '0' >= 0 && s[i] - '0' <= 7 && s[i+1] - '0' >= 0 && s[i+1] - '0' <= 7){
                adder += (s[i] - '0');
            }
            else if(s[i] - '0' >= 0 && s[i] - '0' <= 7){
                adder += (s[i] - '0');
                s2 += adder.ToString();
            }
            else if(i < s.Length - 1 && s[i] == '/' && s[i+1] == '/')
                s2 += "/8";
            else {
                s2 += s[i];
                adder =  0;
            }
        }
        return s2;
    }
    public void playSound(string sound){
        if(sound == "move")
            source.PlayOneShot(moveclip);
        else if(sound == "capture")
            source.PlayOneShot(captureclip);
        else if(sound == "check") {
            source.PlayOneShot(checkclip);
            Coords kingpos = findKing(currentPlayer);
            platepositions[kingpos.x, kingpos.y] = 0;
        }
        else if(sound == "movecapture")
            source.PlayOneShot(movecaptureclip);
        else if(sound == "capturecapture")
            source.PlayOneShot(capturecaptureclip);
        else if(sound == "promote")
            source.PlayOneShot(promoteclip);
        else if(sound == "promotecapture")
            source.PlayOneShot(promotecaptureclip); 
        else if(sound == "movepromote")
            source.PlayOneShot(movepromoteclip); 
        else if(sound == "capturepromote")
            source.PlayOneShot(capturepromoteclip); 
        else if(sound == "promotepromote")
            source.PlayOneShot(promotepromoteclip); 
        else if(sound == "castle")
            source.PlayOneShot(castleclip); 
        else if(sound == "castlecapture")
            source.PlayOneShot(castlecaptureclip); 
        else if(sound == "castlepromote")
            source.PlayOneShot(castlepromoteclip); 
        else if(sound == "gameend")
            source.PlayOneShot(gameendclip); 
    }
    public bool validMouseLocation(int x, int y){
        return x >= 0 && x <= 7 && y >= 0 && y <= 7;
    }
    public string Capital(string b) {
        if (b == "fish") b = "stockfish";
        return char.ToUpper(b[0]) + b.Substring(1);
    }
    public void Update(){

        if(mode == "levels" && (!(validMouseLocation(MouseLocation()[0], MouseLocation()[1])) || !pieceplatepositions[MouseLocation()[0], MouseLocation()[1]]))
            GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = "Select a Level";

        GameObject.FindGameObjectWithTag("TutorialText").GetComponent<Text>().text = Capital(whitebrain) + " vs " + Capital(blackbrain);
        GameObject.FindGameObjectWithTag("MovesText").GetComponent<Text>().text = Capital(currentPlayer) + " to move.";
        GameObject.FindGameObjectWithTag("TutorialText").GetComponent<Text>().enabled = !gameOver;
        GameObject.FindGameObjectWithTag("MovesText").GetComponent<Text>().enabled = !gameOver;
        GameObject.FindGameObjectWithTag("desctext").GetComponent<Text>().enabled = !gameOver;

        homebutton.SetActive(mode != "levels");


            if(GameObject.Find("ReadyButton") != null)
                GameObject.Find("ReadyButton").GetComponentInChildren<Text>().text = "Resetup";
            if(GameObject.Find("RestartButton") != null)
                GameObject.Find("RestartButton").GetComponentInChildren<Text>().text = "Restart";

        if(metacurposition.Contains('x') && !setpiece && !pawnpromote) readybutton.SetActive(true);
        else readybutton.SetActive(false);

        if(mode == "ingame"){ 
            for(int i = 0; i < undo.Count; i++){
                if(undo[i].Contains('x')){
                    undo[i] = removeX(undo[i]);
                }
            }
            for(int i = 0; i < redo.Count; i++){
                if(redo[i].Contains('x')){
                    redo[i] = removeX(redo[i]);
                }
            }
        }

        if (allmoves.Count > 0 && allmoves[allmoves.Count-1].Contains('+')){
            Coords kingpos = findKing(currentPlayer);
            if (kingpos.x != -1 && platepositions[kingpos.x, kingpos.y] == 0) {
                setPlate("checkplate", kingpos.x, kingpos.y);
            }
        }
        
        removeListDuplicates();

        
        if(Input.GetKey("r")){
            restart();
        }

        for(int i = 0; i < levels.GetLength(0); i++){
            if(setpiece) break;
            if(Input.GetKey(levels[i,0])){
                makeLevel(i);
            }
        }
        if(Input.GetKeyUp(KeyCode.DownArrow)){
            Undo();
        }
        else if(Input.GetKeyUp(KeyCode.UpArrow)){
            Redo();
        }
        if (currentPlayer == "black" && mode == "levels") {
            //playSound(recentmove);
            undo.Add(getStringPosition());
            redo.Clear();
            currentPlayer = "white";
        }
        if (canrequest && mode == "ingame" && !gameOver) {  //bot
            GameObject.FindGameObjectWithTag("desctext").GetComponent<Text>().text = "";
            canrequest = false;
            string brain = currentPlayer == "white" ? whitebrain : blackbrain;
            if (nomoves(currentPlayer)) {
                if (inCheck(currentPlayer)) {
                    Winner(currentPlayer == "white" ? "black": "white");
                } else {
                    Winner("stalemate");
                }
                brain = "none";
            }
            if (insufficientMaterial()) {
                Winner("insufficient material");
                brain = "none";
            }   
            if (halfMoves() >= 100) {
                Winner("the 50 move rule");
                brain = "none";
            } 
            switch (brain) {
                case "user":
                    break;
                case "GPT":
                    if (Random.Range(0, 100) < fishhelp) {
                        fishReq();
                        
                    } else {
                        sendReq("gpt-4o-mini");
                    }
                    break;
                case "gemini":
                    if (Random.Range(0, 100) < fishhelp) {
                        fishReq();
                    } else {
                        sendReq("gemini");
                    }
                    break;
                case "fish":
                        fishReq(); break;
                case "random":
                    randomReq(); break;
            }
        }
        if (!gameOver && threeFold()) {
            Winner("threefold repetition");
        }
        if(numPiece("Wking") == 0 && whitekings > 0 && moves > 0){
            whitekings = 0; 
        }
        else{
            whitekings = numPiece("Wking");
        }
        if(pawnpromote){
            promoteVisibility(true);
            GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = true;
            GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = "Select Promotion Piece:";
        }
        else if(setpiece){
            promoteVisibility(true);
            GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = true;
            GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = "Select Piece:";
        }
        else{
            promoteVisibility(false);
            if(!gameOver && mode != "levels")
                GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = false;
        }
        if(Input.GetKeyDown("left shift")){  //shift
            fishReq();
        }
        if(Input.GetKeyDown("tab")){  //shift
            gameOver = !gameOver;
        }
        if(Input.GetKeyDown("right shift") && dev){  //shift
            Debug.Log("Undo");
            foreach (string s in undo) {
                Debug.Log(s);
            }
            Debug.Log("Redo");
            foreach (string s in redo) {
                Debug.Log(s);
            }
            string position = getStringPosition();
            string prev = renderMoves();
            Debug.Log(prev);
            Debug.Log(fishhelp);
        }
        if(Input.GetKeyDown("return")){  //enter
            moveBot(new string[] {"bruh"}, 100);
        }
        if(Input.GetKeyDown("space")){ //space
            sendReq("gpt-4o-mini");
        }
        if(Input.GetMouseButtonUp(0)){ //click
            int[] location = MouseLocation();
            Debug.Log(location[0] + " " + location[1]); 
            Coords cloc = new Coords (location[0], location[1]);
            Debug.Log("Square attacked " + squareAttacked(location[0], location[1], currentPlayer));
            // Chessman m = positions[location[0], location[1]].GetComponent<Chessman>();
            // m.shouldSpawn = false;
            // Debug.Log(m.InitiateMovePlates());
            //Debug.Log("Pinned " + isPinned(cloc));  
        }
        if(Input.GetMouseButtonDown(0)){  //Mouse Down mouse down Mouse down 
            int[] location = MouseLocation();
            if(setpiece && PositionOnBoard(location[0], location[1]) && !pieceplatepositions[location[0],location[1]]){
                cancelPiecePlate();
            }

        }
    }
    public void OnMouseUp(){
        Debug.Log(Input.GetAxis("Mouse X") + " " + Input.GetAxis("Mouse Y"));
    }
    public void Undo(){
        if(pawnpromote) return;
        if (currentPlayer == "white" && whitebrain != "user" && sendingreq) return;
        if (currentPlayer == "black" && blackbrain != "user" && sendingreq) return;
        //gameOver = false;
        GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().enabled = false;
        if (mode != "levels") GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = false;
        if(undo.Count < 2) return;
        currentPlayer = (currentPlayer == "white" ? "black" : "white");
        redo.Add(getStringPosition());
        undo.RemoveAt(undo.Count-1);
        string save = undo[undo.Count-1];
        setStringPosition(save);
        
        if((mode == "ingame" || mode == "sandbox")){
            redomoves.Add(allmoves[allmoves.Count-1]);
            allmoves.RemoveAt(allmoves.Count-1);
            if(allmoves.Count > 0 && allmoves[allmoves.Count-1].Contains("!")){
                enpassant = allmoves[allmoves.Count-1][0] - 'a';
            } 
        }

        if(mode == "ingame" || mode == "sandbox") moves--;
        else if(undospent.Count > 0){
            redospent.Add(spent);
            if(undospent.Count == 1) spent = 0;
            else spent = undospent[undospent.Count-2];
            undospent.RemoveAt(undospent.Count-1);
        }
        DestroyMovePlates();
    }
    public void Redo(){
        if(redo.Count == 0) return;
        if(pawnpromote) return;
        if (currentPlayer == "white" && whitebrain != "user" && sendingreq) return;
        if (currentPlayer == "black" && blackbrain != "user" && sendingreq) return;
        GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().enabled = false;
        if (mode != "levels") GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = false;
        //gameOver = false;
        string save = redo[redo.Count-1];
        setStringPosition(save);
        undo.Add(save);
        redo.RemoveAt(redo.Count-1);

        if((mode == "ingame" || mode == "sandbox")){
            allmoves.Add(redomoves[redomoves.Count-1]);
            redomoves.RemoveAt(redomoves.Count-1);
            if(allmoves.Count > 0 && allmoves.Count > 0 && allmoves[allmoves.Count-1].Contains("!")){
                enpassant = allmoves[allmoves.Count-1][0] - 'a';
            } 
        }

        currentPlayer = (currentPlayer == "white" ? "black" : "white");
        if(mode == "ingame" || mode == "sandbox") moves++;
        else if(redospent.Count > 0){
            spent = redospent[redospent.Count-1];
            undospent.Add(spent);
            redospent.RemoveAt(redospent.Count-1);
        }
        DestroyMovePlates();
    }
    public void DestroyMovePlates() {
        Coords m = findKing(currentPlayer);
        if (m.x == -1) return;
        positions[m.x, m.y].GetComponent<Chessman>().DestroyMovePlates();
    }
    public string getPiece(int x, int y){
        if(!PositionOnBoard(x,y)) return "Null";
        if(GetPosition(x,y) == null) return "Null";
        return GetPosition(x,y).GetComponent<Chessman>().name;
    }


    public void promoteVisibility(bool shown){ 
        if(shown){
            for(int i = 1; i < 5; i++){
                promotes[i].SetActive(shown);
            }
        }
        else{
            for(int i = 0; i < 6; i++){
                promotes[i].SetActive(shown);
            }
        }
    }
    public bool mouseOnPlate(){
        int[] location = MouseLocation();
        if(location[0] == -1 || location[1] == -1) return true;
        if(platepositions[location[0], location[1]] == 1) return true;
        return false;
    }
    public int[] MouseLocation(){
        Canvas canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
            //Canvas canvas = this.GetComponent<Canvas>();
        Vector3 mousePos = Input.mousePosition;
        float f = canvas.scaleFactor;
        int boardx = -1;
        int boardy = -1;
        int cnt = 0;
        float x = mousePos.x/f;
        float y = mousePos.y/f;
        for(float i = 0.0f; i < 800.0f; i += 100.0f){
            if(i < x && x < i + 100.0f){
                boardx = cnt;
            }
            if(i+307.0f < y && y < i + 407.0f){
                boardy = cnt;
            }
            cnt++;
        }
        int[] location = {boardx,boardy};
        if (blackbrain == "user" && whitebrain != "user" && PositionOnBoard(boardx, boardy)) {
            location = new int[]{7-boardx,7-boardy};
        }
        //Debug.Log(boardx + " " + boardy);
        return location;
    }
    public void makeLevel(int i){
        tutorialnum = 0;
        levelnum = i;
        curposition = levels[i,1];
        metacurposition = curposition;
        if (levelnum != 1) {
            whitebrain = levels[i,2];
            blackbrain = levels[i,3];
        }
        mode = levels[i,4];
        title = levels[i,5];
        goal = levels[i,6];
        restart();
        if(mode == "setpiece") budget = int.Parse(levels[i,7]);
    }
    public string pieceAttack(GameObject obj, bool justsquare = false, int cx = -1, int cy = -1, string prey = "white"){
        Chessman piece = obj.GetComponent<Chessman>();
        int x = piece.GetXBoard();
        int y = piece.GetYBoard();
        string name = piece.name.Substring(1);
        if(name == "rook"){
            return (attackLine(x,y,-1,0,obj,justsquare,cx,cy) | attackLine(x,y,0,-1,obj,justsquare,cx,cy) 
            | attackLine(x,y,0,1,obj,justsquare,cx,cy) | attackLine(x,y,1,0,obj,justsquare,cx,cy)) ? (nameToletter(piece.name) + getNotation(x, y) + " ") : "";
        }
        if(name == "bishop"){
            return (attackLine(x,y,-1,1,obj,justsquare,cx,cy) | attackLine(x,y,-1,-1,obj,justsquare,cx,cy) 
            | attackLine(x,y,1,1,obj,justsquare,cx,cy) | attackLine(x,y,1,-1,obj,justsquare,cx,cy)) ? (nameToletter(piece.name) + getNotation(x, y) + " ") : "";
        }
        if(name == "queen"){
            return (attackLine(x,y,-1,1,obj,justsquare,cx,cy) | attackLine(x,y,-1,-1,obj,justsquare,cx,cy) | attackLine(x,y,1,1,obj,justsquare,cx,cy) | attackLine(x,y,1,-1,obj,justsquare,cx,cy) |
                    attackLine(x,y,-1,0,obj,justsquare,cx,cy) | attackLine(x,y,0,-1,obj,justsquare,cx,cy) | attackLine(x,y,0,1,obj,justsquare,cx,cy) | attackLine(x,y,1,0,obj,justsquare,cx,cy))  ? (nameToletter(piece.name) + getNotation(x, y) + " ") : "";
        }
        if(name == "knight"){
            return (attackSquare(x+1,y+2,obj,justsquare,cx,cy) | attackSquare(x-1,y+2,obj,justsquare,cx,cy) | attackSquare(x+2,y+1,obj,justsquare,cx,cy) | attackSquare(x+2,y-1,obj,justsquare,cx,cy) |
            attackSquare(x+1,y-2,obj,justsquare,cx,cy) | attackSquare(x-1,y-2,obj,justsquare,cx,cy) | attackSquare(x-2,y+1,obj,justsquare,cx,cy) | attackSquare(x-2,y-1,obj,justsquare,cx,cy))  ? (nameToletter(piece.name) + getNotation(x, y) + " ") : "";
        }
        if(name == "king"){
            return (attackSquare(x,y+1,obj,justsquare,cx,cy) | attackSquare(x,y-1,obj,justsquare,cx,cy) | attackSquare(x+1,y+1,obj,justsquare,cx,cy) | attackSquare(x+1,y-1,obj,justsquare,cx,cy) |
            attackSquare(x+1,y,obj,justsquare,cx,cy) | attackSquare(x-1,y+1,obj,justsquare,cx,cy) | attackSquare(x-1,y,obj,justsquare,cx,cy) | attackSquare(x-1,y-1,obj,justsquare,cx,cy))  ? (nameToletter(piece.name) + getNotation(x, y) + " ") : "";
        }
        if(name == "pawn"){
            if(prey == "black")
                return (attackSquare(x+1, y+1, obj,justsquare,cx,cy) | attackSquare(x-1, y+1, obj,justsquare,cx,cy))  ? (nameToletter(piece.name) + getNotation(x, y) + " ") : "";
            return (attackSquare(x+1, y-1, obj,justsquare,cx,cy) | attackSquare(x-1, y-1, obj,justsquare,cx,cy))  ? (nameToletter(piece.name) + getNotation(x, y) + " ") : "";
        }
        return "";
    }
    public string squareAttacked(int x, int y, string prey){
        string anyattack = "";
        if(prey == "white"){
            for(int i = 0; i < playerBlack.Length; i++){
                if(playerBlack[i] == null) continue;
                anyattack += (pieceAttack(playerBlack[i], true, x, y, prey));
            }
        }
        else{
            for(int i = 0; i < playerWhite.Length; i++){
                if(playerWhite[i] == null) continue;
                anyattack += (pieceAttack(playerWhite[i], true, x, y, prey));
            }
        }
        return anyattack;
    }
    public int numPiece(string name){
        int cnt = 0;
        for(int x = 0; x < 8; x++){
            for(int y = 0; y < 8; y++){
                if(positions[x,y] != null){
                    if(positions[x,y].GetComponent<Chessman>().name == name)
                        cnt++;
                }

            }
        }
        return cnt;
    }

    public bool attackLine(int x, int y, int dx, int dy, GameObject obj, bool justsquare = false, int cx = -1, int cy = -1){
        x += dx;
        y += dy;
        if(!PositionOnBoard(x,y)) return false;
        if(justsquare){
            if(x == cx && y == cy) return true;
            if(positions[x,y] == null) return attackLine(x, y, dx, dy, obj, justsquare, cx, cy);
            return false;
        }

        if(positions[x,y] == null) return attackLine(x, y, dx, dy, obj);
        if(getPiece(x,y)[0] == 'B') return false;
        if(getPiece(x,y)[0] == 'W'){
            Debug.Log(getPiece(x,y));
            if((preyx == -1 || compare(GetPosition(preyx, preyy), GetPosition(x,y), "white"))){
                preyx = x;
                preyy = y;
                predatorx = obj.GetComponent<Chessman>().GetXBoard();
                predatory = obj.GetComponent<Chessman>().GetYBoard();
                actualpassantx = -1;
                actualpassanty = -1;
            }
            else if(preyx == x && preyy == y && compare(GetPosition(predatorx, predatory), obj, "black")){
                preyx = x;
                preyy = y;
                predatorx = obj.GetComponent<Chessman>().GetXBoard();
                predatory = obj.GetComponent<Chessman>().GetYBoard();
                actualpassantx = -1;
                actualpassanty = -1;
            }
            return true;
        } 
        return false;
    }

    public bool attackSquare(int x, int y, GameObject obj, bool justsquare = false, int cx = -1, int cy = -1){
        if(!PositionOnBoard(x,y)) return false;
        bool attackpassant = (x == enpassant) && y == 2 && PositionOnBoard(x, y+1) && positions[x,y+1] != null && obj.GetComponent<Chessman>().name[1] == 'p';
        if(justsquare) return cx == x && cy == y;
        if(positions[x,y] == null && !attackpassant) return false;
        if(attackpassant){
             y = y+1;
             actualpassantx = obj.GetComponent<Chessman>().GetXBoard();
             actualpassanty = obj.GetComponent<Chessman>().GetYBoard();
        }
        if(getPiece(x,y)[0] == 'W'){
            Debug.Log(getPiece(x,y));
            if((preyx == -1 || compare(GetPosition(preyx, preyy), GetPosition(x,y), "white"))){
                preyx = x;
                preyy = y;
                predatorx = obj.GetComponent<Chessman>().GetXBoard();
                predatory = obj.GetComponent<Chessman>().GetYBoard();
                if(!attackpassant){
                    actualpassantx = -1;
                    actualpassanty = -1;
                }
            }
            else if(preyx == x && preyy == y && compare(GetPosition(predatorx, predatory), obj, "black")){
                preyx = x;
                preyy = y;
                predatorx = obj.GetComponent<Chessman>().GetXBoard();
                predatory = obj.GetComponent<Chessman>().GetYBoard();
                if(!attackpassant){
                    actualpassantx = -1;
                    actualpassanty = -1;
                }
            }
            return true;
        }
        if(attackpassant){
            actualpassantx = -1;
            actualpassanty = -1;
        }
        return false;
    }
    public int completedLevels() {
        int cnt = 0;
        for (int i = 0; i < Menu.Mmedals.Length; i++) {
            if (Menu.Mmedals[i] != "gray") {
                cnt++;
            }
        }
        return cnt;
    }
    public bool compare(GameObject obj, GameObject obj2, string piececolor){ //true if piece < piece2
        if(obj == null || obj2 == null) return false;
        Chessman piece = obj.GetComponent<Chessman>();
        Chessman piece2 = obj2.GetComponent<Chessman>();
        int x1 = piece.GetXBoard();
        int y1 = piece.GetYBoard();
        string name = piece.name;   
        int x2 = piece2.GetXBoard();
        int y2 = piece2.GetYBoard();
        string name2 = piece2.name;

        string [] order;
        order = new string[] {"Wpawn", "Wknight", "Wbishop", "Wrook", "Wqueen", "Wking"};
        if(piececolor == "black")
            order = new string[] {"Bpawn", "Bknight", "Bbishop", "Brook", "Bqueen", "Bking"};
        if(piececolor == "white" && indexOf(name, order) != indexOf(name2, order)) 
            return indexOf(name, order) < indexOf(name2, order);
        if(piececolor == "black" && indexOf(name, order) != indexOf(name2, order)) 
            return indexOf(name, order) > indexOf(name2, order);
        if(y2 != y1) return y1 < y2;
        return x1 < x2;
    }

    public int indexOf(string str, string [] arr){
        for(int i = 0; i < arr.Length; i++){
            if(arr[i] == str) return i;
        }
        return -1;
    }
    public void hideMainButtons(){
        restartbutton.SetActive(false);
        readybutton.SetActive(false);
        undobutton.SetActive(false);
        redobutton.SetActive(false);
    }
    public void showMainButtons(){
        restartbutton.SetActive(true);
        //readybutton.SetActive(true);
        if(mode != "setpiece"){
            undobutton.SetActive(true);
            redobutton.SetActive(true);
        }
    }
    public void ReadStringInput(string s){
        Debug.Log(s);
        setStringPosition(s);
        curposition = s;
        metacurposition = s;
        undo.Clear();
        redo.Clear();
        spent = 0; 
        undobutton.SetActive(true);
        redobutton.SetActive(true);
        undo.Add(curposition);
        goal = "points";
        if(metacurposition.Contains('x')) {
             mode = "setpiece";
             budget = 1000;
        }
        else
            mode = "ingame";
    }
    public string print2D(char[,] arr){
        string s = "";
        for(int i = 0; i < 8; i++){
            for(int j = 0; j < 8; j++){
                s += arr[i,j];
            }
            s += "/";
        }
        return s;
    }
    public int indexOf(string[] arr, string s){
        for(int i = 0; i < arr.Length; i++){
            if(s == arr[i]) return i;
        }
        return -1;
    }
    public void home(){
        // makeLevel(0);
        Menu.GoMenu();
    }
    public bool nomoves(string color) {
        GameObject[] allpieces = playerWhite;
        if (color == "black") allpieces = playerBlack;
        foreach (GameObject obj in allpieces) {
            if (obj == null) continue;
            Chessman m = obj.GetComponent<Chessman>();
            m.shouldSpawn = false;
            string moves = m.InitiateMovePlates();
            if (moves != "") {
                return false;
            }
        }
        return true;
    }
    public void Winner(string playerWinner){
        if(mode == "levels") return;
        gameOver = true;
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = true;

        restartbutton.SetActive(true);
        undobutton.SetActive(true);
        redobutton.SetActive(true);
        if(playerWinner == "black"){
            source.PlayOneShot(winclip); 
            GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = "Checkmate. Black Wins!";
        } else if (playerWinner == "white") {
            source.PlayOneShot(winclip); 
            GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = "Checkmate. White Wins!";
        } else if (playerWinner == "stalemate") {
            source.PlayOneShot(gameendclip); 
            GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = "Stalemate!";
        } else {
            source.PlayOneShot(gameendclip); 
            GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = "Draw by " + playerWinner + ".";
        }
        //GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().enabled = true;
    }
}
