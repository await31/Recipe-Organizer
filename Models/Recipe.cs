﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapstoneProject.Models;

public partial class Recipe {
    [NotMapped]
    public IFormFile? file { get; set; }

    [Key]
    public int Id { get; set; }

    public string Name { get; set; }

    public string? ImgPath { get; set; }

    public int? ServingSize { get; set; }

    public string? Description { get; set; }
    public int? ViewCount { get; set; }
    public int? FkRecipeCategoryId { get; set; }

    public int? FkRecipeId { get; set; }

    public int? PrepTime { get; set; }

    public int? Difficult { get; set; }

    public string? FkUserId { get; set; }

    public Boolean? Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Nutrition? Nutrition { get; set; }

    public virtual Recipe? FkRecipe { get; set; }

    public virtual RecipeCategory? FkRecipeCategory { get; set; }

    public virtual Account? FkUser { get; set; }

    public virtual ICollection<Recipe> InverseFkRecipe { get; set; } = new List<Recipe>();

    public virtual ICollection<RecipeFeedback> RecipeFeedbacks { get; set; } = new List<RecipeFeedback>();

    public virtual ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();

    public virtual ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

    public virtual ICollection<MealPlan> Mealplans { get; set; } = new List<MealPlan>();

    public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
}


public partial class Nutrition {
    [ForeignKey("Recipe")]
    public int Id { get; set; }

    public int? Calories { get; set; }

    public int? Fat { get; set; }

    public int? Protein { get; set; }

    public int? Carbohydrate { get; set; }

    public int? Cholesterol { get; set; }

    public virtual Recipe? Recipe { get; set; }
}

public partial class RecipeIngredient {

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int? IngredientId { get; set; }

    public int? RecipeId { get; set; }

    public double? Quantity { get; set; }

    public string? UnitOfMeasure { get; set; }

    public virtual Ingredient? Ingredient { get; set; }

    public virtual Recipe? Recipe { get; set; }
}




