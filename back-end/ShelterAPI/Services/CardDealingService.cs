using Microsoft.EntityFrameworkCore;
using ShelterAPI.Data;
using ShelterAPI.Models;

namespace ShelterAPI.Services
{
    public class CardDealingService(BunkerDbContext db) : ICardDealingService
    {
        private static readonly string[] CardTypes =
            ["profession", "biology", "health", "hobby", "baggage", "facts"];

        // Пул описів карток бункера (вибираємо 5 випадкових на гру)
        private static readonly string[] BunkerCardPool =
        [
            "Медичний відсік із повним хірургічним обладнанням",
            "Запас їжі та питної води на 10 років",
            "Атомний міні-реактор (необмежена електроенергія)",
            "Бібліотека: 50 000 наукових і технічних книг",
            "Насіннєвий банк — 500 видів їстівних рослин",
            "Система водоочищення та замкнутого циклу",
            "Арсенал зброї та боєприпасів на 20 осіб",
            "Гідропонна ферма (виробництво їжі всередині)",
            "Лабораторія: хімія, біологія, мікробіологія",
            "Зв'язок: захищена радіостанція та супутниковий термінал",
            "Спортивний зал і психологічна розвантажувальна зона",
            "Дитяча зона та школа (розрахована на дітей)",
            "Промислова майстерня: токарний, зварювальний верстат",
            "Захист від радіації: стіни 3 м залізобетону, HEPA-фільтри",
            "Система автономного землеробства (штучне освітлення)",
            "Тюрма-ізолятор на 3 особи (вирішення конфліктів)",
            "Медикаменти та вакцини на 15 років уперед",
            "3D-принтер та запас матеріалів для виробництва",
            "Ємності для тваринництва (кури, кролі, свині)",
            "Психологічна бібліотека та настільні ігри",
        ];

        public async Task DealAsync(Guid gameId, IList<PlayerEntity> players, CancellationToken ct = default)
        {
            var rng = new Random();

            // Завантажуємо шаблони карток, згруповані за типом
            var templatesByType = await db.CardTemplates
                .GroupBy(t => t.Type)
                .ToDictionaryAsync(g => g.Key, g => g.ToList(), ct);

            // Завантажуємо шаблони спеціальних карток
            var specialTemplates = await db.SpecialCardTemplates.ToListAsync(ct);

            var cards = new List<CardEntity>();
            var specialCards = new List<SpecialCardEntity>();

            // Відстежуємо вже використані шаблони, щоб не дублювати
            var usedTemplateIds = new HashSet<Guid>();
            var usedSpecialIds = new HashSet<Guid>();

            foreach (var player in players)
            {
                // Роздаємо по одній картці кожного типу
                foreach (var type in CardTypes)
                {
                    if (!templatesByType.TryGetValue(type, out var pool) || pool.Count == 0)
                        continue;

                    var available = pool.Where(t => !usedTemplateIds.Contains(t.Id)).ToList();

                    // Якщо пул вичерпано — починаємо з початку (дуже багато гравців)
                    if (available.Count == 0)
                    {
                        usedTemplateIds.RemoveWhere(id => pool.Any(t => t.Id == id));
                        available = pool.ToList();
                    }

                    var template = available[rng.Next(available.Count)];
                    usedTemplateIds.Add(template.Id);

                    cards.Add(CardEntity.Create(player.Id, template.Type, template.Content));
                }

                // Роздаємо спеціальну картку
                var availableSpecial = specialTemplates.Where(t => !usedSpecialIds.Contains(t.Id)).ToList();
                if (availableSpecial.Count == 0)
                {
                    usedSpecialIds.Clear();
                    availableSpecial = specialTemplates.ToList();
                }

                var special = availableSpecial[rng.Next(availableSpecial.Count)];
                usedSpecialIds.Add(special.Id);
                specialCards.Add(SpecialCardEntity.Create(player.Id, special.Title, special.Description));
            }

            // Генеруємо 3 картки бункера
            var bunkerPool = BunkerCardPool.OrderBy(_ => rng.Next()).Take(3).ToList();
            var bunkerCards = bunkerPool
                .Select((content, i) => BunkerCardEntity.Create(gameId, i + 1, content))
                .ToList();

            await db.Cards.AddRangeAsync(cards, ct);
            await db.SpecialCards.AddRangeAsync(specialCards, ct);
            await db.BunkerCards.AddRangeAsync(bunkerCards, ct);
            await db.SaveChangesAsync(ct);
        }
    }
}
