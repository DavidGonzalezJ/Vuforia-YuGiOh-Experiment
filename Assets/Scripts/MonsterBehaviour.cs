using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Behaviour for monsters, including attack, defense and click
/// </summary>
public class MonsterBehaviour : MonoBehaviour
{
    //Collider to interact with monsters
    private CapsuleCollider col;

    //The component that manages animations
    private Animator anim;
    AnimatorClipInfo[] m_AnimatorClipInfo;

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

    //Coroutine to attack
    //Recieves the forward to the target, the target position, it's original transform and forward and the number of steps
    //to reach the destination
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
        //Position has to be a little less than enemy position
        float discount = 0.8f; // This will multiply the distance to get less close to the target
        Vector3 moveTo = new Vector3((position.x - transform.position.x) * discount / numPasos, 0, (position.z - transform.position.z) * discount / numPasos);
        for (int i = 0; i < numPasos; i++)
        {
            transform.position = new Vector3(transform.position.x + moveTo.x, transform.position.y, transform.position.z + moveTo.z);
            yield return new WaitForSeconds(.01f);
        }

        //Now attacks and enemy gets hurt
        setAttackAnim(true);
        //Wait till attack animation ends
        m_AnimatorClipInfo = anim.GetCurrentAnimatorClipInfo(0);
        yield return new WaitForSeconds(m_AnimatorClipInfo[0].clip.length);
        setAttackAnim(false);
        _target.setDamageAnim(true);
        yield return new WaitForSeconds(0.5f);
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
        moveTo = new Vector3((oriPosition.x - transform.position.x) / numPasos, 0, (oriPosition.z - transform.position.z) / numPasos);
        for (int i = 0; i < numPasos; i++)
        {
            transform.position = new Vector3(transform.position.x + moveTo.x, transform.position.y, transform.position.z + moveTo.z);
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
        yield return new WaitForSeconds(1.0f);
        _state = MonsterState.Idle;
        GameManager.instance.ToEnemyTurn();
    }

    //Monster actions
    public void Attack() {
        _state = MonsterState.Attack;
        //Gets postions & orientations (actual and to target)
        Vector3 posIni,posDest, oriIni,newOri;
        posIni = transform.position;
        posDest = _target.transform.position;
        oriIni = transform.forward;
        newOri = posDest - posIni;

        //Launches coroutine
        StartCoroutine(AttackCoroutine(newOri,posDest,oriIni,posIni,100));
    }

    //Coroutine called from defense functions
    IEnumerator justWaitAndGoToEnemyTurn() {
        yield return new WaitForSeconds(3.0f);
        GameManager.instance.ToEnemyTurn();
    }

    //Command defend
    public void Defend() {
        _state = MonsterState.Def;
        setDefAnim(true);
        if(GameManager.instance.getCurrentPlayer() == PlayerId.One)
            StartCoroutine(justWaitAndGoToEnemyTurn());
    }

    //If it's defending you call this one to stop
    public void StopDefend() {
        _state = MonsterState.Idle;
        setDefAnim(false);
        if (GameManager.instance.getCurrentPlayer() == PlayerId.One)
            StartCoroutine(justWaitAndGoToEnemyTurn());
    }


    //Functions that manage animations
    void setMoveAnim(bool activate) { anim.SetBool("Moving", activate); }
    void setDefAnim(bool activate) { anim.SetBool("Defending", activate); }
    void setAttackAnim(bool activate) { anim.SetBool("Attacking", activate); }
    void setDamageAnim(bool activate) { anim.SetBool("Damaged", activate); }


    

    //This will be called at the beggining of the game to assign the monster to a player based on it's orientation
    public float getOrientation() {
        Vector3 f = transform.forward;
        return f.z;
    }

    //This one will be called after the monster is assigned to a player
    public void setOwner(Player p)
    {
        _owner = p;
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
 
}
