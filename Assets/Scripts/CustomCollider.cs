using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class CustomCollider : MonoBehaviour
{
    public bool _enabled;
    public static bool IsCalling = false; // con esta controlo que no siempre que haya coalicion se haga swap
    public static VisualCard otherCard = null;
    void Start()
    {
        _enabled = true;
    }
    void OnTriggerEnter2D(Collider2D other) 
    {
        if( _enabled && DragDrop.IsDragDrop) 
        {
            otherCard = other.GetComponent<VisualCard>();
            VisualCard OnMyMouse = DragDrop.GetOnThisMoment();
           if(otherCard is not null && (otherCard.card.type == "Clima" || otherCard.card.type == "Oro" || otherCard.card.type == "Plata"))
           {
                 if (otherCard.card.type == "Clima"& OnMyMouse.card.type == "Despeje")
                 {
                    IsCalling = true;
                 }
                else if((otherCard.card.type == "Oro" || otherCard.card.type == "Plata") && OnMyMouse.card.type == "Senuelo")
                {   
                    IsCalling = true;
                }
           }
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        IsCalling = false;
        otherCard = null;
    }
}
