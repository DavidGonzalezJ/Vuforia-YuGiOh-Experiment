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

    //Coroutine to move the monster
    private IEnumerator MoveToCoroutine(Vector3 position, uint numPasos)
    {
        Vector3 moveTo = new Vector3((position.x - transform.position.x) / numPasos, (position.y - transform.position.y) / numPasos, (position.z - transform.position.z) / numPasos);
        for (int i = 0; i < numPasos; i++)
        {
            transform.position = new Vector3(transform.position.x + moveTo.x, transform.position.y + moveTo.y, transform.position.z + moveTo.z);
            yield return new WaitForSeconds(.01f);
        }
    
    }
    //Coroutine to orientate the monster
    private IEnumerator LookToCoroutine(Vector3 forward, uint numPasos)
    {
        Vector3 LookTo = new Vector3((forward.x - transform.forward.x) / numPasos, (forward.y - transform.forward.y) / numPasos, (forward.z - transform.forward.z) / numPasos);
        for (int i = 0; i < numPasos; i++)
        {
            transform.forward = new Vector3(transform.position.x + LookTo.x, transform.position.y + LookTo.y, transform.position.z + LookTo.z);
            yield return new WaitForSeconds(.01f);
        }

    }

    //Position has to be a little less than enemy position
    private IEnumerator AttackCoroutine(Vector3 forward, Vector3 position, Vector3 oriForward, Vector3 oriPosition, uint numPasos) {
        //Makes the monster look to the new direction (forward)
        Vector3 LookTo = new Vector3((forward.x - transform.forward.x) / numPasos, (forward.y - transform.forward.y) / numPasos, (forward.z - transform.forward.z) / numPasos);
        for (int i = 0; i < numPasos; i++)
        {
            transform.forward = new Vector3(transform.forward.x + LookTo.x, transform.forward.y + LookTo.y, transform.forward.z + LookTo.z);
            yield return new WaitForSeconds(.01f);
        }
        setMoveAnim(true);

        //Makes the monster go to the enemy position
        Vector3 moveTo = new Vector3((position.x - transform.position.x) / numPasos, (position.y - transform.position.y) / numPasos, (position.z - transform.position.z) / numPasos);
        for (int i = 0; i < numPasos; i++)
        {
            transform.position = new Vector3(transform.position.x + moveTo.x, transform.position.y + moveTo.y, transform.position.z + moveTo.z);
            yield return new WaitForSeconds(.01f);
        }
        setAttackAnim(true);
        _target.setDamageAnim(true);
        //Wait till attack animation ends
        yield return new WaitForSeconds(/*animation["clip"].length* animation["clip"].speed*/0.9f);
        setAttackAnim(false);
        _target.setDamageAnim(false);

        //Makes the monster look to the origin position
        Vector3 ToOriginPos = transform.position - oriPosition;
        LookTo = new Vector3((ToOriginPos.x - transform.forward.x) / numPasos, (ToOriginPos.y - transform.forward.y) / numPasos, (ToOriginPos.z - transform.forward.z) / numPasos);
        for (int i = 0; i < numPasos; i++)
        {
            transform.forward = new Vector3(transform.forward.x + LookTo.x, transform.forward.y + LookTo.y, transform.forward.z + LookTo.z);
            yield return new WaitForSeconds(.01f);
        }

        //Makes the monster go to the origin position
        moveTo = new Vector3((oriPosition.x - transform.position.x) / numPasos, (oriPosition.y - transform.position.y) / numPasos, (oriPosition.z - transform.position.z) / numPasos);
        for (int i = 0; i < numPasos; i++)
        {
            transform.position = new Vector3(transform.position.x + moveTo.x, transform.position.y + moveTo.y, transform.position.z + moveTo.z);
            yield return new WaitForSeconds(.01f);
        }
        setMoveAnim(false);

        //Makes the monster look to origin forward
        LookTo = new Vector3((oriForward.x - transform.forward.x) / numPasos, (oriForward.y - transform.forward.y) / numPasos, (oriForward.z - transform.forward.z) / numPasos);
        for (int i = 0; i < numPasos; i++)
        {
            transform.forward = new Vector3(transform.forward.x + LookTo.x, transform.forward.y + LookTo.y, transform.forward.z + LookTo.z);
            yield return new WaitForSeconds(.01f);
        }

        //Finishes turn
        yield return new WaitForSeconds(1.01f);
        GameManager.instance.ToEnemyTurn();
    }

    //Monster actions
    public void Attack() {
        _state = MonsterState.Attack;
        //Guarda orientación y pos inicial
        Vector3 posIni,posDest, oriIni,newOri;
        posIni = transform.position;
        posDest = _target.transform.position;
        oriIni = transform.forward;
        newOri = posDest - posIni;

        StartCoroutine(AttackCoroutine(newOri,posDest,oriIni,posIni,100));
        _state = MonsterState.Idle;
        //Encara al monstruo enemigo y cambia la animación (Corrutina con callback)

        //Se mueve hacia él y cambia la animación(Corrutina con callback)

        //Le pega la hostia (el enemigo la recibe) y cuando termina vuelve a la anim anterior(Callback)

        //Da media vuelta (Callback)
        
        //Vuelve al origen (Callback)

        //Vuelve a mirar a su sitio
    }

    IEnumerator justWaitAndGoToEnemyTurn() {
        yield return new WaitForSeconds(3.0f);
        GameManager.instance.ToEnemyTurn();
    }

    public void Defend() {
        _state = MonsterState.Def;
        setDefAnim(true);
        if(GameManager.instance.getCurrentPlayer() == PlayerId.One)
            StartCoroutine(justWaitAndGoToEnemyTurn());
    }

    public void StopDefend() {
        _state = MonsterState.Idle;
        setDefAnim(false);
        if (GameManager.instance.getCurrentPlayer() == PlayerId.One)
            StartCoroutine(justWaitAndGoToEnemyTurn());
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
                GameManager.instance.getCurrentMonster().setTarget(this);
                GameManager.instance.getCurrentMonster().Attack();
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
