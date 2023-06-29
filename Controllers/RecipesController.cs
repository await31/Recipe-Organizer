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


    [Authorize(Roles = "Admin")]
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
                .Where(a => a.Status == true)
                .Include(x => x.FkRecipeCategory)
                .Include(x => x.FkUser)
                .ToList();

            const int pageSize = 10; 

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
        public IActionResult Details(int? id) {

            if (id == null || _context.Recipes == null) {
                return NotFound();
            }

            var recipe = _context.Recipes
                .Include(x => x.FkUser)
                .Include(y => y.FkRecipeCategory)
                .Include(t => t.Nutrition)
                .Include(a => a.RecipeIngredients)
                .ThenInclude(a => a.Ingredient)
                .FirstOrDefault(m => m.Id == id);

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

            var recipeFeedbacks = _context.RecipeFeedbacks
                .Where(r => r.RecipeId == id)
                .ToList();
            _context.RecipeFeedbacks.RemoveRange(recipeFeedbacks);

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

            return RedirectToAction("Index", "Dashboard");
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

            var recipeIngredient = _context.RecipeIngredient
               .Where(r => r.RecipeId == id)
               .ToList();
            _context.RecipeIngredient.RemoveRange(recipeIngredient);

            await DeleteFromFirebaseStorage(recipe.ImgPath);
            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Dashboard");
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