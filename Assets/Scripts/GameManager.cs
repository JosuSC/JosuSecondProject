using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
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
        VisualCard[] cards = father.transform.GetComponentsInChildren<VisualCard>();
        List<Card> output = new List<Card>();   
        foreach (var card in cards) 
        {
            output.Add(card.card);
        }
        return output;
    }

    public void EffectsForCards(Card card) 
    {
        //actualizamos el contexts
        List<Card> A1 = PasarHijos(this.A1);
        List<Card> A2 = PasarHijos(this.A2);
        List<Card> D1 = PasarHijos(this.D1);
        List<Card> D2 = PasarHijos(this.D2);
        List<Card> G1 = PasarHijos(this.G1);
        List<Card> G2 = PasarHijos(this.G2);
        List<Card> Clima21 = PasarHijos(this.Clima21);
        List<Card> Clima22 = PasarHijos(this.Clima22);
        List<Card> Clima23 = PasarHijos(this.Clima23);
        List<Card> Clima11 = PasarHijos(this.Clima11);
        List<Card> Clima12 = PasarHijos(this.Clima12);
        List<Card> Clima13 = PasarHijos(this.Clima13);
        List<Card> Graveyard1 = PasarHijos(this.Cementery1);
        List<Card> Graveyard2 = PasarHijos(this.Cementery2);
        List<Card> Deck1 = PasarHijos(this.Deck1);
        List<Card> Deck2 = PasarHijos(this.Deck2);
        List<Card> Field1 = new List<Card>();
        List<Card> Field2 = new List<Card>();
        List<Card> Board = new List<Card>();
        foreach (var item in A1)
        {
            Field1.Add(item);
        }
        foreach (var item in D1)
        {
            Field1.Add(item);
        }
        foreach (var item in G1)
        {
            Field1.Add(item);
        }
        foreach (var item in A2)
        {
            Field2.Add(item);
        }
        foreach (var item in D2)
        {
            Field2.Add(item);
        }
        foreach (var item in G2)
        {
            Field2.Add(item);
        }
        foreach (var item in Field1)
        {
            Board.Add(item);
        }
        foreach (var item in Field2)
        {
            Board.Add(item);
        }
        Context newcontext = new Context(CurrentPlayer,Board,this.CardsPlayer1,this.CardsPlayer2,A1,A2,D1,D2,G1,G2,Field1,Field2,Graveyard1,Graveyard2);
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
    //void Update()
    //{
    //    Actualizacion.Actualizar();
    //}
    
}
