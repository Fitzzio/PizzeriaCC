using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PizzeriaCC.Models;

namespace PizzeriaCC.Controllers
{
    public class PedidoesController : Controller
    {
        private readonly PizzeriaBdContext _context;

        public PedidoesController(PizzeriaBdContext context)
        {
            _context = context;
        }

        // GET: Pedidoes
        public async Task<IActionResult> Index()
        {
            var pizzeriaBdContext = _context.Pedidos
                .Include(p => p.Motorizado)
                .Include(p => p.Usuario)
                .Include(p => p.PedidoDetalles)
                    .ThenInclude(pd => pd.Pizza)
                .OrderByDescending(p => p.Fecha);
            return View(await pizzeriaBdContext.ToListAsync());
        }

        // GET: Pedidoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .Include(p => p.Motorizado)
                .Include(p => p.Usuario)
                .Include(p => p.PedidoDetalles)
                    .ThenInclude(pd => pd.Pizza)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // GET: Pedidoes/Create
        public IActionResult Create()
        {
            CargarViewData();
            // Cargar lista de pizzas para el JavaScript
            ViewBag.Pizzas = _context.Pizzas
                .Select(p => new {
                    id = p.Id,
                    nombre = p.Nombre,
                    precioBase = p.PrecioBase
                })
                .ToList();
            return View();
        }

        // POST: Pedidoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("UsuarioId,Estado,MetodoPago,MotorizadoId")] Pedido pedido,
            List<PizzaSeleccionadaViewModel> pizzasSeleccionadas)
        {
            // Establecer la fecha automáticamente en el servidor
            pedido.Fecha = DateTime.Now;

            // Remover validaciones que se generan automáticamente
            ModelState.Remove("Fecha");
            ModelState.Remove("Total");
            ModelState.Remove("Usuario");
            ModelState.Remove("Motorizado");
            ModelState.Remove("PedidoDetalles");

            // Limpiar pizzas seleccionadas vacías (sin PizzaId)
            if (pizzasSeleccionadas != null)
            {
                pizzasSeleccionadas = pizzasSeleccionadas.Where(p => p.PizzaId > 0).ToList();
            }

            // Validar que haya pizzas seleccionadas
            if (pizzasSeleccionadas == null || !pizzasSeleccionadas.Any())
            {
                ModelState.AddModelError("", "Debe seleccionar al menos una pizza");
                CargarViewData(pedido.UsuarioId, pedido.MotorizadoId);
                ViewBag.Pizzas = _context.Pizzas
                    .Select(p => new { id = p.Id, nombre = p.Nombre, precioBase = p.PrecioBase })
                    .ToList();
                return View(pedido);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Calcular el total y crear los detalles del pedido
                    decimal total = 0;
                    var detalles = new List<PedidoDetalle>();

                    foreach (var pizzaSeleccionada in pizzasSeleccionadas)
                    {
                        var pizza = await _context.Pizzas.FindAsync(pizzaSeleccionada.PizzaId);
                        if (pizza != null && pizzaSeleccionada.Cantidad > 0)
                        {
                            var subtotal = pizza.PrecioBase * pizzaSeleccionada.Cantidad;
                            total += subtotal;

                            detalles.Add(new PedidoDetalle
                            {
                                PizzaId = pizza.Id,
                                Cantidad = pizzaSeleccionada.Cantidad,
                                Subtotal = subtotal
                            });
                        }
                    }

                    // Validar que se hayan agregado detalles
                    if (!detalles.Any())
                    {
                        ModelState.AddModelError("", "No se pudieron procesar las pizzas seleccionadas");
                        CargarViewData(pedido.UsuarioId, pedido.MotorizadoId);
                        ViewBag.Pizzas = _context.Pizzas
                            .Select(p => new { id = p.Id, nombre = p.Nombre, precioBase = p.PrecioBase })
                            .ToList();
                        return View(pedido);
                    }

                    pedido.Total = total;
                    pedido.PedidoDetalles = detalles;

                    _context.Add(pedido);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = $"Pedido #{pedido.Id} creado exitosamente. Total: S/ {total:N2}";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al crear el pedido: {ex.Message}");
                }
            }
            else
            {
                // Mostrar todos los errores del ModelState en la consola para debugging
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    System.Diagnostics.Debug.WriteLine($"Error de validación: {error.ErrorMessage}");
                }
            }

            // Si hay errores, volver a cargar los SelectList
            CargarViewData(pedido.UsuarioId, pedido.MotorizadoId);
            ViewBag.Pizzas = _context.Pizzas
                .Select(p => new { id = p.Id, nombre = p.Nombre, precioBase = p.PrecioBase })
                .ToList();
            return View(pedido);
        }

        // GET: Pedidoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .Include(p => p.PedidoDetalles)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
            {
                return NotFound();
            }

            CargarViewData(pedido.UsuarioId, pedido.MotorizadoId);
            ViewBag.Pizzas = _context.Pizzas
                .Select(p => new { id = p.Id, nombre = p.Nombre, precioBase = p.PrecioBase })
                .ToList();
            return View(pedido);
        }

        // POST: Pedidoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,UsuarioId,Fecha,Total,Estado,MetodoPago,MotorizadoId")] Pedido pedido,
            List<PizzaSeleccionadaViewModel> pizzasSeleccionadas)
        {
            if (id != pedido.Id)
            {
                return NotFound();
            }

            // Remover validaciones de navegación
            ModelState.Remove("Usuario");
            ModelState.Remove("Motorizado");
            ModelState.Remove("PedidoDetalles");

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener el pedido existente con sus detalles
                    var pedidoExistente = await _context.Pedidos
                        .Include(p => p.PedidoDetalles)
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (pedidoExistente == null)
                    {
                        return NotFound();
                    }

                    // Actualizar campos básicos
                    pedidoExistente.UsuarioId = pedido.UsuarioId;
                    pedidoExistente.Fecha = pedido.Fecha;
                    pedidoExistente.Estado = pedido.Estado;
                    pedidoExistente.MetodoPago = pedido.MetodoPago;
                    pedidoExistente.MotorizadoId = pedido.MotorizadoId;

                    // Si se enviaron pizzas, actualizar detalles
                    if (pizzasSeleccionadas != null && pizzasSeleccionadas.Any(p => p.PizzaId > 0))
                    {
                        // Eliminar detalles antiguos
                        _context.PedidoDetalles.RemoveRange(pedidoExistente.PedidoDetalles);

                        // Agregar nuevos detalles
                        decimal total = 0;
                        foreach (var pizzaSeleccionada in pizzasSeleccionadas.Where(p => p.PizzaId > 0))
                        {
                            var pizza = await _context.Pizzas.FindAsync(pizzaSeleccionada.PizzaId);
                            if (pizza != null)
                            {
                                var subtotal = pizza.PrecioBase * pizzaSeleccionada.Cantidad;
                                total += subtotal;

                                pedidoExistente.PedidoDetalles.Add(new PedidoDetalle
                                {
                                    PizzaId = pizza.Id,
                                    Cantidad = pizzaSeleccionada.Cantidad,
                                    Subtotal = subtotal
                                });
                            }
                        }
                        pedidoExistente.Total = total;
                    }

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Pedido actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PedidoExists(pedido.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al actualizar: {ex.Message}");
                }
            }

            CargarViewData(pedido.UsuarioId, pedido.MotorizadoId);
            ViewBag.Pizzas = _context.Pizzas
                .Select(p => new { id = p.Id, nombre = p.Nombre, precioBase = p.PrecioBase })
                .ToList();
            return View(pedido);
        }

        // GET: Pedidoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .Include(p => p.Motorizado)
                .Include(p => p.Usuario)
                .Include(p => p.PedidoDetalles)
                    .ThenInclude(pd => pd.Pizza)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // POST: Pedidoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.PedidoDetalles)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido != null)
            {
                // Los detalles se eliminarán automáticamente por la cascada
                _context.Pedidos.Remove(pedido);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Pedido eliminado exitosamente";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PedidoExists(int id)
        {
            return _context.Pedidos.Any(e => e.Id == id);
        }

        // Método auxiliar para cargar los ViewData
        private void CargarViewData(int? usuarioIdSeleccionado = null, int? motorizadoIdSeleccionado = null)
        {
            // SelectList para Usuarios - Mostrar nombre completo
            ViewData["UsuarioId"] = new SelectList(
                _context.Usuarios
                    .Select(u => new {
                        u.Id,
                        NombreCompleto = u.Nombre + " " + u.Apellido + " (" + u.Email + ")"
                    })
                    .AsEnumerable(),
                "Id",
                "NombreCompleto",
                usuarioIdSeleccionado
            );

            // SelectList para Motorizados - Mostrar nombre y disponibilidad
            ViewData["MotorizadoId"] = new SelectList(
                _context.Motorizados
                    .Select(m => new {
                        m.Id,
                        NombreConEstado = m.Nombre + (m.Disponible ? " ✓" : " (No disponible)")
                    })
                    .AsEnumerable(),
                "Id",
                "NombreConEstado",
                motorizadoIdSeleccionado
            );
        }
    }
}