using Microsoft.EntityFrameworkCore;

namespace CapstoneProject.Models;

public partial class RecipeOrganizerContext : DbContext
{
    public RecipeOrganizerContext()
    {
    }

    public RecipeOrganizerContext(DbContextOptions<RecipeOrganizerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Favourite> Favourites { get; set; }

    public virtual DbSet<Ingredient> Ingredients { get; set; }

    public virtual DbSet<IngredientCategory> IngredientCategories { get; set; }

    public virtual DbSet<MealPlan> MealPlans { get; set; }

    public virtual DbSet<Recipe> Recipes { get; set; }

    public virtual DbSet<RecipeCategory> RecipeCategories { get; set; }

    public virtual DbSet<RecipeFeedback> RecipeFeedbacks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=KhoaLab;Initial Catalog=RecipeOrganizer;Trusted_Connection=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Accounts__3213E83F75949339");

            entity.HasIndex(e => e.Name, "UQ__Accounts__72E12F1B5202D650").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Accounts__AB6E61647C0342C5").IsUnique();

            entity.HasIndex(e => e.Phone, "UQ__Accounts__B43B145F8F3C6400").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("date")
                .HasColumnName("created_date");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.ImgPath)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("imgPath");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Favourite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Favourit__3213E83F911C8CFA");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FkUserId).HasColumnName("fk_user_id");

            entity.HasOne(d => d.FkUser).WithMany(p => p.Favourites)
                .HasForeignKey(d => d.FkUserId)
                .HasConstraintName("FK__Favourite__fk_us__52593CB8");

            entity.HasMany(d => d.Recipes).WithMany(p => p.Favourites)
                .UsingEntity<Dictionary<string, object>>(
                    "FavouriteRecipe",
                    r => r.HasOne<Recipe>().WithMany()
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Favourite__recip__5629CD9C"),
                    l => l.HasOne<Favourite>().WithMany()
                        .HasForeignKey("FavouriteId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Favourite__favou__5535A963"),
                    j =>
                    {
                        j.HasKey("FavouriteId", "RecipeId").HasName("PK__Favourit__10B05C1733C539E1");
                        j.ToTable("FavouriteRecipes");
                        j.IndexerProperty<int>("FavouriteId").HasColumnName("favourite_id");
                        j.IndexerProperty<int>("RecipeId").HasColumnName("recipe_id");
                    });
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Ingredie__3213E83F3D7B9EF0");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasColumnType("ntext")
                .HasColumnName("description");
            entity.Property(e => e.FkCategoryId).HasColumnName("fk_category_id");
            entity.Property(e => e.ImgPath)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("imgPath");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");

            entity.HasOne(d => d.FkCategory).WithMany(p => p.Ingredients)
                .HasForeignKey(d => d.FkCategoryId)
                .HasConstraintName("FK__Ingredien__fk_ca__4BAC3F29");
        });

        modelBuilder.Entity<IngredientCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Ingredie__3213E83F8C523201");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .HasColumnName("name");
        });

        modelBuilder.Entity<MealPlan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MealPlan__3213E83FBBCA8702");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("date")
                .HasColumnName("created_date");
            entity.Property(e => e.FkUserId).HasColumnName("fk_user_id");
            entity.Property(e => e.PlannedDate)
                .HasColumnType("date")
                .HasColumnName("planned_date");

            entity.HasOne(d => d.FkUser).WithMany(p => p.MealPlans)
                .HasForeignKey(d => d.FkUserId)
                .HasConstraintName("FK__MealPlans__fk_us__59063A47");

            entity.HasMany(d => d.Recipes).WithMany(p => p.Mealplans)
                .UsingEntity<Dictionary<string, object>>(
                    "MealPlanRecipe",
                    r => r.HasOne<Recipe>().WithMany()
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MealPlanR__recip__5CD6CB2B"),
                    l => l.HasOne<MealPlan>().WithMany()
                        .HasForeignKey("MealplanId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MealPlanR__mealp__5BE2A6F2"),
                    j =>
                    {
                        j.HasKey("MealplanId", "RecipeId").HasName("mealplan_recipe_fk");
                        j.ToTable("MealPlanRecipes");
                        j.IndexerProperty<int>("MealplanId").HasColumnName("mealplan_id");
                        j.IndexerProperty<int>("RecipeId").HasColumnName("recipe_id");
                    });
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Recipes__3213E83F450255C4");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("date")
                .HasColumnName("created_date");
            entity.Property(e => e.Description)
                .HasColumnType("ntext")
                .HasColumnName("description");
            entity.Property(e => e.Difficult).HasColumnName("difficult");
            entity.Property(e => e.FkRecipeCategoryId).HasColumnName("fk_recipe_category_id");
            entity.Property(e => e.FkRecipeId).HasColumnName("fk_recipe_id");
            entity.Property(e => e.FkUserId).HasColumnName("fk_user_id");
            entity.Property(e => e.ImgPath)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("imgPath");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Nutrition).HasColumnName("nutrition");
            entity.Property(e => e.PrepTime).HasColumnName("prep_time");

            entity.HasOne(d => d.FkRecipeCategory).WithMany(p => p.Recipes)
                .HasForeignKey(d => d.FkRecipeCategoryId)
                .HasConstraintName("FK__Recipes__fk_reci__403A8C7D");

            entity.HasOne(d => d.FkRecipe).WithMany(p => p.InverseFkRecipe)
                .HasForeignKey(d => d.FkRecipeId)
                .HasConstraintName("FK__Recipes__fk_reci__412EB0B6");

            entity.HasOne(d => d.FkUser).WithMany(p => p.Recipes)
                .HasForeignKey(d => d.FkUserId)
                .HasConstraintName("FK__Recipes__fk_user__440B1D61");

            entity.HasMany(d => d.Ingredients).WithMany(p => p.Recipes)
                .UsingEntity<Dictionary<string, object>>(
                    "RecipeIngredient",
                    r => r.HasOne<Ingredient>().WithMany()
                        .HasForeignKey("IngredientId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__RecipeIng__ingre__4F7CD00D"),
                    l => l.HasOne<Recipe>().WithMany()
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__RecipeIng__recip__4E88ABD4"),
                    j =>
                    {
                        j.HasKey("RecipeId", "IngredientId").HasName("PK__RecipeIn__DE7FA8A7E2C5E17C");
                        j.ToTable("RecipeIngredient");
                        j.IndexerProperty<int>("RecipeId").HasColumnName("recipe_id");
                        j.IndexerProperty<int>("IngredientId").HasColumnName("ingredient_id");
                    });
        });

        modelBuilder.Entity<RecipeCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RecipeCa__3213E83F72CE092F");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RecipeFeedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RecipeFe__3213E83F8FF468EC");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("date")
                .HasColumnName("created_date");
            entity.Property(e => e.Description)
                .HasColumnType("ntext")
                .HasColumnName("description");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.RecipeId).HasColumnName("recipe_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Recipe).WithMany(p => p.RecipeFeedbacks)
                .HasForeignKey(d => d.RecipeId)
                .HasConstraintName("FK__RecipeFee__recip__46E78A0C");

            entity.HasOne(d => d.User).WithMany(p => p.RecipeFeedbacks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__RecipeFee__user___47DBAE45");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
