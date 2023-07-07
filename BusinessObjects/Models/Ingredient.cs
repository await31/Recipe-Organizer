using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjects.Models;

public partial class Ingredient {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? ImgPath { get; set; }

    public string? Description { get; set; }

    public bool? Status { get; set; }

    [NotMapped]
    public IFormFile? file { get; set; }
    public int? FkCategoryId { get; set; }

    public virtual IngredientCategory? FkCategory { get; set; }

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}




