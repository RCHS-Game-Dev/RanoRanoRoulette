using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;
public class GameManagerScript : MonoBehaviour
{//TODO Migrate UI functions to UIManager
public string modTimeMessage  = "Time until New Mod";
    private UIManagerScript uiManager;
    public GameObject TMProModTimeRemaining;
    public LevelGenerator LevelGenerator;
    public int numberOfChunks;
    public Image gameOverFade;
    [SerializeField]

    [HideInInspector]
    private float delayTimeElapsed;

    public RuleTile currentLevelTile;
    public Tilemap Tilemap;
    
    public Texture2D[] sliceTextures;
    void AddMods(WheelScript script)
    {

    }
    private void UpdateModRemainingTime(TextMeshProUGUI element, float timeRemaining,  string defaultTextVal = "Time until new mod:")
    {
      
      element.text = $"{defaultTextVal} {timeRemaining.ToString("F1")}";
    }
    void GenerateLevelChunks(LevelGenerator levelGenerator, int numOfChunks)
    {

        for (int i = 0; i < numOfChunks; i++)
        {
            levelGenerator.GenerateLevelChunks(numOfChunks);
        }
    }
    void GetCurrentLevelTheme()
    {

    }
    public void GameOver()
    {
        gameOverFade.GetComponent<Animator>().SetTrigger("GameOver");

        StartCoroutine(nameof(LoadGameOverScene));
    }
    public IEnumerator LoadGameOverScene()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(1.85f);
            SceneManager.LoadScene("GameOverScene");
        }
    }


    public bool startUpDelayFinished = false;
    public GameData data;
    public GameObject WheelPrefab;
    public int modifierInterval;
    public GameObject player;
    // public GameObject wheel;
    private WheelScript wheelScript;
    float timeElapsed;
    //the time it takes before mods begin dispersal.
    public float modStartupTime;

    // Start is called before the first frame update
    void Start()
    {
        wheelScript = WheelPrefab.GetComponent<WheelScript>();
        StartCoroutine(nameof(BeginNewMod));

    }

    private IEnumerator BeginNewMod()
    {
        for (; ; )
        {
            
            for (delayTimeElapsed = 0; delayTimeElapsed < modStartupTime; delayTimeElapsed += Time.deltaTime)
            {
                yield return null;
            }
           
            // LaunchNewModifier();
            startUpDelayFinished = true;
            Debug.Log("This message should never appear twice. If it does, there is a bug in modifier generation.");
            yield break;
        }

    }
    void OnEnable()
    {
        ModifierManager.AssignModToggles(data.inspectorModToggles);

    }
    void Awake()
    {

        data.mods = ModifierManager.GenerateRandomMods(data.numOfMods);
        Debug.Log(data.mods[0]);
        Debug.Log(Color.white.ToString("F2"));

        LevelGenerator = new LevelGenerator(Tilemap, currentLevelTile, sliceTextures);
        LevelGenerator.GenerateLevelChunks(numberOfChunks);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateModRemainingTime(TMProModTimeRemaining.GetComponent<TextMeshProUGUI>(), GetTimeUntilNewMod() + (modStartupTime - delayTimeElapsed), modTimeMessage);

        //checking if we need a new modifier
        if (startUpDelayFinished)
        {
            timeElapsed += Time.deltaTime;
        }
        if (timeElapsed >= modifierInterval)
        {
            timeElapsed = 0;
            LaunchNewModifier();

        }

    }

    private float GetTimeUntilNewMod()
    {

       return startUpDelayFinished?  modifierInterval - timeElapsed : (modifierInterval - timeElapsed);//make this return the time truly waited when were hung on the thing
    }

    //: Generates a new modifier. Includes the entire phase of spawning the wheel, spinning, choosing the modifier and ending.
    void LaunchNewModifier()
    {
        //if out of mods
        if (data.numOfMods < 1)
        {
            Debug.Log("NO MORE MODS FOR YOU AAHAHAHAHAHAHAAAAAA  there wer e none to begin with.");
            modTimeMessage = "Nightmare ends in:";
            return;
        }
        var wheelInstance = Instantiate(WheelPrefab, Vector3.zero, Quaternion.identity);
        Destroy(wheelInstance, 3);
        IModifier newMod = wheelScript.Launch();
        //:the modifier is null at this point
        player.GetComponent<bettertestplayablescript>().AddModifier(newMod);

        data.numOfMods--;
        // i am having a stroke


    }
}
//todo create new comments, docs, debug? explain, function?