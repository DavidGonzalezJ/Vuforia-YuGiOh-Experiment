using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerId { One, Two };
public enum MonsterState { Idle, Moving, Def, Attack, Damaged };
public enum GameState { MonsterSet, MonsterSelection, OptionSelection, TargetSelection, Player2Turn };

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

    //List of states
    List<State> _states;

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
        //Inits _states
        _states = new List<State>();


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


        //Game starts in monster set phase
        //(the moment when the game assigns each player monsters
        // based on their orientation)
        _states.Add(new MonsterSetState(_playerOne, _playerTwo, _currentTurn));

        //Starts the monster search
        startSetPhase();
    }

    //This is the main loop of the game
    //All game states are managed here
    void RunGame() {

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

    //Target selection (when a monster is chosen to attack)
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

    ////Monster actions
    public void MonsterAttack()
    {
        //Should create another state
    }

    public void MonsterDefend()
    {
        //Make the animation and stay defending
    }

    public void CancelSelection()
    {
        ToMonsterSelection();
    }


    //Enemy turn
    //  . . .


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

//Abstract class for each one of the states
abstract class State {
    protected State(Player one, Player two, PlayerId actualOne) {
        _playerOne = one;
        _playerTwo = two;
        _actualPlayer = actualOne;
    }

    //Contains the update that will be implemented for each class
    public abstract void stateTick();

    protected Player _playerOne;
    protected Player _playerTwo;
    protected PlayerId _actualPlayer;
}

/// <summary>
/// This is the first state of all. I start dividing the monsters between the
/// player one and the player two.
/// I'm doing this selection by checking each monster orientation.
/// When it's done, click the accept button and next state will be pushed.
/// </summary>
class MonsterSetState : State {

    public MonsterSetState(Player one, Player two, PlayerId actualOne) : base(one,two, actualOne) {
    }
    public override void stateTick() {
        //If a monster is detected, checks its position and assigns it
        // to the player it belongs


        return;
    }
}


/// <summary>
/// This is the first state of the actual game.
/// In this state the current player has to chose one of his monsters.
/// The selected one will have some visual feedback and next state will be pushed.
/// </summary>
class MonsterSelectState : State
{

    public MonsterSelectState(Player one, Player two, PlayerId actualOne) : base(one, two, actualOne)
    {
    }

    public override void stateTick()
    {
        return;
    }
}

/// <summary>
/// This is the second state of the game.
/// Here you have to select an option for the chosen monster to do.
/// If you chose attack, you'll need to chose then the enemy monster you want to attack.
/// When the action is finished, the turn goes to the next player.
/// </summary>
class MonsterActionState : State
{

    public MonsterActionState(Player one, Player two, PlayerId actualOne) : base(one, two, actualOne)
    {
    }
    public override void stateTick()
    {
        return;
    }
}

/// <summary>
/// It isn't an actual state of the game. It's just used for experiments without a player 2.
/// </summary>
class Player2DummyState : State
{

    public Player2DummyState(Player one, Player two, PlayerId actualOne) : base(one, two, actualOne)
    {
    }
    public override void stateTick()
    {
        return;
    }
}