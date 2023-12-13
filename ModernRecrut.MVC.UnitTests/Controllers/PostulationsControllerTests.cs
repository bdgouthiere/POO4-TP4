using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModernRecrut.MVC.Areas.Identity.Data;
using ModernRecrut.MVC.Controllers;
using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Interfaces;
using ModernRecrut.MVC.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernRecrut.MVC.UnitTests.Controllers
{
    public class PostulationsControllerTests
    {
        // HTTPPost de Postualtion
        // => 
        [Fact]
        public async Task Postuler_CVAbsent_Retourne_ViewResultAvecModelStateError()
        {
            // Etant donné
            //// Fixtures
            Fixture fixture = new Fixture();

            string candidatId = fixture.Create<string>();
            string valideLettreMotivation = candidatId + "_LettreDeMotivation_" + fixture.Create<string>();

            //// Fixture OffreEmploi
            OffreEmploi offreEmploi = fixture.Create<OffreEmploi>();

            //// Fixture Documents
            fixture.RepeatCount = 3;
            List<string> listDocuments = fixture.CreateMany<string>().ToList();
            listDocuments.Add(valideLettreMotivation); // Ajout un document lettre de Motivation Valide

            //// Initialisation Instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationService = new Mock<IPostulationsService>();  // Postulation
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            mockDocumentsService.Setup(d => d.ObtenirSelonUtilisateurId(It.IsAny<string>())).ReturnsAsync(listDocuments);
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService
            mockOffreEmploiService.Setup(o => o.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync(offreEmploi);

            var postulationsController = new PostulationsController( mockLogger.Object, mockPostulationService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            RequetePostulation requetePostulation = new RequetePostulation();
            requetePostulation.CandidatId = candidatId;
            requetePostulation.OffreDemploiId = It.IsAny<int>();
            requetePostulation.DateDisponibilite = DateTime.Today.AddDays(1);
            requetePostulation.PretentionSalariale = 50000m;
            
            // Lorsque
            var actionResult = await postulationsController.Postuler(requetePostulation) as ViewResult;

            // Alors
            actionResult.Should().NotBeNull(); // Action Result Not Null
            var requetePostulationResult = actionResult.Model as RequetePostulation;
            requetePostulationResult.Should().Be(requetePostulation);
            mockPostulationService.Verify(p => p.Ajouter(It.IsAny<RequetePostulation>()), Times.Never);
            mockOffreEmploiService.Verify(o => o.ObtenirSelonId(requetePostulation.OffreDemploiId), Times.Once);
            
            //// Validations des erreurs dana le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("CV");
            modelState["CV"].Errors.FirstOrDefault().ErrorMessage.Should().Be("Un CV est obligatoire pour postuler. Veuillez déposer au préalable un CV dans votre espace Documents");

            //// ViewData
            actionResult.ViewData.Should().ContainKey("OffreEmploi").WhoseValue.Should().BeSameAs(offreEmploi);
        }

        [Fact]
        public async Task Postuler_LettreMotivationAbsent_Retourne_ViewResultAvecModelStateError()
        {
            // Etant donné
            //// Fixtures
            Fixture fixture = new Fixture();

            string candidatId = fixture.Create<string>();
            string valideCV = candidatId + "_CV_" + fixture.Create<string>();

            //// Fixture OffreEmploi
            OffreEmploi offreEmploi = fixture.Create<OffreEmploi>();

            //// Fixture Documents
            fixture.RepeatCount = 3;
            //List<string> listDocuments = fixture.Build<string>().Without(s => s.StartsWith(candidatId + "_LettreDeMotivation_")).CreateMany().ToList(); 
            List<string> listDocuments = fixture.CreateMany<string>().ToList();
            listDocuments.Add(valideCV); // Ajout un document lettre de Motivation Valide

            //// Initialisation Instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationService = new Mock<IPostulationsService>();  // Postulation
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            mockDocumentsService.Setup(d => d.ObtenirSelonUtilisateurId(It.IsAny<string>())).ReturnsAsync(listDocuments);
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService
            mockOffreEmploiService.Setup(o => o.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync(offreEmploi);

            var postulationsController = new PostulationsController( mockLogger.Object, mockPostulationService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            RequetePostulation requetePostulation = new RequetePostulation();
            requetePostulation.CandidatId = candidatId;
            requetePostulation.OffreDemploiId = It.IsAny<int>();
            requetePostulation.DateDisponibilite = DateTime.Today.AddDays(1);
            requetePostulation.PretentionSalariale = 50000m;
            
            // Lorsque
            var actionResult = await postulationsController.Postuler(requetePostulation) as ViewResult;

            // Alors
            actionResult.Should().NotBeNull(); // Action Result Not Null
            var requetePostulationResult = actionResult.Model as RequetePostulation;
            requetePostulationResult.Should().Be(requetePostulation);
            mockPostulationService.Verify(p => p.Ajouter(It.IsAny<RequetePostulation>()), Times.Never);
            mockOffreEmploiService.Verify(o => o.ObtenirSelonId(requetePostulation.OffreDemploiId), Times.Once);
            
            //// Validations des erreurs dana le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("LettreMotivation");
            modelState["LettreMotivation"].Errors.FirstOrDefault().ErrorMessage.Should().Be("Une lettre de motivation est obligatoire pour postuler. Veuillez déposer au préalable une lettre de motivation dans votre espace Documents");

            //// ViewData
            actionResult.ViewData.Should().ContainKey("OffreEmploi").WhoseValue.Should().BeSameAs(offreEmploi);
        }

        [Fact]
        public async Task Postuler_DatePasseeInvalide_Retourne_ViewResultAvecModelStateError()
        {
            // Etant donné
            //// Fixtures
            Fixture fixture = new Fixture();

            string candidatId = fixture.Create<string>();
            string valideLettreMotivation = candidatId + "_LettreDeMotivation_" + fixture.Create<string>();
            string valideCV = candidatId + "_CV_" + fixture.Create<string>();

            //// Fixture OffreEmploi
            OffreEmploi offreEmploi = fixture.Create<OffreEmploi>();

            //// Fixture Documents
            List<string> listDocuments = new List<string>(); 
            listDocuments.Add(valideLettreMotivation); // Ajout un document lettre de Motivation Valide
            listDocuments.Add(valideCV); // Ajout un document CV

            //// Initialisation Instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationService = new Mock<IPostulationsService>();  // Postulation
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            mockDocumentsService.Setup(d => d.ObtenirSelonUtilisateurId(It.IsAny<string>())).ReturnsAsync(listDocuments);
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService
            mockOffreEmploiService.Setup(o => o.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync(offreEmploi);

            var postulationsController = new PostulationsController( mockLogger.Object, mockPostulationService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            RequetePostulation requetePostulation = new RequetePostulation();
            requetePostulation.CandidatId = candidatId;
            requetePostulation.OffreDemploiId = It.IsAny<int>();
            requetePostulation.DateDisponibilite = DateTime.Today.AddDays(-1); // Date dans le passé
            requetePostulation.PretentionSalariale = 50000m;
            
            // Lorsque
            var actionResult = await postulationsController.Postuler(requetePostulation) as ViewResult;

            // Alors
            actionResult.Should().NotBeNull(); // Action Result Not Null
            var requetePostulationResult = actionResult.Model as RequetePostulation;
            requetePostulationResult.Should().Be(requetePostulation);
            mockPostulationService.Verify(p => p.Ajouter(It.IsAny<RequetePostulation>()), Times.Never);
            mockOffreEmploiService.Verify(o => o.ObtenirSelonId(requetePostulation.OffreDemploiId), Times.Once);
            
            //// Validations des erreurs dana le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("DateDisponibilite");
            modelState["DateDisponibilite"].Errors.FirstOrDefault().ErrorMessage.Should().Be("La date de disponibilité doit être supérieure à la date du jour et inférieure au < date correspondante à date du jour + 45 jours >");

            //// ViewData
            actionResult.ViewData.Should().ContainKey("OffreEmploi").WhoseValue.Should().BeSameAs(offreEmploi);
        }

        [Fact]
        public async Task Postuler_DateFutureInvalide_Retourne_ViewResultAvecModelStateError()
        {
            // Etant donné
            //// Fixtures
            Fixture fixture = new Fixture();

            string candidatId = fixture.Create<string>();
            string valideLettreMotivation = candidatId + "_LettreDeMotivation_" + fixture.Create<string>();
            string valideCV = candidatId + "_CV_" + fixture.Create<string>();

            //// Fixture OffreEmploi
            OffreEmploi offreEmploi = fixture.Create<OffreEmploi>();

            //// Fixture Documents
            List<string> listDocuments = new List<string>(); 
            listDocuments.Add(valideLettreMotivation); // Ajout un document lettre de Motivation Valide
            listDocuments.Add(valideCV); // Ajout un document CV

            //// Initialisation Instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationService = new Mock<IPostulationsService>();  // Postulation
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            mockDocumentsService.Setup(d => d.ObtenirSelonUtilisateurId(It.IsAny<string>())).ReturnsAsync(listDocuments);
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService
            mockOffreEmploiService.Setup(o => o.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync(offreEmploi);

            var postulationsController = new PostulationsController( mockLogger.Object, mockPostulationService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            RequetePostulation requetePostulation = new RequetePostulation();
            requetePostulation.CandidatId = candidatId;
            requetePostulation.OffreDemploiId = It.IsAny<int>();
            requetePostulation.DateDisponibilite = DateTime.Today.AddDays(46); // Date dans le passé
            requetePostulation.PretentionSalariale = 50000m;
            
            // Lorsque
            var actionResult = await postulationsController.Postuler(requetePostulation) as ViewResult;

            // Alors
            actionResult.Should().NotBeNull(); // Action Result Not Null
            var requetePostulationResult = actionResult.Model as RequetePostulation;
            requetePostulationResult.Should().Be(requetePostulation);
            mockPostulationService.Verify(p => p.Ajouter(It.IsAny<RequetePostulation>()), Times.Never);
            mockOffreEmploiService.Verify(o => o.ObtenirSelonId(requetePostulation.OffreDemploiId), Times.Once);
            
            //// Validations des erreurs dana le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("DateDisponibilite");
            modelState["DateDisponibilite"].Errors.FirstOrDefault().ErrorMessage.Should().Be("La date de disponibilité doit être supérieure à la date du jour et inférieure au < date correspondante à date du jour + 45 jours >");

            //// ViewData
            actionResult.ViewData.Should().ContainKey("OffreEmploi").WhoseValue.Should().BeSameAs(offreEmploi);
        }

        [Fact]
        public async Task Postuler_DateAujourdhuiInvalide_Retourne_ViewResultAvecModelStateError()
        {
            // Etant donné
            //// Fixtures
            Fixture fixture = new Fixture();

            string candidatId = fixture.Create<string>();
            string valideLettreMotivation = candidatId + "_LettreDeMotivation_" + fixture.Create<string>();
            string valideCV = candidatId + "_CV_" + fixture.Create<string>();

            //// Fixture OffreEmploi
            OffreEmploi offreEmploi = fixture.Create<OffreEmploi>();

            //// Fixture Documents
            List<string> listDocuments = new List<string>(); 
            listDocuments.Add(valideLettreMotivation); // Ajout un document lettre de Motivation Valide
            listDocuments.Add(valideCV); // Ajout un document CV

            //// Initialisation Instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationService = new Mock<IPostulationsService>();  // Postulation
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            mockDocumentsService.Setup(d => d.ObtenirSelonUtilisateurId(It.IsAny<string>())).ReturnsAsync(listDocuments);
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService
            mockOffreEmploiService.Setup(o => o.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync(offreEmploi);

            var postulationsController = new PostulationsController( mockLogger.Object, mockPostulationService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            RequetePostulation requetePostulation = new RequetePostulation();
            requetePostulation.CandidatId = candidatId;
            requetePostulation.OffreDemploiId = It.IsAny<int>();
            requetePostulation.DateDisponibilite = DateTime.Today; // Date dans le passé
            requetePostulation.PretentionSalariale = 50000m;
            
            // Lorsque
            var actionResult = await postulationsController.Postuler(requetePostulation) as ViewResult;

            // Alors
            actionResult.Should().NotBeNull(); // Action Result Not Null
            var requetePostulationResult = actionResult.Model as RequetePostulation;
            requetePostulationResult.Should().Be(requetePostulation);
            mockPostulationService.Verify(p => p.Ajouter(It.IsAny<RequetePostulation>()), Times.Never);
            mockOffreEmploiService.Verify(o => o.ObtenirSelonId(requetePostulation.OffreDemploiId), Times.Once);
            
            //// Validations des erreurs dana le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("DateDisponibilite");
            modelState["DateDisponibilite"].Errors.FirstOrDefault().ErrorMessage.Should().Be("La date de disponibilité doit être supérieure à la date du jour et inférieure au < date correspondante à date du jour + 45 jours >");

            //// ViewData
            actionResult.ViewData.Should().ContainKey("OffreEmploi").WhoseValue.Should().BeSameAs(offreEmploi);
        }

        [Fact]
        public async Task Postuler_PretentionSalarialHorsLimite_Retourne_ViewResultAvecModelStateError()
        {
            // Etant donné
            //// Fixtures
            Fixture fixture = new Fixture();

            string candidatId = fixture.Create<string>();
            string valideLettreMotivation = candidatId + "_LettreDeMotivation_" + fixture.Create<string>();
            string valideCV = candidatId + "_CV_" + fixture.Create<string>();

            //// Fixture OffreEmploi
            OffreEmploi offreEmploi = fixture.Create<OffreEmploi>();

            //// Fixture Documents
            List<string> listDocuments = new List<string>(); 
            listDocuments.Add(valideLettreMotivation); // Ajout un document lettre de Motivation Valide
            listDocuments.Add(valideCV); // Ajout un document CV

            //// Initialisation Instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationService = new Mock<IPostulationsService>();  // Postulation
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            mockDocumentsService.Setup(d => d.ObtenirSelonUtilisateurId(It.IsAny<string>())).ReturnsAsync(listDocuments);
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService
            mockOffreEmploiService.Setup(o => o.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync(offreEmploi);

            var postulationsController = new PostulationsController( mockLogger.Object, mockPostulationService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            RequetePostulation requetePostulation = new RequetePostulation();
            requetePostulation.CandidatId = candidatId;
            requetePostulation.OffreDemploiId = It.IsAny<int>();
            requetePostulation.DateDisponibilite = DateTime.Today.AddDays(1); 
            requetePostulation.PretentionSalariale = 151000m;
            
            // Lorsque
            var actionResult = await postulationsController.Postuler(requetePostulation) as ViewResult;

            // Alors
            actionResult.Should().NotBeNull(); // Action Result Not Null
            var requetePostulationResult = actionResult.Model as RequetePostulation;
            requetePostulationResult.Should().Be(requetePostulation);
            mockPostulationService.Verify(p => p.Ajouter(It.IsAny<RequetePostulation>()), Times.Never);
            mockOffreEmploiService.Verify(o => o.ObtenirSelonId(requetePostulation.OffreDemploiId), Times.Once);
            
            //// Validations des erreurs dana le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("PretentionSalariale");
            modelState["PretentionSalariale"].Errors.FirstOrDefault().ErrorMessage.Should().Be("Votre présentation salariale est au-delà de nos limites");

            //// ViewData
            actionResult.ViewData.Should().ContainKey("OffreEmploi").WhoseValue.Should().BeSameAs(offreEmploi);
        }


        [Fact]
        public async Task Postuler_PostulationIsNull_Retourne_ViewResultAvecModelStateError()
        {
            // Etant donné
            //// Fixtures
            Fixture fixture = new Fixture();

            string candidatId = fixture.Create<string>();
            string valideLettreMotivation = candidatId + "_LettreDeMotivation_" + fixture.Create<string>();
            string valideCV = candidatId + "_CV_" + fixture.Create<string>();

            //// Fixture OffreEmploi
            OffreEmploi offreEmploi = fixture.Create<OffreEmploi>();

            //// Fixture Documents
            List<string> listDocuments = new List<string>(); 
            listDocuments.Add(valideLettreMotivation); // Ajout un document lettre de Motivation Valide
            listDocuments.Add(valideCV); // Ajout un document CV

            // requete
            RequetePostulation requetePostulation = new RequetePostulation();
            requetePostulation.CandidatId = candidatId;
            requetePostulation.OffreDemploiId = It.IsAny<int>();
            requetePostulation.DateDisponibilite = DateTime.Today.AddDays(1); 
            requetePostulation.PretentionSalariale = 50000m;
            
            //// Initialisation Instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationService = new Mock<IPostulationsService>();  // Postulation
            mockPostulationService.Setup(p => p.Ajouter(requetePostulation)).ReturnsAsync((Postulation)null); // Postulation null
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            mockDocumentsService.Setup(d => d.ObtenirSelonUtilisateurId(It.IsAny<string>())).ReturnsAsync(listDocuments);
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService
            mockOffreEmploiService.Setup(o => o.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync(offreEmploi);

            var postulationsController = new PostulationsController( mockLogger.Object, mockPostulationService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            // Lorsque
            var actionResult = await postulationsController.Postuler(requetePostulation) as ViewResult;

            // Alors
            actionResult.Should().NotBeNull(); // Action Result Not Null
            var requetePostulationResult = actionResult.Model as RequetePostulation;
            requetePostulationResult.Should().Be(requetePostulation);
            mockPostulationService.Verify(p => p.Ajouter(It.IsAny<RequetePostulation>()), Times.Once);
            mockOffreEmploiService.Verify(o => o.ObtenirSelonId(requetePostulation.OffreDemploiId), Times.Once);
            
            //// Validations des erreurs dana le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("AjoutEchoue");
            modelState["AjoutEchoue"].Errors.FirstOrDefault().ErrorMessage.Should().Be("Problème lors de l'ajout de la postulation, veuillez reessayer");

            //// ViewData
            actionResult.ViewData.Should().ContainKey("OffreEmploi").WhoseValue.Should().BeSameAs(offreEmploi);
        }

        [Fact]
        public async Task Postuler_PostulationValide_Retourne_RedirectToAction()
        {
            // Etant donné
            //// Fixtures
            Fixture fixture = new Fixture();

            string candidatId = fixture.Create<string>();
            string valideLettreMotivation = candidatId + "_LettreDeMotivation_" + fixture.Create<string>();
            string valideCV = candidatId + "_CV_" + fixture.Create<string>();

            //// Fixture OffreEmploi
            OffreEmploi offreEmploi = fixture.Create<OffreEmploi>();

            //// Fixture Documents
            List<string> listDocuments = new List<string>();
            listDocuments.Add(valideLettreMotivation); // Ajout un document lettre de Motivation Valide
            listDocuments.Add(valideCV); // Ajout un document CV

            // requete
            RequetePostulation requetePostulation = new RequetePostulation();
            requetePostulation.CandidatId = candidatId;
            requetePostulation.OffreDemploiId = fixture.Create<int>();
            requetePostulation.DateDisponibilite = DateTime.Today.AddDays(1);
            requetePostulation.PretentionSalariale = 50000m;

            //// Initialisation Instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationService = new Mock<IPostulationsService>();  // Postulation
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            Postulation postulationAjoutee = fixture.Create<Postulation>();
            mockPostulationService.Setup(p => p.Ajouter(requetePostulation)).ReturnsAsync(postulationAjoutee); // Postulation valide
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            mockDocumentsService.Setup(d => d.ObtenirSelonUtilisateurId(It.IsAny<string>())).ReturnsAsync(listDocuments);
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService
            mockOffreEmploiService.Setup(o => o.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync(offreEmploi);

            var postulationsController = new PostulationsController(mockLogger.Object, mockPostulationService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            // Lorsque
            var redirectToActionResult = await postulationsController.Postuler(requetePostulation) as RedirectToActionResult;

            // Alors
            redirectToActionResult.Should().NotBeNull();
            redirectToActionResult.ControllerName.Should().Be("OffreEmplois");
            redirectToActionResult.ActionName.Should().Be("Index");
            mockPostulationService.Verify(p => p.Ajouter(It.IsAny<RequetePostulation>()), Times.Once());
        }

        [Fact]
        public async Task Postuler_OffreEmploiInexistant_Retourne_NotFound()
        {
            // Etant donné
            //// Fixtures
            Fixture fixture = new Fixture();

            string candidatId = fixture.Create<string>();

            // requete
            RequetePostulation requetePostulation = new RequetePostulation();
            requetePostulation.CandidatId = candidatId;
            requetePostulation.OffreDemploiId = fixture.Create<int>();
            requetePostulation.DateDisponibilite = DateTime.Today.AddDays(1);
            requetePostulation.PretentionSalariale = 50000m;

            //// Initialisation Instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationService = new Mock<IPostulationsService>();  // Postulation
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService
            mockOffreEmploiService.Setup(o => o.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync((OffreEmploi)null);

            var postulationsController = new PostulationsController(mockLogger.Object, mockPostulationService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            // Lorsque
            var actionResult = await postulationsController.Postuler(requetePostulation);

            // Alors
            actionResult.Should().BeOfType(typeof(NotFoundResult));
        }

    }
}
