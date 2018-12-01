﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SupplyBlockChain_Backend.Data;

namespace SupplyBlockChain_Backend.Migrations.ProductInfoDb
{
    [DbContext(typeof(ProductInfoDbContext))]
    [Migration("20181201081452_SecondMigration")]
    partial class SecondMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024");

            modelBuilder.Entity("SupplyBlockChain_Backend.Models.ProductInfo", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreationDate");

                    b.Property<int>("ProductCreator");

                    b.Property<string>("ProductID");

                    b.Property<string>("ProductName");

                    b.Property<string>("ProductType");

                    b.HasKey("ID");

                    b.ToTable("ProductsInfos");
                });
#pragma warning restore 612, 618
        }
    }
}
