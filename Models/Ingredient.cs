using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapstoneProject.Models;

public partial class Ingredient {
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? ImgPath { get; set; }

    public string? Description { get; set; }

    [NotMapped]
    public IFormFile file { get; set; }
    public int? FkCategoryId { get; set; }

    public virtual IngredientCategory? FkCategory { get; set; }

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}

[NotMapped]
public partial class Pager {
    public int TotalItems { get; private set; }
    public int CurrentPage { get; private set; }
    public int PageSize { get; private set; }
    public int TotalPages { get; private set; }
    public int StartPage { get; private set; }
    public int EndPage { get; private set; }

    public Pager() { }
    public Pager(int totalItems, int page, int pageSize= 10) {

        int totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
        int currentPage = page;

        int startPage = currentPage - 5;
        int endPage = currentPage + 4;
            
        if(startPage<=0) {
            endPage = endPage - (startPage -1);
            startPage = 1;
        }

        if(endPage > totalPages) {
            endPage = totalPages;
            if(endPage>10) {
                startPage = endPage - 9;
            }
        }

        TotalItems = totalItems;
        CurrentPage = currentPage;
        PageSize = pageSize;
        TotalPages = totalPages;
        StartPage = startPage;
        EndPage = endPage;
    }

}

