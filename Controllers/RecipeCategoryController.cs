﻿using CapstoneProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;

namespace CapstoneProject.Controllers {
    [Authorize(Roles = "Admin")]
    public class RecipeCategoryController : Controller {

        private readonly RecipeOrganizerContext _context;

        public RecipeCategoryController(RecipeOrganizerContext context) {
            _context = context;
        }

        [Breadcrumb("Recipe Categories Management")]
        public IActionResult Index() {
            IEnumerable<RecipeCategory> objRecipeCategoryList = _context.RecipeCategories.ToList();
            return View(objRecipeCategoryList);
        }


        //GET
        [Breadcrumb("Create", FromAction = "Index", FromController = typeof(RecipeCategoryController))]
        public IActionResult Create() {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RecipeCategory obj) {
            if (ModelState.IsValid) {
                _context.RecipeCategories.Add(obj);
                _context.SaveChanges();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET
        [Breadcrumb("Edit", FromAction = "Index", FromController = typeof(RecipeCategoryController))]
        public IActionResult Edit(int? id) {
            if (id == null || id == 0) {
                return NotFound();
            }
            var categoryFromDb = _context.RecipeCategories.Find(id);
            if (categoryFromDb == null) {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(RecipeCategory obj) {
            if (ModelState.IsValid) {
                _context.RecipeCategories.Update(obj);
                _context.SaveChanges();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET
        [Breadcrumb("Delete", FromAction = "Index", FromController = typeof(RecipeCategoryController))]
        public IActionResult Delete(int? id) {
            if (id == null || id == 0) {
                return NotFound();
            }
            var categoryFromDb = _context.RecipeCategories.Find(id);
            if (categoryFromDb == null) {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        //POST
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id) {

            var obj = _context.RecipeCategories.Find(id);
            if (obj == null) {
                return NotFound();
            }
            _context.RecipeCategories.Remove(obj);
            _context.SaveChanges();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
