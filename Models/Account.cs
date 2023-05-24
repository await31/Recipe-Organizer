using System.ComponentModel.DataAnnotations;

namespace CapstoneProject.Models;

public partial class Account
{
    [Key]
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Password { get; set; }

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? ImgPath { get; set; }

    public bool? Status { get; set; }

    public bool? Role { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();

    public virtual ICollection<MealPlan> MealPlans { get; set; } = new List<MealPlan>();

    public virtual ICollection<RecipeFeedback> RecipeFeedbacks { get; set; } = new List<RecipeFeedback>();

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}
