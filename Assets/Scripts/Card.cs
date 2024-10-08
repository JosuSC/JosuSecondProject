//using Skyrim_Interpreter;
using Skyrim_Interpreter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Timeline.TimelinePlaybackControls;

[System.Serializable]
public enum Faction { Dovakin, Alduin }
public enum CardEffects {Leader, oro, plata, clima, aumento, senuelo,distancia,asedio,guerrero ,despeje, No_Effect,Created };

[CreateAssetMenu(fileName = "newCard" , menuName = "Card")]
public class Card : ScriptableObject
{
    //Campos
    public string name;
    public Sprite imagen;
    public int power;
    public string type;
    public bool IsCreated;
    public CardEffects EffectType;
    SpriteRenderer Megacarta;
    GameObject Gigant;
    public string faction;
    public string[] Range;
    public List<ASTnode> OnActivation;
    public bool Owner;
    public Card()
    {
        this.OnActivation = new List<ASTnode>();
        this.Range = new string[0];
        IsCreated = true;
    }
    //private void OnMouseEnter()
    //{
    //    Gigant.transform.localScale = new Vector3(5, 5, 5);
    //    Megacarta.sprite = GetComponent<SpriteRenderer>().sprite;
    //}
    public void ActivateEffect(GameObject DroppedCard)
    {
        if (EffectType == CardEffects.aumento)
        {
            Effects.Aumento(DroppedCard.transform);
        }
        else if (EffectType == CardEffects.oro)
        {
            Effects.Oro(DroppedCard.transform);
        }
        else if (EffectType == CardEffects.plata)
        {
            Effects.Plata(DroppedCard.transform);
        }
        else if (EffectType == CardEffects.clima)
        {
            Effects.ClimaResta(DroppedCard.transform.parent);
        }
        else if (EffectType == CardEffects.senuelo)
        {
            Effects.Senuelo(DroppedCard.transform.parent);
        }
        else if (EffectType == CardEffects.despeje) 
        {
            Effects.Despeje(DroppedCard.transform.parent);
        }
        else if (EffectType == CardEffects.Created)
        {
            Debug.Log("Entro a realizar el effecto");
            GameManager game = GameManager.Instancia;
            if (game is null) Debug.Log("Es   nuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuulllllllllllllllllllllllllllllll");
            game.EffectsForCards(this);
        }
    }
    private void OnMouseExit()
    {
        Gigant.transform.localScale = Vector3.zero; 
    }
     void Start()
     {
        Gigant = GameObject.Find("Mega Carta");
        Megacarta = Gigant.GetComponent<SpriteRenderer>();
        Gigant.transform.localScale = Vector3.zero;
     }
    public bool owner { get { return Owner; } }
}
