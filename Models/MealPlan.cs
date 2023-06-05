using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CapstoneProject.Models;

public partial class MealPlan
{
    [Key]
    public int Id { get; set; }

    public string? FkUserId { get; set; }

    public DateTime? PlannedDate { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Account? FkUser { get; set; }

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}
