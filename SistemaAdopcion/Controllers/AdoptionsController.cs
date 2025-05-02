using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaAdopcion.Models;
using System.Linq;

namespace SistemaAdopcion.Controllers
{
    public class AdoptionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdoptionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Adoptions/Create
        public IActionResult Create()
        {
            var availablePets = _context.Pets.Where(p => p.Adoption == null).ToList();
            if (!availablePets.Any())
            {
                TempData["ErrorMessage"] = "No hay mascotas disponibles para adopción.";
                return RedirectToAction("List");
            }

            ViewBag.Pets = new SelectList(availablePets, "Id", "Name");
            ViewBag.Adopters = new SelectList(_context.Adopters, "Id", "Name");
            return View();
        }

        // POST: Adoptions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Adoption adoption, string AdopterName, string AdopterEmail, string AdopterPhone)
        {
            // Registrar los valores enviados desde el formulario para depuración
            Console.WriteLine($"AdopterName: {AdopterName}");
            Console.WriteLine($"AdopterEmail: {AdopterEmail}");
            Console.WriteLine($"AdopterPhone: {AdopterPhone}");
            Console.WriteLine($"PetId: {adoption.PetId}");

            if (string.IsNullOrEmpty(AdopterName) || string.IsNullOrEmpty(AdopterEmail) || string.IsNullOrEmpty(AdopterPhone))
            {
                TempData["ErrorMessage"] = "Por favor, complete todos los campos del adoptante (nombre, correo y teléfono).";
                ViewBag.Pets = new SelectList(_context.Pets.Where(p => p.Adoption == null), "Id", "Name");
                return View(adoption);
            }

            if (adoption.PetId == 0)
            {
                TempData["ErrorMessage"] = "Por favor, seleccione una mascota.";
                ViewBag.Pets = new SelectList(_context.Pets.Where(p => p.Adoption == null), "Id", "Name");
                return View(adoption);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Buscar o registrar al adoptante
                    var adopter = _context.Adopters.FirstOrDefault(a => a.Name == AdopterName);
                    if (adopter == null)
                    {
                        adopter = new Adopter
                        {
                            Name = AdopterName,
                            Email = AdopterEmail,
                            Phone = AdopterPhone
                        };
                        _context.Adopters.Add(adopter);
                        _context.SaveChanges();
                    }

                    // Verificar si la mascota ya está adoptada
                    var pet = _context.Pets.FirstOrDefault(p => p.Id == adoption.PetId);
                    if (pet == null)
                    {
                        TempData["ErrorMessage"] = "La mascota seleccionada no existe.";
                        ViewBag.Pets = new SelectList(_context.Pets.Where(p => p.Adoption == null), "Id", "Name");
                        return View(adoption);
                    }

                    if (pet.IsAdopted)
                    {
                        TempData["ErrorMessage"] = "La mascota seleccionada ya ha sido adoptada.";
                        ViewBag.Pets = new SelectList(_context.Pets.Where(p => p.Adoption == null), "Id", "Name");
                        return View(adoption);
                    }

                    // Asignar AdopterId antes de validar el modelo
                    adoption.AdopterId = adopter?.Id ?? 0;

                    // Verificar si PetId y AdopterId están asignados correctamente
                    if (adoption.PetId == 0 || adoption.AdopterId == 0)
                    {
                        TempData["ErrorMessage"] = "Por favor, asegúrese de seleccionar una mascota y completar los datos del adoptante.";
                        ViewBag.Pets = new SelectList(_context.Pets.Where(p => p.Adoption == null), "Id", "Name");
                        return View(adoption);
                    }

                    try
                    {
                        // Registrar la adopción
                        adoption.AdopterId = adopter.Id;
                        adoption.AdoptionDate = DateTime.Now;

                        // Mensaje de depuración antes de guardar la adopción
                        Console.WriteLine("Guardando adopción: PetId = " + adoption.PetId + ", AdopterId = " + adoption.AdopterId);

                        _context.Adoptions.Add(adoption);
                        _context.SaveChanges();

                        // Mensaje de depuración después de guardar la adopción
                        Console.WriteLine("Adopción guardada correctamente.");

                        TempData["SuccessMessage"] = "La mascota fue asignada correctamente al adoptante.";
                        return RedirectToAction("List");
                    }
                    catch (DbUpdateException dbEx)
                    {
                        Console.WriteLine("Error al guardar en la base de datos: " + dbEx.InnerException?.Message ?? dbEx.Message);
                        TempData["ErrorMessage"] = "Error al guardar en la base de datos: " + dbEx.InnerException?.Message ?? dbEx.Message;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error inesperado: " + ex.Message);
                        TempData["ErrorMessage"] = "Error inesperado: " + ex.Message;
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Ocurrió un error inesperado: " + ex.Message;
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["ErrorMessage"] = "Errores en el formulario: " + string.Join("; ", errors);
            }

            ViewBag.Pets = new SelectList(_context.Pets.Where(p => p.Adoption == null), "Id", "Name");
            return View(adoption);
        }

        // GET: Adoptions/List
        public IActionResult List()
        {
            var adoptions = _context.Pets
                .Select(p => new {
                    PetName = p.Name,
                    AdopterName = p.Adoption != null ? p.Adoption.Adopter.Name : "Sin adoptante",
                    AdoptionStatus = p.AdoptionStatus
                })
                .ToList();

            ViewBag.Adoptions = adoptions;
            return View();
        }
    }
}