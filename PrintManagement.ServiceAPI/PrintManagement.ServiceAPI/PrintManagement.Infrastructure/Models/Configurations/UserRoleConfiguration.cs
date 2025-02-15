﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrintManagement.Infrastructure;
using System;
using System.Collections.Generic;

namespace PrintManagement.Infrastructure.Configurations
{
    public partial class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> entity)
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("UserRole");

            entity.Property(e => e.Oid).HasDefaultValueSql("(newsequentialid())");

            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.IdRoleNavigation)
                .WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.IdRole)
                .HasConstraintName("FK_UserRole_Role");

            entity.HasOne(d => d.IdUserNavigation)
                .WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK_UserRole_User");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<UserRole> entity);
    }
}
