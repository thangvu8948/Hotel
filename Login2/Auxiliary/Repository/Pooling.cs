using Login2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login2.Auxiliary.Repository
{
    class Pooling
    {
        private static Pooling _instance = new Pooling();
        private int maxConnection = 10;
        public static Pooling Instance { get { return _instance; } }

        private hotelEntities[] pooling = null;

        private void InitPooling()
        {
            pooling = new hotelEntities[maxConnection];
            for (int i = 0; i < maxConnection; i++)
            {
                pooling[i] = new hotelEntities();
            }
        }
        public hotelEntities getFreeContext()
        {
            if (pooling == null)
            {
                InitPooling();
            }
            for (int i = 0; i < maxConnection; i++)
            {
                if (!pooling[i].IsUsing)
                {
                    return pooling[i];
                }
            }
            return null;
        }
    }
}
