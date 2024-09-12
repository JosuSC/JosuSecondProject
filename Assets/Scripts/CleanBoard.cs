using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class CleanBoard : MonoBehaviour
{
    public static void Clean()
    {
        Vector2 nuevaescala = new Vector2(1, 1);

        Transform asedioP1 = GameObject.Find("AsedioP1").transform;
        Transform distanciaP1 = GameObject.Find("DistanciaP1").transform;
        Transform cuerpoP1 = GameObject.Find("CuerpoP1").transform;

        Transform asedioP2 = GameObject.Find("AsedioP2").transform;
        Transform distanciaP2 = GameObject.Find("DistanciaP2").transform;
        Transform cuerpoP2 = GameObject.Find("CuerpoP2").transform;

        Transform cementery1 = GameManager.Instancia.Cementery1.transform;
        Transform cementery2 = GameManager.Instancia.Cementery2.transform;

        MoverHijos(asedioP1, cementery1, nuevaescala);
        MoverHijos(distanciaP1, cementery1, nuevaescala);
        MoverHijos(cuerpoP1, cementery1, nuevaescala);

        MoverHijos(cuerpoP2, cementery2, nuevaescala);
        MoverHijos(distanciaP2, cementery2, nuevaescala);
        MoverHijos(asedioP2, cementery2, nuevaescala);

        MoverClima("Clima21", cementery1, nuevaescala);
        MoverClima("Clima22", cementery1, nuevaescala);
        MoverClima("Clima23", cementery1, nuevaescala);
        MoverClima("Clima11", cementery2, nuevaescala);
        MoverClima("Clima12", cementery2, nuevaescala);
        MoverClima("Clima13", cementery2, nuevaescala); 
    }
    private static void MoverHijos(Transform padre, Transform nuevaPosicion, Vector2 nuevaEscala)
    {
        for (int x = padre.childCount - 1; x >= 0; x--)
        {
            MoverObjeto(padre.GetChild(x), nuevaPosicion, nuevaEscala);
        }
    }
    private static void MoverClima(string nombre, Transform nuevaPosicion, Vector2 nuevaEscala)
    {
        Transform clima = GameObject.Find(nombre).transform;
        if (clima.childCount != 0)
        {
            MoverObjeto(clima.GetChild(0), nuevaPosicion, nuevaEscala);
        }
    }
    private static void MoverObjeto(Transform objeto, Transform nuevaPosicion, Vector2 nuevaEscala)
    {
        objeto.SetParent(nuevaPosicion);
        objeto.localScale = nuevaEscala;
    }
}