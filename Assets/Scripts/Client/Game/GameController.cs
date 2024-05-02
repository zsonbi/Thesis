using Game.World;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{

    public class GameController : MonoBehaviour
    {
        public Player Player { get; private set; }
        public List<Police> Enemies { get; private set; }

        public World.World World { get; private set; }


        public void NewGame()
        {

        }
    }

}