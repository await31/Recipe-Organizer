using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CapstoneProject.Models;

public partial class Recipe
{

    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? ImgPath { get; set; }

    public string? Description { get; set; }

    public int? FkRecipeCategoryId { get; set; }

    public int? FkRecipeId { get; set; }

    public int? Nutrition { get; set; }

    public int? PrepTime { get; set; }

    public int? Difficult { get; set; }

    public int? FkUserId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Recipe? FkRecipe { get; set; }

    public virtual RecipeCategory? FkRecipeCategory { get; set; }

    public virtual Account? FkUser { get; set; }
        
    public virtual ICollection<Recipe> InverseFkRecipe { get; set; } = new List<Recipe>();

    public virtual ICollection<RecipeFeedback> RecipeFeedbacks { get; set; } = new List<RecipeFeedback>();

    public virtual ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();

    public virtual ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

    public virtual ICollection<MealPlan> Mealplans { get; set; } = new List<MealPlan>();
}
