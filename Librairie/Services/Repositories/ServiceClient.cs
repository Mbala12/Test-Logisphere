using Librairie.Entities;
using Librairie.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Librairie.Services.Repositories
{
    public class ServiceClient : IServiceClient
    {
        private readonly IServiceBD _db;

        public ServiceClient(IServiceBD db)
        {
            _db = db;
        }

        public Client CreerClient(string nomClient)
        {
            if (string.IsNullOrWhiteSpace(nomClient))
                throw new ArgumentException("Le nom du client doit être renseigné.");

            if (_db.ObtenirClient(nomClient) != null)
                throw new InvalidOperationException("Ce nom est déjà utilisé.");

            var client = new Client
            {
                Id = Guid.NewGuid(),
                NomUtilisateur = nomClient
            };

            _db.AjouterClient(client);
            return client;
        }

        public void RenommerClient(Guid clientId, string nouveauNomClient)
        {
            if (string.IsNullOrWhiteSpace(nouveauNomClient))
                throw new ArgumentException("Le nouveau nom doit être renseigné.");

            var client = _db.ObtenirClient(clientId)
                ?? throw new KeyNotFoundException("Client introuvable.");

            if (client.NomUtilisateur == nouveauNomClient)
                throw new InvalidOperationException("Le nom n’a pas changé.");

            if (_db.ObtenirClient(nouveauNomClient) != null)
                throw new InvalidOperationException("Nom déjà utilisé.");

            client.NomUtilisateur = nouveauNomClient;
            _db.ModifierClient(client);
        }
    }
}
