﻿// <auto-generated />
using System;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BusinessObjects.Migrations
{
    [DbContext(typeof(RecipeOrganizerContext))]
    [Migration("20230729014544_AddResponseMessageToRecipe")]
    partial class AddResponseMessageToRecipe
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BusinessObjects.Models.Account", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("ImgPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("Status")
                        .HasColumnType("bit");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("BusinessObjects.Models.Contact", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Contacts");
                });

            modelBuilder.Entity("BusinessObjects.Models.Favourite", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AccountId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("isPrivate")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("Favourites");
                });

            modelBuilder.Entity("BusinessObjects.Models.Ingredient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("FkCategoryId")
                        .HasColumnType("int");

                    b.Property<string>("ImgPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("Status")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("FkCategoryId");

                    b.ToTable("Ingredients");
                });

            modelBuilder.Entity("BusinessObjects.Models.IngredientCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("IngredientCategories");
                });

            modelBuilder.Entity("BusinessObjects.Models.IngredientNutrition", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<double?>("Calories")
                        .HasColumnType("float");

                    b.Property<double?>("Carbohydrate")
                        .HasColumnType("float");

                    b.Property<double?>("Cholesterol")
                        .HasColumnType("float");

                    b.Property<double?>("Fat")
                        .HasColumnType("float");

                    b.Property<double?>("Fibre")
                        .HasColumnType("float");

                    b.Property<double?>("Protein")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("IngredientNutrition");
                });

            modelBuilder.Entity("BusinessObjects.Models.MealPlan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Color")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FkUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsFullDay")
                        .HasColumnType("bit");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("FkUserId");

                    b.ToTable("MealPlans");
                });

            modelBuilder.Entity("BusinessObjects.Models.Nutrition", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<double?>("Calories")
                        .HasColumnType("float");

                    b.Property<double?>("Carbohydrate")
                        .HasColumnType("float");

                    b.Property<double?>("Cholesterol")
                        .HasColumnType("float");

                    b.Property<double?>("Fat")
                        .HasColumnType("float");

                    b.Property<double?>("Fibre")
                        .HasColumnType("float");

                    b.Property<int?>("IngredientId")
                        .HasColumnType("int");

                    b.Property<double?>("Protein")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("IngredientId");

                    b.ToTable("Nutrition");
                });

            modelBuilder.Entity("BusinessObjects.Models.Recipe", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Difficult")
                        .HasColumnType("int");

                    b.Property<int?>("FkRecipeCategoryId")
                        .HasColumnType("int");

                    b.Property<int?>("FkRecipeId")
                        .HasColumnType("int");

                    b.Property<string>("FkUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ImgPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PrepTime")
                        .HasColumnType("int");

                    b.Property<string>("ResponseMessage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ServingSize")
                        .HasColumnType("int");

                    b.Property<bool?>("Status")
                        .HasColumnType("bit");

                    b.Property<int?>("ViewCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("FkRecipeCategoryId");

                    b.HasIndex("FkRecipeId");

                    b.HasIndex("FkUserId");

                    b.ToTable("Recipes");
                });

            modelBuilder.Entity("BusinessObjects.Models.RecipeCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("RecipeCategories");
                });

            modelBuilder.Entity("BusinessObjects.Models.RecipeFeedback", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Rating")
                        .HasColumnType("int");

                    b.Property<int?>("RecipeId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RecipeId");

                    b.HasIndex("UserId");

                    b.ToTable("RecipeFeedbacks");
                });

            modelBuilder.Entity("BusinessObjects.Models.RecipeIngredient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("IngredientId")
                        .HasColumnType("int");

                    b.Property<double?>("Quantity")
                        .HasColumnType("float");

                    b.Property<int?>("RecipeId")
                        .HasColumnType("int");

                    b.Property<string>("UnitOfMeasure")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("IngredientId");

                    b.HasIndex("RecipeId");

                    b.ToTable("RecipeIngredient");
                });

            modelBuilder.Entity("FavouriteRecipe", b =>
                {
                    b.Property<int>("FavouritesId")
                        .HasColumnType("int");

                    b.Property<int>("RecipesId")
                        .HasColumnType("int");

                    b.HasKey("FavouritesId", "RecipesId");

                    b.HasIndex("RecipesId");

                    b.ToTable("FavouriteRecipe");
                });

            modelBuilder.Entity("IngredientRecipe", b =>
                {
                    b.Property<int>("IngredientsId")
                        .HasColumnType("int");

                    b.Property<int>("RecipesId")
                        .HasColumnType("int");

                    b.HasKey("IngredientsId", "RecipesId");

                    b.HasIndex("RecipesId");

                    b.ToTable("IngredientRecipe");
                });

            modelBuilder.Entity("MealPlanRecipe", b =>
                {
                    b.Property<int>("MealplansId")
                        .HasColumnType("int");

                    b.Property<int>("RecipesId")
                        .HasColumnType("int");

                    b.HasKey("MealplansId", "RecipesId");

                    b.HasIndex("RecipesId");

                    b.ToTable("MealPlanRecipe");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("BusinessObjects.Models.Favourite", b =>
                {
                    b.HasOne("BusinessObjects.Models.Account", "Account")
                        .WithMany("Favourites")
                        .HasForeignKey("AccountId");

                    b.Navigation("Account");
                });

            modelBuilder.Entity("BusinessObjects.Models.Ingredient", b =>
                {
                    b.HasOne("BusinessObjects.Models.IngredientCategory", "FkCategory")
                        .WithMany("Ingredients")
                        .HasForeignKey("FkCategoryId");

                    b.Navigation("FkCategory");
                });

            modelBuilder.Entity("BusinessObjects.Models.IngredientNutrition", b =>
                {
                    b.HasOne("BusinessObjects.Models.Ingredient", "Ingredient")
                        .WithOne("IngredientNutrition")
                        .HasForeignKey("BusinessObjects.Models.IngredientNutrition", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ingredient");
                });

            modelBuilder.Entity("BusinessObjects.Models.MealPlan", b =>
                {
                    b.HasOne("BusinessObjects.Models.Account", "FkUser")
                        .WithMany("MealPlans")
                        .HasForeignKey("FkUserId");

                    b.Navigation("FkUser");
                });

            modelBuilder.Entity("BusinessObjects.Models.Nutrition", b =>
                {
                    b.HasOne("BusinessObjects.Models.Recipe", "Recipe")
                        .WithOne("Nutrition")
                        .HasForeignKey("BusinessObjects.Models.Nutrition", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObjects.Models.Ingredient", "Ingredient")
                        .WithMany()
                        .HasForeignKey("IngredientId");

                    b.Navigation("Ingredient");

                    b.Navigation("Recipe");
                });

            modelBuilder.Entity("BusinessObjects.Models.Recipe", b =>
                {
                    b.HasOne("BusinessObjects.Models.RecipeCategory", "FkRecipeCategory")
                        .WithMany("Recipes")
                        .HasForeignKey("FkRecipeCategoryId");

                    b.HasOne("BusinessObjects.Models.Recipe", "FkRecipe")
                        .WithMany("InverseFkRecipe")
                        .HasForeignKey("FkRecipeId");

                    b.HasOne("BusinessObjects.Models.Account", "FkUser")
                        .WithMany("Recipes")
                        .HasForeignKey("FkUserId");

                    b.Navigation("FkRecipe");

                    b.Navigation("FkRecipeCategory");

                    b.Navigation("FkUser");
                });

            modelBuilder.Entity("BusinessObjects.Models.RecipeFeedback", b =>
                {
                    b.HasOne("BusinessObjects.Models.Recipe", "Recipe")
                        .WithMany("RecipeFeedbacks")
                        .HasForeignKey("RecipeId");

                    b.HasOne("BusinessObjects.Models.Account", "User")
                        .WithMany("RecipeFeedbacks")
                        .HasForeignKey("UserId");

                    b.Navigation("Recipe");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BusinessObjects.Models.RecipeIngredient", b =>
                {
                    b.HasOne("BusinessObjects.Models.Ingredient", "Ingredient")
                        .WithMany()
                        .HasForeignKey("IngredientId");

                    b.HasOne("BusinessObjects.Models.Recipe", "Recipe")
                        .WithMany("RecipeIngredients")
                        .HasForeignKey("RecipeId");

                    b.Navigation("Ingredient");

                    b.Navigation("Recipe");
                });

            modelBuilder.Entity("FavouriteRecipe", b =>
                {
                    b.HasOne("BusinessObjects.Models.Favourite", null)
                        .WithMany()
                        .HasForeignKey("FavouritesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObjects.Models.Recipe", null)
                        .WithMany()
                        .HasForeignKey("RecipesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("IngredientRecipe", b =>
                {
                    b.HasOne("BusinessObjects.Models.Ingredient", null)
                        .WithMany()
                        .HasForeignKey("IngredientsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObjects.Models.Recipe", null)
                        .WithMany()
                        .HasForeignKey("RecipesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MealPlanRecipe", b =>
                {
                    b.HasOne("BusinessObjects.Models.MealPlan", null)
                        .WithMany()
                        .HasForeignKey("MealplansId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObjects.Models.Recipe", null)
                        .WithMany()
                        .HasForeignKey("RecipesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("BusinessObjects.Models.Account", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("BusinessObjects.Models.Account", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObjects.Models.Account", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("BusinessObjects.Models.Account", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BusinessObjects.Models.Account", b =>
                {
                    b.Navigation("Favourites");

                    b.Navigation("MealPlans");

                    b.Navigation("RecipeFeedbacks");

                    b.Navigation("Recipes");
                });

            modelBuilder.Entity("BusinessObjects.Models.Ingredient", b =>
                {
                    b.Navigation("IngredientNutrition");
                });

            modelBuilder.Entity("BusinessObjects.Models.IngredientCategory", b =>
                {
                    b.Navigation("Ingredients");
                });

            modelBuilder.Entity("BusinessObjects.Models.Recipe", b =>
                {
                    b.Navigation("InverseFkRecipe");

                    b.Navigation("Nutrition");

                    b.Navigation("RecipeFeedbacks");

                    b.Navigation("RecipeIngredients");
                });

            modelBuilder.Entity("BusinessObjects.Models.RecipeCategory", b =>
                {
                    b.Navigation("Recipes");
                });
#pragma warning restore 612, 618
        }
    }
}
