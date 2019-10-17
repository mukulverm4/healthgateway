﻿// <auto-generated />
using System;
using HealthGateway.Medication.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Medication.Migrations
{
    [DbContext(typeof(MedicationDBContext))]
    [Migration("20191017182150_InitialDIN")]
    partial class InitialDIN
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("Relational:Sequence:.trace_seq", "'trace_seq', '', '1', '1', '1', '999999', 'Int64', 'True'");

            modelBuilder.Entity("HealthGateway.DIN.Models.DrugProduct", b =>
                {
                    b.Property<Guid>("DrugProductId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AccessionNumber")
                        .HasMaxLength(5);

                    b.Property<string>("AiGroupNumber")
                        .HasMaxLength(10);

                    b.Property<string>("BrandName")
                        .HasMaxLength(200);

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<string>("Descriptor")
                        .HasMaxLength(150);

                    b.Property<string>("DrugClass")
                        .HasMaxLength(40);

                    b.Property<string>("DrugIdentificationNumber")
                        .HasMaxLength(29);

                    b.Property<DateTime>("LastUpdate");

                    b.Property<string>("NumberOfAis")
                        .HasMaxLength(10);

                    b.Property<string>("PediatricFlag")
                        .HasMaxLength(1);

                    b.Property<string>("ProductCategorization")
                        .HasMaxLength(80);

                    b.Property<string>("UpdatedBy");

                    b.Property<string>("UpdatedDateTime");

                    b.HasKey("DrugProductId");

                    b.ToTable("Drugs");
                });
#pragma warning restore 612, 618
        }
    }
}
