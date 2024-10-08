﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;

namespace Skyrim_Interpreter
{
    public class Context
    {
        public List<Card> Board { get; set; }
        public bool TriggerPlayer { get; set; }
        private static Context _current;
        public List<Card> Hand1 { get; set; }
        public List<Card> Hand2 { get; set; }
        public List<Card> Deck1 { get; set; }
        public List<Card> Deck2 { get; set; }
        public List<Card> F1 { get; set; }
        public List<Card> F2 { get; set; }
        public List<Card> Graveyard1 { get; set; }
        public List<Card> Graveyard2 { get; set; }
        public static Dictionary<string, List<Card>> Asignments;
        public static Context Current
        {
        get { return _current; }
        set { _current = value; }
        }
       
        public Context(bool id, List<Card> board,List<Card> H1,List<Card>H2,List<Card> deck1,List<Card> deck2,List<Card> f1,List<Card> f2,List<Card>Grave1,List<Card>Grave2)
        {
            this.TriggerPlayer = id;
            this.Board = board;
            F1= f1; F2= f2; 
            Deck1 = deck1;  Deck2 = deck2;
            this.Hand1 = H1; this.Hand2 = H2;
            this.Graveyard1 = Grave1; this.Graveyard2 = Grave2;
            Asignments = new Dictionary<string, List<Card>>
            {
                { "board", Board },
                {"hand1",Hand1 },
                {"hand2",Hand2 },
                {"deck1",Deck1 },
                {"deck2",Deck2 },
                { "field1",F1},
                {"field2",F2 },
                {"graveyard1",Graveyard1 },
                {"graveyard2",Graveyard2}
            };
        }   
        public Hand HandOfPlayer(bool id) 
        {
            Hand hand = new Hand();
            if (!id)
            {
                hand.cards = this.Hand1;  
            }
            else 
            {
                hand.cards = this.Hand2;
            }
            return hand;
        }
        public Deck DeckOfPlayer(bool id) 
        {
          Deck  deck = new Deck();
            if (!id)
            {
                deck.cards = this.Deck1;
            }
            else 
            {
                deck.cards = this.Deck2;
            }
            return deck;
        }

        public Graveyard GraveyardOfPlayer(bool id) 
        {
            Graveyard graveyard = new Graveyard();
            if (!id)
            {
                graveyard.cards = this.Graveyard1;  
            }
            else 
            {
                graveyard.cards = this.Graveyard2;
            }
            return graveyard;   
        }
         public Field FieldOfPlayer(bool id) 
         {
          Field field = new Field();
            if (!id)
            {
                field.cards = this.F1;
            }
            else 
            {
                field.cards = this.F2;  
            }
            return field;   
         } 
         
        public Deck Deck
        {
            get { return DeckOfPlayer(TriggerPlayer); }
        }
        public Hand Hand
        {
            get { return HandOfPlayer(TriggerPlayer); }
        }
        public Field Field
        {
            get { return FieldOfPlayer(TriggerPlayer); }
        }
        public Graveyard Graveyard
        {
            get { return GraveyardOfPlayer(TriggerPlayer); }
        }
        public static List<Card> nc;

     
    }
    public class Targets 
    {
        public List<Card> targets { get; set; }  
        public Targets()
        {
            targets= new List<Card>();    
        }
        public void Add(Card card) 
        {
            targets.Add(card);  
        }
    }
    public interface ICard
    {
        public List<Card> Find(Predicate<Card> predicate);
        public List<Card> cards { get; set; }
        public void Push(Card card);
        public Card Pop();
        public void SendBottom(Card card);
        public void Remove(Card card);
        public void Shuffle();
    }
    public class CardsComponent : ICard
    {
        public List<Card> cards { get; set; } = new List<Card>();
        public void Push(Card card) => cards.Insert(0, card);
        public Card Pop()
        {
            if (cards.Count == 0) return null;
            Card card = cards[0];
            cards.RemoveAt(0);
            return card;
        }
        public void SendBottom(Card card) => cards.Add(card);
        public List<Card> Find(Predicate<Card> predicate) => cards.FindAll(predicate);
        public void Shuffle()
        {
            System.Random rng = new System.Random();
            cards = cards.OrderBy(_ => rng.Next()).ToList();
        }
        public void Remove(Card  card) => cards.Remove(card);

        public void Add(Card card) 
        {
            cards.Add(card);
        }
    }
    public class Hand : CardsComponent { }

    public class Graveyard : CardsComponent { }

    public class Field : CardsComponent { }

    public class Deck : CardsComponent { }
}
