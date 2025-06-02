using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Node : NghiaMono
{
    public bool isUsable;
   
    public GameObject Gem;
  
    public Node(bool _isUsable, GameObject _gem)
    {
        this.isUsable = _isUsable;
        Gem = _gem;
    }
}
