using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SocialNetwork.Infrastructure.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddDirectChatEntityAndMessageEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "direct_chats",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user1id = table.Column<long>(type: "bigint", nullable: true),
                    user2id = table.Column<long>(type: "bigint", nullable: true),
                    last_message_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_direct_chats", x => x.id);
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

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    chat_id = table.Column<long>(type: "bigint", nullable: false),
                    sender_id = table.Column<long>(type: "bigint", nullable: true),
                    content = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_messages", x => x.id);
                    table.ForeignKey(
                        name: "fk_messages_direct_chats_chat_id",
                        column: x => x.chat_id,
                        principalTable: "direct_chats",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_messages_users_sender_id",
                        column: x => x.sender_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

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

            migrationBuilder.CreateIndex(
                name: "ix_messages_chat_id",
                table: "messages",
                column: "chat_id");

            migrationBuilder.CreateIndex(
                name: "ix_messages_sender_id",
                table: "messages",
                column: "sender_id");

            migrationBuilder.AddForeignKey(
                name: "fk_direct_chats_messages_last_message_id",
                table: "direct_chats",
                column: "last_message_id",
                principalTable: "messages",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_direct_chats_messages_last_message_id",
                table: "direct_chats");

            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "direct_chats");
        }
    }
}
