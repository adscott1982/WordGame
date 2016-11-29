using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    #region Variables

    // Delegates

    public delegate void Action();

    // GameState Enumerators

    public enum OverallState { Splash, Menu, Game }
    public OverallState overallState = OverallState.Menu;

    public enum MenuState { Main, Options, Statistics, Credits, Help }
    public MenuState menuState = MenuState.Main;

    public enum GameState { Initialize, Starting, Countdown, Playing, Ending, Ended }
    public GameState gameState;

    // Resource paths
    private string allWordsEnglishPath = "Dictionaries/WordsAll";
    private string all9LetterWordsEnglishPath = "Dictionaries/Words9Letter";
    private string tileObjectPath = "Prefabs/Tile";
    private string wordObjectPath = "Prefabs/Selected Word";
    private string scoreObjectPath = "Prefabs/Score";
    private string timerObjectPath = "Prefabs/Timer";
    private string shuffleButtonObjectPath = "Prefabs/ShuffleButton";
    private string menuButtonObjectPath = "Prefabs/MenuButton";

    // Static object names
    private string canvasGameOverMenuName = "CanvasGameOverMenu";
    private string canvasMainMenuName = "CanvasMainMenu";
    private string sceneName = "game scene";
  
    private enum Language { English, French };

    private List<string> wordListAll;
    private List<string> wordList9Letter;
    private List<string> wordListFiltered;
    private List<string> wordListUsed;
    private List<char> letterList;

    private string wordFileAll;
    private string wordFile9Letter;

    // Tile Variables
    private const string tileObjectName = "Tile";
    private List<Tile> tiles = new List<Tile>();
    private List<Tile> selectedTiles = new List<Tile>();

    // Screen Positioning Variables
    private List<TileLayoutPosition> tilePositions;
    private enum TilePositions { Start, FiveFour, Line, End };
    TilePositions currentTilePosition;
    private const int numberofTiles = 9;
    private const int tileDepthMultiplier = 10;
    private const float tileScreenBufferX = 50f;
    private const float tileToTileVerticalBuffer = 288f;
    private const float tileRow1Y = 800f;
    private const float tileRow2Y = 500f;
    private const float selectedWordY = 232f;
    private const float tileSize = 192f;
    Rect cameraBounds;

    // Selected Word variables
    SelectedWord selectedWord;
    private Vector2 selectedWordPosition;
    bool isSelectedWordValid = false;
    bool isSelectedWordUsed = false;

    // Score variables
    public Score score;
    private Vector2 scorePosition;
    private float scorePositionXOffset = -20f;
    private float scorePositionYOffset = 20f;

    // Timer variables
    Timer timer;
    private Vector2 timerPosition;
    private float timerPositionXOffset = 20f;
    private float timerPositionYOffset = 20f;

    // Button variables
    Button shuffleButton;
    Button menuButton;
    private Vector2 shuffleButtonPosition;
    private Vector2 menuButtonPosition;
    public Color shuffleButtonColor = Color.white;
    public Color menuButtonColor = Color.white;

    // General Public variables
    public float tileSmoothTime = 1f;
    public Color tileUnselectedColor;
    public Color tileSelectedColor;
    public Color wordValidColor;
    public Color wordUsedColor;
    public Color wordInvalidColor;
    public Color scoreColor;
    public Color timerColor;
    public int timerSeconds = 60;

    // Base object variables
    GameObject canvasGameOverMenu;
    GameObject canvasMainMenu;

    #endregion

    #region Start and Initialize Methods

    // Use this for initialization
	void Start ()
    {
        // Create base objects
        LoadBaseObjectReferences();

        switch (overallState)
        {
            case OverallState.Splash:
                SplashInit();
                break;
            case OverallState.Menu:
                MenuInit();
                break;
            case OverallState.Game:
                GameInit();
                break;
        }
	}

    /// <summary>
    /// Splash screen gamestate method
    /// </summary>
    void SplashInit()
    {
        overallState = OverallState.Splash;
    }

    /// <summary>
    /// Menu screen gamestate method
    /// </summary>
    void MenuInit()
    {
        overallState = OverallState.Menu;
        SetupScreen();
        canvasMainMenu.SetActive(true);
    }

    /// <summary>
    /// In game gamestate method
    /// </summary>
    void GameInit()
    {
        overallState = OverallState.Game;
        SetWordFile(Language.English);
        LoadDictionary();
        ChooseWord();
        FilterWordList();

        SetLayouts();
        CreateTiles();
        CreateSelectedWordObject();
        CreateScoreObject();
        CreateTimerObject();
        CreateButtons();

        gameState = GameState.Starting;
    }

    void DestroyGameObjects()
    {
        DestroyTiles();
        DestroySelectedWordObject();
        DestroyScoreObject();
        DestroyTimerObject();
        DestroyButtons();
    }

    #endregion

    #region Update Methods
    // Update is called once per frame
    void Update () 
    {
        switch (overallState)
        {
            case OverallState.Splash:
                SplashUpdate();
                break;
            case OverallState.Menu:
                MenuUpdate();
                break;
            case OverallState.Game:
                GameUpdate();
                break;
        }
	}

    /// <summary>
    ///  Splash Screen update method
    /// </summary>
    void SplashUpdate()
    {

    }

    /// <summary>
    /// Menu update method
    /// </summary>
    void MenuUpdate()
    {

    }

    /// <summary>
    ///  Game update method
    /// </summary>
    void GameUpdate()
    {
        switch(gameState)
        {
            case GameState.Initialize:
                GameInit();
                break;

            case GameState.Starting:
                // Show countdown timer
                gameState = GameState.Playing;
                break;

            case GameState.Countdown:
                // Wait for countdown to finish
                if (true)
                {
                    gameState = GameState.Playing;
                }
                break;

            case GameState.Playing:
                // Run game
                StartTimer(true);
                CheckTileSelection();
                break;

            case GameState.Ending:
                Debug.Log("Ending");
                canvasGameOverMenu.SetActive(true);
                gameState = GameState.Ended;
                break;

            case GameState.Ended:
                // Await input
                Debug.Log("Ended");
                break;
        } 
    }

    #endregion

    #region Tile Management

    void CheckTileSelection()
    {
        foreach(Tile tile in tiles)
        {
            if (tile.tileController.selected)
            {
                tile.sprite.color = tileSelectedColor;
            }
            else
            {
                tile.sprite.color = tileUnselectedColor;
            }
        }
    }

    void SetTilePositions(TilePositions targetTilePosition)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            switch (targetTilePosition)
            {
                case TilePositions.Start:
                    tiles[i].tileController.target = new Vector3(tilePositions[i].Start.x, tilePositions[i].Start.y, 0);
                    break;
                case TilePositions.FiveFour:
                    tiles[i].tileController.target = new Vector3(tilePositions[i].FiveFour.x, tilePositions[i].FiveFour.y, 0);
                    break;
                case TilePositions.Line:
                    tiles[i].tileController.target = new Vector3(tilePositions[i].Line.x, tilePositions[i].Line.y, 0);
                    break;
                case TilePositions.End:
                    tiles[i].tileController.target = new Vector3(tilePositions[i].End.x, tilePositions[i].End.y, 0);
                    break;
            }

            currentTilePosition = targetTilePosition;
        }
    }


    #endregion

    #region Word Methods
    void SetWordFile(Language language)
    {
        if (language == Language.English)
        {
            wordFileAll = allWordsEnglishPath;
            wordFile9Letter = all9LetterWordsEnglishPath;
        }
    }

    void ChooseWord()
    {
        // Select random 9 letter word
        int count = wordList9Letter.Count;
        int i = Random.Range(0, count - 1);
        string word = wordList9Letter[i];

        // Make uppercase
        word = word.ToUpper();
        Debug.Log("Word selected is \"" + word + "\".");

        letterList = new List<char>();
        foreach(char letter in word)
        {
            letterList.Add(letter);
        }

        // Shuffle letters
        Utility.ShuffleList<char>(letterList);
    }

    void LoadDictionary()
    {
        wordListAll = new List<string>();
        wordList9Letter = new List<string>();

        string line;


        string words = Resources.Load<TextAsset>(wordFileAll).text;

        System.IO.StringReader sr = new System.IO.StringReader(words);

        while ((line = sr.ReadLine()) != null)
        {
            wordListAll.Add(line);
        }

        words = Resources.Load<TextAsset>(wordFile9Letter).text;

        sr = new System.IO.StringReader(words);
        while ((line = sr.ReadLine()) != null)
        {
            wordList9Letter.Add(line);
        }

        Debug.Log("WordListAll loaded. Count is " + wordListAll.Count + " words.");
        Debug.Log("WordList9Letter loaded. Count is " + wordList9Letter.Count + " words.");
    }

    void FilterWordList()
    {
        wordListFiltered = new List<string>();
        wordListUsed = new List<string>();
        foreach(string word in wordListAll)
        {
            // Store list of 9 characters as temp list
            List<char> tempLetterList = new List<char>(letterList);

            //  Store current word in new string in upper case and get length
            string letters = word.ToUpper();
            int length = letters.Length;

            bool isMatch = true;

            // Step through each letter in the string
            for (int i = 0; i < length; i++)
            {
                if (tempLetterList.Contains(letters[i]))
                {
                    // If letter list contains the letter, remove the letter ready for the next letter to be checked
                    tempLetterList.Remove(letters[i]);
                }
                else
                {
                    // Letter is not present, set as no match and break out of loop
                    isMatch = false;
                    break;
                }
            }
            
            if (isMatch)
            {
                // Word was a match, add to filtered word list
                wordListFiltered.Add(letters);
            }
        }

        string filteredWords = "";
        foreach(string word in wordListFiltered)
        {
            filteredWords += word + "\n";
        }

        Debug.Log("Total words for current 9 letter word is " + wordListFiltered.Count + "\n" + filteredWords);
    }
    #endregion

    #region General Utility

    void SetupScreen()
    {
        // Disable auto screen orientation and set to landscape
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;

        Screen.orientation = ScreenOrientation.Landscape;

        // Work out screen boundaries

        Camera camera = Camera.main;

        float height = camera.orthographicSize * 2;
        float width = height * camera.aspect;

        cameraBounds.x = camera.transform.position.x - (width / 2);
        cameraBounds.y = camera.transform.position.y - (height / 2);
        cameraBounds.width = width;
        cameraBounds.height = height;

        string bounds = "Camera Coords:\n";
        bounds += "Left = " + cameraBounds.xMin + " Right = " + cameraBounds.xMax;
        bounds += "\nBottom = " + cameraBounds.yMin + " Top = " + cameraBounds.yMax;

        Debug.Log(bounds);
    }

    void SetLayouts()
    {
        int count = letterList.Count;

        tilePositions = new List<TileLayoutPosition>();

        for(int i = 0; i < count; i++)
        {
            tilePositions.Add(new TileLayoutPosition());
        }

        // Set Standard Positions Variables
        float tileXSpacing = (cameraBounds.width) / (count + 1);
        float tileXStart = cameraBounds.xMin;
        //float tileYRow1 = (cameraBounds.height / 2) + (tileToTileVerticalBuffer / 2);
        //float tileYRow2 = (cameraBounds.height / 2) - (tileToTileVerticalBuffer / 2);
        float tileYCentre = cameraBounds.height / 2;
        float wordXPosition = cameraBounds.xMin + (cameraBounds.width / 2);
        //float wordYPosition = (cameraBounds.height / 2) - ((tileToTileVerticalBuffer / 2)*2);

        selectedWordPosition = new Vector2(wordXPosition, selectedWordY);
        scorePosition = new Vector2(cameraBounds.xMax + scorePositionXOffset, cameraBounds.yMax + scorePositionYOffset);
        timerPosition = new Vector2(cameraBounds.xMin + timerPositionXOffset, cameraBounds.yMax + timerPositionYOffset);

        // Set Start/End Y position variables
        float tileYStart = cameraBounds.height + (tileSize / 2);
        float tileYEnd = 0 - (tileSize / 2);

        bool isTopRow = true;
        for (int i = 0; i < count; i++)
        {
            TileLayoutPosition tilePosition = tilePositions[i];

            float x = tileXStart + (tileXSpacing * (i + 1));
            float y = 0f;

            if (isTopRow)
            {
                y = tileRow1Y;
            }
            else
            {
                y = tileRow2Y;
            }

            tilePosition.Start = new Vector3(x, tileYStart, 0);
            tilePosition.FiveFour = new Vector3(x, y, 0);
            tilePosition.Line = new Vector3(x, tileYCentre, 0);
            tilePosition.End = new Vector3(x, tileYEnd, 0);

            tilePositions[i] = tilePosition;

            // Swap row for next tile
            isTopRow = !isTopRow;
        }
    }

    void SeedRandom()
    {
        // Set random seed;
        Random.seed = ((System.DateTime.Now.Minute * System.DateTime.Now.Second) + System.DateTime.Now.Month) * (System.DateTime.Now.Millisecond / System.DateTime.Now.Hour);
        Debug.Log("Random seed is " + Random.seed);

        // Skip a random number of random nexts up to 10
        int count = Random.Range(0, 10);
        Debug.Log("Random next count is " + count);
        for (int i = 0; i < count; i++)
        {
            float a = Random.value;
        }
    }

    bool CheckSelectedWordValid(string word)
    {
        return wordListFiltered.Contains(word);
    }

    bool CheckSelectedWordUsed(string word)
    {
        return wordListUsed.Contains(word);
    }

    #endregion

    #region Gameplay Methods

    void ConvertWordToScore()
    {
        string word = GetWord();
        int i = 1;
        int totalScore = 0;

        foreach(Tile tile in selectedTiles)
        {
            totalScore += i;
            i++;
        }

        score.controller.AddScore(totalScore);
        wordListFiltered.Remove(word);
        wordListUsed.Add(word);
    }

    void DisplaySelectedWord()
    {
        string word = GetWord();

        isSelectedWordValid = CheckSelectedWordValid(word);
        isSelectedWordUsed = CheckSelectedWordUsed(word);

        if (isSelectedWordValid)
        {
            selectedWord.textMesh.color = wordValidColor;
        }
        else if (isSelectedWordUsed)
        {
            selectedWord.textMesh.color = wordUsedColor;
        }
        else
        {
            selectedWord.textMesh.color = wordInvalidColor;
        }

        selectedWord.textMesh.text = word;
        selectedWord.controller.LettersChanged();
    }

    string GetWord()
    {
        string word = "";

        foreach (Tile tile in selectedTiles)
        {
            word += tile.letter.text;
        }

        return word;
    }

    #endregion

    #region Public Methods

    // Temp
    public void ReloadGame()
    {
        Application.LoadLevel(sceneName);
    }

    public void StartTimer(bool start)
    {
        timer.controller.timerActive = start;
    }

    public void TimerEnded()
    {
        // TODO: End game logic
        // Store score, process statistics etc.
        Debug.Log("Timer ended");

        gameState = GameState.Ending;
    }

    public void StartGame()
    {
        Debug.Log("Start Game");
        canvasGameOverMenu.SetActive(false);
        canvasMainMenu.SetActive(false);
        DestroyGameObjects();
        GameInit();
    }

    public void ReturnToMenu()
    {
        Debug.Log("Return to menu");
        DestroyGameObjects();
        canvasGameOverMenu.SetActive(false);
        MenuInit();
    }

    public void QuitGame()
    {
        Debug.Log("Quit game selected.");
        Application.Quit();
    }

    public void ShowGameMenu()
    {
        if (gameState == GameState.Playing)
        {
            // TODO: Menu pressed during game
            Debug.Log("Menu button pressed");
        }
    }

    public void ShuffleTiles()
    {
        if (gameState == GameState.Playing)
        {
            Utility.ShuffleList<Tile>(tiles);
            SetTilePositions(currentTilePosition);
        }
    }

    public void CycleTilePositions()
    {
        //Debug.Log("CycleTilePositions() called");

        switch (currentTilePosition)
        {
            case TilePositions.Start:
                SetTilePositions(TilePositions.FiveFour);
                break;
            case TilePositions.FiveFour:
                SetTilePositions(TilePositions.Line);
                break;
            case TilePositions.Line:
                SetTilePositions(TilePositions.End);
                break;
            case TilePositions.End:
                SetTilePositions(TilePositions.Start);
                break;
        }
    }

    public void LetterSelected(GameObject tileObject)
    {
        if (gameState == GameState.Playing)
        {
            Tile tile = tiles.Find(Tile => Tile.tileObject == tileObject);

            tile.tileController.selected = !tile.tileController.selected;

            bool selected = tile.tileController.selected;

            if (selected)
            {
                selectedTiles.Add(tile);
            }
            else
            {
                selectedTiles.Remove(tile);
            }

            DisplaySelectedWord();
        }
    }

    public void WordSelected()
    {
        if (gameState == GameState.Playing)
        {
            if (isSelectedWordValid)
            {
                ConvertWordToScore();

                ClearSelectedTiles();
            }
            else
            {
                ClearSelectedTiles();
            }

            DisplaySelectedWord();
        }

    }

    public void ClearSelectedTiles()
    {
        foreach (Tile tile in selectedTiles)
        {
            tile.tileController.selected = false;
        }

        selectedTiles = new List<Tile>();
    }

    #endregion

    #region Object Creation

    void LoadBaseObjectReferences()
    {
        canvasGameOverMenu = GameObject.Find(canvasGameOverMenuName);
        canvasGameOverMenu.SetActive(false);

        canvasMainMenu = GameObject.Find(canvasMainMenuName);
        canvasMainMenu.SetActive(false);
    }

    void CreateButtons()
    {
        shuffleButton = new Button();
        menuButton = new Button();

        shuffleButton.gameObject = Instantiate(Resources.Load<GameObject>(shuffleButtonObjectPath)) as GameObject;
        menuButton.gameObject = Instantiate(Resources.Load<GameObject>(menuButtonObjectPath)) as GameObject;

        shuffleButton.controller = shuffleButton.gameObject.GetComponent<ButtonController>();
        menuButton.controller = menuButton.gameObject.GetComponent<ButtonController>();

        shuffleButton.gameObject.transform.position = new Vector2(cameraBounds.xMin, cameraBounds.yMin);
        menuButton.gameObject.transform.position = new Vector2(cameraBounds.xMax, cameraBounds.yMin);

        shuffleButton.controller.action = new Action(ShuffleTiles);
        menuButton.controller.action = new Action(ShowGameMenu);

        shuffleButton.controller.color = shuffleButtonColor;
        menuButton.controller.color = menuButtonColor;
    }

    void CreateSelectedWordObject()
    {
        selectedWord = new SelectedWord();

        selectedWord.gameObject = Instantiate(Resources.Load<GameObject>(wordObjectPath)) as GameObject;
        selectedWord.gameObject.name = "Selected Word";
        selectedWord.textMesh = selectedWord.gameObject.GetComponent<TextMesh>();
        selectedWord.collider = selectedWord.gameObject.GetComponent<BoxCollider2D>();
        selectedWord.gameObject.transform.position = selectedWordPosition;
        selectedWord.controller = selectedWord.gameObject.GetComponent<SelectedWordController>();
    }

    void CreateScoreObject()
    {
        score = new Score();

        score.gameObject = Instantiate(Resources.Load<GameObject>(scoreObjectPath)) as GameObject;
        score.gameObject.name = "Score";
        score.controller = score.gameObject.GetComponent<ScoreController>();
        score.textMesh = score.gameObject.GetComponent<TextMesh>();

        score.gameObject.transform.position = scorePosition;
        score.textMesh.color = scoreColor;
    }

    void CreateTimerObject()
    {
        timer = new Timer();

        timer.gameObject = Instantiate(Resources.Load<GameObject>(timerObjectPath)) as GameObject;
        timer.gameObject.name = "Timer";
        timer.controller = timer.gameObject.GetComponent<TimerController>();
        timer.textMesh = timer.gameObject.GetComponent<TextMesh>();

        timer.gameObject.transform.position = timerPosition;
        timer.textMesh.color = timerColor;
        timer.controller.timeTotal = timerSeconds;
    }

    void CreateTiles()
    {
        tiles = new List<Tile>();

        for (int i = 0; i < numberofTiles; i++)
        {
            Tile tile = new Tile();

            GameObject tileObject = Instantiate(Resources.Load(tileObjectPath)) as GameObject;
            TextMesh letter = tileObject.GetComponentInChildren<TextMesh>();
            SpriteRenderer sprite = tileObject.GetComponent<SpriteRenderer>();
            TileController tileController = tileObject.GetComponent<TileController>();

            float depth = tileDepthMultiplier * i;
            string name = tileObjectName + i.ToString("D2");

            // Set starting position
            currentTilePosition = TilePositions.FiveFour;
            tileObject.transform.position = new Vector3(tilePositions[i].FiveFour.x, tilePositions[i].FiveFour.y, depth);
            tileController.target = tileObject.transform.position;
            tileController.smoothTime = tileSmoothTime;

            // Set object name
            tileObject.name = name;

            // Set letter
            letter.text = letterList[i].ToString();

            // Set color
            sprite.color = tileUnselectedColor;

            // Assign components to Tile struct
            tile.tileObject = tileObject;
            tile.letter = letter;
            tile.sprite = sprite;
            tile.tileController = tileController;

            // Add struct to list;
            tiles.Add(tile);
        }
    }

    #endregion

    #region Object Destruction

    void DestroyButtons()
    {
        Destroy(shuffleButton.gameObject);
        Destroy(menuButton.gameObject);
    }

    void DestroySelectedWordObject()
    {
        Destroy(selectedWord.gameObject);
    }

    void DestroyScoreObject()
    {
        Destroy(score.gameObject);
    }

    void DestroyTimerObject()
    {
        Destroy(timer.gameObject);
    }

    void DestroyTiles()
    {
        foreach(Tile tile in tiles)
        {
            Destroy(tile.tileObject);
        }

        selectedTiles = new List<Tile>();
        isSelectedWordUsed = false;
        isSelectedWordValid = false;
    }

    #endregion

    #region Structs

    public struct TileLayoutPosition
    {
        public Vector3 FiveFour;
        public Vector3 Line;
        public Vector3 Start;
        public Vector3 End;
    }

    public struct Tile
    {
        public GameObject tileObject;
        public TextMesh letter;
        public SpriteRenderer sprite;
        public TileController tileController;
    }

    public struct SelectedWord
    {
        public GameObject gameObject;
        public TextMesh textMesh;
        public BoxCollider2D collider;
        public SelectedWordController controller;
    }

    public struct Score
    {
        public GameObject gameObject;
        public TextMesh textMesh;
        public ScoreController controller;
    }

    public struct Timer
    {
        public GameObject gameObject;
        public TextMesh textMesh;
        public TimerController controller;
    }

    public struct Button
    {
        public GameObject gameObject;
        public ButtonController controller;
    }

    #endregion
}