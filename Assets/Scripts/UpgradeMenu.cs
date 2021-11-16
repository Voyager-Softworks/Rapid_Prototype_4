using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeMenu : MonoBehaviour
{
    public GameObject platform;
    public GameObject upgradeMenu;
    public GameObject canvas;
    public GameObject mainPannel;

    [Header("Weapon")]
    public GameObject weaponButton;
    public GameObject weaponPannel;
    public List<GameObject> weaponItems;
    
    [Header("Module")]
    public GameObject moduleButton;
    public GameObject modulePannel;
    public List<GameObject> moduleItems;
}
