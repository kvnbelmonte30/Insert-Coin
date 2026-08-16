using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Maquinitas.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditoriaEventos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioNombre = table.Column<string>(type: "text", nullable: false),
                    LocalId = table.Column<Guid>(type: "uuid", nullable: true),
                    Accion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Entidad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntidadId = table.Column<string>(type: "text", nullable: false),
                    ValorAnterior = table.Column<string>(type: "text", nullable: true),
                    ValorNuevo = table.Column<string>(type: "text", nullable: true),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditoriaEventos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Denominaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ValorPorBolsa = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Denominaciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Direccion = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Premios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Denominacion = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Premios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Propietarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Telefono = table.Column<string>(type: "text", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Propietarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposMaquina",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposMaquina", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notificaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocalId = table.Column<Guid>(type: "uuid", nullable: true),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Mensaje = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ReferenciaId = table.Column<Guid>(type: "uuid", nullable: true),
                    Leida = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notificaciones_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notificaciones_Locales_LocalId",
                        column: x => x.LocalId,
                        principalTable: "Locales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Semanas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LocalId = table.Column<Guid>(type: "uuid", nullable: false),
                    Numero = table.Column<int>(type: "integer", nullable: false),
                    FechaInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaFin = table.Column<DateOnly>(type: "date", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Semanas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Semanas_Locales_LocalId",
                        column: x => x.LocalId,
                        principalTable: "Locales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioLocales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocalId = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioLocales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioLocales_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioLocales_Locales_LocalId",
                        column: x => x.LocalId,
                        principalTable: "Locales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Maquinas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    TipoMaquinaId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocalId = table.Column<Guid>(type: "uuid", nullable: false),
                    PropietarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    Marca = table.Column<string>(type: "text", nullable: true),
                    Modelo = table.Column<string>(type: "text", nullable: true),
                    NumeroSerie = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    FechaAlta = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maquinas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Maquinas_Locales_LocalId",
                        column: x => x.LocalId,
                        principalTable: "Locales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Maquinas_Propietarios_PropietarioId",
                        column: x => x.PropietarioId,
                        principalTable: "Propietarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Maquinas_TiposMaquina_TipoMaquinaId",
                        column: x => x.TipoMaquinaId,
                        principalTable: "TiposMaquina",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CierresDiarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LocalId = table.Column<Guid>(type: "uuid", nullable: false),
                    SemanaId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpleadoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    TotalReportado = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalEsperado = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Editable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CierresDiarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CierresDiarios_AspNetUsers_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CierresDiarios_Locales_LocalId",
                        column: x => x.LocalId,
                        principalTable: "Locales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CierresDiarios_Semanas_SemanaId",
                        column: x => x.SemanaId,
                        principalTable: "Semanas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CierresSemanales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SemanaId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocalId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalReportado = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalEsperado = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    EstadoDiferencia = table.Column<int>(type: "integer", nullable: false),
                    Confirmado = table.Column<bool>(type: "boolean", nullable: false),
                    CreadoPorUsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConfirmadoPorUsuarioId = table.Column<Guid>(type: "uuid", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaConfirmacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CierresSemanales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CierresSemanales_AspNetUsers_ConfirmadoPorUsuarioId",
                        column: x => x.ConfirmadoPorUsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CierresSemanales_AspNetUsers_CreadoPorUsuarioId",
                        column: x => x.CreadoPorUsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CierresSemanales_Locales_LocalId",
                        column: x => x.LocalId,
                        principalTable: "Locales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CierresSemanales_Semanas_SemanaId",
                        column: x => x.SemanaId,
                        principalTable: "Semanas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cuentas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LocalId = table.Column<Guid>(type: "uuid", nullable: false),
                    SemanaId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreadoPorUsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Activa = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cuentas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cuentas_AspNetUsers_CreadoPorUsuarioId",
                        column: x => x.CreadoPorUsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cuentas_Locales_LocalId",
                        column: x => x.LocalId,
                        principalTable: "Locales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cuentas_Semanas_SemanaId",
                        column: x => x.SemanaId,
                        principalTable: "Semanas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistorialEstadosMaquina",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaquinaId = table.Column<Guid>(type: "uuid", nullable: false),
                    EstadoAnterior = table.Column<int>(type: "integer", nullable: false),
                    EstadoNuevo = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    Comentario = table.Column<string>(type: "text", nullable: true),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialEstadosMaquina", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialEstadosMaquina_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialEstadosMaquina_Maquinas_MaquinaId",
                        column: x => x.MaquinaId,
                        principalTable: "Maquinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventariosPremios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaquinaId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventariosPremios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventariosPremios_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventariosPremios_Maquinas_MaquinaId",
                        column: x => x.MaquinaId,
                        principalTable: "Maquinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaquinaPremios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaquinaId = table.Column<Guid>(type: "uuid", nullable: false),
                    PremioId = table.Column<Guid>(type: "uuid", nullable: false),
                    CantidadAsignada = table.Column<int>(type: "integer", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaquinaPremios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaquinaPremios_Maquinas_MaquinaId",
                        column: x => x.MaquinaId,
                        principalTable: "Maquinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaquinaPremios_Premios_PremioId",
                        column: x => x.PremioId,
                        principalTable: "Premios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReportesAveria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LocalId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaquinaId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpleadoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Problema = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    EstadoAlReportar = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportesAveria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportesAveria_AspNetUsers_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReportesAveria_Locales_LocalId",
                        column: x => x.LocalId,
                        principalTable: "Locales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReportesAveria_Maquinas_MaquinaId",
                        column: x => x.MaquinaId,
                        principalTable: "Maquinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CierreDiarioDetalles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CierreDiarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    Concepto = table.Column<int>(type: "integer", nullable: false),
                    DenominacionId = table.Column<Guid>(type: "uuid", nullable: true),
                    PremioId = table.Column<Guid>(type: "uuid", nullable: true),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CierreDiarioDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CierreDiarioDetalles_CierresDiarios_CierreDiarioId",
                        column: x => x.CierreDiarioId,
                        principalTable: "CierresDiarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CierreDiarioDetalles_Denominaciones_DenominacionId",
                        column: x => x.DenominacionId,
                        principalTable: "Denominaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CierreDiarioDetalles_Premios_PremioId",
                        column: x => x.PremioId,
                        principalTable: "Premios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Gastos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LocalId = table.Column<Guid>(type: "uuid", nullable: false),
                    CierreDiarioId = table.Column<Guid>(type: "uuid", nullable: true),
                    EmpleadoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Monto = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gastos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gastos_AspNetUsers_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Gastos_CierresDiarios_CierreDiarioId",
                        column: x => x.CierreDiarioId,
                        principalTable: "CierresDiarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Gastos_Locales_LocalId",
                        column: x => x.LocalId,
                        principalTable: "Locales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CierreSemanalDetalles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CierreSemanalId = table.Column<Guid>(type: "uuid", nullable: false),
                    Concepto = table.Column<int>(type: "integer", nullable: false),
                    DenominacionId = table.Column<Guid>(type: "uuid", nullable: true),
                    PremioId = table.Column<Guid>(type: "uuid", nullable: true),
                    EsPremioPuesto = table.Column<bool>(type: "boolean", nullable: false),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CierreSemanalDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CierreSemanalDetalles_CierresSemanales_CierreSemanalId",
                        column: x => x.CierreSemanalId,
                        principalTable: "CierresSemanales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CierreSemanalDetalles_Denominaciones_DenominacionId",
                        column: x => x.DenominacionId,
                        principalTable: "Denominaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CierreSemanalDetalles_Premios_PremioId",
                        column: x => x.PremioId,
                        principalTable: "Premios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CuentaDetalles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CuentaId = table.Column<Guid>(type: "uuid", nullable: false),
                    DenominacionId = table.Column<Guid>(type: "uuid", nullable: true),
                    PremioId = table.Column<Guid>(type: "uuid", nullable: true),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Origen = table.Column<string>(type: "text", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RegistradoPorUsuarioId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentaDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CuentaDetalles_AspNetUsers_RegistradoPorUsuarioId",
                        column: x => x.RegistradoPorUsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CuentaDetalles_Cuentas_CuentaId",
                        column: x => x.CuentaId,
                        principalTable: "Cuentas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CuentaDetalles_Denominaciones_DenominacionId",
                        column: x => x.DenominacionId,
                        principalTable: "Denominaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CuentaDetalles_Premios_PremioId",
                        column: x => x.PremioId,
                        principalTable: "Premios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CuentaModificacionLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CuentaId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    Concepto = table.Column<string>(type: "text", nullable: false),
                    ValorAnterior = table.Column<string>(type: "text", nullable: false),
                    ValorNuevo = table.Column<string>(type: "text", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NotificacionEnviada = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentaModificacionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CuentaModificacionLogs_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CuentaModificacionLogs_Cuentas_CuentaId",
                        column: x => x.CuentaId,
                        principalTable: "Cuentas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventarioPremioDetalles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InventarioPremioId = table.Column<Guid>(type: "uuid", nullable: false),
                    PremioId = table.Column<Guid>(type: "uuid", nullable: false),
                    CantidadConfigurada = table.Column<int>(type: "integer", nullable: false),
                    CantidadEncontrada = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioPremioDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventarioPremioDetalles_InventariosPremios_InventarioPremi~",
                        column: x => x.InventarioPremioId,
                        principalTable: "InventariosPremios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventarioPremioDetalles_Premios_PremioId",
                        column: x => x.PremioId,
                        principalTable: "Premios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EvidenciasAveria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReporteAveriaId = table.Column<Guid>(type: "uuid", nullable: false),
                    RutaArchivo = table.Column<string>(type: "text", nullable: false),
                    NombreArchivo = table.Column<string>(type: "text", nullable: false),
                    FechaSubida = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvidenciasAveria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvidenciasAveria_ReportesAveria_ReporteAveriaId",
                        column: x => x.ReporteAveriaId,
                        principalTable: "ReportesAveria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Evidencias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GastoId = table.Column<Guid>(type: "uuid", nullable: false),
                    RutaArchivo = table.Column<string>(type: "text", nullable: false),
                    NombreArchivo = table.Column<string>(type: "text", nullable: false),
                    FechaSubida = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evidencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evidencias_Gastos_GastoId",
                        column: x => x.GastoId,
                        principalTable: "Gastos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriaEventos_Entidad_EntidadId",
                table: "AuditoriaEventos",
                columns: new[] { "Entidad", "EntidadId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriaEventos_Fecha",
                table: "AuditoriaEventos",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_CierreDiarioDetalles_CierreDiarioId",
                table: "CierreDiarioDetalles",
                column: "CierreDiarioId");

            migrationBuilder.CreateIndex(
                name: "IX_CierreDiarioDetalles_DenominacionId",
                table: "CierreDiarioDetalles",
                column: "DenominacionId");

            migrationBuilder.CreateIndex(
                name: "IX_CierreDiarioDetalles_PremioId",
                table: "CierreDiarioDetalles",
                column: "PremioId");

            migrationBuilder.CreateIndex(
                name: "IX_CierresDiarios_EmpleadoId",
                table: "CierresDiarios",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_CierresDiarios_LocalId_EmpleadoId_Fecha",
                table: "CierresDiarios",
                columns: new[] { "LocalId", "EmpleadoId", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_CierresDiarios_SemanaId",
                table: "CierresDiarios",
                column: "SemanaId");

            migrationBuilder.CreateIndex(
                name: "IX_CierreSemanalDetalles_CierreSemanalId",
                table: "CierreSemanalDetalles",
                column: "CierreSemanalId");

            migrationBuilder.CreateIndex(
                name: "IX_CierreSemanalDetalles_DenominacionId",
                table: "CierreSemanalDetalles",
                column: "DenominacionId");

            migrationBuilder.CreateIndex(
                name: "IX_CierreSemanalDetalles_PremioId",
                table: "CierreSemanalDetalles",
                column: "PremioId");

            migrationBuilder.CreateIndex(
                name: "IX_CierresSemanales_ConfirmadoPorUsuarioId",
                table: "CierresSemanales",
                column: "ConfirmadoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_CierresSemanales_CreadoPorUsuarioId",
                table: "CierresSemanales",
                column: "CreadoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_CierresSemanales_LocalId",
                table: "CierresSemanales",
                column: "LocalId");

            migrationBuilder.CreateIndex(
                name: "IX_CierresSemanales_SemanaId",
                table: "CierresSemanales",
                column: "SemanaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CuentaDetalles_CuentaId",
                table: "CuentaDetalles",
                column: "CuentaId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentaDetalles_DenominacionId",
                table: "CuentaDetalles",
                column: "DenominacionId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentaDetalles_PremioId",
                table: "CuentaDetalles",
                column: "PremioId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentaDetalles_RegistradoPorUsuarioId",
                table: "CuentaDetalles",
                column: "RegistradoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentaModificacionLogs_CuentaId",
                table: "CuentaModificacionLogs",
                column: "CuentaId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentaModificacionLogs_UsuarioId",
                table: "CuentaModificacionLogs",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Cuentas_CreadoPorUsuarioId",
                table: "Cuentas",
                column: "CreadoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Cuentas_LocalId",
                table: "Cuentas",
                column: "LocalId");

            migrationBuilder.CreateIndex(
                name: "IX_Cuentas_SemanaId",
                table: "Cuentas",
                column: "SemanaId");

            migrationBuilder.CreateIndex(
                name: "IX_Evidencias_GastoId",
                table: "Evidencias",
                column: "GastoId");

            migrationBuilder.CreateIndex(
                name: "IX_EvidenciasAveria_ReporteAveriaId",
                table: "EvidenciasAveria",
                column: "ReporteAveriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Gastos_CierreDiarioId",
                table: "Gastos",
                column: "CierreDiarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Gastos_EmpleadoId",
                table: "Gastos",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Gastos_LocalId",
                table: "Gastos",
                column: "LocalId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEstadosMaquina_MaquinaId",
                table: "HistorialEstadosMaquina",
                column: "MaquinaId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEstadosMaquina_UsuarioId",
                table: "HistorialEstadosMaquina",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioPremioDetalles_InventarioPremioId",
                table: "InventarioPremioDetalles",
                column: "InventarioPremioId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioPremioDetalles_PremioId",
                table: "InventarioPremioDetalles",
                column: "PremioId");

            migrationBuilder.CreateIndex(
                name: "IX_InventariosPremios_MaquinaId",
                table: "InventariosPremios",
                column: "MaquinaId");

            migrationBuilder.CreateIndex(
                name: "IX_InventariosPremios_UsuarioId",
                table: "InventariosPremios",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_MaquinaPremios_MaquinaId_PremioId",
                table: "MaquinaPremios",
                columns: new[] { "MaquinaId", "PremioId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaquinaPremios_PremioId",
                table: "MaquinaPremios",
                column: "PremioId");

            migrationBuilder.CreateIndex(
                name: "IX_Maquinas_LocalId",
                table: "Maquinas",
                column: "LocalId");

            migrationBuilder.CreateIndex(
                name: "IX_Maquinas_PropietarioId",
                table: "Maquinas",
                column: "PropietarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Maquinas_TipoMaquinaId",
                table: "Maquinas",
                column: "TipoMaquinaId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_LocalId",
                table: "Notificaciones",
                column: "LocalId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_UsuarioId_Leida",
                table: "Notificaciones",
                columns: new[] { "UsuarioId", "Leida" });

            migrationBuilder.CreateIndex(
                name: "IX_ReportesAveria_EmpleadoId",
                table: "ReportesAveria",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportesAveria_LocalId",
                table: "ReportesAveria",
                column: "LocalId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportesAveria_MaquinaId",
                table: "ReportesAveria",
                column: "MaquinaId");

            migrationBuilder.CreateIndex(
                name: "IX_Semanas_LocalId_Numero",
                table: "Semanas",
                columns: new[] { "LocalId", "Numero" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposMaquina_Nombre",
                table: "TiposMaquina",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioLocales_LocalId",
                table: "UsuarioLocales",
                column: "LocalId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioLocales_UsuarioId_LocalId",
                table: "UsuarioLocales",
                columns: new[] { "UsuarioId", "LocalId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AuditoriaEventos");

            migrationBuilder.DropTable(
                name: "CierreDiarioDetalles");

            migrationBuilder.DropTable(
                name: "CierreSemanalDetalles");

            migrationBuilder.DropTable(
                name: "CuentaDetalles");

            migrationBuilder.DropTable(
                name: "CuentaModificacionLogs");

            migrationBuilder.DropTable(
                name: "Evidencias");

            migrationBuilder.DropTable(
                name: "EvidenciasAveria");

            migrationBuilder.DropTable(
                name: "HistorialEstadosMaquina");

            migrationBuilder.DropTable(
                name: "InventarioPremioDetalles");

            migrationBuilder.DropTable(
                name: "MaquinaPremios");

            migrationBuilder.DropTable(
                name: "Notificaciones");

            migrationBuilder.DropTable(
                name: "UsuarioLocales");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "CierresSemanales");

            migrationBuilder.DropTable(
                name: "Denominaciones");

            migrationBuilder.DropTable(
                name: "Cuentas");

            migrationBuilder.DropTable(
                name: "Gastos");

            migrationBuilder.DropTable(
                name: "ReportesAveria");

            migrationBuilder.DropTable(
                name: "InventariosPremios");

            migrationBuilder.DropTable(
                name: "Premios");

            migrationBuilder.DropTable(
                name: "CierresDiarios");

            migrationBuilder.DropTable(
                name: "Maquinas");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Semanas");

            migrationBuilder.DropTable(
                name: "Propietarios");

            migrationBuilder.DropTable(
                name: "TiposMaquina");

            migrationBuilder.DropTable(
                name: "Locales");
        }
    }
}
