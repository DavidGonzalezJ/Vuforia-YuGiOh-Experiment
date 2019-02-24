﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MonsterBehaviour : MonoBehaviour
{
    //You can/can't click it
    bool _clickable;
    //Is the monster selected
    bool _selected;
    //The current state of the monster
    MonsterState _state;


    // Start is called before the first frame update
    void Start()
    {
        _clickable = false;
        _selected = false;
        _state = MonsterState.Idle;
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/
}