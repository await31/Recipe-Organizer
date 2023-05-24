using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CapstoneProject.Models;

public partial class RecipeFeedback
{

    [Key]
    public int Id { get; set; }

    public int? RecipeId { get; set; }

    public int? UserId { get; set; }

    public string? Description { get; set; }

    public int? Rating { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Recipe? Recipe { get; set; }

    public virtual Account? User { get; set; }
}
