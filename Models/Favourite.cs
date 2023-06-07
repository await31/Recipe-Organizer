using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapstoneProject.Models;

public partial class Favourite
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int FavouriteId { get; set; }

    public virtual Account Account { get; set; }

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    
}
