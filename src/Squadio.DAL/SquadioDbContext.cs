﻿using Microsoft.EntityFrameworkCore;
using Squadio.Domain.Models.Companies;
using Squadio.Domain.Models.Invites;
using Squadio.Domain.Models.Logs;
using Squadio.Domain.Models.Projects;
using Squadio.Domain.Models.Teams;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL
{
    public class SquadioDbContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<UserRegistrationStepModel> UsersRegistrationStep { get; set; }
        public DbSet<UserRestorePasswordRequestModel> UserRestorePasswordRequests { get; set; }
        public DbSet<UserChangeEmailRequestModel> UserChangeEmailRequests { get; set; }
        public DbSet<UserConfirmEmailRequestModel> UserConfirmEmailRequests { get; set; }
        public DbSet<CompanyModel> Companies { get; set; }
        public DbSet<CompanyUserModel> CompaniesUsers { get; set; }
        public DbSet<TeamModel> Teams { get; set; }
        public DbSet<TeamUserModel> TeamsUsers { get; set; }
        public DbSet<ProjectModel> Projects { get; set; }
        public DbSet<ProjectUserModel> ProjectsUsers { get; set; }
        public DbSet<InviteModel> Invites { get; set; }
        public DbSet<LogModel> Logs { get; set; }
        

        public SquadioDbContext(DbContextOptions<SquadioDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>(item =>
            {
                item.HasKey(c => c.Id);
                item.HasIndex(p => p.Email)
                    .IsUnique();
            });
            modelBuilder.Entity<UserRegistrationStepModel>(item =>
            {
                item.HasKey(c => c.Id);
                item.HasIndex(p => p.UserId)
                    .IsUnique();
                item.HasOne(p => p.User)
                    .WithOne();
            });
            modelBuilder.Entity<UserRestorePasswordRequestModel>(item =>
            {
                item.HasKey(c => c.Id);
                item.HasOne(p => p.User)
                    .WithMany();
            });
            modelBuilder.Entity<UserChangeEmailRequestModel>(item =>
            {
                item.HasKey(c => c.Id);
                item.HasOne(p => p.User)
                    .WithMany();
            });
            modelBuilder.Entity<UserConfirmEmailRequestModel>(item =>
            {
                item.HasKey(c => c.Id);
                item.HasOne(p => p.User)
                    .WithMany();
            });
            
            
            modelBuilder.Entity<CompanyModel>(item =>
            {
                item.HasKey(c => c.Id);
                item.Property(x => x.Name)
                    .IsRequired();
                item.Property(x => x.CreatedDate)
                    .IsRequired();
            });
            modelBuilder.Entity<CompanyUserModel>(item =>
            {
                item.HasKey(c => c.Id);
                item.HasOne(p => p.User)
                    .WithMany();
                item.HasOne(p => p.Company)
                    .WithMany();
                item.HasIndex(p => new { p.CompanyId, p.UserId })
                    .IsUnique();
            });
            
            
            modelBuilder.Entity<TeamModel>(item =>
            {
                item.HasKey(c => c.Id);
                item.Property(x => x.Name)
                    .IsRequired();
                item.Property(x => x.CreatedDate)
                    .IsRequired();
                item.HasOne(p => p.Company)
                    .WithMany()
                    .IsRequired();
            });
            modelBuilder.Entity<TeamUserModel>(item =>
            {
                item.HasKey(c => c.Id);
                item.HasOne(p => p.User)
                    .WithMany();
                item.HasOne(p => p.Team)
                    .WithMany();
                item.HasIndex(p => new { p.TeamId, p.UserId })
                    .IsUnique();
            });
            
            
            modelBuilder.Entity<ProjectModel>(item =>
            {
                item.HasKey(c => c.Id);
                item.Property(x => x.Name)
                    .IsRequired();
                item.Property(x => x.CreatedDate)
                    .IsRequired();
                item.HasOne(p => p.Team)
                    .WithMany()
                    .IsRequired();
            });
            modelBuilder.Entity<ProjectUserModel>(item =>
            {
                item.HasKey(c => c.Id);
                item.HasOne(p => p.User)
                    .WithMany();
                item.HasOne(p => p.Project)
                    .WithMany();
                item.HasIndex(p => new { p.ProjectId, p.UserId })
                    .IsUnique();
            });
            
            modelBuilder.Entity<InviteModel>(item =>
            {
                item.HasKey(c => c.Id);
                item.HasIndex(p => p.Code)
                    .IsUnique();
            });
            
            modelBuilder.Entity<LogModel>(item =>
            {
                item.HasKey(c => c.Id);
            });
        }
    }
}
