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
using SmartBreadcrumbs.Attributes;

namespace CapstoneProject.Controllers {

    [Authorize(Roles ="Admin")]
    public class IngredientsController : Controller {

        private readonly RecipeOrganizerContext _context;

        public static string ApiKey = "AIzaSyDIXdDdvo8NguMgxLvn4DWMNS-vXkUxoag";
        public static string Bucket = "cookez-cloud.appspot.com";
        public static string AuthEmail = "cookez.mail@gmail.com";
        public static string AuthPassword = "cookez";

        public IngredientsController(RecipeOrganizerContext context) {
            _context = context;
        }

        [Breadcrumb("Ingredients Management")]
        public IActionResult Index() {
            IEnumerable<Ingredient> objIngredient = _context.Ingredients.Include(r=>r.FkCategory).ToList();
            return View(objIngredient);
        }

        // GET
        [Breadcrumb("Create", FromAction = "Index", FromController = typeof(IngredientsController))]

        public IActionResult Create() {
            ViewData["FkCategoryId"] = new SelectList(_context.IngredientCategories, "Id", "Name");
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ingredient model) {
            if (ModelState.IsValid) {
                if (model.file != null && model.file.Length > 0) {
                    IFormFile file = model.file;
                    // Generate a unique file name
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    // Upload the file to Firebase Storage
                    string imageUrl = await UploadFirebase(file.OpenReadStream(), uniqueFileName);
                    Uri imageUrlUri = new Uri(imageUrl);
                    string baseUrl = $"{imageUrlUri.GetLeftPart(UriPartial.Path)}?alt=media";
                    model.ImgPath = baseUrl;
                    model.Status = true;
                    _context.Ingredients.Add(model);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Ingredient created successfully";
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAjax(Ingredient model) {
            if (ModelState.IsValid) {
                if (model.file != null && model.file.Length > 0) {
                    IFormFile file = model.file;
                    // Generate a unique file name
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    // Upload the file to Firebase Storage
                    string imageUrl = await UploadFirebase(file.OpenReadStream(), uniqueFileName);
                    Uri imageUrlUri = new Uri(imageUrl);
                    string baseUrl = $"{imageUrlUri.GetLeftPart(UriPartial.Path)}?alt=media";
                    model.ImgPath = baseUrl;
                    model.Status = false;
                    _context.Ingredients.Add(model);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Ingredient created successfully";
                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }

        // GET: Ingredient/Detail/id
        public IActionResult Detail(int? id) {

            if (id == null || _context.Ingredients == null) {
                return NotFound();
            }

            var ingredient = _context.Ingredients
                        .FirstOrDefault(m => m.Id == id);

            if (ingredient == null) {
                return NotFound();
            }

            return View(ingredient);
        }


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
                        .Child("ingredients")
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


        //GET
        [Breadcrumb("Edit", FromAction = "Index", FromController = typeof(IngredientsController))]
        public async Task<IActionResult> Edit(int? id) {
            if (id == null || _context.Ingredients == null) {
                return NotFound();
            }
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null) {
                return NotFound();
            }
            ViewData["FkCategoryId"] = new SelectList(_context.IngredientCategories, "Id", "Name");
            return View(ingredient);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Name, ImgPath, Description, FkCategoryId")] Ingredient ingredient) {
            if (id != ingredient.Id) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(ingredient);
                    await _context.SaveChangesAsync();
                } catch (DbUpdateConcurrencyException) {
                    if (!IngredientExists(ingredient.Id)) {
                        return NotFound();
                    } else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FkCategoryId"] = new SelectList(_context.IngredientCategories, "Id", "Name", ingredient.FkCategoryId);
            return View(ingredient);
        }

        private bool IngredientExists(int id) {
            return (_context.Ingredients?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        //GET
        [Breadcrumb("Delete", FromAction = "Index", FromController = typeof(IngredientsController))]
        public IActionResult Delete(int? id) {
            if (id == null || id == 0) {
                return NotFound();
            }
            var ingredientFromDb = _context.Ingredients.Find(id);
            if (ingredientFromDb == null) {
                return NotFound();
            }
            return View(ingredientFromDb);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST(int? id) {
            var obj = _context.Ingredients.Find(id);
            if (obj == null) {
                return NotFound();
            }

            await DeleteFromFirebaseStorage(obj.ImgPath);

            _context.Ingredients.Remove(obj);
            _context.SaveChanges();
            TempData["success"] = "Ingredient deleted successfully";
            return RedirectToAction("Index");
        }


        // POST: Recipes/Approve
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id) {
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null) {
                return NotFound();
            }

            ingredient.Status = true; // Set the status to approved
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Dashboard");
        }
        [Authorize]
        // POST: Recipes/Deny
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deny(int id) {
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null) {
                return NotFound();
            }

            _context.Ingredients.RemoveRange(ingredient);

            await DeleteFromFirebaseStorage(ingredient.ImgPath);
            _context.Ingredients.Remove(ingredient);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Dashboard");
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
                    .Child("ingredients")
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


