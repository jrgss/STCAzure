using Microsoft.AspNetCore.Mvc;
using STC.Models;
using STC.Repository.Interfaces;

namespace STC.ViewComponents
{
    public class MenuCompeticionesViewComponent : ViewComponent
    {
        private IRepositoryCompeticion repo;
        public MenuCompeticionesViewComponent(IRepositoryCompeticion repo)
        {
            this.repo = repo;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Competicion> compes = await this.repo.GetCompeticiones();
            return View(compes);
        }

    }
}
