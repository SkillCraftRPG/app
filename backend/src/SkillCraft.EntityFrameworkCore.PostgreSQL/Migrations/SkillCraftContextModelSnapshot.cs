﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SkillCraft.EntityFrameworkCore;

#nullable disable

namespace SkillCraft.EntityFrameworkCore.PostgreSQL.Migrations
{
    [DbContext(typeof(SkillCraftContext))]
    partial class SkillCraftContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SkillCraft.EntityFrameworkCore.Entities.AspectEntity", b =>
                {
                    b.Property<int>("AspectId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("AspectId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("DiscountedSkill1")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("DiscountedSkill2")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("MandatoryAttribute1")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("MandatoryAttribute2")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("OptionalAttribute1")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("OptionalAttribute2")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.Property<int>("WorldId")
                        .HasColumnType("integer");

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
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("CasteId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Skill")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("TraitsSerialized")
                        .HasColumnType("text")
                        .HasColumnName("Traits");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.Property<string>("WealthRoll")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int>("WorldId")
                        .HasColumnType("integer");

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

            modelBuilder.Entity("SkillCraft.EntityFrameworkCore.Entities.CustomizationEntity", b =>
                {
                    b.Property<int>("CustomizationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("CustomizationId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.Property<int>("WorldId")
                        .HasColumnType("integer");

                    b.HasKey("CustomizationId");

                    b.HasIndex("AggregateId")
                        .IsUnique();

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("Name");

                    b.HasIndex("Type");

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Version");

                    b.HasIndex("WorldId");

                    b.ToTable("Customizations", (string)null);
                });

            modelBuilder.Entity("SkillCraft.EntityFrameworkCore.Entities.EducationEntity", b =>
                {
                    b.Property<int>("EducationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("EducationId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Skill")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.Property<double?>("WealthMultiplier")
                        .HasColumnType("double precision");

                    b.Property<int>("WorldId")
                        .HasColumnType("integer");

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

            modelBuilder.Entity("SkillCraft.EntityFrameworkCore.Entities.LanguageEntity", b =>
                {
                    b.Property<int>("LanguageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("LanguageId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Script")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("TypicalSpeakers")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.Property<int>("WorldId")
                        .HasColumnType("integer");

                    b.HasKey("LanguageId");

                    b.HasIndex("AggregateId")
                        .IsUnique();

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("Name");

                    b.HasIndex("Script");

                    b.HasIndex("TypicalSpeakers");

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Version");

                    b.HasIndex("WorldId");

                    b.ToTable("Languages", (string)null);
                });

            modelBuilder.Entity("SkillCraft.EntityFrameworkCore.Entities.StorageDetailEntity", b =>
                {
                    b.Property<int>("StorageDetailId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("StorageDetailId"));

                    b.Property<Guid>("EntityId")
                        .HasColumnType("uuid");

                    b.Property<string>("EntityType")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uuid");

                    b.Property<long>("Size")
                        .HasColumnType("bigint");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<int>("WorldId")
                        .HasColumnType("integer");

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
                        .HasColumnType("integer");

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<long>("AllocatedBytes")
                        .HasColumnType("bigint");

                    b.Property<long>("AvailableBytes")
                        .HasColumnType("bigint");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uuid");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

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
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("UserId"));

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("EmailAddress")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("PictureUrl")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

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
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("WorldId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uuid");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("SlugNormalized")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

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

            modelBuilder.Entity("SkillCraft.EntityFrameworkCore.Entities.CustomizationEntity", b =>
                {
                    b.HasOne("SkillCraft.EntityFrameworkCore.Entities.WorldEntity", "World")
                        .WithMany("Customizations")
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

            modelBuilder.Entity("SkillCraft.EntityFrameworkCore.Entities.LanguageEntity", b =>
                {
                    b.HasOne("SkillCraft.EntityFrameworkCore.Entities.WorldEntity", "World")
                        .WithMany("Languages")
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

                    b.Navigation("Customizations");

                    b.Navigation("Educations");

                    b.Navigation("Languages");

                    b.Navigation("StorageDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
