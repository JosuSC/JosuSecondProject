using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skyrim_Interpreter
{
    public class Game
    {
      public static  Player[] players { get; set;} 
        public Game()
        {
            players = new Player[2];
        }
        public  static Player PlayerID(Guid id) 
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].ID == id) return players[i];
            }
            Console.WriteLine("ID de jugador no valido");
            return null;
        }

    }
}
