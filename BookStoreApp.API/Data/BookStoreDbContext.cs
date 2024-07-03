using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.API.Data;

public partial class BookStoreDbContext : IdentityDbContext<ApiUser>
{
    public BookStoreDbContext()
    {
    }

    public BookStoreDbContext(DbContextOptions<BookStoreDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Permette di eseguire il metodo OnModelCreating della classe base
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Authors__3214EC078E928C16");

            entity.Property(e => e.Bio).HasMaxLength(250);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Books__3214EC07CED0248B");

            entity.HasIndex(e => e.Isbn, "UQ__Books__447D36EABDF19C20").IsUnique();

            entity.Property(e => e.Immage).HasMaxLength(50);
            entity.Property(e => e.Isbn)
                .HasMaxLength(50)
                .HasColumnName("ISBN");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Summary).HasMaxLength(250);
            entity.Property(e => e.Title).HasMaxLength(50);

            entity.HasOne(d => d.Author).WithMany(p => p.Books)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK_Books_ToTable");
        });

        //------------------------SEEDING RUOLI E UTENTI-----------------------------

        // creazione RUOLI di default per seedind del database
        modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER",
                    // generato tramite GUID https://guidgenerator.com/
                    Id = "a56d0879-dbf0-4e18-ae87-2a64d2269314"
                },
                new IdentityRole
                {
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR",
                    Id = "55dc6311-e149-41a5-9f45-bb3ca5e99eee"
                }
            );

        // oggetto per l'hashing della password
        var hasher = new PasswordHasher<ApiUser>();

        // creazione UTENTI per seeding del database dalla classe personalizzata ApiUser
        modelBuilder.Entity<ApiUser>().HasData(
                new ApiUser
                {
                    Id = "25749bc2-d43e-4643-8060-fef24bd93df6",
                    Email = "admin@bookstore.com",
                    NormalizedEmail = "ADMIN@BOOKSTORE.COM",
                    UserName = "admin@bookstore.com",
                    NormalizedUserName = "ADMIN@BOOKSTORE.COM",
                    FirstName = "System",
                    LastName = "Admin",
                    // metodo per l'hashing della password
                    PasswordHash = hasher.HashPassword(null, "P@ssword1")
                },
                new ApiUser
                {
                    Id = "39dab0b5-1fb0-4907-974a-ab901de45cf6",
                    Email = "user@bookstore.com",
                    NormalizedEmail = "USER@BOOKSTORE.COM",
                    UserName = "user@bookstore.com",
                    NormalizedUserName = "USER@BOOKSTORE.COM",
                    FirstName = "System",
                    LastName = "User",
                    PasswordHash = hasher.HashPassword(null, "P@ssword1")
                }
            );

        // creazione relazione tra UTENTI e RUOLI
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(

                // USER
                new IdentityUserRole<string>
                {
                    // id per assegnare il RUOLO UTENTE (motivo per cui l'id deve essere univoco)
                    RoleId = "a56d0879-dbf0-4e18-ae87-2a64d2269314",
                    // id per assegnare l'UTENTE
                    UserId = "39dab0b5-1fb0-4907-974a-ab901de45cf6"
                },

                // ADMIN
                new IdentityUserRole<string>
                {
                    // id per assegnare il RUOLO ADMIN
                    RoleId = "55dc6311-e149-41a5-9f45-bb3ca5e99eee",
                    // id per assegnare l'ADMIN
                    UserId = "25749bc2-d43e-4643-8060-fef24bd93df6"
                }
            );
        //-----------------------------------------------------------------------

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
