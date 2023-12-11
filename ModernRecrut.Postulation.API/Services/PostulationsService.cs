using ModernRecrut.Postulation.API.DTO;
using ModernRecrut.Postulation.API.Interfaces;

namespace ModernRecrut.Postulation.API.Services
{
    public class PostulationsService : IPostulationsService
    {
        private readonly IAsyncRepository<Models.Postulation> _postulationRepository;

        public PostulationsService(IAsyncRepository<Models.Postulation> postulationRepository)
        {
            _postulationRepository = postulationRepository;
        }

        public async Task<Models.Postulation?> Ajouter(RequetePostulation requetePostulation)
        {
            Models.Postulation postulation = new Models.Postulation()
            {
                CandidatId = requetePostulation.CandidatId,
                OffreDEmploiId = requetePostulation.OffreDemploiId,
                PretentionSalariale = requetePostulation.PretentionSalariale,
                DateDisponibilite = requetePostulation.DateDisponibilite
            };
            return await _postulationRepository.AddAsync(postulation);
        }

        public async Task<bool> Modifier(Models.Postulation postulation)
        {
            await _postulationRepository.EditAsync(postulation);
            return true;
        }

        public async Task<Models.Postulation?> ObtenirSelonId(int id)
        {
            return await _postulationRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Models.Postulation>> ObtenirTout()
        {
            return await _postulationRepository.ListAsync();
        }

        public Task Supprimer(Models.Postulation postulation)
        {
            return _postulationRepository.DeleteAsync(postulation);
        }
    }
}
