﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CapstoneProject.Models;

namespace CapstoneProject.Views.UserRecipes
{
    public class EditModel : PageModel
    {
        private readonly BusinessObjects.Models.RecipeOrganizerContext _context;

        public EditModel(BusinessObjects.Models.RecipeOrganizerContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BusinessObjects.Models.Recipe Recipe { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound", "Error", new { errorMessage = "The requested recipe was not found." });
            }

            Recipe = await _context.Recipes
                .Include(r => r.FkRecipe)
                .Include(r => r.FkRecipeCategory).FirstOrDefaultAsync(m => m.Id == id);

            if (Recipe == null)
            {
                return RedirectToAction("NotFound", "Error", new { errorMessage = "The requested recipe was not found." });
            }
           ViewData["FkRecipeId"] = new SelectList(_context.Recipes, "Id", "Id");
           ViewData["FkRecipeCategoryId"] = new SelectList(_context.RecipeCategories, "Id", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Recipe).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecipeExists(Recipe.Id))
                {
                    return RedirectToAction("NotFound", "Error", new { errorMessage = "The requested recipe was not found." });
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.Id == id);
        }
    }
}
