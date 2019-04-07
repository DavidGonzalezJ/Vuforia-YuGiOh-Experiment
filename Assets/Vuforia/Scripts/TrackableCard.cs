using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.UI;

public class TrackableCard : MonoBehaviour, ITrackableEventHandler
{
    private TrackableBehaviour TB;

    [SerializeField]
    private MonsterBehaviour myMonster = null;
    [SerializeField]
    private TitleShow Ts = null;

    [SerializeField]
    private RawImage gTick,rTick = null;


    private Player playerOne, playerTwo;

    // Start is called before the first frame update
    void Start()
    {
        TB = GetComponent<TrackableBehaviour>();
        if (TB)
        {
            TB.RegisterTrackableEventHandler(this);
        }
    }

    public void initPlayers(Player p1,Player p2) {
        playerOne = p1; playerTwo = p2;
    } 

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus,TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED) {
            OnDetection();
        }
    }

    void OnDetection() {
        if (myMonster.getOrientation() >= 0)
        {
            playerOne.addMonsterToThisPlayer(myMonster);
            Ts.showTitle(gTick);
        }
        else
        {
            playerTwo.addMonsterToThisPlayer(myMonster);
            Ts.showTitle(rTick);
        }
    }
}
