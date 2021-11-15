using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEditor;

[Serializable]
public class UpgradeLevel
{
    public List<Resources.PlayerResource> cost;
    public UnityEvent onUpgrade;
}
