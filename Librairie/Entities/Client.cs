using System;
using System.Collections.Generic;

namespace Librairie.Entities
{
    public class Client
    {
        public Guid Id { get; set; }
        public string NomUtilisateur { get; set; }
        public Dictionary<Guid, int> LivresAchetes { get; set; }

        public Client()
        {
            LivresAchetes = new Dictionary<Guid, int>();
        }
    }
}
