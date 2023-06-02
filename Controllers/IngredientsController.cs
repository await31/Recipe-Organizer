using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using CapstoneProject.Models;
using Microsoft.AspNetCore.Authorization;
using Firebase.Auth;
using Firebase.Storage;
using System.IO;

namespace CapstoneProject.Controllers {


    [Authorize(Roles = "Admin")]
    public class IngredientsController : Controller {

        private readonly RecipeOrganizerContext _context;

        public static string ApiKey = "AIzaSyDIXdDdvo8NguMgxLvn4DWMNS-vXkUxoag";
        public static string Bucket = "cookez-cloud.appspot.com";
        public static string AuthEmail = "cookez.mail@gmail.com";
        public static string AuthPassword = "cookez";

        public IngredientsController(RecipeOrganizerContext context) {
            _context = context;
        }

        public IActionResult Index() {
            IEnumerable<Ingredient> objIngredient = _context.Ingredients.ToList();
            return View(objIngredient);
        }

        // GET
        public IActionResult Create() {
            ViewData["FkCategoryId"] = new SelectList(_context.IngredientCategories, "Id", "Name");
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ingredient model) {
            IFormFile file = model.file;
            if (ModelState.IsValid) {
                if (file != null && file.Length > 0) {
                    // Generate a unique file name
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;

                    // Upload the file to Firebase Storage
                    string imageUrl = await Upload(file.OpenReadStream(), uniqueFileName);
                    model.ImgPath = imageUrl;
                    _context.Ingredients.Add(model);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        public static async Task<string> Upload(Stream stream, string fileName) {

            string imageFromFirebaseStorage = "";

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
                .PutAsync(stream, cancellationToken.Token);

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


