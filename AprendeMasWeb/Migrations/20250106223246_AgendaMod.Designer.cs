﻿// <auto-generated />
using System;
using AprendeMasWeb.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AprendeMasWeb.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20250106223246_AgendaMod")]
    partial class AgendaMod
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AprendeMasWeb.Models.Actividades", b =>
                {
                    b.Property<int>("ActividadId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ActividadId"));

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FechaLimite")
                        .HasColumnType("datetime2");

                    b.Property<int>("MateriaId")
                        .HasColumnType("int");

                    b.Property<string>("NombreActividad")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TipoActividadId")
                        .HasColumnType("int");

                    b.HasKey("ActividadId");

                    b.HasIndex("TipoActividadId");

                    b.ToTable("tbActividades");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.Alumnos", b =>
                {
                    b.Property<int>("AlumnoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AlumnoId"));

                    b.Property<string>("ApellidoMaterno")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ApellidoPaterno")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("AlumnoId");

                    b.HasIndex("UserId");

                    b.ToTable("tbAlumnos");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.AlumnosActividades", b =>
                {
                    b.Property<int>("AlumnoActividadId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("ActividadId")
                        .HasColumnType("int");

                    b.Property<int>("AlumnoId")
                        .HasColumnType("int");

                    b.Property<bool>("EstatusEntrega")
                        .HasColumnType("bit");

                    b.Property<DateTime>("FechaEntrega")
                        .HasColumnType("datetime2");

                    b.HasKey("AlumnoActividadId");

                    b.HasIndex("ActividadId");

                    b.ToTable("tbAlumnosActividades");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.AlumnosGrupos", b =>
                {
                    b.Property<int>("AlumnoGrupoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AlumnoGrupoId"));

                    b.Property<int>("AlumnoId")
                        .HasColumnType("int");

                    b.Property<int>("GrupoId")
                        .HasColumnType("int");

                    b.HasKey("AlumnoGrupoId");

                    b.HasIndex("AlumnoId");

                    b.HasIndex("GrupoId");

                    b.ToTable("tbAlumnosGrupos");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.AlumnosMaterias", b =>
                {
                    b.Property<int>("AlumnoMateriaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AlumnoMateriaId"));

                    b.Property<int>("AlumnoId")
                        .HasColumnType("int");

                    b.Property<int>("MateriaId")
                        .HasColumnType("int");

                    b.HasKey("AlumnoMateriaId");

                    b.HasIndex("AlumnoId");

                    b.HasIndex("MateriaId");

                    b.ToTable("tbAlumnosMaterias");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.AlumnosTokens", b =>
                {
                    b.Property<int>("FCMToken")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FCMToken"));

                    b.Property<int>("AlumnoId")
                        .HasColumnType("int");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("FCMToken");

                    b.HasIndex("AlumnoId");

                    b.ToTable("tbAlumnosTokens");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.Avisos", b =>
                {
                    b.Property<int>("AvisoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AvisoId"));

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DocenteId")
                        .HasColumnType("int");

                    b.Property<string>("Titulo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AvisoId");

                    b.HasIndex("DocenteId");

                    b.ToTable("tbAvisos");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.Calificaciones", b =>
                {
                    b.Property<int>("CalificacionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CalificacionId"));

                    b.Property<int>("ActividadId")
                        .HasColumnType("int");

                    b.Property<int>("Calificacion")
                        .HasColumnType("int");

                    b.Property<string>("Comentarios")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("FechaCalificacionAsignada")
                        .HasColumnType("datetime2");

                    b.HasKey("CalificacionId");

                    b.HasIndex("ActividadId");

                    b.ToTable("tbCalificaciones");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.Docentes", b =>
                {
                    b.Property<int>("DocenteId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DocenteId"));

                    b.Property<string>("ApellidoMaterno")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ApellidoPaterno")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("DocenteId");

                    b.HasIndex("UserId");

                    b.ToTable("tbDocentes");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.EntregablesAlumno", b =>
                {
                    b.Property<int>("EntregaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EntregaId"));

                    b.Property<int>("AlumnoActividadId")
                        .HasColumnType("int");

                    b.Property<string>("Archivo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Enlace")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EntregaId");

                    b.ToTable("tbEntregablesAlumno");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.EventosAgenda", b =>
                {
                    b.Property<int>("EventoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EventoId"));

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DocenteId")
                        .HasColumnType("int");

                    b.Property<DateTime>("FechaFinal")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FechaInicio")
                        .HasColumnType("datetime2");

                    b.Property<string>("Titulo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EventoId");

                    b.HasIndex("DocenteId");

                    b.ToTable("tbEventosAgenda");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.EventosGrupos", b =>
                {
                    b.Property<int>("EventoGrupoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EventoGrupoId"));

                    b.Property<int>("FechaId")
                        .HasColumnType("int");

                    b.Property<int>("GrupoId")
                        .HasColumnType("int");

                    b.HasKey("EventoGrupoId");

                    b.HasIndex("FechaId");

                    b.HasIndex("GrupoId");

                    b.ToTable("tbEventosGrupos");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.EventosMaterias", b =>
                {
                    b.Property<int>("EventoMateriaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EventoMateriaId"));

                    b.Property<int>("FechaId")
                        .HasColumnType("int");

                    b.Property<int>("MateriaId")
                        .HasColumnType("int");

                    b.HasKey("EventoMateriaId");

                    b.HasIndex("FechaId");

                    b.HasIndex("MateriaId");

                    b.ToTable("tbEventosMaterias");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.Grupos", b =>
                {
                    b.Property<int>("GrupoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GrupoId"));

                    b.Property<string>("CodigoAcceso")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CodigoColor")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Descripcion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DocenteId")
                        .HasColumnType("int");

                    b.Property<string>("NombreGrupo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("GrupoId");

                    b.HasIndex("DocenteId");

                    b.ToTable("tbGrupos");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.GruposMaterias", b =>
                {
                    b.Property<int>("GrupoMateriasId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GrupoMateriasId"));

                    b.Property<int>("GrupoId")
                        .HasColumnType("int");

                    b.Property<int>("MateriaId")
                        .HasColumnType("int");

                    b.HasKey("GrupoMateriasId");

                    b.HasIndex("GrupoId");

                    b.HasIndex("MateriaId");

                    b.ToTable("tbGruposMaterias");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.Materias", b =>
                {
                    b.Property<int>("MateriaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MateriaId"));

                    b.Property<string>("CodigoAcceso")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CodigoColor")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Descripcion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DocenteId")
                        .HasColumnType("int");

                    b.Property<string>("NombreMateria")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("MateriaId");

                    b.HasIndex("DocenteId");

                    b.ToTable("tbMaterias");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.MateriasActividades", b =>
                {
                    b.Property<int>("MateriaActividad")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MateriaActividad"));

                    b.Property<int>("ActividadId")
                        .HasColumnType("int");

                    b.Property<int>("MateriaId")
                        .HasColumnType("int");

                    b.HasKey("MateriaActividad");

                    b.HasIndex("ActividadId");

                    b.HasIndex("MateriaId");

                    b.ToTable("tbMateriasActividades");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.TiposActividades", b =>
                {
                    b.Property<int>("TipoActividadId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TipoActividadId"));

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TipoActividadId");

                    b.ToTable("cTiposActividades");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("AprendeMasWeb.Models.Actividades", b =>
                {
                    b.HasOne("AprendeMasWeb.Models.DBModels.TiposActividades", "TiposActividades")
                        .WithMany("Actividades")
                        .HasForeignKey("TipoActividadId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("TiposActividades");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.Alumnos", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", "IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("IdentityUser");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.AlumnosActividades", b =>
                {
                    b.HasOne("AprendeMasWeb.Models.Actividades", "Actividades")
                        .WithMany("AlumnosActividades")
                        .HasForeignKey("ActividadId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("AprendeMasWeb.Models.DBModels.Alumnos", "Alumnos")
                        .WithMany("AlumnosActividades")
                        .HasForeignKey("ActividadId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("AprendeMasWeb.Models.DBModels.EntregablesAlumno", "EntregablesAlumno")
                        .WithOne("AlumnosActividades")
                        .HasForeignKey("AprendeMasWeb.Models.DBModels.AlumnosActividades", "AlumnoActividadId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Actividades");

                    b.Navigation("Alumnos");

                    b.Navigation("EntregablesAlumno");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.AlumnosGrupos", b =>
                {
                    b.HasOne("AprendeMasWeb.Models.DBModels.Alumnos", "Alumnos")
                        .WithMany("AlumnosGrupos")
                        .HasForeignKey("AlumnoId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("AprendeMasWeb.Models.DBModels.Grupos", "Grupos")
                        .WithMany("AlumnosGrupos")
                        .HasForeignKey("GrupoId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Alumnos");

                    b.Navigation("Grupos");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.AlumnosMaterias", b =>
                {
                    b.HasOne("AprendeMasWeb.Models.DBModels.Alumnos", "Alumnos")
                        .WithMany("AlumnosMaterias")
                        .HasForeignKey("AlumnoId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("AprendeMasWeb.Models.DBModels.Materias", "Materias")
                        .WithMany("AlumnosMaterias")
                        .HasForeignKey("MateriaId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Alumnos");

                    b.Navigation("Materias");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.AlumnosTokens", b =>
                {
                    b.HasOne("AprendeMasWeb.Models.DBModels.Alumnos", "Alumnos")
                        .WithMany("AlumnosTokens")
                        .HasForeignKey("AlumnoId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Alumnos");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.Avisos", b =>
                {
                    b.HasOne("AprendeMasWeb.Models.DBModels.Docentes", "Docentes")
                        .WithMany("Avisos")
                        .HasForeignKey("DocenteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Docentes");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.Calificaciones", b =>
                {
                    b.HasOne("AprendeMasWeb.Models.Actividades", "Actividades")
                        .WithMany("Calificaciones")
                        .HasForeignKey("ActividadId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Actividades");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.Docentes", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", "IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("IdentityUser");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.EventosAgenda", b =>
                {
                    b.HasOne("AprendeMasWeb.Models.DBModels.Docentes", "Docentes")
                        .WithMany("EventosAgendas")
                        .HasForeignKey("DocenteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Docentes");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.EventosGrupos", b =>
                {
                    b.HasOne("AprendeMasWeb.Models.DBModels.EventosAgenda", "EventosAgenda")
                        .WithMany("EventosGrupos")
                        .HasForeignKey("FechaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AprendeMasWeb.Models.DBModels.Grupos", "Grupos")
                        .WithMany("EventosGrupos")
                        .HasForeignKey("GrupoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EventosAgenda");

                    b.Navigation("Grupos");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.EventosMaterias", b =>
                {
                    b.HasOne("AprendeMasWeb.Models.DBModels.EventosAgenda", "EventosAgenda")
                        .WithMany("EventosMaterias")
                        .HasForeignKey("FechaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AprendeMasWeb.Models.DBModels.Materias", "Materias")
                        .WithMany("EventosMaterias")
                        .HasForeignKey("MateriaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EventosAgenda");

                    b.Navigation("Materias");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.Grupos", b =>
                {
                    b.HasOne("AprendeMasWeb.Models.DBModels.Docentes", "Docentes")
                        .WithMany("Grupos")
                        .HasForeignKey("DocenteId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Docentes");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.GruposMaterias", b =>
                {
                    b.HasOne("AprendeMasWeb.Models.DBModels.Grupos", "Grupos")
                        .WithMany("GruposMaterias")
                        .HasForeignKey("GrupoId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("AprendeMasWeb.Models.DBModels.Materias", "Materias")
                        .WithMany("GruposMaterias")
                        .HasForeignKey("MateriaId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Grupos");

                    b.Navigation("Materias");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.Materias", b =>
                {
                    b.HasOne("AprendeMasWeb.Models.DBModels.Docentes", "Docentes")
                        .WithMany("Materias")
                        .HasForeignKey("DocenteId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Docentes");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.MateriasActividades", b =>
                {
                    b.HasOne("AprendeMasWeb.Models.Actividades", "Actividades")
                        .WithMany("MateriasActividades")
                        .HasForeignKey("ActividadId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("AprendeMasWeb.Models.DBModels.Materias", "Materias")
                        .WithMany("MateriasActividades")
                        .HasForeignKey("MateriaId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Actividades");

                    b.Navigation("Materias");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AprendeMasWeb.Models.Actividades", b =>
                {
                    b.Navigation("AlumnosActividades");

                    b.Navigation("Calificaciones");

                    b.Navigation("MateriasActividades");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.Alumnos", b =>
                {
                    b.Navigation("AlumnosActividades");

                    b.Navigation("AlumnosGrupos");

                    b.Navigation("AlumnosMaterias");

                    b.Navigation("AlumnosTokens");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.Docentes", b =>
                {
                    b.Navigation("Avisos");

                    b.Navigation("EventosAgendas");

                    b.Navigation("Grupos");

                    b.Navigation("Materias");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.EntregablesAlumno", b =>
                {
                    b.Navigation("AlumnosActividades");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.EventosAgenda", b =>
                {
                    b.Navigation("EventosGrupos");

                    b.Navigation("EventosMaterias");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.Grupos", b =>
                {
                    b.Navigation("AlumnosGrupos");

                    b.Navigation("EventosGrupos");

                    b.Navigation("GruposMaterias");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.Materias", b =>
                {
                    b.Navigation("AlumnosMaterias");

                    b.Navigation("EventosMaterias");

                    b.Navigation("GruposMaterias");

                    b.Navigation("MateriasActividades");
                });

            modelBuilder.Entity("AprendeMasWeb.Models.DBModels.TiposActividades", b =>
                {
                    b.Navigation("Actividades");
                });
#pragma warning restore 612, 618
        }
    }
}
