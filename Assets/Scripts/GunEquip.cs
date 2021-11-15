using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GunEquip : MonoBehaviour
{
    public enum weapon
    {
        Cannon,
        Railgun,
        Minigun
    }

    [System.Serializable]
    public class Unlock
    {
        [SerializeField] public weapon m_weapon;

        [SerializeField] public bool m_isLUnlocked;
        [SerializeField] public GameObject m_left;
        [SerializeField] public GameObject m_lbutton;

        [SerializeField] public bool m_isRUnlocked;
        [SerializeField] public GameObject m_right;
        [SerializeField] public GameObject m_rbutton;

        // [SerializeField] public Upgrade.ResourceCost m_cost;
    }

    [SerializeField] GameObject m_player;
    [SerializeField] List<Unlock> m_unlocks;

    // Start is called before the first frame update
    void Start()
    {
        if (!m_player) m_player = GameObject.Find("Player");

        foreach (Unlock _ul in m_unlocks) {
            _ul.m_lbutton.GetComponentInChildren<TextMeshProUGUI>().text += " BUY"; 
            _ul.m_rbutton.GetComponentInChildren<TextMeshProUGUI>().text += " BUY"; 
        }

        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EquipCannon(bool _isLeft)
    {
        EquipWeapon(weapon.Cannon, _isLeft);
    }

    public void EquipMinigun(bool _isLeft)
    {
        EquipWeapon(weapon.Minigun, _isLeft);
    }

    public void EquipRailgun(bool _isLeft)
    {
        EquipWeapon(weapon.Railgun, _isLeft);
    }

    public void EquipWeapon(weapon _weapon, bool _isLeft)
    {
        foreach (Unlock _ul in m_unlocks)
        {
            if (_ul.m_weapon == _weapon)
            {
                if (_isLeft)
                {
                    bool doEquip = false;

                    if (_ul.m_isLUnlocked)
                    {
                        doEquip = true;
                    }
                    else
                    {
                        // if (TryBuyGun(_ul))
                        // {
                        //     _ul.m_isLUnlocked = true;
                        //     doEquip = true;
                        // }
                    }

                    if (doEquip)
                    {
                        foreach (Unlock __unlock in m_unlocks)
                        {
                            __unlock.m_left.SetActive(false);
                        }

                        _ul.m_left.SetActive(true);
                    }

                }
                else if (!_isLeft)
                {
                    bool doEquip = false;

                    if (_ul.m_isRUnlocked)
                    {
                        doEquip = true;
                    }
                    else
                    {
                        // if (TryBuyGun(_ul))
                        // {
                        //     _ul.m_isRUnlocked = true;
                        //     doEquip = true;
                        // }
                    }

                    if (doEquip)
                    {
                        foreach (Unlock __unlock in m_unlocks)
                        {
                            __unlock.m_right.SetActive(false);
                        }

                        _ul.m_right.SetActive(true);
                    }
                }

                break;
            }
        }

        UpdateText();
    }

    // private bool TryBuyGun(Unlock _ul)
    // {
    //     bool hasAll = true;

    //     if (!m_player.GetComponent<Inventory>().DoesHave(_ul.m_cost.m_type, _ul.m_cost.m_amount))
    //     {
    //         hasAll = false;
    //     }

    //     if (hasAll)
    //     {
    //         m_player.GetComponent<Inventory>().Remove(_ul.m_cost.m_type, _ul.m_cost.m_amount);

    //         return true;
    //     }
    //     else
    //     {
    //         return false;
    //     }
    // }

    private void UpdateText()
    {
        foreach (Unlock _ul in m_unlocks)
        {
            _ul.m_lbutton.GetComponentInChildren<TextMeshProUGUI>().text = _ul.m_lbutton.GetComponentInChildren<TextMeshProUGUI>().text.Replace("BUY", _ul.m_isLUnlocked ? "EQUIP" : "BUY");
            _ul.m_rbutton.GetComponentInChildren<TextMeshProUGUI>().text = _ul.m_rbutton.GetComponentInChildren<TextMeshProUGUI>().text.Replace("BUY", _ul.m_isRUnlocked ? "EQUIP" : "BUY");
        }
    }
}
