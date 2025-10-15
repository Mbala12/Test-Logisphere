using System;

namespace Librairie.Services.Interfaces
{
    public interface IServiceLivre
    {
        decimal AcheterLivre(Guid idClient, Guid idLivre, decimal montant);

        decimal RembourserLivre(Guid idClient, Guid idLivre);
    }
}
