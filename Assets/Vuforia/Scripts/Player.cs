using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Player
{
    //List of monsters this player posesses
    List<MonsterBehaviour> _monsters;
    //This is player one or two
    PlayerId _id;



    //This is called from the manager to init each player
    public void initPlayer(PlayerId pID)
    {
        _id = pID;
    }

    void addMonsterToThisPlayer(MonsterBehaviour monster) {
        _monsters.Add(monster);
    }
}
