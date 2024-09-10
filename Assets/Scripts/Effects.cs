using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
    public static void Aumento(Transform MyPosition)
    {
        for(int x = 0 ; x < MyPosition.parent.childCount-1 ; x ++)
        {
            VisualCard OnTarjet = MyPosition.parent.GetChild(x).GetComponent<VisualCard>();
            if(OnTarjet.card.type == "oro" || OnTarjet.card.type == "Plata")
            {
                // OnTarjet.card.Power = OnTarjet.card.Power += 2;
                OnTarjet.Power.text = (OnTarjet.card.power += 2).ToString();
            }
            else continue;
        }
    }
    public static void ClimaResta(Transform MyPosition)
    {
        Transform ToAfect = null;
        if(MyPosition.tag == "Clima1" && GameObject.Find("AsedioP1") is not null) ToAfect = GameObject.Find("AsedioP1").transform;
        else if(MyPosition.tag == "Clima2" && GameObject.Find("DistanciaP1") is not null) ToAfect = GameObject.Find("DistanciaP1").transform;
        else if(MyPosition.tag == "Clima3" && GameObject.Find("CuerpoP1") is not null) ToAfect = GameObject.Find("CuerpoP1").transform;
        else if(MyPosition.tag == "Clima4" && GameObject.Find("CuerpoP2") is not null) ToAfect = GameObject.Find("CuerpoP2").transform;
        else if(MyPosition.tag == "Clima5" && GameObject.Find("DistanciaP2") is not null) ToAfect = GameObject.Find("DistanciaP2").transform;
        else if(MyPosition.tag == "Clima6" && GameObject.Find("AsedioP2") is not null) ToAfect = GameObject.Find("AsedioP2").transform;

        for(int x = 0 ; x < ToAfect.childCount ; x ++)
        {
            VisualCard OnTarjet = ToAfect.GetChild(x).GetComponent<VisualCard>();
            if(OnTarjet.card.type == "Oro" || OnTarjet.card.type == "Plata")
            {
                OnTarjet.card.power = OnTarjet.card.power -= 2;
                OnTarjet.Power.text = OnTarjet.card.power.ToString();
            }
            else continue;
        }
    }
    //public static void Senuelo(Transform Father)
    //{
    //    if(Father.tag == "Clima1" || Father.tag == "Clima2" || Father.tag == "Clima2")
    //    {
    //        CustomCollider.otherCard.transform.SetParent(GameObject.Find("HandPlayer1").transform);
    //    }
    //    else
    //    {
    //        CustomCollider.otherCard.transform.SetParent(GameObject.Find("HandPlayer2").transform);
    //    }
    //}
    //public static void Despeje(Transform Father)
    //{
    //    Vector2 nuevaescala = new Vector2(1,1);

    //    if(Father.tag == "Clima1" || Father.tag == "Clima2" || Father.tag == "Clima2")
    //    {
    //        MoverObjeto(CustomCollider.otherCard.transform,GameManager.Instancia.Cementery2.transform,nuevaescala);
    //        MoverObjeto(Father.GetChild(0),GameManager.Instancia.Cementery1.transform,nuevaescala);
    //    }
    //    else
    //    {
    //        MoverObjeto(CustomCollider.otherCard.transform,GameManager.Instancia.Cementery1.transform,nuevaescala);
    //        MoverObjeto(Father.GetChild(0),GameManager.Instancia.Cementery2.transform,nuevaescala);
    //    }
    //}
    private static void MoverObjeto(Transform objeto, Transform nuevaPosicion, Vector2 nuevaEscala)
    {
        objeto.transform.SetParent(nuevaPosicion);
        objeto.transform.localScale = nuevaEscala;
    }
    public static void Oro(Transform Father)
    {
        int n = Father.GetSiblingIndex();
        if(Father.parent.tag == "Clima1P" || Father.parent.tag == "Clima2P" || Father.parent.tag == "Clima3P")
        {
            if(Father.parent.tag == "Clima1P" && n < GameObject.Find("AsedioP2").transform.childCount)
            {
                Transform NewObject = GameObject.Find("AsedioP2").transform.GetChild(n);
                NewObject.SetParent(Father.parent);
            }
            else if(Father.parent.tag == "Clima2P" && n <  GameObject.Find("DistanciaP2").transform.childCount)
            {
                Transform NewObject = GameObject.Find("DistanciaP2").transform.GetChild(n);
                NewObject.SetParent(Father.parent);
            }
            else if(n < GameObject.Find("CuerpoP2").transform.childCount)
            {
                Transform NewObject = GameObject.Find("CuerpoP2").transform.GetChild(n);
                NewObject.SetParent(Father.parent);
            }
        }
        else
        {
            if(Father.parent.tag == "Clima4P" &&  n < GameObject.Find("CuerpoP1").transform.childCount)
            {
                Transform NewObject = GameObject.Find("CuerpoP1").transform.GetChild(n);
                NewObject.SetParent(Father.parent);
            }
            else if(Father.parent.tag == "Clima5P" && n < GameObject.Find("DistanciaP1").transform.childCount)
            {
                Transform NewObject = GameObject.Find("DistanciaP1").transform.GetChild(n);
                NewObject.SetParent(Father.parent);
            }
            else if(n < GameObject.Find("AsedioP1").transform.childCount)
            {
                Transform NewObject = GameObject.Find("AsedioP1").transform.GetChild(n);
                NewObject.SetParent(Father.parent);
            }
        }
    }
    public static void Plata(Transform Father)
    {

        Vector2 nuevaescala = new Vector2(1,1);

        if(Father.parent.tag == "Clima1P" || Father.parent.tag == "Clima2P" || Father.parent.tag == "Clima3P")
        {
            if(Father.parent.tag == "Clima1P" && GameObject.Find("AsedioP2").transform.childCount != 0)
            {
                MoverObjeto(GameObject.Find("AsedioP2").transform.GetChild(0),GameManager.Instancia.Cementery2.transform,nuevaescala);
            }
            else if(Father.parent.tag == "Clima2P" && GameObject.Find("DistanciaP2").transform.childCount != 0)
            {
                MoverObjeto(GameObject.Find("DistanciaP2").transform.GetChild(0),GameManager.Instancia.Cementery2.transform,nuevaescala);
            }
            else if(GameObject.Find("CuerpoP2").transform.childCount != 0)
            {
                MoverObjeto(GameObject.Find("CuerpoP2").transform.GetChild(0),GameManager.Instancia.Cementery2.transform,nuevaescala);
            }
        }
        else
        {
            if(Father.parent.tag == "Clima4P" &&  GameObject.Find("CuerpoP1").transform.childCount != 0)
            {
                MoverObjeto(GameObject.Find("CuerpoP1").transform.GetChild(0),GameManager.Instancia.Cementery1.transform,nuevaescala);
            }
            else if(Father.parent.tag == "Clima5P" && GameObject.Find("DistanciaP1").transform.childCount != 0)
            {
                MoverObjeto(GameObject.Find("DistanciaP1").transform.GetChild(0),GameManager.Instancia.Cementery1.transform,nuevaescala);
            }
            else if(GameObject.Find("AsedioP1").transform.childCount != 0)
            {
                MoverObjeto(GameObject.Find("AsedioP1").transform.GetChild(0),GameManager.Instancia.Cementery1.transform,nuevaescala);
            }
        }
    }
}
