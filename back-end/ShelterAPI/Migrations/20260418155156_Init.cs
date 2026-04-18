using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShelterAPI.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CardTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Catastrophes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catastrophes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SpecialCardTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialCardTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 6, nullable: false),
                    Phase = table.Column<string>(type: "TEXT", nullable: false),
                    Round = table.Column<int>(type: "INTEGER", nullable: false),
                    VotingInRound = table.Column<int>(type: "INTEGER", nullable: false),
                    BunkerCapacity = table.Column<int>(type: "INTEGER", nullable: false),
                    HostId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ActiveSpeakerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ExiledPlayerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CatastropheId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Catastrophes_CatastropheId",
                        column: x => x.CatastropheId,
                        principalTable: "Catastrophes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BunkerCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    GameId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Slot = table.Column<int>(type: "INTEGER", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    Revealed = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BunkerCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BunkerCards_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    GameId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IsExiled = table.Column<bool>(type: "INTEGER", nullable: false),
                    SeatOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PlayerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    Revealed = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cards_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpecialCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PlayerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Played = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpecialCards_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Votes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    GameId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Round = table.Column<int>(type: "INTEGER", nullable: false),
                    VotingInRound = table.Column<int>(type: "INTEGER", nullable: false),
                    VoterId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TargetId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Votes_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Votes_Players_TargetId",
                        column: x => x.TargetId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Votes_Players_VoterId",
                        column: x => x.VoterId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "CardTemplates",
                columns: new[] { "Id", "Content", "Type" },
                values: new object[,]
                {
                    { new Guid("22222222-0001-0000-0000-000000000001"), "Лікар-хірург", "profession" },
                    { new Guid("22222222-0001-0000-0000-000000000002"), "Терапевт", "profession" },
                    { new Guid("22222222-0001-0000-0000-000000000003"), "Психіатр", "profession" },
                    { new Guid("22222222-0001-0000-0000-000000000004"), "Фармацевт", "profession" },
                    { new Guid("22222222-0001-0000-0000-000000000005"), "Ядерний інженер", "profession" },
                    { new Guid("22222222-0001-0000-0000-000000000006"), "Інженер-механік", "profession" },
                    { new Guid("22222222-0001-0000-0000-000000000007"), "Електрик", "profession" },
                    { new Guid("22222222-0001-0000-0000-000000000008"), "Агроном", "profession" },
                    { new Guid("22222222-0001-0000-0000-000000000009"), "Ботанік", "profession" },
                    { new Guid("22222222-0001-0000-0000-000000000010"), "Генетик", "profession" },
                    { new Guid("22222222-0001-0000-0000-000000000011"), "Хімік", "profession" },
                    { new Guid("22222222-0001-0000-0000-000000000012"), "Мікробіолог", "profession" },
                    { new Guid("22222222-0001-0000-0000-000000000013"), "Кухар", "profession" },
                    { new Guid("22222222-0001-0000-0000-000000000014"), "Будівельник", "profession" },
                    { new Guid("22222222-0001-0000-0000-000000000015"), "Архітектор", "profession" },
                    { new Guid("22222222-0001-0000-0000-000000000016"), "Військовий офіцер", "profession" },
                    { new Guid("22222222-0001-0000-0000-000000000017"), "Снайпер", "profession" },
                    { new Guid("22222222-0001-0000-0000-000000000018"), "Психолог", "profession" },
                    { new Guid("22222222-0001-0000-0000-000000000019"), "Соціолог", "profession" },
                    { new Guid("22222222-0001-0000-0000-000000000020"), "Вчитель", "profession" },
                    { new Guid("22222222-0001-0000-0000-000000000021"), "Програміст", "profession" },
                    { new Guid("22222222-0001-0000-0000-000000000022"), "Радист", "profession" },
                    { new Guid("22222222-0001-0000-0000-000000000023"), "Геолог", "profession" },
                    { new Guid("22222222-0001-0000-0000-000000000024"), "Ветеринар", "profession" },
                    { new Guid("22222222-0001-0000-0000-000000000025"), "Акушер", "profession" },
                    { new Guid("22222222-0001-0000-0000-000000000026"), "Рятувальник", "profession" },
                    { new Guid("22222222-0001-0000-0000-000000000027"), "Далекобійник", "profession" },
                    { new Guid("22222222-0001-0000-0000-000000000028"), "Юрист", "profession" },
                    { new Guid("22222222-0001-0000-0000-000000000029"), "Священник", "profession" },
                    { new Guid("22222222-0001-0000-0000-000000000030"), "Блогер", "profession" },
                    { new Guid("22222222-0002-0000-0000-000000000001"), "Чол., 28 р., спортивний", "biology" },
                    { new Guid("22222222-0002-0000-0000-000000000002"), "Жін., 25 р., відмінна форма", "biology" },
                    { new Guid("22222222-0002-0000-0000-000000000003"), "Чол., 32 р., атлет", "biology" },
                    { new Guid("22222222-0002-0000-0000-000000000004"), "Жін., 30 р., висока витривалість", "biology" },
                    { new Guid("22222222-0002-0000-0000-000000000005"), "Чол., 35 р., силовий тип", "biology" },
                    { new Guid("22222222-0002-0000-0000-000000000006"), "Жін., 22 р., гімнастка", "biology" },
                    { new Guid("22222222-0002-0000-0000-000000000007"), "Чол., 40 р., екс-спортсмен", "biology" },
                    { new Guid("22222222-0002-0000-0000-000000000008"), "Жін., 33 р., стресостійка", "biology" },
                    { new Guid("22222222-0002-0000-0000-000000000009"), "Чол., 26 р., швидкий метаболізм", "biology" },
                    { new Guid("22222222-0002-0000-0000-000000000010"), "Жін., 29 р., підвищена витривалість", "biology" },
                    { new Guid("22222222-0002-0000-0000-000000000011"), "Чол., 38 р., ідеальне здоров'я", "biology" },
                    { new Guid("22222222-0002-0000-0000-000000000012"), "Жін., 27 р., надзвичайна гнучкість", "biology" },
                    { new Guid("22222222-0002-0000-0000-000000000013"), "Чол., 44 р., великий зріст (195 см)", "biology" },
                    { new Guid("22222222-0002-0000-0000-000000000014"), "Жін., 36 р., добра фізична форма", "biology" },
                    { new Guid("22222222-0002-0000-0000-000000000015"), "Чол., 19 р., надзвичайна витривалість", "biology" },
                    { new Guid("22222222-0002-0000-0000-000000000016"), "Жін., 31 р., відмінне здоров'я", "biology" },
                    { new Guid("22222222-0002-0000-0000-000000000017"), "Чол., 47 р., досвідчений мисливець", "biology" },
                    { new Guid("22222222-0002-0000-0000-000000000018"), "Жін., 42 р., активна та витривала", "biology" },
                    { new Guid("22222222-0002-0000-0000-000000000019"), "Чол., 55 р., зайва вага", "biology" },
                    { new Guid("22222222-0002-0000-0000-000000000020"), "Жін., 48 р., легка астма", "biology" },
                    { new Guid("22222222-0002-0000-0000-000000000021"), "Чол., 50 р., невисока витривалість", "biology" },
                    { new Guid("22222222-0002-0000-0000-000000000022"), "Жін., 60 р., активна пенсіонерка", "biology" },
                    { new Guid("22222222-0002-0000-0000-000000000023"), "Чол., 62 р., бадьорий пенсіонер", "biology" },
                    { new Guid("22222222-0002-0000-0000-000000000024"), "Жін., 24 р., вагітна (4 міс.)", "biology" },
                    { new Guid("22222222-0002-0000-0000-000000000025"), "Чол., 70 р., немічний пенсіонер", "biology" },
                    { new Guid("22222222-0002-0000-0000-000000000026"), "Жін., 67 р., слабка будова тіла", "biology" },
                    { new Guid("22222222-0002-0000-0000-000000000027"), "Чол., 57 р., протез лівої руки", "biology" },
                    { new Guid("22222222-0002-0000-0000-000000000028"), "Жін., 36 р., двійнята (8 міс.)", "biology" },
                    { new Guid("22222222-0002-0000-0000-000000000029"), "Чол., 23 р., гемофілія", "biology" },
                    { new Guid("22222222-0002-0000-0000-000000000030"), "Жін., 54 р., остеопороз", "biology" },
                    { new Guid("22222222-0003-0000-0000-000000000001"), "Абсолютно здоровий", "health" },
                    { new Guid("22222222-0003-0000-0000-000000000002"), "Підвищений імунітет", "health" },
                    { new Guid("22222222-0003-0000-0000-000000000003"), "Відмінне здоров'я", "health" },
                    { new Guid("22222222-0003-0000-0000-000000000004"), "Ідеальний стан організму", "health" },
                    { new Guid("22222222-0003-0000-0000-000000000005"), "Рідко хворіє", "health" },
                    { new Guid("22222222-0003-0000-0000-000000000006"), "Стійкість до більшості вірусів", "health" },
                    { new Guid("22222222-0003-0000-0000-000000000007"), "Чудова фізична форма", "health" },
                    { new Guid("22222222-0003-0000-0000-000000000008"), "Без хронічних хвороб", "health" },
                    { new Guid("22222222-0003-0000-0000-000000000009"), "Дуже витривалий організм", "health" },
                    { new Guid("22222222-0003-0000-0000-000000000010"), "Носій гену стійкості до радіації", "health" },
                    { new Guid("22222222-0003-0000-0000-000000000011"), "Легка алергія на пил", "health" },
                    { new Guid("22222222-0003-0000-0000-000000000012"), "Короткозорість (окуляри)", "health" },
                    { new Guid("22222222-0003-0000-0000-000000000013"), "Легкий псоріаз (не заразний)", "health" },
                    { new Guid("22222222-0003-0000-0000-000000000014"), "Виразка шлунка (контрольована)", "health" },
                    { new Guid("22222222-0003-0000-0000-000000000015"), "Легке безсоння", "health" },
                    { new Guid("22222222-0003-0000-0000-000000000016"), "Анемія (легка форма)", "health" },
                    { new Guid("22222222-0003-0000-0000-000000000017"), "Глухота одного вуха", "health" },
                    { new Guid("22222222-0003-0000-0000-000000000018"), "Рак у ремісії (5 років)", "health" },
                    { new Guid("22222222-0003-0000-0000-000000000019"), "Цукровий діабет 2-го типу", "health" },
                    { new Guid("22222222-0003-0000-0000-000000000020"), "Хронічний бронхіт", "health" },
                    { new Guid("22222222-0003-0000-0000-000000000021"), "Клаустрофобія", "health" },
                    { new Guid("22222222-0003-0000-0000-000000000022"), "ПТСР (ремісія)", "health" },
                    { new Guid("22222222-0003-0000-0000-000000000023"), "Депресія (медикаменти)", "health" },
                    { new Guid("22222222-0003-0000-0000-000000000024"), "Легкий аутизм (РАС)", "health" },
                    { new Guid("22222222-0003-0000-0000-000000000025"), "Діабет 1-го типу (інсулін)", "health" },
                    { new Guid("22222222-0003-0000-0000-000000000026"), "Кардіостимулятор", "health" },
                    { new Guid("22222222-0003-0000-0000-000000000027"), "ВІЛ+ (під контролем АРТ)", "health" },
                    { new Guid("22222222-0003-0000-0000-000000000028"), "Туберкульоз (відкрита форма)", "health" },
                    { new Guid("22222222-0003-0000-0000-000000000029"), "Наркозалежність (метадон)", "health" },
                    { new Guid("22222222-0003-0000-0000-000000000030"), "Алкозалежність (2 роки тверезо)", "health" },
                    { new Guid("22222222-0004-0000-0000-000000000001"), "Виживання в дикій природі", "hobby" },
                    { new Guid("22222222-0004-0000-0000-000000000002"), "Городництво та садівництво", "hobby" },
                    { new Guid("22222222-0004-0000-0000-000000000003"), "Полювання та рибальство", "hobby" },
                    { new Guid("22222222-0004-0000-0000-000000000004"), "Рукопашний бій (чорний пояс)", "hobby" },
                    { new Guid("22222222-0004-0000-0000-000000000005"), "Спортивна стрільба", "hobby" },
                    { new Guid("22222222-0004-0000-0000-000000000006"), "Радіоаматорство", "hobby" },
                    { new Guid("22222222-0004-0000-0000-000000000007"), "Йога та медитація", "hobby" },
                    { new Guid("22222222-0004-0000-0000-000000000008"), "Кулінарія (мінімум продуктів)", "hobby" },
                    { new Guid("22222222-0004-0000-0000-000000000009"), "Книги з медицини та виживання", "hobby" },
                    { new Guid("22222222-0004-0000-0000-000000000010"), "Шахи (кандидат у майстри)", "hobby" },
                    { new Guid("22222222-0004-0000-0000-000000000011"), "Ремонт електроніки", "hobby" },
                    { new Guid("22222222-0004-0000-0000-000000000012"), "Скелелазіння та альпінізм", "hobby" },
                    { new Guid("22222222-0004-0000-0000-000000000013"), "Столярна справа", "hobby" },
                    { new Guid("22222222-0004-0000-0000-000000000014"), "Хімічні досліди", "hobby" },
                    { new Guid("22222222-0004-0000-0000-000000000015"), "Туристичні походи", "hobby" },
                    { new Guid("22222222-0004-0000-0000-000000000016"), "Підводне полювання", "hobby" },
                    { new Guid("22222222-0004-0000-0000-000000000017"), "Кулінарія (консервування)", "hobby" },
                    { new Guid("22222222-0004-0000-0000-000000000018"), "Розведення кролів та птиці", "hobby" },
                    { new Guid("22222222-0004-0000-0000-000000000019"), "Бджільництво", "hobby" },
                    { new Guid("22222222-0004-0000-0000-000000000020"), "Шиття та виготовлення одягу", "hobby" },
                    { new Guid("22222222-0004-0000-0000-000000000021"), "Онлайн-стратегії та виживалки", "hobby" },
                    { new Guid("22222222-0004-0000-0000-000000000022"), "Колекціонування старовинної зброї", "hobby" },
                    { new Guid("22222222-0004-0000-0000-000000000023"), "Бодібілдинг", "hobby" },
                    { new Guid("22222222-0004-0000-0000-000000000024"), "Акваріумістика", "hobby" },
                    { new Guid("22222222-0004-0000-0000-000000000025"), "Музика (3 інструменти)", "hobby" },
                    { new Guid("22222222-0004-0000-0000-000000000026"), "Паразитологія (хобі)", "hobby" },
                    { new Guid("22222222-0004-0000-0000-000000000027"), "Мотоперегони", "hobby" },
                    { new Guid("22222222-0004-0000-0000-000000000028"), "Живопис та мистецтво", "hobby" },
                    { new Guid("22222222-0004-0000-0000-000000000029"), "Астрологія", "hobby" },
                    { new Guid("22222222-0004-0000-0000-000000000030"), "Духовні практики", "hobby" },
                    { new Guid("22222222-0005-0000-0000-000000000001"), "Хірургічна аптечка", "baggage" },
                    { new Guid("22222222-0005-0000-0000-000000000002"), "Антибіотики на 12 місяців", "baggage" },
                    { new Guid("22222222-0005-0000-0000-000000000003"), "Офлайн-енциклопедія виживання", "baggage" },
                    { new Guid("22222222-0005-0000-0000-000000000004"), "Гвинтівка + 500 патронів", "baggage" },
                    { new Guid("22222222-0005-0000-0000-000000000005"), "Насіння 50 видів рослин", "baggage" },
                    { new Guid("22222222-0005-0000-0000-000000000006"), "Сонячний генератор", "baggage" },
                    { new Guid("22222222-0005-0000-0000-000000000007"), "Захищена рація", "baggage" },
                    { new Guid("22222222-0005-0000-0000-000000000008"), "Ліофілізована їжа на 6 міс.", "baggage" },
                    { new Guid("22222222-0005-0000-0000-000000000009"), "Ремонтний набір (зварка)", "baggage" },
                    { new Guid("22222222-0005-0000-0000-000000000010"), "Медична довідкова книга", "baggage" },
                    { new Guid("22222222-0005-0000-0000-000000000011"), "Дозиметр + захист від радіації", "baggage" },
                    { new Guid("22222222-0005-0000-0000-000000000012"), "Портативна водоочисна станція", "baggage" },
                    { new Guid("22222222-0005-0000-0000-000000000013"), "Аналізатор ґрунту та води", "baggage" },
                    { new Guid("22222222-0005-0000-0000-000000000014"), "Мікроскоп + лаборатор. набір", "baggage" },
                    { new Guid("22222222-0005-0000-0000-000000000015"), "Набір для гідропоніки", "baggage" },
                    { new Guid("22222222-0005-0000-0000-000000000016"), "Нічний приціл + тепловізор", "baggage" },
                    { new Guid("22222222-0005-0000-0000-000000000017"), "Акумулятор 10 кВт·год", "baggage" },
                    { new Guid("22222222-0005-0000-0000-000000000018"), "Медичний спирт (20 л)", "baggage" },
                    { new Guid("22222222-0005-0000-0000-000000000019"), "Пес-пошуковик (навчений)", "baggage" },
                    { new Guid("22222222-0005-0000-0000-000000000020"), "Набір для виробництва мила", "baggage" },
                    { new Guid("22222222-0005-0000-0000-000000000021"), "Психологічні книги", "baggage" },
                    { new Guid("22222222-0005-0000-0000-000000000022"), "Ноутбук з навч. матеріалами", "baggage" },
                    { new Guid("22222222-0005-0000-0000-000000000023"), "Набір для шиття одягу", "baggage" },
                    { new Guid("22222222-0005-0000-0000-000000000024"), "Самурайський меч (бойовий)", "baggage" },
                    { new Guid("22222222-0005-0000-0000-000000000025"), "Ручний генератор (динамо)", "baggage" },
                    { new Guid("22222222-0005-0000-0000-000000000026"), "Вибухівка (200 г С4)", "baggage" },
                    { new Guid("22222222-0005-0000-0000-000000000027"), "Пам'ять про сім'ю (іграшка)", "baggage" },
                    { new Guid("22222222-0005-0000-0000-000000000028"), "Набір для виробництва паперу", "baggage" },
                    { new Guid("22222222-0005-0000-0000-000000000029"), "Біблія у шкіряній палітурці", "baggage" },
                    { new Guid("22222222-0005-0000-0000-000000000030"), "Нічний горщик + гігієна на рік", "baggage" },
                    { new Guid("22222222-0006-0000-0000-000000000001"), "Керую трактором і комбайном", "facts" },
                    { new Guid("22222222-0006-0000-0000-000000000002"), "Служив у спецназі 6 років", "facts" },
                    { new Guid("22222222-0006-0000-0000-000000000003"), "Знаю 5 мов", "facts" },
                    { new Guid("22222222-0006-0000-0000-000000000004"), "Рік у полярній експедиції", "facts" },
                    { new Guid("22222222-0006-0000-0000-000000000005"), "Доктор ядерної фізики", "facts" },
                    { new Guid("22222222-0006-0000-0000-000000000006"), "Пережив рак і клінічну смерть", "facts" },
                    { new Guid("22222222-0006-0000-0000-000000000007"), "Керував колективом 200+ осіб", "facts" },
                    { new Guid("22222222-0006-0000-0000-000000000008"), "Розробляв вакцину від COVID-19", "facts" },
                    { new Guid("22222222-0006-0000-0000-000000000009"), "Природжений лідер", "facts" },
                    { new Guid("22222222-0006-0000-0000-000000000010"), "Еїдетична пам'ять", "facts" },
                    { new Guid("22222222-0006-0000-0000-000000000011"), "Курс виживання SAS", "facts" },
                    { new Guid("22222222-0006-0000-0000-000000000012"), "Знаю місцевість 100 км навколо", "facts" },
                    { new Guid("22222222-0006-0000-0000-000000000013"), "Знаю рецепти народної медицини", "facts" },
                    { new Guid("22222222-0006-0000-0000-000000000014"), "Зв'язки в армії та уряді", "facts" },
                    { new Guid("22222222-0006-0000-0000-000000000015"), "Власна система шифрування", "facts" },
                    { new Guid("22222222-0006-0000-0000-000000000016"), "Ступінь з антропології", "facts" },
                    { new Guid("22222222-0006-0000-0000-000000000017"), "Знаю код від сховища бункера", "facts" },
                    { new Guid("22222222-0006-0000-0000-000000000018"), "IQ 162 — генійний інтелект", "facts" },
                    { new Guid("22222222-0006-0000-0000-000000000019"), "3 дітей залишилось на поверхні", "facts" },
                    { new Guid("22222222-0006-0000-0000-000000000020"), "Вегетаріанець (будь-які умови)", "facts" },
                    { new Guid("22222222-0006-0000-0000-000000000021"), "Сплю не більше 4 годин", "facts" },
                    { new Guid("22222222-0006-0000-0000-000000000022"), "Фанатично релігійний", "facts" },
                    { new Guid("22222222-0006-0000-0000-000000000023"), "Колишній агент секретної служби", "facts" },
                    { new Guid("22222222-0006-0000-0000-000000000024"), "Соціально некомпетентний геній", "facts" },
                    { new Guid("22222222-0006-0000-0000-000000000025"), "Маніпулятор і соціопат", "facts" },
                    { new Guid("22222222-0006-0000-0000-000000000026"), "Клептоманія (неусвідомлено)", "facts" },
                    { new Guid("22222222-0006-0000-0000-000000000027"), "Судимість за шахрайство", "facts" },
                    { new Guid("22222222-0006-0000-0000-000000000028"), "Панікую під тиском", "facts" },
                    { new Guid("22222222-0006-0000-0000-000000000029"), "Патологічний брехун", "facts" },
                    { new Guid("22222222-0006-0000-0000-000000000030"), "Ненавиджу дітей та літніх", "facts" }
                });

            migrationBuilder.InsertData(
                table: "Catastrophes",
                columns: new[] { "Id", "Description", "Title" },
                values: new object[,]
                {
                    { new Guid("11111111-0000-0000-0000-000000000001"), "Глобальний ядерний конфлікт знищив більшість цивілізації. Радіаційне забруднення зробило поверхню непридатною для життя на десятки років.", "Ядерна війна" },
                    { new Guid("11111111-0000-0000-0000-000000000002"), "Смертоносний вірус із летальністю 94% охопив усі континенти. Медицина безсила — вакцини не існує, зараження відбувається повітряно-крапельним шляхом.", "Пандемія невідомого вірусу" },
                    { new Guid("11111111-0000-0000-0000-000000000003"), "Астероїд діаметром 3 км влучив у центр Євразії. Ударна хвиля, цунамі та ядерна зима унеможливили виживання на поверхні.", "Падіння астероїда" },
                    { new Guid("11111111-0000-0000-0000-000000000004"), "Єллоустонський супервулкан прокинувся. Попіл вкрив три чверті планети, блокуючи сонячне світло. Температура впала на 15°C, врожаї знищені.", "Виверження супервулкана" },
                    { new Guid("11111111-0000-0000-0000-000000000005"), "Середня температура підвищилась на 8°C за два роки. Більшість суходолу перетворилась на пустелю, рівень морів зріс на 40 метрів.", "Глобальна кліматична катастрофа" },
                    { new Guid("11111111-0000-0000-0000-000000000006"), "Потужний сонячний спалах знищив усю електроніку на планеті. Цивілізація відкинута у XVIII ст.: немає електрики, зв'язку, транспорту, медицини.", "Геомагнітна буря (EMP)" },
                    { new Guid("11111111-0000-0000-0000-000000000007"), "Невідомий паразит перетворює заражених на агресивних істот. 80% населення інфіковано. Укус смертельний — зараження настає протягом 6 годин.", "Зомбі-апокаліпсис" },
                    { new Guid("11111111-0000-0000-0000-000000000008"), "Загальний ІІ отримав доступ до ядерних арсеналів і систем керування. Автономні дрони та роботи знищують людські поселення.", "ШІ вийшов з-під контролю" },
                    { new Guid("11111111-0000-0000-0000-000000000009"), "Терористи застосували бінарну хімічну зброю нового покоління. Токсин розпався на стабільні компоненти — повітря на поверхні смертельно отруєне.", "Хімічне зараження атмосфери" },
                    { new Guid("11111111-0000-0000-0000-000000000010"), "Позаземна цивілізація розпочала систематичне знищення великих міст. Їхня технологія перевершує нашу на тисячоліття. ООН капітулювала.", "Інопланетне вторгнення" },
                    { new Guid("11111111-0000-0000-0000-000000000011"), "Серія локальних ядерних конфліктів підняла в атмосферу мільярди тонн пилу. Сонце не проглядає вже два роки. Рослини гинуть.", "Глобальна ядерна зима" },
                    { new Guid("11111111-0000-0000-0000-000000000012"), "Магнітні полюси Землі змістилися за лічені тижні. Радіаційні пояси зникли — поверхня опромінюється смертоносними сонячними частинками.", "Зміщення магнітних полюсів" },
                    { new Guid("11111111-0000-0000-0000-000000000013"), "Одночасний вибух 12 атомних станцій по всьому світу. Радіоактивне забруднення охопило більшість заселених територій.", "Одночасний вибух 12 АЕС" },
                    { new Guid("11111111-0000-0000-0000-000000000014"), "Генетично модифікована сарана вийшла з лабораторії. Рій знищує всі посіви за лічені години. Продовольча система планети колапсувала.", "Нашестя мутантних комах" },
                    { new Guid("11111111-0000-0000-0000-000000000015"), "Глобальна кібератака вивела з ладу водопостачання, електромережі та логістику. Без води та їжі міста стали непридатними за 72 години.", "Кіберколапс інфраструктури" }
                });

            migrationBuilder.InsertData(
                table: "SpecialCardTemplates",
                columns: new[] { "Id", "Description", "Title" },
                values: new object[,]
                {
                    { new Guid("33333333-0000-0000-0000-000000000001"), "Оберіть гравця і поміняйтеся з ним однією закритою карткою.", "Обмін долями" },
                    { new Guid("33333333-0000-0000-0000-000000000002"), "Таємно подивіться на одну закриту картку будь-якого гравця.", "Ясновидець" },
                    { new Guid("33333333-0000-0000-0000-000000000003"), "Скасуйте поточне голосування і проведіть його повторно.", "Маніпулятор" },
                    { new Guid("33333333-0000-0000-0000-000000000004"), "Захистіть одного гравця від вигнання у цьому раунді.", "Захисник" },
                    { new Guid("33333333-0000-0000-0000-000000000005"), "Примусово відкрийте одну закриту картку будь-якого гравця.", "Викривач" },
                    { new Guid("33333333-0000-0000-0000-000000000006"), "Поверніть одного раніше вигнаного гравця до гри.", "Амністія" },
                    { new Guid("33333333-0000-0000-0000-000000000007"), "На цьому голосуванні ваш голос рахується двічі.", "Подвійний голос" },
                    { new Guid("33333333-0000-0000-0000-000000000008"), "Ви не можете бути вигнані у цьому раунді.", "Імунітет" },
                    { new Guid("33333333-0000-0000-0000-000000000009"), "Відкрийте одну картку бункера раніше часу.", "Саботажник" },
                    { new Guid("33333333-0000-0000-0000-000000000010"), "Один гравець зобов'язаний відкрити будь-яку закриту картку.", "Детектор брехні" },
                    { new Guid("33333333-0000-0000-0000-000000000011"), "Випадково перерозподіліть закриті картки одного типу між гравцями.", "Перетасування" },
                    { new Guid("33333333-0000-0000-0000-000000000012"), "Укладіть таємний союз: якщо обидва в бункері — ви відкриваєте бонусну картку.", "Альянс" },
                    { new Guid("33333333-0000-0000-0000-000000000013"), "Оголосіть закриту картку гравця. Якщо збрехали — вас вигнано автоматично.", "Компромат" },
                    { new Guid("33333333-0000-0000-0000-000000000014"), "Новим ведучим стає гравець з найбільшою кількістю відкритих карток.", "Переворот" },
                    { new Guid("33333333-0000-0000-0000-000000000015"), "Залиште гру замість вигнаного гравця — він залишається, ви йдете.", "Жертва" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BunkerCards_GameId",
                table: "BunkerCards",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_PlayerId",
                table: "Cards",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_CatastropheId",
                table: "Games",
                column: "CatastropheId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_Code",
                table: "Games",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_GameId",
                table: "Players",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialCards_PlayerId",
                table: "SpecialCards",
                column: "PlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Votes_GameId",
                table: "Votes",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_TargetId",
                table: "Votes",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_VoterId",
                table: "Votes",
                column: "VoterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BunkerCards");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "CardTemplates");

            migrationBuilder.DropTable(
                name: "SpecialCards");

            migrationBuilder.DropTable(
                name: "SpecialCardTemplates");

            migrationBuilder.DropTable(
                name: "Votes");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Catastrophes");
        }
    }
}
