using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SocialNetwork.Infrastructure.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class DeleteDirectChatEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_messages_direct_chats_chat_id",
                table: "messages");

            migrationBuilder.DropTable(
                name: "direct_chats");

            migrationBuilder.DropIndex(
                name: "ix_messages_chat_id",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "chat_id",
                table: "messages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "chat_id",
                table: "messages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "direct_chats",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    last_message_id = table.Column<long>(type: "bigint", nullable: true),
                    user1id = table.Column<long>(type: "bigint", nullable: true),
                    user2id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_direct_chats", x => x.id);
                    table.ForeignKey(
                        name: "fk_direct_chats_messages_last_message_id",
                        column: x => x.last_message_id,
                        principalTable: "messages",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_direct_chats_users_user1id",
                        column: x => x.user1id,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_direct_chats_users_user2id",
                        column: x => x.user2id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_messages_chat_id",
                table: "messages",
                column: "chat_id");

            migrationBuilder.CreateIndex(
                name: "ix_direct_chats_last_message_id",
                table: "direct_chats",
                column: "last_message_id");

            migrationBuilder.CreateIndex(
                name: "ix_direct_chats_user1id_user2id",
                table: "direct_chats",
                columns: new[] { "user1id", "user2id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_direct_chats_user2id",
                table: "direct_chats",
                column: "user2id");

            migrationBuilder.AddForeignKey(
                name: "fk_messages_direct_chats_chat_id",
                table: "messages",
                column: "chat_id",
                principalTable: "direct_chats",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
