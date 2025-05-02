using Microsoft.AspNetCore.Mvc;
using SistemaAdopcion.Models;

namespace SistemaAdopcion.Controllers
{
    public class PetsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Pets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Pet pet)
        {
            if (ModelState.IsValid)
            {
                _context.Pets.Add(pet);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(pet);
        }
    }
}