using Microsoft.AspNetCore.Mvc;
using ModernRecrut.Documents.API.Models;
using  ModernRecrut.Documents.API.Services;
using ModernRecrut.Documents.API.Interfaces;
using ModernRecrut.Documents.API.DTO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ModernRecrut.Documents.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentsService _uploadService;

        public DocumentsController(IDocumentsService uploadService)
        {
            _uploadService = uploadService;
        }

        // 200
        [HttpGet("{utilisateurId}")]
        public ActionResult Get(string utilisateurId)
        {
            IEnumerable<string> documents = _uploadService.ObtenirSelonUtilisateurId(utilisateurId);
            return Ok(documents);
        }

        // 201 // 400
        [HttpPost]
        public async Task<ActionResult> Post([FromForm] RequeteDocument requeteDocument)
        {
            // Vérifier l'extension du fichier
            string[] extensionPermises = new string[] { ".docx", ".pdf" };
            string? extensionFichier = Path.GetExtension(requeteDocument.Document.DocumentDetails.FileName).ToLower();


            if (!extensionPermises.Contains(extensionFichier))
            {
                ModelState.AddModelError("Document.DocumentDetails", $"Les fichiers avec l'extension {extensionFichier} ne sont pas autorisés. Les extensions autorisées sont : {string.Join(", ", extensionPermises)}");
            }

            if (ModelState.IsValid)
            {
                await _uploadService.PostFileAsync(requeteDocument.Document, requeteDocument.UtilisateurId);
                return Ok();
            }

            return BadRequest(ModelState);

        }

        [HttpDelete("{fichierNom}")]
        public ActionResult Delete(string fichierNom)
        {
            bool estSupprimer = _uploadService.Supprimer(fichierNom);
            if (!estSupprimer)
                return BadRequest();

            return NoContent();
        }
    }
}
