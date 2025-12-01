using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PizzeriaCC.Models;

public partial class PizzeriaBdContext : DbContext
{
    public PizzeriaBdContext()
    {
    }

    public PizzeriaBdContext(DbContextOptions<PizzeriaBdContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Ingrediente> Ingredientes { get; set; }

    public virtual DbSet<Motorizado> Motorizados { get; set; }

    public virtual DbSet<Pedido> Pedidos { get; set; }

    public virtual DbSet<PedidoDetalle> PedidoDetalles { get; set; }

    public virtual DbSet<Pizza> Pizzas { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=FABRIZIO_T\\SQLEXPRESS;database=PizzeriaBD;Trusted_Connection=True; TrustServerCertificate=True; Encrypt=false;");
    */
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ingrediente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Ingredie__3214EC07604DDC2E");

            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.PrecioExtra).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Motorizado>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Motoriza__3214EC076913BBD0");

            entity.Property(e => e.Disponible).HasDefaultValue(true);
            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Pedidos__3214EC0711EA80AF");

            entity.Property(e => e.Estado).HasMaxLength(50);
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MetodoPago).HasMaxLength(50);
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Motorizado).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.MotorizadoId)
                .HasConstraintName("FK__Pedidos__Motoriz__59FA5E80");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Pedidos__Usuario__59063A47");
        });

        modelBuilder.Entity<PedidoDetalle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PedidoDe__3214EC071712724B");

            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Pedido).WithMany(p => p.PedidoDetalles)
                .HasForeignKey(d => d.PedidoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PedidoDet__Pedid__5CD6CB2B");

            entity.HasOne(d => d.Pizza).WithMany(p => p.PedidoDetalles)
                .HasForeignKey(d => d.PizzaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PedidoDet__Pizza__5DCAEF64");
        });

        modelBuilder.Entity<Pizza>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Pizzas__3214EC07384CE080");

            entity.Property(e => e.Descripcion).HasMaxLength(300);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.PrecioBase).HasColumnType("decimal(10, 2)");

            entity.HasMany(d => d.IdIngredientes).WithMany(p => p.IdPizzas)
                .UsingEntity<Dictionary<string, object>>(
                    "PizzaIngrediente",
                    r => r.HasOne<Ingrediente>().WithMany()
                        .HasForeignKey("IdIngrediente")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PizzaIngr__IdIng__5165187F"),
                    l => l.HasOne<Pizza>().WithMany()
                        .HasForeignKey("IdPizza")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PizzaIngr__IdPiz__5070F446"),
                    j =>
                    {
                        j.HasKey("IdPizza", "IdIngrediente").HasName("PK__PizzaIng__41DD61AE208BDBD0");
                        j.ToTable("PizzaIngredientes");
                    });
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3214EC0739E3A4BE");

            entity.HasIndex(e => e.Email, "UQ__Usuarios__A9D1053445CB3307").IsUnique();

            entity.Property(e => e.Apellido).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(250);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
