using System;
using System.Collections.Generic;
using System.Linq;

namespace Questions
{
    // Améliorer le code de cette classe, ainsi que sa relation avec la classe Collaborateur.

    public interface ICollaborateur
    {
        void AjouterContenuBD(string contenu);
    }

    public class CollaborateurService : ICollaborateur
    {
        public void AjouterContenuBD(string contenu)
        {
            Collaborateur.AjouterContenuBD(contenu);
        }
    }

    public class Question
    {
        private readonly ICollaborateur _collaborateur;

        public Question(ICollaborateur collaborateur)
        {
            _collaborateur = collaborateur;
        }

        public void Traiter(List<string> listeContenu)
        {
            if (listeContenu == null || !listeContenu.Any())
                throw new ArgumentException("La liste de contenu est vide.");

            var listeContenuValide = new List<string>();

            foreach (var contenu in listeContenu.Distinct())
            {
                if (!Valider(contenu, out string message))
                    throw new Exception(message);

                var extrait = contenu.Length > 10 ? contenu.Substring(0, 10) : contenu;
                listeContenuValide.Add(extrait);
            }

            foreach (var contenu in listeContenuValide)
            {
                _collaborateur.AjouterContenuBD(contenu);
            }
        }

        private bool Valider(string contenu, out string message)
        {
            message = null;

            if (string.IsNullOrEmpty(contenu))
            {
                message = "Le contenu ne peut être vide.";
                return false;
            }

            if (contenu.Length > 50)
            {
                message = "Le contenu est trop long.";
                return false;
            }

            return true;
        }
    }
}
