using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    public class BookingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
