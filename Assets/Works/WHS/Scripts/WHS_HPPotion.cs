using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class WHS_HPPotion : WHS_Item
{
    public int grade;

    public void UpdateGrade(int newGrade)
    {
        grade = newGrade;
        value = 50 + (grade * 50);
    }

    [PunRPC]
    protected override void DestroyItemObj()
    {
        base.DestroyItemObj();
    }
}