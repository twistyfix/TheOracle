using Microsoft.EntityFrameworkCore.Migrations;

namespace TheOracle.Migrations.ThirdPartyContent
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CountingAssetTrack",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    StartingValue = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountingAssetTrack", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MultiFieldAssetTrack",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultiFieldAssetTrack", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NumericAssetTrack",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Min = table.Column<int>(nullable: false),
                    Max = table.Column<int>(nullable: false),
                    ActiveNumber = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumericAssetTrack", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSubscriptions",
                columns: table => new
                {
                    UserId = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSubscriptions", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "AssetEmbedField",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    ActiveText = table.Column<string>(nullable: true),
                    InactiveText = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    MultiFieldAssetTrackId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetEmbedField", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetEmbedField_MultiFieldAssetTrack_MultiFieldAssetTrackId",
                        column: x => x.MultiFieldAssetTrackId,
                        principalTable: "MultiFieldAssetTrack",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAssets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IconUrl = table.Column<string>(nullable: true),
                    AssetType = table.Column<string>(nullable: true),
                    Game = table.Column<int>(nullable: false),
                    MultiFieldAssetTrackId = table.Column<int>(nullable: true),
                    CountingAssetTrackId = table.Column<int>(nullable: true),
                    NumericAssetTrackId = table.Column<int>(nullable: true),
                    UserSubscriptionsUserId = table.Column<ulong>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAssets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAssets_CountingAssetTrack_CountingAssetTrackId",
                        column: x => x.CountingAssetTrackId,
                        principalTable: "CountingAssetTrack",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserAssets_MultiFieldAssetTrack_MultiFieldAssetTrackId",
                        column: x => x.MultiFieldAssetTrackId,
                        principalTable: "MultiFieldAssetTrack",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserAssets_NumericAssetTrack_NumericAssetTrackId",
                        column: x => x.NumericAssetTrackId,
                        principalTable: "NumericAssetTrack",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserAssets_UserSubscriptions_UserSubscriptionsUserId",
                        column: x => x.UserSubscriptionsUserId,
                        principalTable: "UserSubscriptions",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserOracles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Chance = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Prompt = table.Column<string>(nullable: true),
                    StandardOracleId = table.Column<int>(nullable: true),
                    UserSubscriptionsUserId = table.Column<ulong>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOracles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserOracles_UserOracles_StandardOracleId",
                        column: x => x.StandardOracleId,
                        principalTable: "UserOracles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserOracles_UserSubscriptions_UserSubscriptionsUserId",
                        column: x => x.UserSubscriptionsUserId,
                        principalTable: "UserSubscriptions",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetField",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Text = table.Column<string>(nullable: true),
                    Enabled = table.Column<bool>(nullable: false),
                    AssetId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetField", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetField_UserAssets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "UserAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InputField",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(nullable: true),
                    AssetFieldId = table.Column<int>(nullable: true),
                    AssetId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InputField", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InputField_AssetField_AssetFieldId",
                        column: x => x.AssetFieldId,
                        principalTable: "AssetField",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InputField_UserAssets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "UserAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetEmbedField_MultiFieldAssetTrackId",
                table: "AssetEmbedField",
                column: "MultiFieldAssetTrackId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetField_AssetId",
                table: "AssetField",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_InputField_AssetFieldId",
                table: "InputField",
                column: "AssetFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_InputField_AssetId",
                table: "InputField",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssets_CountingAssetTrackId",
                table: "UserAssets",
                column: "CountingAssetTrackId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssets_MultiFieldAssetTrackId",
                table: "UserAssets",
                column: "MultiFieldAssetTrackId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssets_NumericAssetTrackId",
                table: "UserAssets",
                column: "NumericAssetTrackId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssets_UserSubscriptionsUserId",
                table: "UserAssets",
                column: "UserSubscriptionsUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOracles_StandardOracleId",
                table: "UserOracles",
                column: "StandardOracleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOracles_UserSubscriptionsUserId",
                table: "UserOracles",
                column: "UserSubscriptionsUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetEmbedField");

            migrationBuilder.DropTable(
                name: "InputField");

            migrationBuilder.DropTable(
                name: "UserOracles");

            migrationBuilder.DropTable(
                name: "AssetField");

            migrationBuilder.DropTable(
                name: "UserAssets");

            migrationBuilder.DropTable(
                name: "CountingAssetTrack");

            migrationBuilder.DropTable(
                name: "MultiFieldAssetTrack");

            migrationBuilder.DropTable(
                name: "NumericAssetTrack");

            migrationBuilder.DropTable(
                name: "UserSubscriptions");
        }
    }
}
