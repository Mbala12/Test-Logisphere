using Librairie.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Librairie.Services.Repositories
{
    public class ServiceLivre : IServiceLivre
    {
        private readonly IServiceBD _db;

        public ServiceLivre(IServiceBD db)
        {
            _db = db;
        }

        public decimal AcheterLivre(Guid idClient, Guid idLivre, decimal montant)
        {
            var client = _db.ObtenirClient(idClient) ?? throw new KeyNotFoundException("Client inexistant.");
            var livre = _db.ObtenirLivre(idLivre) ?? throw new KeyNotFoundException("Livre inexistant.");

            if (montant <= 0)
                throw new ArgumentException("Montant invalide.");
            if (livre.Quantite <= 0)
                throw new InvalidOperationException("Livre épuisé.");
            if (montant < livre.Prix)
                throw new InvalidOperationException("Montant insuffisant.");

            livre.Quantite--;
            if (client.LivresAchetes.ContainsKey(livre.Id))
                client.LivresAchetes[livre.Id]++;
            else
                client.LivresAchetes[livre.Id] = 1;

            _db.ModifierLivre(livre);
            _db.ModifierClient(client);

            return montant - livre.Prix;
        }

        public decimal RembourserLivre(Guid idClient, Guid idLivre)
        {
            var client = _db.ObtenirClient(idClient) ?? throw new KeyNotFoundException("Client inexistant.");
            var livre = _db.ObtenirLivre(idLivre) ?? throw new KeyNotFoundException("Livre inexistant.");

            if (!client.LivresAchetes.ContainsKey(idLivre) || client.LivresAchetes[idLivre] <= 0)
                throw new InvalidOperationException("Le client n’a pas acheté ce livre.");

            client.LivresAchetes[idLivre]--;
            if (client.LivresAchetes[idLivre] == 0)
                client.LivresAchetes.Remove(idLivre);

            livre.Quantite++;
            _db.ModifierLivre(livre);
            _db.ModifierClient(client);

            return livre.Prix;
        }
    }
}
