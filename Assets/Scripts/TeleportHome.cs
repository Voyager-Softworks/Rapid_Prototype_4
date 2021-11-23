using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TeleportHome : MonoBehaviour
{
    private GameObject persistent;
    public float holdTime = 1.0f;
    public float outDelay = 0.5f;
    public float inDelay = 0.5f;
    private bool isTeleporting = false;
    private float timeCopy = 0;
    private GameObject holdParticles = null;
    public GameObject holdPrefab;
    public GameObject outPrefab;
    public GameObject inPrefab;
    private bool loadLock = true;
    private LevelManager.LevelType goingTo = LevelManager.LevelType.HUB;

    private void Start() {
        timeCopy = holdTime;

        if (!persistent)
        {
           persistent = DontDestroy.instance;
        }

        if (Keyboard.current.rKey.isPressed) loadLock = true;

        Instantiate(inPrefab, transform.position, Quaternion.identity, null);
        
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        //loop through all of the weapons and disable them
        foreach (PlayerWeapons.WeaponEquip weapon in GetComponent<PlayerWeapons>().leftWeapons)
        {
            weapon.weapon.GetComponent<SpriteRenderer>().enabled = false;
        }
        foreach (PlayerWeapons.WeaponEquip weapon in GetComponent<PlayerWeapons>().rightWeapons)
        {
            weapon.weapon.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
    private void Update() {
        if (inDelay > 0){
            inDelay -= Time.deltaTime;
            
            if (inDelay <= 0){
                GetComponent<Rigidbody2D>().isKinematic = false;
                GetComponent<PlayerMovement>().enabled = true;
                GetComponent<SpriteRenderer>().enabled = true;
                //loop through all of the weapons and disable them
                foreach (PlayerWeapons.WeaponEquip weapon in GetComponent<PlayerWeapons>().leftWeapons)
                {
                    weapon.weapon.GetComponent<SpriteRenderer>().enabled = true;
                }
                foreach (PlayerWeapons.WeaponEquip weapon in GetComponent<PlayerWeapons>().rightWeapons)
                {
                    weapon.weapon.GetComponent<SpriteRenderer>().enabled = true;
                }
            }
        }

        if (isTeleporting){
            outDelay -= Time.deltaTime;

            if (outDelay <= 0) {
                isTeleporting = false;
                outDelay = 0.5f;
                if (persistent) {
                    persistent.GetComponent<SceneController>().LoadLevel(goingTo);
                }
            }

            return;
        }

        if (Keyboard.current.rKey.isPressed) {
            if (!loadLock){
                holdTime -= Time.deltaTime;
                if (!holdParticles) {
                    holdParticles = Instantiate(holdPrefab, transform.position, Quaternion.identity, transform);
                }
            }
        }
        else{
            loadLock = false;
            holdTime = timeCopy;
            if (holdParticles) {
                Destroy(holdParticles);
            }
        }

        if (holdTime <= 0) {
            TeleportOut();
        }
    }

    public void TeleportOut(LevelManager.LevelType level = LevelManager.LevelType.HUB) {
        goingTo = level;
        holdTime = timeCopy;
        Instantiate(outPrefab, transform.position, Quaternion.identity, transform);
        isTeleporting = true;
        if (holdParticles) {
            Destroy(holdParticles);
        }

        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        //loop through all of the weapons and disable them
        foreach (PlayerWeapons.WeaponEquip weapon in GetComponent<PlayerWeapons>().leftWeapons)
        {
            weapon.weapon.SetActive(false);
        }
        foreach (PlayerWeapons.WeaponEquip weapon in GetComponent<PlayerWeapons>().rightWeapons)
        {
            weapon.weapon.SetActive(false);
        }
    }
}
