using Microsoft.AspNetCore.Mvc;
using STC.Models;
using STC.Repository.Interfaces;
using STC.Services;

namespace STC.ViewComponents
{
    public class MenuCompeticionesViewComponent : ViewComponent
    {
      
        private ServiceApiSTC service;
        public MenuCompeticionesViewComponent(ServiceApiSTC service)
        {
            this.service = service;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Competicion> compes = await this.service.GetCompeticiones();
            return View(compes);
        }

    }
}
