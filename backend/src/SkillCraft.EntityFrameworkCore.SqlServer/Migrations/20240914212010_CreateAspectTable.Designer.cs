﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillCraft.EntityFrameworkCore;

#nullable disable

namespace SkillCraft.EntityFrameworkCore.SqlServer.Migrations
{
    [DbContext(typeof(SkillCraftContext))]
    [Migration("20240914212010_CreateAspectTable")]
    partial class CreateAspectTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SkillCraft.EntityFrameworkCore.Entities.AspectEntity", b =>
                {
                    b.Property<int>("AspectId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AspectId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DiscountedSkill1")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("DiscountedSkill2")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("MandatoryAttribute1")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("MandatoryAttribute2")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("OptionalAttribute1")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("OptionalAttribute2")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.Property<int>("WorldId")
                        .HasColumnType("int");

                    b.HasKey("AspectId");

                    b.HasIndex("AggregateId")
                        .IsUnique();

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("Name");

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Version");

                    b.HasIndex("WorldId");

                    b.ToTable("Aspects", (string)null);
                });

            modelBuilder.Entity("SkillCraft.EntityFrameworkCore.Entities.CasteEntity", b =>
                {
                    b.Property<int>("CasteId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CasteId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Skill")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("TraitsSerialized")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Traits");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.Property<string>("WealthRoll")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("WorldId")
                        .HasColumnType("int");

                    b.HasKey("CasteId");

                    b.HasIndex("AggregateId")
                        .IsUnique();

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("Name");

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Version");

                    b.HasIndex("WorldId");

                    b.ToTable("Castes", (string)null);
                });

            modelBuilder.Entity("SkillCraft.EntityFrameworkCore.Entities.EducationEntity", b =>
                {
                    b.Property<int>("EducationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EducationId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Skill")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.Property<double?>("WealthMultiplier")
                        .HasColumnType("float");

                    b.Property<int>("WorldId")
                        .HasColumnType("int");

                    b.HasKey("EducationId");

                    b.HasIndex("AggregateId")
                        .IsUnique();

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("Name");

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Version");

                    b.HasIndex("WorldId");

                    b.ToTable("Educations", (string)null);
                });

            modelBuilder.Entity("SkillCraft.EntityFrameworkCore.Entities.StorageDetailEntity", b =>
                {
                    b.Property<int>("StorageDetailId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StorageDetailId"));

                    b.Property<Guid>("EntityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("EntityType")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("Size")
                        .HasColumnType("bigint");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("WorldId")
                        .HasColumnType("int");

                    b.HasKey("StorageDetailId");

                    b.HasIndex("OwnerId");

                    b.HasIndex("Size");

                    b.HasIndex("UserId");

                    b.HasIndex("WorldId");

                    b.HasIndex("EntityType", "EntityId")
                        .IsUnique();

                    b.ToTable("StorageDetails", (string)null);
                });

            modelBuilder.Entity("SkillCraft.EntityFrameworkCore.Entities.StorageSummaryEntity", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<long>("AllocatedBytes")
                        .HasColumnType("bigint");

                    b.Property<long>("AvailableBytes")
                        .HasColumnType("bigint");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<long>("UsedBytes")
                        .HasColumnType("bigint");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("UserId");

                    b.HasIndex("AggregateId")
                        .IsUnique();

                    b.HasIndex("AllocatedBytes");

                    b.HasIndex("AvailableBytes");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("OwnerId")
                        .IsUnique();

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("UsedBytes");

                    b.HasIndex("Version");

                    b.ToTable("StorageSummaries", (string)null);
                });

            modelBuilder.Entity("SkillCraft.EntityFrameworkCore.Entities.UserEntity", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("EmailAddress")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("PictureUrl")
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)");

                    b.HasKey("UserId");

                    b.HasIndex("DisplayName");

                    b.HasIndex("EmailAddress");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("IsDeleted");

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("SkillCraft.EntityFrameworkCore.Entities.WorldEntity", b =>
                {
                    b.Property<int>("WorldId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("WorldId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("SlugNormalized")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("WorldId");

                    b.HasIndex("AggregateId")
                        .IsUnique();

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("Name");

                    b.HasIndex("OwnerId");

                    b.HasIndex("Slug");

                    b.HasIndex("SlugNormalized")
                        .IsUnique();

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("UserId");

                    b.HasIndex("Version");

                    b.ToTable("Worlds", (string)null);
                });

            modelBuilder.Entity("SkillCraft.EntityFrameworkCore.Entities.AspectEntity", b =>
                {
                    b.HasOne("SkillCraft.EntityFrameworkCore.Entities.WorldEntity", "World")
                        .WithMany("Aspects")
                        .HasForeignKey("WorldId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("World");
                });

            modelBuilder.Entity("SkillCraft.EntityFrameworkCore.Entities.CasteEntity", b =>
                {
                    b.HasOne("SkillCraft.EntityFrameworkCore.Entities.WorldEntity", "World")
                        .WithMany("Castes")
                        .HasForeignKey("WorldId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("World");
                });

            modelBuilder.Entity("SkillCraft.EntityFrameworkCore.Entities.EducationEntity", b =>
                {
                    b.HasOne("SkillCraft.EntityFrameworkCore.Entities.WorldEntity", "World")
                        .WithMany("Educations")
                        .HasForeignKey("WorldId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("World");
                });

            modelBuilder.Entity("SkillCraft.EntityFrameworkCore.Entities.StorageDetailEntity", b =>
                {
                    b.HasOne("SkillCraft.EntityFrameworkCore.Entities.UserEntity", "Owner")
                        .WithMany("StorageDetails")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SkillCraft.EntityFrameworkCore.Entities.WorldEntity", "World")
                        .WithMany("StorageDetails")
                        .HasForeignKey("WorldId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Owner");

                    b.Navigation("World");
                });

            modelBuilder.Entity("SkillCraft.EntityFrameworkCore.Entities.StorageSummaryEntity", b =>
                {
                    b.HasOne("SkillCraft.EntityFrameworkCore.Entities.UserEntity", "Owner")
                        .WithOne("StorageSummary")
                        .HasForeignKey("SkillCraft.EntityFrameworkCore.Entities.StorageSummaryEntity", "UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("SkillCraft.EntityFrameworkCore.Entities.WorldEntity", b =>
                {
                    b.HasOne("SkillCraft.EntityFrameworkCore.Entities.UserEntity", "Owner")
                        .WithMany("Worlds")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("SkillCraft.EntityFrameworkCore.Entities.UserEntity", b =>
                {
                    b.Navigation("StorageDetails");

                    b.Navigation("StorageSummary");

                    b.Navigation("Worlds");
                });

            modelBuilder.Entity("SkillCraft.EntityFrameworkCore.Entities.WorldEntity", b =>
                {
                    b.Navigation("Aspects");

                    b.Navigation("Castes");

                    b.Navigation("Educations");

                    b.Navigation("StorageDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
