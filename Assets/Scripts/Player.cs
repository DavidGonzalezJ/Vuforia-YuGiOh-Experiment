using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Player
{
    //List of monsters this player posesses
    List<MonsterBehaviour> _monsters;
    //This is player one or two
    public PlayerId _id;

    public MonsterBehaviour getAMoster() {
        return _monsters[0];
    }

    //This is called from the manager to init each player
    public void initPlayer(PlayerId pID)
    {
        _id = pID;
        _monsters = new List<MonsterBehaviour>();
    }

    public void addMonsterToThisPlayer(MonsterBehaviour monster) {
        _monsters.Add(monster);
        monster.setOwner(this);
    }

    //Enables to click it's monsters
    public void enableMonsterClick() {
        foreach (MonsterBehaviour m in _monsters) {
            m.enableSelection();
        }
    }

    //Disables to click it's monsters
    public void disableMonsterClick()
    {
        foreach (MonsterBehaviour m in _monsters)
        {
            m.disableSelection();
        }
    }
}
