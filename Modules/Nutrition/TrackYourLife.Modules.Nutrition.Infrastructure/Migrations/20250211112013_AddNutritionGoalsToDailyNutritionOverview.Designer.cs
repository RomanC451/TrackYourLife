﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NpgsqlTypes;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data;

#nullable disable

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Migrations
{
    [DbContext(typeof(NutritionWriteDbContext))]
    [Migration("20250211112013_AddNutritionGoalsToDailyNutritionOverview")]
    partial class AddNutritionGoalsToDailyNutritionOverview
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Nutrition")
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews.DailyNutritionOverview", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<float>("CaloriesGoal")
                        .HasColumnType("real");

                    b.Property<float>("CarbohydratesGoal")
                        .HasColumnType("real");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<float>("FatGoal")
                        .HasColumnType("real");

                    b.Property<float>("ProteinGoal")
                        .HasColumnType("real");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.ComplexProperty<Dictionary<string, object>>("NutritionalContent", "TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews.DailyNutritionOverview.NutritionalContent#NutritionalContent", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<float>("Calcium")
                                .HasColumnType("real");

                            b1.Property<float>("Carbohydrates")
                                .HasColumnType("real");

                            b1.Property<float>("Cholesterol")
                                .HasColumnType("real");

                            b1.Property<float>("Fat")
                                .HasColumnType("real");

                            b1.Property<float>("Fiber")
                                .HasColumnType("real");

                            b1.Property<float>("Iron")
                                .HasColumnType("real");

                            b1.Property<float>("MonounsaturatedFat")
                                .HasColumnType("real");

                            b1.Property<float>("NetCarbs")
                                .HasColumnType("real");

                            b1.Property<float>("PolyunsaturatedFat")
                                .HasColumnType("real");

                            b1.Property<float>("Potassium")
                                .HasColumnType("real");

                            b1.Property<float>("Protein")
                                .HasColumnType("real");

                            b1.Property<float>("SaturatedFat")
                                .HasColumnType("real");

                            b1.Property<float>("Sodium")
                                .HasColumnType("real");

                            b1.Property<float>("Sugar")
                                .HasColumnType("real");

                            b1.Property<float>("TransFat")
                                .HasColumnType("real");

                            b1.Property<float>("VitaminA")
                                .HasColumnType("real");

                            b1.Property<float>("VitaminC")
                                .HasColumnType("real");

                            b1.ComplexProperty<Dictionary<string, object>>("Energy", "TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews.DailyNutritionOverview.NutritionalContent#NutritionalContent.Energy#Energy", b2 =>
                                {
                                    b2.IsRequired();

                                    b2.Property<string>("Unit")
                                        .IsRequired()
                                        .HasColumnType("text");

                                    b2.Property<float>("Value")
                                        .HasColumnType("real");
                                });
                        });

                    b.HasKey("Id");

                    b.HasIndex("UserId", "Date")
                        .IsUnique();

                    b.ToTable("DailyNutritionOverviews", "Nutrition");
                });

            modelBuilder.Entity("TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries.FoodDiary", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOnUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<Guid>("FoodId")
                        .HasColumnType("uuid");

                    b.Property<string>("MealType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("ModifiedOnUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<float>("Quantity")
                        .HasColumnType("real");

                    b.Property<Guid>("ServingSizeId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("FoodId");

                    b.HasIndex("ServingSizeId");

                    b.ToTable("FoodDiary", "Nutrition");
                });

            modelBuilder.Entity("TrackYourLife.Modules.Nutrition.Domain.Features.FoodServingSizes.FoodServingSize", b =>
                {
                    b.Property<Guid>("FoodId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ServingSizeId")
                        .HasColumnType("uuid");

                    b.Property<int>("Index")
                        .HasColumnType("integer");

                    b.HasKey("FoodId", "ServingSizeId");

                    b.HasIndex("ServingSizeId");

                    b.ToTable("FoodServingSize", "Nutrition");
                });

            modelBuilder.Entity("TrackYourLife.Modules.Nutrition.Domain.Features.Foods.Food", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<long?>("ApiId")
                        .HasColumnType("bigint");

                    b.Property<string>("BrandName")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasDefaultValue("");

                    b.Property<string>("CountryCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<NpgsqlTsVector>("SearchVector")
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("tsvector")
                        .HasAnnotation("Npgsql:TsVectorConfig", "english")
                        .HasAnnotation("Npgsql:TsVectorProperties", new[] { "Name", "BrandName" });

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.ComplexProperty<Dictionary<string, object>>("NutritionalContents", "TrackYourLife.Modules.Nutrition.Domain.Features.Foods.Food.NutritionalContents#NutritionalContent", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<float>("Calcium")
                                .HasColumnType("real");

                            b1.Property<float>("Carbohydrates")
                                .HasColumnType("real");

                            b1.Property<float>("Cholesterol")
                                .HasColumnType("real");

                            b1.Property<float>("Fat")
                                .HasColumnType("real");

                            b1.Property<float>("Fiber")
                                .HasColumnType("real");

                            b1.Property<float>("Iron")
                                .HasColumnType("real");

                            b1.Property<float>("MonounsaturatedFat")
                                .HasColumnType("real");

                            b1.Property<float>("NetCarbs")
                                .HasColumnType("real");

                            b1.Property<float>("PolyunsaturatedFat")
                                .HasColumnType("real");

                            b1.Property<float>("Potassium")
                                .HasColumnType("real");

                            b1.Property<float>("Protein")
                                .HasColumnType("real");

                            b1.Property<float>("SaturatedFat")
                                .HasColumnType("real");

                            b1.Property<float>("Sodium")
                                .HasColumnType("real");

                            b1.Property<float>("Sugar")
                                .HasColumnType("real");

                            b1.Property<float>("TransFat")
                                .HasColumnType("real");

                            b1.Property<float>("VitaminA")
                                .HasColumnType("real");

                            b1.Property<float>("VitaminC")
                                .HasColumnType("real");

                            b1.ComplexProperty<Dictionary<string, object>>("Energy", "TrackYourLife.Modules.Nutrition.Domain.Features.Foods.Food.NutritionalContents#NutritionalContent.Energy#Energy", b2 =>
                                {
                                    b2.IsRequired();

                                    b2.Property<string>("Unit")
                                        .IsRequired()
                                        .HasColumnType("text");

                                    b2.Property<float>("Value")
                                        .HasColumnType("real");
                                });
                        });

                    b.HasKey("Id");

                    b.HasIndex("ApiId")
                        .IsUnique();

                    b.HasIndex("SearchVector");

                    NpgsqlIndexBuilderExtensions.HasMethod(b.HasIndex("SearchVector"), "GIN");

                    b.ToTable("Food", "Nutrition");
                });

            modelBuilder.Entity("TrackYourLife.Modules.Nutrition.Domain.Features.FoodsHistory.FoodHistory", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("FoodId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("LastUsedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("FoodId");

                    b.HasIndex("LastUsedAt");

                    b.HasIndex("UserId", "FoodId")
                        .IsUnique();

                    b.ToTable("FoodHistory", "Nutrition");
                });

            modelBuilder.Entity("TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients.Ingredient", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOnUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("FoodId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("ModifiedOnUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<float>("Quantity")
                        .HasColumnType("real");

                    b.Property<Guid>("RecipeId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ServingSizeId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("FoodId");

                    b.HasIndex("RecipeId");

                    b.HasIndex("ServingSizeId");

                    b.ToTable("Ingredient", "Nutrition");
                });

            modelBuilder.Entity("TrackYourLife.Modules.Nutrition.Domain.Features.OutboxMessages.OutboxMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Error")
                        .HasColumnType("text");

                    b.Property<DateTime>("OccurredOnUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("ProcessedOnUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("OutboxMessages", "Nutrition");
                });

            modelBuilder.Entity("TrackYourLife.Modules.Nutrition.Domain.Features.OutboxMessages.OutboxMessageConsumer", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id", "Name");

                    b.ToTable("OutboxMessageConsumers", "Nutrition");
                });

            modelBuilder.Entity("TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries.RecipeDiary", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOnUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<string>("MealType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("ModifiedOnUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<float>("Quantity")
                        .HasColumnType("real");

                    b.Property<Guid>("RecipeId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("RecipeId");

                    b.ToTable("RecipeDiary", "Nutrition");
                });

            modelBuilder.Entity("TrackYourLife.Modules.Nutrition.Domain.Features.Recipes.Recipe", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOnUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsOld")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("ModifiedOnUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<uint>("Xmin")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.ComplexProperty<Dictionary<string, object>>("NutritionalContents", "TrackYourLife.Modules.Nutrition.Domain.Features.Recipes.Recipe.NutritionalContents#NutritionalContent", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<float>("Calcium")
                                .HasColumnType("real");

                            b1.Property<float>("Carbohydrates")
                                .HasColumnType("real");

                            b1.Property<float>("Cholesterol")
                                .HasColumnType("real");

                            b1.Property<float>("Fat")
                                .HasColumnType("real");

                            b1.Property<float>("Fiber")
                                .HasColumnType("real");

                            b1.Property<float>("Iron")
                                .HasColumnType("real");

                            b1.Property<float>("MonounsaturatedFat")
                                .HasColumnType("real");

                            b1.Property<float>("NetCarbs")
                                .HasColumnType("real");

                            b1.Property<float>("PolyunsaturatedFat")
                                .HasColumnType("real");

                            b1.Property<float>("Potassium")
                                .HasColumnType("real");

                            b1.Property<float>("Protein")
                                .HasColumnType("real");

                            b1.Property<float>("SaturatedFat")
                                .HasColumnType("real");

                            b1.Property<float>("Sodium")
                                .HasColumnType("real");

                            b1.Property<float>("Sugar")
                                .HasColumnType("real");

                            b1.Property<float>("TransFat")
                                .HasColumnType("real");

                            b1.Property<float>("VitaminA")
                                .HasColumnType("real");

                            b1.Property<float>("VitaminC")
                                .HasColumnType("real");

                            b1.ComplexProperty<Dictionary<string, object>>("Energy", "TrackYourLife.Modules.Nutrition.Domain.Features.Recipes.Recipe.NutritionalContents#NutritionalContent.Energy#Energy", b2 =>
                                {
                                    b2.IsRequired();

                                    b2.Property<string>("Unit")
                                        .IsRequired()
                                        .HasColumnType("text");

                                    b2.Property<float>("Value")
                                        .HasColumnType("real");
                                });
                        });

                    b.HasKey("Id");

                    b.ToTable("Recipe", "Nutrition");
                });

            modelBuilder.Entity("TrackYourLife.Modules.Nutrition.Domain.Features.SearchedFoods.SearchedFood", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("SearchedFood", "Nutrition");
                });

            modelBuilder.Entity("TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes.ServingSize", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<long?>("ApiId")
                        .HasColumnType("bigint");

                    b.Property<float>("NutritionMultiplier")
                        .HasColumnType("real");

                    b.Property<string>("Unit")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<float>("Value")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.HasIndex("ApiId")
                        .IsUnique();

                    b.ToTable("ServingSize", "Nutrition");
                });

            modelBuilder.Entity("TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries.FoodDiary", b =>
                {
                    b.HasOne("TrackYourLife.Modules.Nutrition.Domain.Features.Foods.Food", null)
                        .WithMany()
                        .HasForeignKey("FoodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes.ServingSize", null)
                        .WithMany()
                        .HasForeignKey("ServingSizeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TrackYourLife.Modules.Nutrition.Domain.Features.FoodServingSizes.FoodServingSize", b =>
                {
                    b.HasOne("TrackYourLife.Modules.Nutrition.Domain.Features.Foods.Food", null)
                        .WithMany("FoodServingSizes")
                        .HasForeignKey("FoodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes.ServingSize", null)
                        .WithMany()
                        .HasForeignKey("ServingSizeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TrackYourLife.Modules.Nutrition.Domain.Features.FoodsHistory.FoodHistory", b =>
                {
                    b.HasOne("TrackYourLife.Modules.Nutrition.Domain.Features.Foods.Food", null)
                        .WithMany()
                        .HasForeignKey("FoodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients.Ingredient", b =>
                {
                    b.HasOne("TrackYourLife.Modules.Nutrition.Domain.Features.Foods.Food", null)
                        .WithMany()
                        .HasForeignKey("FoodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TrackYourLife.Modules.Nutrition.Domain.Features.Recipes.Recipe", null)
                        .WithMany("Ingredients")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes.ServingSize", null)
                        .WithMany()
                        .HasForeignKey("ServingSizeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries.RecipeDiary", b =>
                {
                    b.HasOne("TrackYourLife.Modules.Nutrition.Domain.Features.Recipes.Recipe", null)
                        .WithMany()
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TrackYourLife.Modules.Nutrition.Domain.Features.Foods.Food", b =>
                {
                    b.Navigation("FoodServingSizes");
                });

            modelBuilder.Entity("TrackYourLife.Modules.Nutrition.Domain.Features.Recipes.Recipe", b =>
                {
                    b.Navigation("Ingredients");
                });
#pragma warning restore 612, 618
        }
    }
}
