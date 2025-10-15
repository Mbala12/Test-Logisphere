using Librairie.Entities;
using Librairie.Services.Interfaces;
using Librairie.Services.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LibrairieTest
{
    public class ServiceLivreTests
    {
        private readonly Mock<IServiceBD> _mockBD;
        private readonly ServiceLivre _serviceLivre;
        private readonly Client _client;
        private readonly Livre _livre;

        public ServiceLivreTests()
        {
            _mockBD = new Mock<IServiceBD>();
            _serviceLivre = new ServiceLivre(_mockBD.Object);
            _client = new Client { Id = Guid.NewGuid(), NomUtilisateur = "Alice" };
            _livre = new Livre { Id = Guid.NewGuid(), Quantite = 2, Prix = 10m };
        }

        [Fact]
        public void AcheterLivre_ClientInexistant_DevraitLancerException()
        {
            _mockBD.Setup(db => db.ObtenirClient(It.IsAny<Guid>())).Returns((Client)null);

            Assert.Throws<KeyNotFoundException>(() => _serviceLivre.AcheterLivre(Guid.NewGuid(), _livre.Id, 10));
        }

        [Fact]
        public void AcheterLivre_LivreInexistant_DevraitLancerException()
        {
            _mockBD.Setup(db => db.ObtenirClient(_client.Id)).Returns(_client);
            _mockBD.Setup(db => db.ObtenirLivre(It.IsAny<Guid>())).Returns((Livre)null);

            Assert.Throws<KeyNotFoundException>(() => _serviceLivre.AcheterLivre(_client.Id, Guid.NewGuid(), 10));
        }

        [Fact]
        public void AcheterLivre_QuantiteInsuffisante_DevraitLancerException()
        {
            _livre.Quantite = 0;
            _mockBD.Setup(db => db.ObtenirClient(_client.Id)).Returns(_client);
            _mockBD.Setup(db => db.ObtenirLivre(_livre.Id)).Returns(_livre);

            Assert.Throws<InvalidOperationException>(() => _serviceLivre.AcheterLivre(_client.Id, _livre.Id, 10));
        }

        [Fact]
        public void AcheterLivre_Valide_DevraitReduireQuantiteEtRetournerMonnaie()
        {
            _mockBD.Setup(db => db.ObtenirClient(_client.Id)).Returns(_client);
            _mockBD.Setup(db => db.ObtenirLivre(_livre.Id)).Returns(_livre);

            var monnaie = _serviceLivre.AcheterLivre(_client.Id, _livre.Id, 15);

            Assert.Equal(5, monnaie);
            Assert.Equal(1, _livre.Quantite);
            Assert.Equal(1, _client.LivresAchetes[_livre.Id]);
            _mockBD.Verify(db => db.ModifierLivre(_livre), Times.Once);
            _mockBD.Verify(db => db.ModifierClient(_client), Times.Once);
        }

        [Fact]
        public void RembourserLivre_Valide_DevraitAugmenterQuantiteEtRetournerPrix()
        {
            _client.LivresAchetes[_livre.Id] = 1;
            _mockBD.Setup(db => db.ObtenirClient(_client.Id)).Returns(_client);
            _mockBD.Setup(db => db.ObtenirLivre(_livre.Id)).Returns(_livre);

            var remboursement = _serviceLivre.RembourserLivre(_client.Id, _livre.Id);

            Assert.Equal(10, remboursement);
            Assert.Equal(3, _livre.Quantite);
            Assert.False(_client.LivresAchetes.ContainsKey(_livre.Id));
        }
    }
}
