using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CapstoneProject.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Collections;
using Microsoft.IdentityModel.Tokens;
using SmartBreadcrumbs.Attributes;
using NuGet.Packaging;
using Firebase.Auth;
using Firebase.Storage;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats;

namespace CapstoneProject.Controllers {

    public class UserRecipesController : Controller {

        private readonly RecipeOrganizerContext _context;
        private readonly UserManager<Account> _userManager;

        public static string ApiKey = "AIzaSyDIXdDdvo8NguMgxLvn4DWMNS-vXkUxoag";
        public static string Bucket = "cookez-cloud.appspot.com";
        public static string AuthEmail = "cookez.mail@gmail.com";
        public static string AuthPassword = "cookez";
        public UserRecipesController(RecipeOrganizerContext context, UserManager<Account> userManager) {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public JsonResult SearchAutoComplete(string term) {
            var result = (_context.Recipes.Where(t => t.Name.ToLower().Contains(term.ToLower()))
                 .Select(t => new { t.Name }))
                 .ToList();
            return Json(result);
        }
        [HttpPost]
        public JsonResult IngredientsAutoComplete(string term) {
            var result = (_context.Ingredients.Where(t => t.Name.ToLower().Contains(term.ToLower()))
                 .Select(t => new { t.Name }))
                 .ToList();
            return Json(result);
        }

        // GET: Recipes
        [Breadcrumb("Recipes")]
        public async Task<IActionResult> Index(int pg = 1) {
            const int pageSize = 6; // Number of recipes in 1 page
            if (pg < 1)
                pg = 1;
            var recipes = _context.Recipes
                .Where(a => a.Status == true)
                .Include(b => b.Ingredients)
                .Select(b => b);
            if (recipes != null) {
                string? searchString = Request.Query["SearchString"];
                string? prepTime = Request.Query["PrepTime"];
                string? recipeCategory = Request.Query["RecipeCategory"];
                string? difficulty = Request.Query["Difficulty"];
                string? sortBy = Request.Query["SortBy"];
                string? includeList = Request.Query["IncludeList"];
                string? excludeList = Request.Query["ExcludeList"];

                //Add all recipecategories
                if (String.IsNullOrEmpty(prepTime))
                    prepTime = "All";
                if (String.IsNullOrEmpty(difficulty))
                    difficulty = "All";
                if (String.IsNullOrEmpty(recipeCategory))
                    recipeCategory = "All";
                if (String.IsNullOrEmpty(sortBy))
                    sortBy = "SortPopular";
                ViewBag.FkRecipeCategoryId = new SelectList(_context.RecipeCategories, "Id", "Name", recipeCategory);
                ViewBag.SortBy = new SelectList(
                new List<SelectListItem>
                {
                new SelectListItem { Text = "Popularity", Value = "SortPopular"},
                new SelectListItem { Text = "Name", Value = "SortName"},
                new SelectListItem { Text = "Date", Value = "SortDate"},
                new SelectListItem { Text = "Preparation time", Value = "SortPrepTime"},
                }
                , "Value", "Text", sortBy);

                //View Data
                ViewData["FilterSearch"] = searchString;
                ViewData["FilterPrepTime"] = prepTime;
                ViewData["FilterDifficulty"] = difficulty;
                ViewData["FilterIncludeList"] = includeList;
                ViewData["FilterExcludeList"] = excludeList;
                //Include Ingredients
                if (!prepTime.Equals("All")) {
                    recipes = recipes.Where(b => b.PrepTime <= int.Parse(prepTime));
                }
                if (!difficulty.Equals("All")) {
                    recipes = recipes.Where(b => b.Difficult == int.Parse(difficulty));
                }
                if (!recipeCategory.Equals("All")) {
                    recipes = recipes.Where(b => b.FkRecipeCategoryId == int.Parse(recipeCategory));
                }
                if (!String.IsNullOrEmpty(searchString)) {
                    recipes = recipes.Where(b => b.Name.ToLower().Contains(searchString.ToLower()));
                }
                //Sort
                switch (sortBy) {
                    case "SortDate":
                        recipes = recipes.OrderBy(b => b.CreatedDate);
                        break;
                    case "SortPrepTime":
                        recipes = recipes.OrderBy(b => b.PrepTime);
                        break;
                    case "SortName":
                        recipes = recipes.OrderBy(b => b.Name);
                        break;
                    default: // Sort by most popular
                        recipes = recipes.OrderByDescending(b => b.ViewCount);
                        break;
                }
                if (!String.IsNullOrEmpty(includeList)) {
                    var ingredientsList = includeList.Split(',');
                    ViewBag.IncludeList = ingredientsList;
                    foreach (var item in ingredientsList) {
                        recipes = recipes.Where(b => b.Ingredients.Any(i => i.Name.Equals(item)));
                    }
                } else
                    ViewBag.IncludeList = new List<String>();
                //Exclude
                if (!String.IsNullOrEmpty(excludeList)) {
                    var ingredientsList = excludeList.Split(',');
                    ViewBag.ExcludeList = ingredientsList;
                    foreach (var item in ingredientsList) {
                        recipes = recipes.Where(b => b.Ingredients.All(i => !i.Name.Equals(item)));
                    }
                } else
                    ViewBag.ExcludeList = new List<String>();

                //Favourite list
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                    ViewBag.FavouriteList = _context.Accounts.Include(u => u.Favourites).FirstOrDefault(u => u.Id == currentUser.Id).Favourites.Select(f => new { f.Id, f.Name }).ToList();
                else
                    ViewBag.FavouriteList = null;

                int recsCount = recipes.Count();

                var pager = new Pager(recsCount, pg, pageSize, includeList, excludeList, recipeCategory, prepTime, difficulty, sortBy);

                int recSkip = (pg - 1) * pageSize;

                var data = recipes.Skip(recSkip).Take(pager.PageSize).ToList();

                ViewData["count"] = recsCount;

                this.ViewBag.Pager = pager;
                return View(data);
            }
            return View();
        }
        [HttpPost]
        public IActionResult Filter(string searchString, string recipeCategory, string prepTime, string difficulty,
            string sortBy, string includeIngredients, string includeIngredientsList, string excludeIngredients, string excludeIngredientsList, int pg = 1) {
            string? includeList = includeIngredientsList;
            string? excludeList = excludeIngredientsList;
            if (!String.IsNullOrEmpty(includeIngredients)) {
                //Remove from exclude list if it has the include ingre inputed
                if (!String.IsNullOrEmpty(excludeList)) {
                    if (excludeList.IndexOf(includeIngredients, StringComparison.OrdinalIgnoreCase) != -1)
                        excludeList = RemoveFromStringWithComma(includeIngredients, excludeList);
                }
                if (String.IsNullOrEmpty(includeList)) {
                    includeList = includeIngredients.Trim();
                } else
                    includeList += "," + includeIngredients.Trim();
            }
            if (!String.IsNullOrEmpty(excludeIngredients)) {
                //Remove from include list if it has the exclude ingre inputed
                if (!String.IsNullOrEmpty(includeList)) {
                    if (includeList.IndexOf(excludeIngredients, StringComparison.OrdinalIgnoreCase) != -1)
                        includeList = RemoveFromStringWithComma(excludeIngredients, includeList);
                }
                if (String.IsNullOrEmpty(excludeList)) {
                    excludeList = excludeIngredients.Trim();
                } else
                    excludeList += "," + excludeIngredients.Trim();
            }
            var parameter = new RouteValueDictionary
            {
                { "pg",  pg},
                { "SearchString", searchString },
                { "RecipeCategory", recipeCategory },
                { "PrepTime", prepTime },
                { "Difficulty", difficulty },
                { "SortBy", sortBy },
                { "IncludeList", includeList },
                { "ExcludeList", excludeList }
            };
            return RedirectToAction("Index", parameter);
        }
        public async Task<IActionResult> Favorite(int? id, int? favouriteId, string returnUrl, string parameters) {
            var entity = _context.Recipes.FirstOrDefault(item => item.Id == id);
            if (entity != null) {
                var currentUser = await _userManager.GetUserAsync(User);
                var userFavouriteId = _context.Accounts.Include(u => u.Favourites).FirstOrDefault(u => u.Id == currentUser.Id).Favourites.ToArray();
                favouriteId = userFavouriteId[0].Id;
                //Get user from dbContext which include favorites
                var favourite = _context.Favourites.Include(a => a.Recipes).FirstOrDefault(a => a.Id == favouriteId);

                if (!favourite.Recipes.Contains(entity)) {
                    favourite.Recipes.Add(entity);
                    TempData["success"] = "Add to favourites successfully";
                } else {
                    favourite.Recipes.Remove(entity);
                    TempData["success"] = "Removed from favourites successfully";
                }
                await _context.SaveChangesAsync();
            }

            return LocalRedirect(returnUrl + parameters);
        }

        private string RemoveFromStringWithComma(string remove, string full) {
            full = full.Replace("," + remove, "");
            full = full.Replace(remove + ",", "");
            full = full.Replace(remove, "");
            return full;
        }

        [Breadcrumb("Details")]
        // GET: Recipes/Details/5
        public IActionResult Details(int? id) {
                var recipe = _context.Recipes
                    .Where(a => a.Status == true)
                    .Include(x => x.FkUser)
                    .Include(y => y.FkRecipeCategory)
                    .Include(z => z.Ingredients)
                    .Include(t => t.Nutrition)
                    .Include(a => a.RecipeIngredients)
                    .FirstOrDefault(a => a.Id == id);
                recipe.ViewCount++;
                _context.SaveChanges();
                return View(recipe);
        }

        // GET: Recipes/Create
        [Breadcrumb("Create recipe")]
        public IActionResult Create() {
            ViewData["FkRecipeId"] = new SelectList(_context.Recipes, "Id", "Id");
            ViewData["FkRecipeCategoryId"] = new SelectList(_context.RecipeCategories, "Id", "Name");

            // lay danh sach ingredient tu database
            var ingredients = _context.Ingredients.ToList();

            ViewData["Ingredients"] = new SelectList(ingredients, "Id", "Name"); // truyen qua view create

            return View();
        }

        // POST: Recipes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Recipe recipe, int[] IngredientIds, double[] Quantities, string[] UnitOfMeasures) {
            if (ModelState.IsValid) {
                if (recipe.file != null && recipe.file.Length > 0) {
                    IFormFile file = recipe.file;
                    // Generate a unique file name
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (currentUser != null) {
                        // Upload the file to Firebase Storage
                        string imageUrl = await UploadFirebase(file.OpenReadStream(), uniqueFileName);
                        Uri imageUrlUri = new(imageUrl);
                        string baseUrl = $"{imageUrlUri.GetLeftPart(UriPartial.Path)}?alt=media";
                        recipe.ImgPath = baseUrl;
                        recipe.Status = false;
                        recipe.ViewCount = 0;
                        recipe.CreatedDate = DateTime.Now;
                        recipe.FkUserId = await _userManager.GetUserIdAsync(currentUser);
                        // Save ingredient, recipe to IngredientRecipe
                        // Save ingredient IDs to recipe
                        if (IngredientIds != null && IngredientIds.Length > 0) {
                            var ingredients = _context.Ingredients.Where(i => IngredientIds.Contains(i.Id)).ToList();
                            recipe.Ingredients.AddRange(ingredients);
                            _context.Recipes.Add(recipe);
                            await _context.SaveChangesAsync();

                            // Create recipe ingredients with quantities and unit of measure
                            var recipeIngredients = new List<RecipeIngredient>();

                            for (int i = 0; i < IngredientIds.Length; i++) {
                                recipeIngredients.Add(new RecipeIngredient {
                                    IngredientId = IngredientIds[i],
                                    RecipeId = recipe.Id,
                                    Quantity = Quantities[i],
                                    UnitOfMeasure = UnitOfMeasures[i]
                                });
                            }
                            _context.RecipeIngredient.AddRange(recipeIngredients);
                        }
                    } else {
                        recipe.ImgPath = "untitle.jpg";
                    }

                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }

                ViewData["FkRecipeId"] = new SelectList(_context.Recipes, "Id", "Id", recipe.FkRecipeId);
                ViewData["FkRecipeCategoryId"] = new SelectList(_context.RecipeCategories, "Id", "Name", recipe.FkRecipeCategoryId);
            }

            return View(recipe);
        }

        // GET: Recipes/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null || _context.Recipes == null) {
                return NotFound();
            }

            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null) {
                return NotFound();
            }
            ViewData["FkRecipeId"] = new SelectList(_context.Recipes, "Id", "Id", recipe.FkRecipeId);
            ViewData["FkRecipeCategoryId"] = new SelectList(_context.RecipeCategories, "Id", "Name", recipe.FkRecipeCategoryId);
            return View(recipe);
        }

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ImgPath,Description,FkRecipeCategoryId,FkRecipeId,Nutrition,PrepTime,Difficult,FkUserId,CreatedDate")] Recipe recipe) {
            if (id != recipe.Id) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(recipe);
                    await _context.SaveChangesAsync();
                } catch (DbUpdateConcurrencyException) {
                    if (!RecipeExists(recipe.Id)) {
                        return NotFound();
                    } else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FkRecipeId"] = new SelectList(_context.Recipes, "Id", "Id", recipe.FkRecipeId);
            ViewData["FkRecipeCategoryId"] = new SelectList(_context.RecipeCategories, "Id", "Name", recipe.FkRecipeCategoryId);
            return View(recipe);
        }

        // GET: Recipes/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            ViewBag.Title = "Create recipe";
            if (id == null || _context.Recipes == null) {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.FkRecipe)
                .Include(r => r.FkRecipeCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recipe == null) {
                return NotFound();
            }

            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            if (_context.Recipes == null) {
                return Problem("Entity set 'RecipeOrganizerContext.Recipes'  is null.");
            }
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe != null) {
                _context.Recipes.Remove(recipe);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecipeExists(int id) {
            return (_context.Recipes?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        //firebase
        public static async Task<string> UploadFirebase(Stream stream, string fileName) {
            string imageFromFirebaseStorage = "";

            using (Image image = Image.Load(stream)) {

                // Resize the image to a smaller size if needed
                int maxWidth = 1000; // Set your desired maximum width here
                int maxHeight = 1000; // Set your desired maximum height here
                if (image.Width > maxWidth || image.Height > maxHeight) {
                    image.Mutate(x => x.Resize(new ResizeOptions {
                        Size = new Size(maxWidth, maxHeight),
                        Mode = ResizeMode.Max
                    }));
                }

                // Compress the image
                IImageEncoder imageEncoder;
                string fileExtension = Path.GetExtension(fileName).ToLower();
                if (fileExtension == ".png") {
                    imageEncoder = new PngEncoder { CompressionLevel = PngCompressionLevel.BestCompression };
                } else if (fileExtension == ".webp") {
                    imageEncoder = new SixLabors.ImageSharp.Formats.Webp.WebpEncoder { Quality = 100 }; // Adjust the quality level as needed
                } else {
                    imageEncoder = new JpegEncoder { Quality = 80 }; // Adjust the quality level as needed
                }

                using (MemoryStream webpStream = new MemoryStream()) {
                    image.Save(webpStream, new SixLabors.ImageSharp.Formats.Webp.WebpEncoder());

                    webpStream.Position = 0;

                    FirebaseAuthProvider firebaseConfiguration = new(new FirebaseConfig(ApiKey));

                    FirebaseAuthLink authConfiguration = await firebaseConfiguration
                        .SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

                    CancellationTokenSource cancellationToken = new();

                    FirebaseStorageTask storageManager = new FirebaseStorage(
                        Bucket,
                        new FirebaseStorageOptions {
                            AuthTokenAsyncFactory = () => Task.FromResult(authConfiguration.FirebaseToken),
                            ThrowOnCancel = true
                        })
                        .Child("images")
                        .Child("recipes")
                        .Child(fileName)
                        .PutAsync(webpStream, cancellationToken.Token);

                    try {
                        imageFromFirebaseStorage = await storageManager;
                        firebaseConfiguration.Dispose();
                        return imageFromFirebaseStorage;
                    } catch (Exception ex) {
                        Console.WriteLine("Exception was thrown: {0}", ex);
                        return null;
                    }
                }
            }
        }
    }
}