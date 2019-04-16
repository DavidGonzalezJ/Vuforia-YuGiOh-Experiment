using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.UI;

/// <summary>
/// This behaviour will be assigned to the markers, so when they're detected,
/// we can check it's monsters orientation and put them into player one or two team
/// </summary>
public class TrackableCard : MonoBehaviour, ITrackableEventHandler
{
    private TrackableBehaviour TB;
    private bool _alreadyAssigned;

    //Monster will be assigned in editor
    [SerializeField]
    private MonsterBehaviour myMonster = null;
    //Images to give feedback after detection
    [SerializeField]
    private RawImage gTick , rTick = null;


    private Player playerOne, playerTwo;

    // Start is called before the first frame update
    void Start()
    {
        _alreadyAssigned = false;
        TB = GetComponent<TrackableBehaviour>();
        if (TB)
        {
            TB.RegisterTrackableEventHandler(this);
        }
    }

    //This is called from the GM
    public void initPlayers(Player p1,Player p2) {
        playerOne = p1; playerTwo = p2;
    } 

    //This will be called automatically after a marker detection
    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus,TrackableBehaviour.Status newStatus)
    {
        if (!_alreadyAssigned)
        {
            if (newStatus == TrackableBehaviour.Status.TRACKED)
            {
                _alreadyAssigned = true;
                OnDetection();
                //Must disable this comp when it's job is done
                this.enabled = false;
            }
        }
    }

    //When monster is awaken, goes to a player or another
    //Shows green tick if it goes to p1, and red to p2
    void OnDetection() {
        if (myMonster.getOrientation() >= 0)
        {
            playerOne.addMonsterToThisPlayer(myMonster);
            GameManager.instance.getTS().showTitle(gTick);
        }
        else
        {
            playerTwo.addMonsterToThisPlayer(myMonster);
            GameManager.instance.getTS().showTitle(rTick);
        }
    }
}
