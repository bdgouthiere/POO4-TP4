using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModernRecrut.MVC.Areas.Identity.Data;

namespace ModernRecrut.MVC.Controllers
{
	public class PostulationsController : Controller
	{
        #region Attributs
		private readonly ILogger _logger;
		private readonly UserManager<Utilisateur> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
        #endregion

        #region Constructeur
        public PostulationsController(ILogger<PostulationsController> logger,
			UserManager<Utilisateur> userManager,
			RoleManager<IdentityRole> roleManager)
        {
			_logger = logger;
			_userManager = userManager;
			_roleManager = roleManager;
        }
		#endregion

		#region Méthodes publiques
		// Postuler (Accessible - Candidat ou Admin)
		[Authorize(Roles="Admin, Candidat")]
		public ActionResult Postuler()
		{
			// Journalisation
			_logger.LogInformation($"Visite de la page postuler par l'utilisateur {User.Identity.Name}");

			return View();
		}

        // Liste Postulation (Accessible - Employé ou Admin)
		[Authorize(Roles="Admin, Employé")]
        public ActionResult ListePostulations()
		{
			// Journalisation
			_logger.LogInformation($"Visite de la page liste des postulation par l'utilisateur {User.Identity.Name}");

			return View();
		}

		// Notes (Accessible RH ou Admin)
		[Authorize(Roles="Admin, RH")]
		public ActionResult Notes()
		{
			// Journalisation
			_logger.LogInformation($"Visite de la page notes() par l'utilisateur {User.Identity.Name}");

			return View();
		}
		#endregion
	}
}
