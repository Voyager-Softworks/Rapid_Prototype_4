using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerWeapons : MonoBehaviour
{
    [Serializable]
    public class WeaponEquip {
        public UpgradeManager.WeaponType weaponType = UpgradeManager.WeaponType.NONE;
        public GameObject weapon;
    }

    public List<WeaponEquip> leftWeapons = new List<WeaponEquip>();
    public List<WeaponEquip> rightWeapons = new List<WeaponEquip>();

    public void EquipWeapon(UpgradeManager.WeaponType _type, bool left)
    {
        if (left)
        {
            foreach (WeaponEquip we in leftWeapons)
            {
                if (!we.weapon) continue;

                if (we.weaponType == _type)
                {
                    we.weapon.SetActive(true);
                }
                else
                {
                    we.weapon.SetActive(false);
                }
            }
        }
        else
        {
            foreach (WeaponEquip we in rightWeapons)
            {
                if (!we.weapon) continue;
                
                if (we.weaponType == _type)
                {
                    we.weapon.SetActive(true);
                }
                else
                {
                    we.weapon.SetActive(false);
                }
            }
        }
    }
}
