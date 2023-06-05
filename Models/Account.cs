using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace CapstoneProject.Models;

public partial class Account : IdentityUser
{

    public string? ImgPath { get; set; }

    public bool? Status { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? FkFavouriteId { get; set; }

    public virtual ICollection<MealPlan> MealPlans { get; set; } = new List<MealPlan>();

    public virtual ICollection<RecipeFeedback> RecipeFeedbacks { get; set; } = new List<RecipeFeedback>();

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}
