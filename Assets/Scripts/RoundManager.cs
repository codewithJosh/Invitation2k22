using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class RoundManager : MonoBehaviour
{

    [SerializeField] private Image resultHUD;
    [SerializeField] private Image levelExpFillHUD;
    [SerializeField] private Sprite[] resources;
    [SerializeField] private TextMeshProUGUI initialCountdownUIText;
    [SerializeField] private TextMeshProUGUI roundCountdownUIText;
    [SerializeField] private TextMeshProUGUI roundStepUIText;
    [SerializeField] private TextMeshProUGUI goldDivisionUIText;
    [SerializeField] private TextMeshProUGUI silverDivisionUIText;
    [SerializeField] private TextMeshProUGUI bronzeDivisionUIText;
    [SerializeField] private TextMeshProUGUI resultUIText;
    [SerializeField] private TextMeshProUGUI resultCountdownUIText;
    [SerializeField] private TextMeshProUGUI levelExpUIText;
    [SerializeField] private TextMeshProUGUI levelExpUpUIText;
    [SerializeField] private TextMeshProUGUI levelUIText;
    [SerializeField] private Toggle audioUIButton;

    [HideInInspector] public enum GameStates { idle, pause, restart, startMenu, result};
    [HideInInspector] public GameStates gameState = GameStates.idle;

    private IEnumerator coroutine;
    [HideInInspector] public Vector3 respawnPoint;

    private bool isDied;
    private float levelEXP;
    private float nextLevelEXP;
    private int initialCountdown;
    private int roundCountdown;
    private int lastRoundStep;
    private int roundStep;
    private int isMale;
    private int lastMapUsed;
    private int lastDivisionUsed;
    private int level;
    [HideInInspector] public string lastTag;

    private void Awake()
    {

        respawnPoint = new Vector3(transform.position.x, GetRandomYPosition(), transform.position.z);
        transform.position = respawnPoint;
        FindObjectOfType<PlayerMovement>().playerState = PlayerMovement.PlayerStates.idle;

    }

    private float GetRandomYPosition()
    {

        return Random.Range(0, 1) switch
        {

            0 => 21.86727f,

            1 => 11.83727f,

            _ => 1.782271f,

        };

    }

    void Start()
    {

        audioUIButton.isOn = true;

        initialCountdown = 3;
        coroutine = CountdownToStart();

        FindObjectOfType<PlayerManager>().LoadPlayer();

        isMale = FindObjectOfType<PlayerManager>().isMale;
        lastMapUsed = FindObjectOfType<PlayerManager>().lastMapUsed;
        lastDivisionUsed = FindObjectOfType<PlayerManager>().lastDivisionUsed;
        roundCountdown = FindObjectOfType<PlayerManager>().MAP_INT[isMale, lastMapUsed, lastDivisionUsed, 1];
        roundStep = FindObjectOfType<PlayerManager>().lastRoundStepUsed;
        level = FindObjectOfType<PlayerManager>().level;
        levelEXP = FindObjectOfType<PlayerManager>().levelEXP;
        nextLevelEXP = FindObjectOfType<PlayerManager>().nextLevelEXP;

        int goldDivison = FindObjectOfType<PlayerManager>().MAP_INT[isMale, lastMapUsed, 2, 0];
        int silverDivision = FindObjectOfType<PlayerManager>().MAP_INT[isMale, lastMapUsed, 1, 0];
        int bronzeDivision = FindObjectOfType<PlayerManager>().MAP_INT[isMale, lastMapUsed, 0, 0];

        goldDivisionUIText.text = GetTime(goldDivison);
        silverDivisionUIText.text = GetTime(silverDivision);
        bronzeDivisionUIText.text = GetTime(bronzeDivision);

        StartCoroutine(coroutine);
        OnStepState();

    }

    IEnumerator CountdownToStart()
    {

        initialCountdownUIText.color = Color.green;

        while (initialCountdown > 0)
        {

            initialCountdownUIText.text = "READY---" + initialCountdown.ToString();

            yield return new WaitForSeconds(1f);

            initialCountdown--;

        }

        initialCountdownUIText.color = Color.red;
        initialCountdownUIText.text = "GO!";
        FindObjectOfType<PlayerMovement>().canMove = true;
        coroutine = TimeLeftToStart();
        StartCoroutine(coroutine);

        yield return new WaitForSeconds(1f);

        initialCountdownUIText.gameObject.SetActive(false);

    }

    IEnumerator TimeLeftToStart()
    {

        while (roundCountdown >= 0)
        {

            float minutes = Mathf.FloorToInt(roundCountdown / 60);
            float seconds = Mathf.FloorToInt(roundCountdown % 60);

            if (minutes == 0 && seconds < 16)
            {

                roundCountdownUIText.color = Color.red;

            }

            roundCountdownUIText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            yield return new WaitForSeconds(1f);

            roundCountdown--;

        }

        FindObjectOfType<PlayerMovement>().canMove = false;
        FindObjectOfType<PlayerMovement>().isDying = true;
        OnMissionFailed();

        yield return new WaitForSeconds(1f);

    }

    void Update()
    {

        if (SimpleInput.GetButtonDown("OnIdle"))
        {

            gameState = GameStates.idle;
            StartCoroutine(coroutine);

        }

        if (SimpleInput.GetButtonDown("OnPause"))
        {

            gameState = GameStates.pause;
            StopCoroutine(coroutine);

        }

        if (SimpleInput.GetButtonDown("OnRestart"))
        {

            gameState = GameStates.restart;

        }

        if (SimpleInput.GetButtonDown("OnStartMenu"))
        {

            gameState = GameStates.startMenu;

        }

        if (gameState == GameStates.restart)
        {

            if (SimpleInput.GetButtonDown("OnAffirmativeRestart"))
            {

                gameState = GameStates.pause;
                PlayerPrefs.SetInt("index", lastMapUsed + 3);
                SceneManager.LoadScene(1);

            }

            if (SimpleInput.GetButtonDown("OnNegativeRestart"))
            {

                gameState = GameStates.pause;

            }

        }

        if (gameState == GameStates.startMenu)
        {

            if (SimpleInput.GetButtonDown("OnAffirmativeStartMenu"))
            {

                gameState = GameStates.pause;
                PlayerPrefs.SetInt("index", 2);
                SceneManager.LoadScene(1);

            }

            if (SimpleInput.GetButtonDown("OnNegativeStartMenu"))
            {

                gameState = GameStates.pause;

            }

        }


        FindObjectOfType<GameManager>().GetAnimator.SetInteger("gameState", (int) gameState);

    }

    void FixedUpdate()
    {

        if (transform.position.y < -20f)
        {

            if (!isDied)
            {

                isDied = true;
                FindObjectOfType<PlayerMovement>().isDied = true;
                
            }

            int countdown = 1;
            StartCoroutine(OnRespawnToStart(countdown));

        }

    }

    public void OnStepState()
    {

        lastRoundStep += 1;
        roundStepUIText.text = lastRoundStep + "/" + roundStep;

    }

    private string GetTime(int _divisionTimeInSeconds)
    {

        float minutes = Mathf.FloorToInt(_divisionTimeInSeconds / 60);
        float seconds = Mathf.FloorToInt(_divisionTimeInSeconds % 60);

        return string.Format("{0:00}:{1:00}", minutes, seconds);

    }

    IEnumerator OnRespawnToStart(int _countdown)
    {

        while (_countdown > 0)
        {

            yield return new WaitForSeconds(1f);

            _countdown--;

        }

        isDied = false;
        FindObjectOfType<PlayerMovement>().isDied = false;
        transform.position = respawnPoint;

    }

    private void OnMissionFailed()
    {

        gameState = GameStates.result;
        resultHUD.sprite = resources[0];
        resultUIText.text = "TIME TO BEAT";
        resultUIText.color = Color.red;
        resultCountdownUIText.text = GetTime(FindObjectOfType<PlayerManager>().MAP_INT[isMale, lastMapUsed, lastDivisionUsed, 1]);
        resultCountdownUIText.color = Color.red;
        int countdown = 2;
        StartCoroutine(OnLevelExpUpToStart(countdown, false));

    }

    public void OnMissionSuccess()
    {

        StopCoroutine(coroutine);
        gameState = GameStates.result;
        resultHUD.sprite = resources[1];
        resultUIText.text = "NEW RECORD";
        resultUIText.color = Color.green;
        resultCountdownUIText.text = roundCountdownUIText.text;
        resultCountdownUIText.color = Color.green;
        int countdown = 2;
        StartCoroutine(OnLevelExpUpToStart(countdown, true));

        

    }

    IEnumerator OnLevelExpUpToStart(int _countdown, bool isSuccess)
    {

        levelUIText.text = string.Format("LEVEL\n{0}", level);
        levelExpUIText.text = string.Format("{0} / {1}", levelEXP.ToString(), nextLevelEXP.ToString());
        levelExpFillHUD.fillAmount = levelEXP / nextLevelEXP;

        if (levelEXP + GetDivisionEXP(lastDivisionUsed) > nextLevelEXP)
        {

            level++;

        }

        while (_countdown > 0)
        {

            yield return new WaitForSeconds(1f);

            _countdown--;

        }

        levelEXP += isSuccess
                ? GetDivisionEXP(lastDivisionUsed)
                : 750;
        
        levelUIText.text = string.Format("LEVEL\n{0}", level);
        levelExpUIText.text = string.Format("{0} / {1}", levelEXP.ToString(), nextLevelEXP.ToString());
        levelExpFillHUD.fillAmount = levelEXP / nextLevelEXP;
        levelExpUpUIText.text = string.Format("+{0}", levelEXP);

    }

    private int GetDivisionEXP(int _division)
    {

        return _division switch
        {

            1 => 300,

            2 => 500,

            _ => 150,

        };

    }

}
