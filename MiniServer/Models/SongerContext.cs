using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace MiniServer.Models
{
    public partial class SongerContext : DbContext
    {

        public SongerContext() : base()
        {
        }

        public SongerContext(DbContextOptions<SongerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Albums> Albums { get; set; }
        public virtual DbSet<Artists> Artists { get; set; }
        public virtual DbSet<Tracks> Tracks { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseNpgsql(Configuration.GetConnectionString("SongerContext"));
        //    }
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Albums>(entity =>
            {
                entity.ToTable("albums");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Artistid)
                    .IsRequired()
                    .HasColumnName("artistid");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("character varying");

                entity.Property(e => e.Releasedate)
                    .HasColumnName("releasedate")
                    .HasColumnType("date");

                entity.Property(e => e.Picture)
                    .HasColumnName("picture")
                    .HasColumnType("character varying");

                entity.HasOne(d => d.Artist)
                    .WithMany(p => p.Albums)
                    .HasForeignKey(d => d.Artistid)
                    .HasConstraintName("albums_artistid_fkey");
            });

            modelBuilder.Entity<Artists>(entity =>
            {
                entity.ToTable("artists");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("character varying");

                entity.Property(e => e.Picture)
                    .HasColumnName("picture")
                    .HasColumnType("character varying");
            });

            modelBuilder.Entity<Tracks>(entity =>
            {
                entity.ToTable("tracks");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Albumid)
                .IsRequired()
                .HasColumnName("albumid");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("character varying");

                entity.Property(e => e.Tracknumber)
                    .IsRequired()
                    .HasColumnName("tracknumber");

                entity.Property(e => e.Duration)
                    .IsRequired()
                    .HasColumnName("duration");

                entity.Property(e => e.Filepath)
                    .IsRequired()
                    .HasColumnName("filepath")
                    .HasColumnType("character varying");

                entity.HasOne(d => d.Album)
                    .WithMany(p => p.Tracks)
                    .HasForeignKey(d => d.Albumid)
                    .HasConstraintName("tracks_albumid_fkey");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
