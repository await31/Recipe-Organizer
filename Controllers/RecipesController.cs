using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using CapstoneProject.Models;
using Microsoft.AspNetCore.Authorization;
using Firebase.Auth;
using Firebase.Storage;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Webp;
using Microsoft.AspNetCore.WebUtilities;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using MailKit;
using System.Text.RegularExpressions;
using NuGet.Packaging;
using SmartBreadcrumbs.Attributes;
using Microsoft.AspNetCore.Identity;

namespace CapstoneProject.Controllers {


    [Authorize]
    public class RecipesController : Controller {
        private readonly RecipeOrganizerContext _context;
        private readonly UserManager<Account> _userManager;

        public static string ApiKey = "AIzaSyDIXdDdvo8NguMgxLvn4DWMNS-vXkUxoag";
        public static string Bucket = "cookez-cloud.appspot.com";
        public static string AuthEmail = "cookez.mail@gmail.com";
        public static string AuthPassword = "cookez";

        public RecipesController(RecipeOrganizerContext context, UserManager<Account> userManager) {
            _context = context;
            _userManager = userManager;
        }

        // GET/POST: Recipes
        [Breadcrumb("Recipes Management")]

        public IActionResult Index(int pg = 1) {
            var recipes = _context.Recipes
                .Include(x => x.FkRecipeCategory)
                .Include(x => x.FkUser)
                .ToList();

            /*
             if (recipes != null) {
                string? searchString = Request.Query["SearchString"];
                string? prepTime = Request.Query["PrepTime"];
                string? recipeCategory = Request.Query["RecipeCategory"];
                string? difficulty = Request.Query["Difficulty"];
                string? sortBy = Request.Query["SortBy"];

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
                new SelectListItem { Text = "Sort By Popularity", Value = "SortPopular"},
                new SelectListItem { Text = "Sort By Name", Value = "SortName"},
                new SelectListItem { Text = "Sort By Date", Value = "SortDate"},
                new SelectListItem { Text = "Sort By PrepTime", Value = "SortPrepTime"},
                }
                , "Value", "Text", sortBy);

                ViewData["FilterSearch"] = searchString;
                ViewData["FilterPrepTime"] = prepTime;
                ViewData["FilterDifficulty"] = difficulty;
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
            }
            //Favorite
            var currentUser = await _userManager.GetUserAsync(User);
            ViewBag.FavoriteList = null;

            if (currentUser != null) {
                //Get user from dbContext which include favorites
                var user = _context.Accounts.Include(u => u.Favourites).FirstOrDefault(u => u.Id == currentUser.Id);
                List<int> favoritedRecipes = user.Favourites.SelectMany(c => c.Recipes).Select(r => r.Id).ToList();
                ViewBag.FavoriteList = favoritedRecipes;
            }
             */
            const int pageSize = 10; // Number of recipes in 1 page

            if (pg < 1)
                pg = 1;

            int recsCount = recipes.Count();

            var pager = new Pager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            var data = recipes.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;

            return View(data);
        }

        // GET: Recipes/Details/5
        public async Task<IActionResult> Details(int? id) {
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

        [Authorize]
        // GET: Recipes/Delete/5
        public async Task<IActionResult> Delete(int? id) {
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
        [Authorize]
        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            if (_context.Recipes == null) {
                return Problem("Entity set 'RecipeOrganizerContext.Recipes'  is null.");
            }

            var recipeIngredient = _context.RecipeIngredient
                .Where(r => r.RecipeId == id)
                .ToList();
            _context.RecipeIngredient.RemoveRange(recipeIngredient);

            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe != null) {
                if (recipe.ImgPath != null) {
                    await DeleteFromFirebaseStorage(recipe.ImgPath);
                }
                _context.Recipes.Remove(recipe);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecipeExists(int id) {
            return (_context.Recipes?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [Authorize]
        // POST: Recipes/Approve
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id) {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null) {
                return NotFound();
            }

            recipe.Status = true; // Set the status to approved
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [Authorize]
        // POST: Recipes/Deny
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deny(int id) {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null) {
                return NotFound();
            }

            await DeleteFromFirebaseStorage(recipe.ImgPath);
            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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

                using (MemoryStream webpStream = new()) {

                    await image.SaveAsync(webpStream, new WebpEncoder());

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

        private async Task DeleteFromFirebaseStorage(string fileName) {
            if (!string.IsNullOrEmpty(fileName)) {
                string exactFileName = GetImageNameFromUrl(fileName);
                FirebaseAuthProvider firebaseConfiguration = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                FirebaseAuthLink authConfiguration = await firebaseConfiguration
                    .SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

                FirebaseStorage storage = new FirebaseStorage(
                    Bucket,
                    new FirebaseStorageOptions {
                        AuthTokenAsyncFactory = () => Task.FromResult(authConfiguration.FirebaseToken),
                        ThrowOnCancel = true
                    });

                await storage
                    .Child("images")
                    .Child("recipes")
                    .Child(exactFileName) // Use the fileName directly as the child reference
                    .DeleteAsync();

                firebaseConfiguration.Dispose();
            }
        }

        public static string GetImageNameFromUrl(string url) {
            int lastSeparatorIndex = url.LastIndexOf("%2F"); // Get the index of the last "%2F"
            int startIndex = lastSeparatorIndex + 3; // Start index of the desired string
            int endIndex = url.IndexOf("?alt", startIndex); // End index of the desired string
            string extractedString = url.Substring(startIndex, endIndex - startIndex); // Extract the string between the last "%2F" and before "?alt"
            return extractedString;
        }
    }
}