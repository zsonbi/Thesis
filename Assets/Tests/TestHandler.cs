using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Tests
{
    internal class TestHandler
    {
        private static TestHandler instance;

        public ServerConnection ServerConnection { get; private set; }

        public static TestHandler Instance => GetOrSet();

        private static TestHandler GetOrSet()
        {
            if (instance is null)
            {
                instance = new TestHandler();
            }
            return instance;
        }

        private TestHandler()
        {
        }

        public void SetServerConnection(ServerConnection serverConnection)
        {
            this.ServerConnection = serverConnection;
        }
    }
}