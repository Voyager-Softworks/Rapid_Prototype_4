using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BountyBoard : MonoBehaviour
{
    [Header("Board")]
    public GameObject bountyBoard;
    public GameObject canvas;

    [Header("Main")]
    public GameObject mainPannel;
    public GameObject mainTitle;

    [Header("Missions")]
    public List<GameObject> missionButtons;

    [Header("Info")]
    public GameObject infoPannel;
    public GameObject infoTitle;
    public GameObject icon;
    public GameObject infoLevel;
    public GameObject infoReward;
    public GameObject infoPenalty;
    public GameObject infoAcceptButton;
}