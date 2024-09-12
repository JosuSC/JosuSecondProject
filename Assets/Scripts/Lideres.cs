using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Lideres : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.pointerPress.transform.parent.tag == "Eli_Shane") 
        {
            Debug.Log("se llamo al metodo al hacer click");
            Eli_Shane();
        }
        else if(eventData.pointerPress.transform.parent.tag == "Tork") Tork();
    }
    public static void Eli_Shane()
    {
        Vector2 nuevaescala = new Vector2(1,1);
        Lideres InMoment = GameManager.Instancia.lidersqr1.transform.GetChild(0).GetComponent<Lideres>();
        InMoment.enabled = false;
        if(GameObject.Find("ClimaF1P1").transform.childCount != 0)
        {
            MoverObjeto(GameObject.Find("ClimaF1P1").transform,GameManager.Instancia.Cementery2.transform,nuevaescala);
        }
        if(GameObject.Find("ClimaF2P1").transform.childCount != 0)
        {
            MoverObjeto(GameObject.Find("ClimaF2P1").transform,GameManager.Instancia.Cementery2.transform,nuevaescala);
        }
        if(GameObject.Find("ClimaF3P1").transform.childCount != 0 )
        {
            MoverObjeto(GameObject.Find("ClimaF3P1").transform,GameManager.Instancia.Cementery2.transform,nuevaescala);
        }
        GameManager.Instancia.StarGame(GameManager.Instancia.CurrentPlayer);
    }
    public static void Tork()
    {
        Vector2 nuevaescala = new Vector2(1,1);
        Lideres InMoment = GameManager.Instancia.lidersqr2.transform.GetChild(0).GetComponent<Lideres>();
        InMoment.enabled = false;
        if(GameObject.Find("Cementery2").transform.childCount >= 1 && GameManager.Instancia.Hand2.transform.childCount <= 9)
        {
            int n = 10 - GameManager.Instancia.Hand2.transform.childCount;
            if(n > 2) n = 2;
            for(int x = 0 ; x < n ; x++)
            {
                MoverObjeto(GameObject.Find("Cementery2").transform.GetChild(x),GameManager.Instancia.Hand2.transform,nuevaescala);
            }
        }
        GameManager.Instancia.StarGame(GameManager.Instancia.CurrentPlayer);
    }
    private static void MoverObjeto(Transform objeto, Transform nuevaPosicion, Vector2 nuevaEscala)
    {
        objeto.transform.SetParent(nuevaPosicion);
        objeto.transform.localScale = nuevaEscala;
    }
}
