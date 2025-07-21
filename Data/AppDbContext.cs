using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NotationTB.Models;

namespace NotationTB.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BasesRulesOperation> BasesRulesOperations { get; set; }

    public virtual DbSet<ClassificationDesignation> ClassificationDesignations { get; set; }

    public virtual DbSet<ExceptionRulesOperation> ExceptionRulesOperations { get; set; }

    public virtual DbSet<MaterialsAndProductsCombination> MaterialsAndProductsCombinations { get; set; }

    public virtual DbSet<MaterialsStamp> MaterialsStamps { get; set; }

    public virtual DbSet<MaterialsStandard> MaterialsStandards { get; set; }

    public virtual DbSet<MaterialsType> MaterialsTypes { get; set; }

    public virtual DbSet<OperationsType> OperationsTypes { get; set; }

    public virtual DbSet<OptionalRule> OptionalRules { get; set; }

    public virtual DbSet<ProductsStandard> ProductsStandards { get; set; }

    public virtual DbSet<ProductsType> ProductsTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=NotationTB;Username=postgres;Password=Uhbif6556576");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BasesRulesOperation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("BasesRulesOperations_pkey");

            entity.ToTable(tb => tb.HasComment("основная логика"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DesignationId).HasColumnName("designationId");
            entity.Property(e => e.MaterialTypeId).HasColumnName("materialTypeID");
            entity.Property(e => e.OperationTypeId).HasColumnName("operationTypeID");
            entity.Property(e => e.ProductTypeId).HasColumnName("productTypeID");
            entity.Property(e => e.Value).HasColumnName("value");

            entity.HasOne(d => d.Designation).WithMany(p => p.BasesRulesOperations)
                .HasForeignKey(d => d.DesignationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BasesRulesOperations_designationId_fkey");

            entity.HasOne(d => d.MaterialType).WithMany(p => p.BasesRulesOperations)
                .HasForeignKey(d => d.MaterialTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BasesRulesOperations_materialTypeID_fkey");

            entity.HasOne(d => d.OperationType).WithMany(p => p.BasesRulesOperations)
                .HasForeignKey(d => d.OperationTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BasesRulesOperations_operationTypeID_fkey");

            entity.HasOne(d => d.ProductType).WithMany(p => p.BasesRulesOperations)
                .HasForeignKey(d => d.ProductTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BasesRulesOperations_productTypeID_fkey");
        });

        modelBuilder.Entity<ClassificationDesignation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ClassificationDesignations_pkey");

            entity.ToTable(tb => tb.HasComment("Класификационные обоначения"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<ExceptionRulesOperation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ExceptionRulesOperations_pkey");

            entity.ToTable(tb => tb.HasComment("Правила для формирования ТУ"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CombinationId).HasColumnName("combinationID");
            entity.Property(e => e.DesignationId).HasColumnName("designationId");
            entity.Property(e => e.OperationTypeId).HasColumnName("operationTypeID");
            entity.Property(e => e.Value).HasColumnName("value");

            entity.HasOne(d => d.Combination).WithMany(p => p.ExceptionRulesOperations)
                .HasForeignKey(d => d.CombinationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ExceptionRulesOperations_combinationID_fkey");

            entity.HasOne(d => d.Designation).WithMany(p => p.ExceptionRulesOperations)
                .HasForeignKey(d => d.DesignationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ExceptionRulesOperations_designationId_fkey");

            entity.HasOne(d => d.OperationType).WithMany(p => p.ExceptionRulesOperations)
                .HasForeignKey(d => d.OperationTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ExceptionRulesOperations_operationTypeID_fkey");
        });

        modelBuilder.Entity<MaterialsAndProductsCombination>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("MaterialsAndProductsCombinations_pkey");

            entity.ToTable(tb => tb.HasComment("комбинации типа материала и стандартов материала и полуфобриката"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MatStandardId).HasColumnName("matStandardId");
            entity.Property(e => e.MaterialId).HasColumnName("materialId");
            entity.Property(e => e.ProStandardId).HasColumnName("proStandardId");

            entity.HasOne(d => d.MatStandard).WithMany(p => p.MaterialsAndProductsCombinations)
                .HasForeignKey(d => d.MatStandardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("MaterialsAndProductsCombinations_matStandardId_fkey");

            entity.HasOne(d => d.Material).WithMany(p => p.MaterialsAndProductsCombinations)
                .HasForeignKey(d => d.MaterialId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("MaterialsAndProductsCombinations_materialId_fkey");

            entity.HasOne(d => d.ProStandard).WithMany(p => p.MaterialsAndProductsCombinations)
                .HasForeignKey(d => d.ProStandardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("MaterialsAndProductsCombinations_proStandardId_fkey");
        });

        modelBuilder.Entity<MaterialsStamp>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("MaterialsStamps_pkey");

            entity.ToTable(tb => tb.HasComment("Марки материала"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.TypeId).HasColumnName("typeID");

            entity.HasOne(d => d.Type).WithMany(p => p.MaterialsStamps)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("MaterialsStamps_typeID_fkey");
        });

        modelBuilder.Entity<MaterialsStandard>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("MaterialsStandards_pkey");

            entity.ToTable(tb => tb.HasComment("Стандарты материалов"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<MaterialsType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("MaterialsTypes_pkey");

            entity.ToTable(tb => tb.HasComment("Типы материалов(структурный класс)"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsAustenit).HasColumnName("isAustenit");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<OperationsType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("OperationsTypes_pkey");

            entity.ToTable(tb => tb.HasComment("Типы операций"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<OptionalRule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("OptionalRules_pkey");

            entity.ToTable(tb => tb.HasComment("Дополнительные правила"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DesignationId).HasColumnName("designationId");
            entity.Property(e => e.ForAll).HasColumnName("forAll");
            entity.Property(e => e.MaterialTypeId).HasColumnName("materialTypeID");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.OperationTypeId).HasColumnName("operationTypeID");

            entity.HasOne(d => d.Designation).WithMany(p => p.OptionalRules)
                .HasForeignKey(d => d.DesignationId)
                .HasConstraintName("OptionalRules_designationId_fkey");

            entity.HasOne(d => d.MaterialType).WithMany(p => p.OptionalRules)
                .HasForeignKey(d => d.MaterialTypeId)
                .HasConstraintName("OptionalRules_materialTypeID_fkey");

            entity.HasOne(d => d.OperationType).WithMany(p => p.OptionalRules)
                .HasForeignKey(d => d.OperationTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("OptionalRules_operationTypeID_fkey");
        });

        modelBuilder.Entity<ProductsStandard>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ProductsStandards_pkey");

            entity.ToTable(tb => tb.HasComment("Стандарты изделий и полуфабрикатов"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.TypeId).HasColumnName("typeId");

            entity.HasOne(d => d.Type).WithMany(p => p.ProductsStandards)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ProductsStandards_typeId_fkey");
        });

        modelBuilder.Entity<ProductsType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ProductsTypes_pkey");

            entity.ToTable(tb => tb.HasComment("Вид полуфабриката или изделия"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Users_pkey");

            entity.ToTable(tb => tb.HasComment("Пользователи"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Login).HasColumnName("login");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.Surname).HasColumnName("surname");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
