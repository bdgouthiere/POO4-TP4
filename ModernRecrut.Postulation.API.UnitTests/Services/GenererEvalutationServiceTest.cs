using FluentAssertions;
using ModernRecrut.Postulation.API.Interfaces;
using ModernRecrut.Postulation.API.Models;
using ModernRecrut.Postulation.API.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernRecrut.Postulation.API.UnitTests.Services
{
    public class GenererEvalutationServiceTest
    {
        [Fact]
        public async Task GenererEvaluation_PretentionSalarial15000_Retourne_NoteNoteDetailSalaireInferieur()
        {
            // Etant donné
            decimal pretentionSalariale = 15000m;

            //var mockGenererEvaluationService = new Mock<IGenererEvaluationService>();

            GenererEvaluationService genererEvaluationService = new GenererEvaluationService();

            // Lorsque
            var note = genererEvaluationService.GenererEvaluation(pretentionSalariale);

            // Alors

            // Verifier Note est
            note.Should().NotBeNull();
            note.Should().BeOfType(typeof(Note));
            note.NomEmeteur.Should().Be("ApplicationPostulation");
            note.NoteDetail.Should().Be("Salaire inférieur à la norme");
        }
    }
}
