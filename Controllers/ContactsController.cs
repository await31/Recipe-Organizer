using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CapstoneProject.Models;

namespace CapstoneProject.Controllers {
    public class ContactsController : Controller {
        private readonly RecipeOrganizerContext _context;

        public ContactsController(RecipeOrganizerContext context) {
            _context = context;
        }

        // GET: Contacts
        public async Task<IActionResult> Index() {
            return _context.Contacts != null ?
                        View(await _context.Contacts.ToListAsync()) :
                        Problem("Entity set 'RecipeOrganizerContext.Contact'  is null.");
        }

        // GET: Contacts/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null || _context.Contacts == null) {
                return NotFound();
            }

            var contact = await _context.Contacts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null) {
                return NotFound();
            }

            return View(contact);
        }

        // GET: Contacts/Create
        public IActionResult Create() {

            return View();
        }

        // POST: Contacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Message,Email,Message,UserId")] Contact contact) {
            if (ModelState.IsValid) {
                _context.Add(contact);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(contact);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAjax([Bind("UserName,Email,Message,UserId")] Contact model) {
            if (ModelState.IsValid) {

                _context.Add(model);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
        // GET: Contacts/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null || _context.Contacts == null) {
                return NotFound();
            }

            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null) {
                return NotFound();
            }
            return View(contact);
        }

        // POST: Contacts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Message,UserId")] Contact contact) {
            if (id != contact.Id) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(contact);
                    await _context.SaveChangesAsync();
                } catch (DbUpdateConcurrencyException) {
                    if (!ContactExists(contact.Id)) {
                        return NotFound();
                    } else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(contact);
        }

        // GET: Contacts/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null || _context.Contacts == null) {
                return NotFound();
            }

            var contact = await _context.Contacts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null) {
                return NotFound();
            }

            return View(contact);
        }

        // POST: Contacts/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            if (_context.Contacts == null) {
                return Problem("Entity set 'RecipeOrganizerContext.Contact'  is null.");
            }
            var contact = await _context.Contacts.FindAsync(id);
            if (contact != null) {
                _context.Contacts.Remove(contact);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Dashboard");
        }

        private bool ContactExists(int id) {
            return (_context.Contacts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}