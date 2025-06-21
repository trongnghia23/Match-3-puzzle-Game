using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Node
{
    public bool isUsable;
    public GameObject Gem;

    public Node(bool isUsable, GameObject gem)
    {
        this.isUsable = isUsable;
        this.Gem = gem;
       
    }
}