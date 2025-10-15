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
    public class ServiceClientTests
    {
        private readonly Mock<IServiceBD> _mockBD;
        private readonly ServiceClient _serviceClient;

        public ServiceClientTests() //IServiceClient serviceClient, Mock<IServiceBD> mockBD
        {
            _mockBD = new Mock<IServiceBD>();
            _serviceClient = new ServiceClient(_mockBD.Object);
        }

        [Fact]
        public void CreerClient_NomVide_DevraitLancerException()
        {
            Assert.Throws<ArgumentException>(() => _serviceClient.CreerClient(""));
        }

        [Fact]
        public void CreerClient_NomDejaUtilise_DevraitLancerException()
        {
            _mockBD.Setup(db => db.ObtenirClient("Alice"))
                   .Returns(new Client { Id = Guid.NewGuid(), NomUtilisateur = "Alice" });

            Assert.Throws<InvalidOperationException>(() => _serviceClient.CreerClient("Alice"));
        }

        [Fact]
        public void CreerClient_Valide_DevraitRetournerClientEtLAppelerAjouterClient()
        {
            _mockBD.Setup(db => db.ObtenirClient("Bob")).Returns((Client)null);

            var result = _serviceClient.CreerClient("Bob");

            Assert.Equal("Bob", result.NomUtilisateur);
            _mockBD.Verify(db => db.AjouterClient(It.IsAny<Client>()), Times.Once);
        }

        [Fact]
        public void RenommerClient_ClientInexistant_DevraitLancerException()
        {
            _mockBD.Setup(db => db.ObtenirClient(It.IsAny<Guid>())).Returns((Client)null);

            Assert.Throws<KeyNotFoundException>(() => _serviceClient.RenommerClient(Guid.NewGuid(), "NouveauNom"));
        }

        [Fact]
        public void RenommerClient_NouveauNomDejaPris_DevraitLancerException()
        {
            var client = new Client { Id = Guid.NewGuid(), NomUtilisateur = "Alice" };
            _mockBD.Setup(db => db.ObtenirClient(client.Id)).Returns(client);
            _mockBD.Setup(db => db.ObtenirClient("Bob")).Returns(new Client { NomUtilisateur = "Bob" });

            Assert.Throws<InvalidOperationException>(() => _serviceClient.RenommerClient(client.Id, "Bob"));
        }

        [Fact]
        public void RenommerClient_Valide_DevraitModifierClient()
        {
            var client = new Client { Id = Guid.NewGuid(), NomUtilisateur = "Alice" };
            _mockBD.Setup(db => db.ObtenirClient(client.Id)).Returns(client);
            _mockBD.Setup(db => db.ObtenirClient("Bob")).Returns((Client)null);

            _serviceClient.RenommerClient(client.Id, "Bob");

            Assert.Equal("Bob", client.NomUtilisateur);
            _mockBD.Verify(db => db.ModifierClient(client), Times.Once);
        }
    }
}
