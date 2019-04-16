using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerId { One, Two };
public enum MonsterState { Idle, Moving, Def, Attack, Damaged };
public enum GameState { MonsterSet, MonsterSelection, OptionSelection, TargetSelection, Player2Turn };

// Manages the states of the game, the gui and the turns
public class GameManager : MonoBehaviour
{
    //Singleton
    public static GameManager instance = null;

    //Current game state
    private GameState _currentGameState;
    public GameState GetGameState() { return _currentGameState; }

    
    //Text to be shown in every turn
    public RawImage monsterSelect, yourTurn, enemyTurn;
    //Ticks to be shown when a monster is detected
    public RawImage redTick, greenTick;
    //The behaviour that shows texts
    TitleShow ts;
    public TitleShow getTS() { return ts; }

    //Trackable cards
    [SerializeField]
    private TrackableCard kuriboh;
    [SerializeField]
    private TrackableCard jinzo;


    //Players in the game
    Player _playerOne;
    Player _playerTwo;
    PlayerId _currentTurn;
    public PlayerId getCurrentPlayer() {
        return _currentTurn;
    }

    //Awake to create the singleton
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
    }

        // Start is called before the first frame update
    void Start()
    {
        //Inits the ts
        ts = this.gameObject.GetComponent<TitleShow>();

        //Creates both players
        _playerOne = new Player();
        _playerOne.initPlayer(PlayerId.One);
        _playerTwo = new Player();
        _playerTwo.initPlayer(PlayerId.Two);
        _currentTurn = PlayerId.One;

        //Inits cards
        kuriboh.initPlayers(_playerOne, _playerTwo);
        jinzo.initPlayers(_playerOne, _playerTwo);

        //Starts the monster search
        startSetPhase();
    }

    //Monster Recognition state
    [SerializeField]
    private Button endSetPhase;
    //This method will be called at the beggining of each game
    //And just once per game
    public void startSetPhase() {
        _currentGameState = GameState.MonsterSet;
        endSetPhase.gameObject.SetActive(true);
        ts.showTitle(monsterSelect);
    }


    //Monster choosing state
    [SerializeField]
    private RawImage _tapText;
    void activateTapText() { _tapText.gameObject.SetActive(true); }
    //Monster Selection only has the "tap one of your monsters" message
    //This method is called from Cancel button in action selection & End button in monster set phase
    public void ToMonsterSelection() {
        if (_currentGameState == GameState.MonsterSet || _currentGameState == GameState.Player2Turn)
        {
            endSetPhase.gameObject.SetActive(false);
            ts.showTitle(yourTurn,activateTapText);
            _currentTurn = PlayerId.One;
        }
        else
        {
            _actionText.gameObject.SetActive(false);
            Attack.gameObject.SetActive(false);
            Defend.gameObject.SetActive(false);
            Cancel.gameObject.SetActive(false);
            activateTapText();
        }
        _currentGameState = GameState.MonsterSelection;
        
        //Make monsters selectable
        clickEnabled();
    }

    //Option selection (when a monster is chosen) state
    [SerializeField]
    private RawImage _actionText;
    [SerializeField]
    private Button Attack, Defend, Cancel;
    private MonsterBehaviour _monsterSelected;
    public void setCurrentMonster(MonsterBehaviour m) { _monsterSelected = m; }
    public MonsterBehaviour getCurrentMonster() { return _monsterSelected; }
    //Option selection displays three buttons
    //This method will be called when a monster is tapped
    public void ToOptionSelection(){
        endSetPhase.gameObject.SetActive(false);
        _tapText.gameObject.SetActive(false);
        _actionText.gameObject.SetActive(true);
        Attack.gameObject.SetActive(true);
        Defend.gameObject.SetActive(true);
        Cancel.gameObject.SetActive(true);
        _currentGameState = GameState.OptionSelection;

        //Make monsters unselectable
        clickDisabled();
    }
    public void DefButton() {
        if (_monsterSelected.GetState() == MonsterState.Idle)
            _monsterSelected.Defend();
        else if (_monsterSelected.GetState() == MonsterState.Def)
            _monsterSelected.StopDefend();
    }

    //Target selection (when a monster is chosen to be attacked)
    [SerializeField]
    private RawImage _targetText;
    public void ToTargetSelection()
    {
        _actionText.gameObject.SetActive(false);
        _targetText.gameObject.SetActive(true);
        Attack.gameObject.SetActive(false);
        Defend.gameObject.SetActive(false);
        Cancel.gameObject.SetActive(false);
        _currentGameState = GameState.TargetSelection;

        //Make monsters selectable
        clickEnabled();
    }

    //Enemy turn (for now, just swaps its defense position)
    public void ToEnemyTurn()
    {
        _actionText.gameObject.SetActive(false);
        _targetText.gameObject.SetActive(false);
        Attack.gameObject.SetActive(false);
        Defend.gameObject.SetActive(false);
        Cancel.gameObject.SetActive(false);
        _currentGameState = GameState.Player2Turn;
        _currentTurn = PlayerId.Two;
        ts.showTitle(enemyTurn);
        clickDisabled();
        StartCoroutine(EnemyTurnCoroutine());
    }

    //Just an example, no AI
    IEnumerator EnemyTurnCoroutine() {
        yield return new WaitForSeconds(4.0f);
        MonsterBehaviour mons = _playerTwo.getAMoster();
        if (mons.GetState() == MonsterState.Def)
            mons.StopDefend();
        else
            mons.Defend();
        yield return new WaitForSeconds(3.0f);
        ToMonsterSelection();
    }

    //For cancel button
    public void CancelSelection()
    {
        ToMonsterSelection();
    }


    //Click on monsters enabled
    public void clickEnabled() {
        _playerOne.enableMonsterClick();
        _playerTwo.enableMonsterClick();
    }

    //Disabled
    public void clickDisabled()
    {
        _playerOne.disableMonsterClick();
        _playerTwo.disableMonsterClick();
    }
}