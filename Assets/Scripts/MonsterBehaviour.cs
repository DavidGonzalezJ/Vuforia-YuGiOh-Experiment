using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBehaviour : MonoBehaviour
{
    //Collider to interact with monsters
    private CapsuleCollider col;

    //The component that manages animations
    private Animator anim;

    //You can/can't click it
    bool _clickable;
    //Is the monster selected
    bool _selected;
    //The current state of the monster
    MonsterState _state;
    public MonsterState GetState() { return _state; }

    //Owner
    Player _owner;

    //Target of the attack
    MonsterBehaviour _target = null;
    public void setTarget(MonsterBehaviour m) { _target = m; }

    //Monster actions
    public void Attack() {
        _state = MonsterState.Attack;
        //Guarda orientación y pos inicial

        //Encara al monstruo enemigo y cambia la animación (Corrutina con callback)

        //Se mueve hacia él y cambia la animación(Corrutina con callback)

        //Le pega la hostia (el enemigo la recibe) y cuando termina vuelve a la anim anterior(Callback)

        //Da media vuelta (Callback)
        
        //Vuelve al origen (Callback)

        //Vuelve a mirar a su sitio
    }

    public void Defend() {
        _state = MonsterState.Def;
        setDefAnim(true);
    }

    public void StopDefend() {
        _state = MonsterState.Idle;
        setDefAnim(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider>();
        col.enabled = false;
        _clickable = false;
        _selected = false;
        _state = MonsterState.Idle;
    }

    //Functions that manage animations
    void setMoveAnim(bool activate) { anim.SetBool("Moving", activate); }
    void setDefAnim(bool activate) { anim.SetBool("Defending", activate); }
    void setAttackAnim(bool activate) { anim.SetBool("Attacking", activate); }
    void setDamageAnim(bool activate) { anim.SetBool("Damaged", activate); }


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
                GameManager.instance.setCurrentMonster(this);
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
                //Sets the target
                setTarget(GameManager.instance.getCurrentMonster());
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
