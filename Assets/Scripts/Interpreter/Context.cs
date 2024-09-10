using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;

namespace Skyrim_Interpreter
{
    public class Context
    {
        public Guid TriggerPlayer { get; set; }
        private static Context _current;
       
    public static Context Current
    {
        get { return _current; }
        set { _current = value; }
    }
        public List<Card> Board { get; set; }
        public Context(Guid id, List<Card> board)
        {
            this.TriggerPlayer = id;
            this.Board = board;
        }   
        public Hand HandOfPlayer(Guid id) 
        {
            Player player = PlayerID(id);
            return player.hand;
        }

        public Deck DeckOfPlayer(Guid id) 
        {
            Player player = PlayerID(id);
            return player.deck;
        }

        public Graveyard GraveyardOfPlayer(Guid id) 
        {
            Player player = PlayerID(id);
            return player.graveyard;
        }

         public Field FieldOfPlayer(Guid id) 
         {
            Player player = PlayerID(id);
            return player.field;
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
        public Player PlayerID(Guid id)
        {
          return Game.PlayerID(id);
        }
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
    public class EffectDef
    {
        public string Name { get; set; }
        List<string> Parameters { get; set; }
        public EffectDef()
        {
            Parameters = new List<string>();
        }
    }
    public class Player
    {
        public Guid ID { get; set; }
        public Deck deck { get; set; }
        public Hand hand { get; set; }
        public Graveyard graveyard { get; set; }
        public Field field { get; set; }

        public Player(Guid id)
        {
            this.ID = id;
            deck = new Deck();
            hand = new Hand();
            graveyard = new Graveyard();
            field = new Field();
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
    }
    public class Hand : CardsComponent { }

    public class Graveyard : CardsComponent { }

    public class Field : CardsComponent { }

    public class Deck : CardsComponent { }
}
