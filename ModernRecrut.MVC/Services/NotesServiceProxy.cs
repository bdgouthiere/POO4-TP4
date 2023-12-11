using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Interfaces;
using ModernRecrut.MVC.Models;

namespace ModernRecrut.MVC.Services
{
    public class NotesServiceProxy : INotesService
    {
        #region Attributs
        private readonly HttpClient _httpClient;
        private const string _noteApiUrl = "api/Notes/";
        private readonly ILogger<NotesServiceProxy> _logger;
        #endregion

        #region Constructeur
        public NotesServiceProxy(HttpClient httpClient, ILogger<NotesServiceProxy> logger)
        {
            _httpClient = new HttpClient();
            _logger = logger;
        }
        #endregion

        #region Méthodes publiques
        public async Task<Note> Ajouter(RequeteNote requeteNote)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_noteApiUrl, requeteNote);

            if (!response.IsSuccessStatusCode)
            {
                // Journalisation
                return null;
            }
            return await response.Content.ReadFromJsonAsync<Note>();
        }

        public async Task Modifier(Note note)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync(_noteApiUrl + note.Id, note);

            if (!response.IsSuccessStatusCode)
            {
                // Journalisation - TODO
            }
        }

        public async Task<Note?> ObtenirSelonId(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_noteApiUrl + id);
            if (!response.IsSuccessStatusCode)
            {
                // Journalisation - TODO
                return null;
            }
            return await response.Content.ReadFromJsonAsync<Note>();
        }

        public async Task<IEnumerable<Note>?> ObtenirTout()
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_noteApiUrl);

            if (!response.IsSuccessStatusCode)
            {
                // Journalisation - TODO
                return null;
            }
            IEnumerable<Note>? notes = await response.Content.ReadFromJsonAsync<IEnumerable<Note>>();
            return notes ?? Enumerable.Empty<Note>();

        }

        public async Task Supprimer(Note note)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync(_noteApiUrl + note.Id);

            if (!response.IsSuccessStatusCode)
            {
                // Journalisation - TODO
            }
        }
        #endregion
    }
}
