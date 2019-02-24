using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerId { One, Two };
public enum MonsterState { Idle, Moving, Def, Attack, Damaged };
public enum GameState { MonsterSelection, OptionSelection, Player2Turn };

public class GameManager : MonoBehaviour
{
    //Players in the game
    Player _playerOne;
    Player _playerTwo;
    PlayerId _currentTurn;

    //List of states
    List<State> states;

    // Start is called before the first frame update
    void Start()
    {
        //Creates both players
        _playerOne = new Player();
        _playerOne.initPlayer(PlayerId.One);
        _playerTwo = new Player();
        _playerTwo.initPlayer(PlayerId.Two);
        _currentTurn = PlayerId.One;



    }

    //This is the main loop of the game
    //All game states are managed here
    void RunGame() {

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

    MonsterSetState(Player one, Player two, PlayerId actualOne) : base(one,two, actualOne) {
    }
    public override void stateTick() {
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

    MonsterSelectState(Player one, Player two, PlayerId actualOne) : base(one, two, actualOne)
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

    MonsterActionState(Player one, Player two, PlayerId actualOne) : base(one, two, actualOne)
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

    Player2DummyState(Player one, Player two, PlayerId actualOne) : base(one, two, actualOne)
    {
    }
    public override void stateTick()
    {
        return;
    }
}