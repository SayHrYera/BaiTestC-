using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace App_Data.Models
{
    public partial class exam_distribution_testContext : DbContext
    {
        public exam_distribution_testContext()
        {
        }

        public exam_distribution_testContext(DbContextOptions<exam_distribution_testContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<DepartmentFacility> DepartmentFacilities { get; set; } = null!;
        public virtual DbSet<Facility> Facilities { get; set; } = null!;
        public virtual DbSet<Major> Majors { get; set; } = null!;
        public virtual DbSet<MajorFacility> MajorFacilities { get; set; } = null!;
        public virtual DbSet<StaffMajorFacility> StaffMajorFacilities { get; set; } = null!;
        public virtual DbSet<staff> staff { get; set; } = null!;
        public virtual DbSet<ImportHistory> ImportHistories { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=MSI;Database=exam_distribution_test;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("department");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Code)
                    .HasMaxLength(255)
                    .HasColumnName("code");

                entity.Property(e => e.CreatedDate).HasColumnName("created_date");

                entity.Property(e => e.LastModifiedDate).HasColumnName("last_modified_date");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<DepartmentFacility>(entity =>
            {
                entity.ToTable("department_facility");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.CreatedDate).HasColumnName("created_date");

                entity.Property(e => e.IdDepartment).HasColumnName("id_department");

                entity.Property(e => e.IdFacility).HasColumnName("id_facility");

                entity.Property(e => e.IdStaff).HasColumnName("id_staff");

                entity.Property(e => e.LastModifiedDate).HasColumnName("last_modified_date");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.IdDepartmentNavigation)
                    .WithMany(p => p.DepartmentFacilities)
                    .HasForeignKey(d => d.IdDepartment)
                    .HasConstraintName("FK__departmen__id_de__3D5E1FD2");

                entity.HasOne(d => d.IdFacilityNavigation)
                    .WithMany(p => p.DepartmentFacilities)
                    .HasForeignKey(d => d.IdFacility)
                    .HasConstraintName("FK__departmen__id_fa__3E52440B");

                entity.HasOne(d => d.IdStaffNavigation)
                    .WithMany(p => p.DepartmentFacilities)
                    .HasForeignKey(d => d.IdStaff)
                    .HasConstraintName("FK__departmen__id_st__3F466844");
            });

            modelBuilder.Entity<Facility>(entity =>
            {
                entity.ToTable("facility");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Code)
                    .HasMaxLength(255)
                    .HasColumnName("code");

                entity.Property(e => e.CreatedDate).HasColumnName("created_date");

                entity.Property(e => e.LastModifiedDate).HasColumnName("last_modified_date");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<Major>(entity =>
            {
                entity.ToTable("major");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Code)
                    .HasMaxLength(255)
                    .HasColumnName("code");

                entity.Property(e => e.CreatedDate).HasColumnName("created_date");

                entity.Property(e => e.LastModifiedDate).HasColumnName("last_modified_date");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<MajorFacility>(entity =>
            {
                entity.ToTable("major_facility");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.CreatedDate).HasColumnName("created_date");

                entity.Property(e => e.IdDepartmentFacility).HasColumnName("id_department_facility");

                entity.Property(e => e.IdMajor).HasColumnName("id_major");

                entity.Property(e => e.LastModifiedDate).HasColumnName("last_modified_date");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.IdDepartmentFacilityNavigation)
                    .WithMany(p => p.MajorFacilities)
                    .HasForeignKey(d => d.IdDepartmentFacility)
                    .HasConstraintName("FK__major_fac__id_de__440B1D61");

                entity.HasOne(d => d.IdMajorNavigation)
                    .WithMany(p => p.MajorFacilities)
                    .HasForeignKey(d => d.IdMajor)
                    .HasConstraintName("FK__major_fac__id_ma__44FF419A");
            });

            modelBuilder.Entity<StaffMajorFacility>(entity =>
            {
                entity.ToTable("staff_major_facility");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.CreatedDate).HasColumnName("created_date");

                entity.Property(e => e.IdMajorFacility).HasColumnName("id_major_facility");

                entity.Property(e => e.IdStaff).HasColumnName("id_staff");

                entity.Property(e => e.LastModifiedDate).HasColumnName("last_modified_date");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.IdMajorFacilityNavigation)
                    .WithMany(p => p.StaffMajorFacilities)
                    .HasForeignKey(d => d.IdMajorFacility)
                    .HasConstraintName("FK__staff_maj__id_ma__47DBAE45");

                entity.HasOne(d => d.IdStaffNavigation)
                    .WithMany(p => p.StaffMajorFacilities)
                    .HasForeignKey(d => d.IdStaff)
                    .HasConstraintName("FK__staff_maj__id_st__48CFD27E");
            });

            modelBuilder.Entity<staff>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.AccountFe)
                    .HasMaxLength(255)
                    .HasColumnName("account_fe");

                entity.Property(e => e.AccountFpt)
                    .HasMaxLength(255)
                    .HasColumnName("account_fpt");

                entity.Property(e => e.CreatedDate).HasColumnName("created_date");

                entity.Property(e => e.LastModifiedDate).HasColumnName("last_modified_date");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.StaffCode)
                    .HasMaxLength(255)
                    .HasColumnName("staff_code");

                entity.Property(e => e.Status).HasColumnName("status");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
