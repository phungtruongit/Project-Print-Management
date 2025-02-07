﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrintManagement.Infrastructure;
using System;
using System.Collections.Generic;

namespace PrintManagement.Infrastructure.Configurations
{
    public partial class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("User");

            entity.Property(e => e.Oid).HasDefaultValueSql("(newsequentialid())");

            entity.Property(e => e.CreatedBy).HasMaxLength(255);

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.ModifiedBy).HasMaxLength(255);

            entity.Property(e => e.ModifiedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.Name).HasMaxLength(255);

            entity.Property(e => e.Note).HasMaxLength(255);

            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.Phone)
                .HasMaxLength(11)
                .IsUnicode(false);

            entity.Property(e => e.ResetBy).HasMaxLength(255);

            entity.Property(e => e.ResetDate).HasColumnType("datetime");

            entity.Property(e => e.UserName)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdDepartmentNavigation)
                .WithMany(p => p.Users)
                .HasForeignKey(d => d.IdDepartment)
                .HasConstraintName("FK_User_Department");

            entity.HasOne(d => d.IdUserGroupNavigation)
                .WithMany(p => p.Users)
                .HasForeignKey(d => d.IdUserGroup)
                .HasConstraintName("FK_User_UserGroup");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<User> entity);
    }
}
