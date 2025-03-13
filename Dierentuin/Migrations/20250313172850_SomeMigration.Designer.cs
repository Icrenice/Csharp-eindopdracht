﻿// <auto-generated />
using System;
using Dierentuin.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Dierentuin.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250313172850_SomeMigration")]
    partial class SomeMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AnimalAnimal", b =>
                {
                    b.Property<int>("AnimalId")
                        .HasColumnType("int");

                    b.Property<int>("PreyId")
                        .HasColumnType("int");

                    b.HasKey("AnimalId", "PreyId");

                    b.HasIndex("PreyId");

                    b.ToTable("AnimalAnimal");
                });

            modelBuilder.Entity("Dierentuin.Models.Animal", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ActivityPattern")
                        .HasColumnType("int");

                    b.Property<int?>("CategoryId")
                        .HasColumnType("int");

                    b.Property<int>("DietaryClass")
                        .HasColumnType("int");

                    b.Property<int?>("EnclosureId")
                        .HasColumnType("int");

                    b.Property<bool>("IsAwake")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SecurityRequirement")
                        .HasColumnType("int");

                    b.Property<int>("Size")
                        .HasColumnType("int");

                    b.Property<double>("SpaceRequirement")
                        .HasColumnType("float");

                    b.Property<string>("Species")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ZooId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("EnclosureId");

                    b.HasIndex("ZooId");

                    b.ToTable("Animals");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ActivityPattern = 1,
                            CategoryId = 2,
                            DietaryClass = 3,
                            EnclosureId = 4,
                            IsAwake = false,
                            Name = "Quinton",
                            SecurityRequirement = 1,
                            Size = 2,
                            SpaceRequirement = 17.061462915264364,
                            Species = "Sleek Frozen Ball"
                        },
                        new
                        {
                            Id = 2,
                            ActivityPattern = 1,
                            CategoryId = 2,
                            DietaryClass = 4,
                            EnclosureId = 2,
                            IsAwake = false,
                            Name = "Luisa",
                            SecurityRequirement = 1,
                            Size = 4,
                            SpaceRequirement = 4.8233524841469144,
                            Species = "Intelligent Metal Tuna"
                        },
                        new
                        {
                            Id = 3,
                            ActivityPattern = 2,
                            CategoryId = 2,
                            DietaryClass = 4,
                            EnclosureId = 3,
                            IsAwake = true,
                            Name = "Irwin",
                            SecurityRequirement = 0,
                            Size = 1,
                            SpaceRequirement = 18.323993091912502,
                            Species = "Unbranded Granite Gloves"
                        },
                        new
                        {
                            Id = 4,
                            ActivityPattern = 0,
                            CategoryId = 5,
                            DietaryClass = 2,
                            IsAwake = true,
                            Name = "Maximilian",
                            SecurityRequirement = 1,
                            Size = 0,
                            SpaceRequirement = 6.0893575872574406,
                            Species = "Handmade Soft Ball"
                        },
                        new
                        {
                            Id = 5,
                            ActivityPattern = 1,
                            DietaryClass = 0,
                            EnclosureId = 3,
                            IsAwake = false,
                            Name = "Blake",
                            SecurityRequirement = 2,
                            Size = 4,
                            SpaceRequirement = 45.195269010161297,
                            Species = "Gorgeous Steel Salad"
                        },
                        new
                        {
                            Id = 6,
                            ActivityPattern = 0,
                            DietaryClass = 2,
                            EnclosureId = 1,
                            IsAwake = false,
                            Name = "Cooper",
                            SecurityRequirement = 1,
                            Size = 2,
                            SpaceRequirement = 24.515541356937074,
                            Species = "Ergonomic Frozen Chicken"
                        },
                        new
                        {
                            Id = 7,
                            ActivityPattern = 1,
                            CategoryId = 2,
                            DietaryClass = 1,
                            EnclosureId = 4,
                            IsAwake = true,
                            Name = "Adella",
                            SecurityRequirement = 1,
                            Size = 0,
                            SpaceRequirement = 5.8608185240561932,
                            Species = "Refined Cotton Bike"
                        },
                        new
                        {
                            Id = 8,
                            ActivityPattern = 0,
                            CategoryId = 5,
                            DietaryClass = 0,
                            IsAwake = false,
                            Name = "Bertram",
                            SecurityRequirement = 2,
                            Size = 3,
                            SpaceRequirement = 27.825042189699275,
                            Species = "Incredible Concrete Shoes"
                        },
                        new
                        {
                            Id = 9,
                            ActivityPattern = 1,
                            CategoryId = 2,
                            DietaryClass = 2,
                            EnclosureId = 1,
                            IsAwake = true,
                            Name = "Rozella",
                            SecurityRequirement = 2,
                            Size = 0,
                            SpaceRequirement = 6.3044910729036019,
                            Species = "Generic Cotton Ball"
                        },
                        new
                        {
                            Id = 10,
                            ActivityPattern = 2,
                            CategoryId = 3,
                            DietaryClass = 1,
                            EnclosureId = 1,
                            IsAwake = true,
                            Name = "Robin",
                            SecurityRequirement = 0,
                            Size = 1,
                            SpaceRequirement = 48.921927422109214,
                            Species = "Handcrafted Steel Pants"
                        },
                        new
                        {
                            Id = 11,
                            ActivityPattern = 0,
                            CategoryId = 4,
                            DietaryClass = 1,
                            EnclosureId = 4,
                            IsAwake = true,
                            Name = "Abraham",
                            SecurityRequirement = 2,
                            Size = 3,
                            SpaceRequirement = 47.586565004404491,
                            Species = "Practical Granite Shoes"
                        },
                        new
                        {
                            Id = 12,
                            ActivityPattern = 0,
                            CategoryId = 1,
                            DietaryClass = 4,
                            IsAwake = true,
                            Name = "Bernie",
                            SecurityRequirement = 2,
                            Size = 0,
                            SpaceRequirement = 49.882688286090435,
                            Species = "Unbranded Wooden Computer"
                        },
                        new
                        {
                            Id = 13,
                            ActivityPattern = 2,
                            CategoryId = 1,
                            DietaryClass = 2,
                            EnclosureId = 4,
                            IsAwake = true,
                            Name = "Roslyn",
                            SecurityRequirement = 1,
                            Size = 5,
                            SpaceRequirement = 48.481234028365563,
                            Species = "Handcrafted Wooden Keyboard"
                        },
                        new
                        {
                            Id = 14,
                            ActivityPattern = 2,
                            CategoryId = 5,
                            DietaryClass = 0,
                            EnclosureId = 1,
                            IsAwake = true,
                            Name = "Rowena",
                            SecurityRequirement = 0,
                            Size = 5,
                            SpaceRequirement = 45.497496432700451,
                            Species = "Licensed Rubber Sausages"
                        },
                        new
                        {
                            Id = 15,
                            ActivityPattern = 2,
                            CategoryId = 2,
                            DietaryClass = 4,
                            EnclosureId = 2,
                            IsAwake = false,
                            Name = "Casper",
                            SecurityRequirement = 0,
                            Size = 2,
                            SpaceRequirement = 39.367417353890112,
                            Species = "Generic Metal Car"
                        });
                });

            modelBuilder.Entity("Dierentuin.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Zoogdieren"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Vogels"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Reptielen"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Insecten"
                        },
                        new
                        {
                            Id = 5,
                            Name = "Amfibieën"
                        });
                });

            modelBuilder.Entity("Dierentuin.Models.Enclosure", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Climate")
                        .HasColumnType("int");

                    b.Property<int>("HabitatType")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SecurityLevel")
                        .HasColumnType("int");

                    b.Property<double>("Size")
                        .HasColumnType("float");

                    b.Property<int?>("ZooId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ZooId");

                    b.ToTable("Enclosures");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Climate = 0,
                            HabitatType = 0,
                            Name = "Verblijf Outdoors, Home & Kids",
                            SecurityLevel = 2,
                            Size = 294.56150201904995
                        },
                        new
                        {
                            Id = 2,
                            Climate = 1,
                            HabitatType = 8,
                            Name = "Verblijf Home",
                            SecurityLevel = 1,
                            Size = 292.42826669770767
                        },
                        new
                        {
                            Id = 3,
                            Climate = 0,
                            HabitatType = 2,
                            Name = "Verblijf Grocery",
                            SecurityLevel = 0,
                            Size = 354.56484657522913
                        },
                        new
                        {
                            Id = 4,
                            Climate = 2,
                            HabitatType = 2,
                            Name = "Verblijf Electronics & Automotive",
                            SecurityLevel = 0,
                            Size = 300.37898042568861
                        });
                });

            modelBuilder.Entity("Dierentuin.Models.Zoo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Zoos");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Bogus Dierentuin"
                        });
                });

            modelBuilder.Entity("AnimalAnimal", b =>
                {
                    b.HasOne("Dierentuin.Models.Animal", null)
                        .WithMany()
                        .HasForeignKey("AnimalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Dierentuin.Models.Animal", null)
                        .WithMany()
                        .HasForeignKey("PreyId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Dierentuin.Models.Animal", b =>
                {
                    b.HasOne("Dierentuin.Models.Category", "Category")
                        .WithMany("Animals")
                        .HasForeignKey("CategoryId");

                    b.HasOne("Dierentuin.Models.Enclosure", "Enclosure")
                        .WithMany("Animals")
                        .HasForeignKey("EnclosureId");

                    b.HasOne("Dierentuin.Models.Zoo", null)
                        .WithMany("AllAnimals")
                        .HasForeignKey("ZooId");

                    b.Navigation("Category");

                    b.Navigation("Enclosure");
                });

            modelBuilder.Entity("Dierentuin.Models.Enclosure", b =>
                {
                    b.HasOne("Dierentuin.Models.Zoo", null)
                        .WithMany("Enclosures")
                        .HasForeignKey("ZooId");
                });

            modelBuilder.Entity("Dierentuin.Models.Category", b =>
                {
                    b.Navigation("Animals");
                });

            modelBuilder.Entity("Dierentuin.Models.Enclosure", b =>
                {
                    b.Navigation("Animals");
                });

            modelBuilder.Entity("Dierentuin.Models.Zoo", b =>
                {
                    b.Navigation("AllAnimals");

                    b.Navigation("Enclosures");
                });
#pragma warning restore 612, 618
        }
    }
}
