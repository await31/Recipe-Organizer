using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapstoneProject.Models;

public partial class Favourite {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    public virtual Account Account { get; set; }

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();


}