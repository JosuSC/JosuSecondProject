using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using NUnit.Framework;
using Skyrim_Interpreter;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia {get; private set;}
    public GameObject Prefab;
    public GameObject Hand1;
    public GameObject Hand2;
    public GameObject Deck1;
    public GameObject Deck2;
    public GameObject A1, A2,D1,D2,G1,G2;
    public GameObject Clima21, Clima22, Clima23, Clima11, Clima12, Clima13;
    public GameObject Cementery1;
    public GameObject Cementery2;
    //public GameObject Deck1Back;
    //public GameObject Deck2Back;
    public GameObject lidersqr1;
    public GameObject lidersqr2;
    public List<Card> CardsPlayer1 = new List<Card>();
    public List<Card> CardsPlayer2 = new List<Card>();
    public Card LiderDovakin;
    public Card LiderAlduin;
    public bool CurrentPlayer = false;
    public static int playedcard;
    void Awake()
    {
        if(Instancia is null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
        playedcard = 0;
    }
    void Start()
    {
        //Debug.Log(CardsPlayer1[0].GetComponent<CustomCollider>());
        // agregar cartas a los deck de los jugadores
        if (GameContext.Cards.Count != 0) 
        {
            foreach (var item in GameContext.Cards) 
            {
                Card addcard = item.Value;
                if (addcard.faction == "Alduin") CardsPlayer1.Add(addcard);
                else if (addcard.faction == "Dovakin") CardsPlayer2.Add(addcard);
                else { throw new Exception($"La carta {item.Key} no pertene a ninguna faccion del juego"); }
            }
        }
        PrepareGame(CardsPlayer1,Deck1);
        PrepareGame(CardsPlayer2,Deck2);
        Debug.Log("Se preparo el juego");
        InstanciarLideres();
        MixCards(Deck1.transform);
        MixCards(Deck2.transform);
        Stole(Deck1.transform,10,false);
        Stole(Deck2.transform,10,true);
        StarGame(CurrentPlayer);
        CurrentPlayer = false;
    }
    //metodo para obtener las cartas hijas de los objects del juego
    public List<Card> PasarHijos(GameObject father) 
    {
         //Debug.Log (father.transform.childCount);
        Debug.Log("no entro en no hijos");
        VisualCard[] cards = father.transform.GetComponentsInChildren<VisualCard>();
        Debug.Log("Paso por hijos");
        List<Card> output = new List<Card>();   
        for (int i = 0;i < cards.Length;i++) 
        {
            output.Add(cards[i].card);
        }
        return output;
    }
    public void EffectsForCards(Card card) 
    {
        //actualizamos el contexts
        List<Card> A1list = PasarHijos(A1);
        List<Card> A2list = PasarHijos(A2);
        List<Card> D1list = PasarHijos(D1);
        List<Card> D2list = PasarHijos(D2);
        List<Card> G1list = PasarHijos(G1);
        List<Card> G2list = PasarHijos(G2);
        List<Card> Clima21list = PasarHijos(Clima21);
        List<Card> Clima22list = PasarHijos(Clima22);
        List<Card> Clima23list = PasarHijos(Clima23);
        List<Card> Clima11list = PasarHijos(Clima11);
        List<Card> Clima12list = PasarHijos(Clima12);
        List<Card> Clima13list = PasarHijos(Clima13);
        List<Card> Graveyard1list = PasarHijos(Cementery1);
        List<Card> Graveyard2list = PasarHijos(Cementery2);
        List<Card> Deck1list = PasarHijos(Deck1);
        List<Card> Deck2list = PasarHijos(Deck2);
        List<Card> Field1list = new List<Card>();
        List<Card> Field2list = new List<Card>();
        List<Card> Boardlist = new List<Card>();
        foreach (var item in A1list)
        {
            Field1list.Add(item);
        }
        foreach (var item in D1list)
        {
            Field1list.Add(item);
        }
        foreach (var item in G1list)
        {
            Field1list.Add(item);
        }
        foreach (var item in A2list)
        {
            Field2list.Add(item);
        }
        foreach (var item in D2list)
        {
            Field2list.Add(item);
        }
        foreach (var item in G2list)
        {
            Field2list.Add(item);
        }
        foreach (var item in Field1list)
        {
            Boardlist.Add(item);
        }
        foreach (var item in Field2list)
        {
            Boardlist.Add(item);
        }
        Context newcontext = new Context(CurrentPlayer,Boardlist,this.CardsPlayer1,this.CardsPlayer2, A1list, A2list, D1list, D2list, G1list, G2list, Field1list, Field2list, Graveyard1list, Graveyard2list);
        GameContext.MiContext("context",newcontext);
        OnPlay.Play(card,newcontext);
    }
    public void InstanciarLideres()
    {
        GameObject game = GameObject.Instantiate(Prefab, lidersqr1.transform);
        VisualCard Scriptable = game.GetComponent<VisualCard>();
        Scriptable.card = LiderDovakin;
        Scriptable.InicializaCarta();
        //lidersqr1.transform.GetChild(0).AddComponent<Lideres>();
        GameObject game1 = GameObject.Instantiate(Prefab, lidersqr2.transform);
        VisualCard Scriptable1 = game1.GetComponent<VisualCard>();
        Scriptable1.card = LiderAlduin;
        Scriptable1.InicializaCarta();
        //lidersqr2.transform.GetChild(0).AddComponent<Lideres>();
    }
    public void PrepareGame(List<Card> PlayerCards , GameObject CustomDeck)
    {
        for(int item = 0 ; item < PlayerCards.Count ; item ++)
        {
            GameObject CardInstance = GameObject.Instantiate(Prefab,CustomDeck.transform);
            VisualCard Scriptable = CardInstance.GetComponent<VisualCard>();
            Scriptable.card = PlayerCards[item];
            Scriptable.InicializaCarta();
        }
    }
    public void MixCards(Transform Deck1)
    {
        Transform [] transforms = new Transform [Deck1.childCount];
        for(int x=0 ; x<transforms.Length ; x++)
        {
            transforms [x] = Deck1.GetChild(x);
        }
        System.Random Random= new System.Random();
        for(int x=0 ; x<transforms.Length-1 ; x++)
        {
            int n = Random.Next(0,transforms.Length-1);
            Transform Temporal = transforms[n];
            transforms[n] = transforms[x];
            transforms[x] = Temporal; 
        }
        for(int x=0 ; x<transforms.Length ; x++)
        {
            transforms[x].SetParent(null);
            transforms[x].SetParent(Deck1);
        }
    }
    public void Stole(Transform Father, int n, bool playertostole)
    {
        Vector2 nuevaescala = new Vector2(1,1);
        
        if(!playertostole)
        {
            Transform [] hijos = new Transform[n];
            for(int x=0 ; x<n ; x++)
            {
                hijos[x] = Father.GetChild(x).transform;
                GameObject hijo = hijos[x].gameObject;
                hijo.SetActive(true);
                hijo.GetComponent<DragDrop>().enabled = true ;
            }
            for(int x=0 ; x<n ; x++)
            {
                hijos[x].SetParent(Hand1.transform);
                MoverObjeto(hijos[x] , Hand1.transform , nuevaescala);
            }
        }
        else
        {
            Transform [] hijos = new Transform[n];
            for(int x = 0 ; x < n ; x ++)
            {
                hijos[x] = Father.GetChild(x).transform;
                GameObject hijo = hijos[x].gameObject;
                hijo.SetActive(true);
                hijo.GetComponent<DragDrop>().enabled = true ;
            }
            for(int x = 0 ; x < n ; x ++)
            {
                hijos[x].SetParent(Hand2.transform);
                MoverObjeto(hijos[x] , Hand2.transform , nuevaescala);
            }
        }  
    }
    void MoverObjeto(Transform objeto, Transform nuevaPosicion, Vector2 nuevaEscala)
    {
        objeto.transform.SetParent(nuevaPosicion);
        objeto.transform.localScale = nuevaEscala;
    }
    public void StarGame(bool CurrentPlayer)
    {
        playedcard = 0;
        if(!CurrentPlayer)
        {
            Hand2.SetActive(false);
            Deck2.SetActive(false);
           // Deck2Back.SetActive(false);
            lidersqr2.SetActive(false);
            Hand1.SetActive(true);
            Deck1.SetActive(true);
            //Deck1Back.SetActive(true);
            lidersqr1.SetActive(true);

            CurrentPlayer = true;
        }
        else 
        {
            Hand1.SetActive(false);
            Deck1.SetActive(false);
           // Deck1Back.SetActive(false);
            lidersqr1.SetActive(false);

            Hand2.SetActive(true);
            Deck2.SetActive(true);
           // Deck2Back.SetActive(true);
            lidersqr2.SetActive(true);

            CurrentPlayer = false;
        }
    }
    void Update()
    {
        Instancia.CardsPlayer1 = CardsPlayer1;
        Instancia.CardsPlayer2 = CardsPlayer2;    
        Instancia.Deck1 = Deck1;    
        Instancia.Deck2 = Deck2;
        Instancia.Cementery1 = Cementery1;
        Instancia.Cementery2 = Deck2;   
        Instancia.A1 = A1;  
        Instancia.A2 = A2;
        Instancia.D1= D1;
        Instancia.D2= D2;
        Instancia.G1= G1;
        Instancia.G2= G2;
        Instancia.Clima11= Clima11; 
        Instancia.Clima12= Clima12;
        Instancia.Clima13= Clima13;
        Instancia.Clima21= Clima21;
        Instancia.Clima22= Clima22; 
        Instancia.Clima23= Clima23;
    }
    
}
