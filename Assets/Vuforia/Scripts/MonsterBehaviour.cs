using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBehaviour : MonoBehaviour
{
    //Collider to interact with monsters
    CapsuleCollider col;

    //You can/can't click it
    bool _clickable;
    //Is the monster selected
    bool _selected;
    //The current state of the monster
    MonsterState _state;
    //Owner
    Player _owner;


    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<CapsuleCollider>();
        col.enabled = false;
        _clickable = false;
        _selected = false;
        _state = MonsterState.Idle;
    }

    public void setOwner(Player p) {
        _owner = p;
    }

    public float getOrientation() {
        Vector3 f = transform.forward;
        return f.z;
    }

    //Enable collider
    public void enableSelection() {
        _clickable = true;
        col.enabled = true;
    }

    //Disable collider
    public void disableSelection()
    {
        _clickable = false;
        col.enabled = false;
    }

    //This method will be called when you touch a monster
    //If his collider is active
    private void OnMouseDown()
    {
        if (GameManager.instance.GetGameState() == GameState.MonsterSelection)
        {
            //If monster is mine (belongs to the person whom turn is playing now)
            if (GameManager.instance.getCurrentPlayer() == _owner._id)
            {
                GameManager.instance.clickDisabled();
                GameManager.instance.ToOptionSelection();
            }
            else
            {

                //SHOW MESSAGE
                Debug.Log("NOT YOUR MONSTER");
            }
        }

        else if (GameManager.instance.GetGameState() == GameState.TargetSelection)
        {
            //If monster is not mine (can be selected as a target)
            if (GameManager.instance.getCurrentPlayer() != _owner._id)
            {
                GameManager.instance.clickDisabled();
                // ATTACK
                Debug.Log("ATTACKKK");
            }
            else
            {

                //SHOW MESSAGE
                Debug.Log("CAN'T SELECT YOUR MONSTER AS A TARGET");
            }
        }
    }
    // Update is called once per frame
    /*void Update()
    {
        
    }*/
}
