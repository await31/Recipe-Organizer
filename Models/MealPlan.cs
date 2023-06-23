using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapstoneProject.Models;

public partial class MealPlan
{
    [Key]
    public int Id { get; set; }

    public string? FkUserId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    [BindProperty, DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
    public DateTime? Date { get; set; }

    public string? Color { get; set; }

    [NotMapped]
    public int? Day { get; set; }

    [NotMapped]
    public int? WeekLast { get; set; }

    public bool IsFullDay { get; set; }

    //public DateTime? CreatedDate { get; set; }

    public virtual Account? FkUser { get; set; }

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();


}

