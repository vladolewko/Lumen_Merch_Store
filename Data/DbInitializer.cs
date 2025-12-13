using Lumen_Merch_Store.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Lumen_Merch_Store.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context)
        {
            // Перевірка, чи база даних вже заповнена
            if (await context.Products.AnyAsync())
            {
                return; // База даних вже містить дані
            }

            // ========== 1. LANGUAGES ==========
            var languages = new Language[]
            {
                new Language { Code = "uk", Name = "Українська" },
                new Language { Code = "en", Name = "English" }
            };
            context.Languages.AddRange(languages);
            await context.SaveChangesAsync();

            // ========== 2. UNIVERSES ==========
            var universes = new Universe[]
            {
                new Universe(), // Star Wars
                new Universe(), // Cyberpunk 2077
                new Universe(), // Harry Potter
                new Universe(), // The Witcher
                new Universe(), // Marvel
                new Universe()  // DC Comics
            };
            context.Universes.AddRange(universes);
            await context.SaveChangesAsync();

            var starWarsId = universes[0].Id;
            var cyberpunkId = universes[1].Id;
            var harryPotterId = universes[2].Id;
            var witcherId = universes[3].Id;
            var marvelId = universes[4].Id;
            var dcId = universes[5].Id;

            // ========== 3. UNIVERSE TRANSLATIONS ==========
            var universeTranslations = new UniverseTranslation[]
            {
                // Star Wars
                new UniverseTranslation { UniverseId = starWarsId, LanguageCode = "uk", Name = "Зоряні Війни", Description = "Далека-далека галактика, де джедаї та ситхи ведуть вічну боротьбу." },
                new UniverseTranslation { UniverseId = starWarsId, LanguageCode = "en", Name = "Star Wars", Description = "A galaxy far, far away where Jedi and Sith wage an eternal battle." },
                // Cyberpunk 2077
                new UniverseTranslation { UniverseId = cyberpunkId, LanguageCode = "uk", Name = "Кіберпанк 2077", Description = "Похмурий Найт-Сіті, де технології змінили людство назавжди." },
                new UniverseTranslation { UniverseId = cyberpunkId, LanguageCode = "en", Name = "Cyberpunk 2077", Description = "The dark Night City where technology has changed humanity forever." },
                // Harry Potter
                new UniverseTranslation { UniverseId = harryPotterId, LanguageCode = "uk", Name = "Гаррі Поттер", Description = "Чарівний світ Гоґвортсу та магічних пригод." },
                new UniverseTranslation { UniverseId = harryPotterId, LanguageCode = "en", Name = "Harry Potter", Description = "The magical world of Hogwarts and enchanting adventures." },
                // The Witcher
                new UniverseTranslation { UniverseId = witcherId, LanguageCode = "uk", Name = "Відьмак", Description = "Темний світ фентезі, де відьмаки полюють на монстрів." },
                new UniverseTranslation { UniverseId = witcherId, LanguageCode = "en", Name = "The Witcher", Description = "A dark fantasy world where witchers hunt monsters." },
                // Marvel
                new UniverseTranslation { UniverseId = marvelId, LanguageCode = "uk", Name = "Marvel", Description = "Всесвіт супергероїв: Месники, Люди Ікс та багато інших." },
                new UniverseTranslation { UniverseId = marvelId, LanguageCode = "en", Name = "Marvel", Description = "Universe of superheroes: Avengers, X-Men and many more." },
                // DC Comics
                new UniverseTranslation { UniverseId = dcId, LanguageCode = "uk", Name = "DC Comics", Description = "Світ Бетмена, Супермена та Ліги Справедливості." },
                new UniverseTranslation { UniverseId = dcId, LanguageCode = "en", Name = "DC Comics", Description = "The world of Batman, Superman and the Justice League." }
            };
            context.UniverseTranslations.AddRange(universeTranslations);
            await context.SaveChangesAsync();

            // ========== 4. CATEGORIES ==========
            var categories = new Category[]
            {
                new Category(), // Clothing / Одяг
                new Category(), // Figures / Фігурки
                new Category(), // Accessories / Аксесуари
                new Category(), // Posters / Постери
                new Category()  // Collectibles / Колекційні предмети
            };
            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();

            var clothingId = categories[0].Id;
            var figuresId = categories[1].Id;
            var accessoriesId = categories[2].Id;
            var postersId = categories[3].Id;
            var collectiblesId = categories[4].Id;

            // ========== 5. CATEGORY TRANSLATIONS ==========
            var categoryTranslations = new CategoryTranslation[]
            {
                // Clothing
                new CategoryTranslation { CategoryId = clothingId, LanguageCode = "uk", Name = "Одяг", Description = "Футболки, худі, світшоти та інший одяг для фанатів." },
                new CategoryTranslation { CategoryId = clothingId, LanguageCode = "en", Name = "Clothing", Description = "T-shirts, hoodies, sweatshirts and other fan apparel." },
                // Figures
                new CategoryTranslation { CategoryId = figuresId, LanguageCode = "uk", Name = "Фігурки", Description = "Колекційні фігурки та статуетки улюблених персонажів." },
                new CategoryTranslation { CategoryId = figuresId, LanguageCode = "en", Name = "Figures", Description = "Collectible figures and statuettes of favorite characters." },
                // Accessories
                new CategoryTranslation { CategoryId = accessoriesId, LanguageCode = "uk", Name = "Аксесуари", Description = "Чашки, брелоки, сумки та інші аксесуари." },
                new CategoryTranslation { CategoryId = accessoriesId, LanguageCode = "en", Name = "Accessories", Description = "Mugs, keychains, bags and other accessories." },
                // Posters
                new CategoryTranslation { CategoryId = postersId, LanguageCode = "uk", Name = "Постери", Description = "Постери та плакати для декору кімнати." },
                new CategoryTranslation { CategoryId = postersId, LanguageCode = "en", Name = "Posters", Description = "Posters and prints for room decoration." },
                // Collectibles
                new CategoryTranslation { CategoryId = collectiblesId, LanguageCode = "uk", Name = "Колекційні предмети", Description = "Рідкісні та лімітовані колекційні предмети." },
                new CategoryTranslation { CategoryId = collectiblesId, LanguageCode = "en", Name = "Collectibles", Description = "Rare and limited edition collectible items." }
            };
            context.CategoryTranslations.AddRange(categoryTranslations);
            await context.SaveChangesAsync();

            // ========== 6. PRODUCTS ==========
            var now = DateTime.UtcNow;
            var products = new Product[]
            {
                // Star Wars Products
                new Product { UniverseId = starWarsId, CategoryId = clothingId, Price = 899.00M, Stock = 50, ImageUrl = "/images/products/sw-tshirt-vader.jpg", CreatedAt = now, UpdatedAt = now },
                new Product { UniverseId = starWarsId, CategoryId = clothingId, Price = 1599.00M, Stock = 30, ImageUrl = "/images/products/sw-hoodie-jedi.jpg", CreatedAt = now, UpdatedAt = now },
                new Product { UniverseId = starWarsId, CategoryId = figuresId, Price = 2899.00M, Stock = 15, ImageUrl = "/images/products/sw-figure-yoda.jpg", CreatedAt = now, UpdatedAt = now },
                new Product { UniverseId = starWarsId, CategoryId = accessoriesId, Price = 349.00M, Stock = 100, ImageUrl = "/images/products/sw-mug-mandalorian.jpg", CreatedAt = now, UpdatedAt = now },
                
                // Cyberpunk 2077 Products
                new Product { UniverseId = cyberpunkId, CategoryId = clothingId, Price = 1499.00M, Stock = 40, ImageUrl = "/images/products/cp-hoodie-samurai.jpg", CreatedAt = now, UpdatedAt = now },
                new Product { UniverseId = cyberpunkId, CategoryId = clothingId, Price = 799.00M, Stock = 60, ImageUrl = "/images/products/cp-tshirt-johnny.jpg", CreatedAt = now, UpdatedAt = now },
                new Product { UniverseId = cyberpunkId, CategoryId = figuresId, Price = 3499.00M, Stock = 10, ImageUrl = "/images/products/cp-figure-v.jpg", CreatedAt = now, UpdatedAt = now },
                new Product { UniverseId = cyberpunkId, CategoryId = postersId, Price = 299.00M, Stock = 80, ImageUrl = "/images/products/cp-poster-nightcity.jpg", CreatedAt = now, UpdatedAt = now },
                
                // Harry Potter Products
                new Product { UniverseId = harryPotterId, CategoryId = clothingId, Price = 1299.00M, Stock = 45, ImageUrl = "/images/products/hp-hoodie-hogwarts.jpg", CreatedAt = now, UpdatedAt = now },
                new Product { UniverseId = harryPotterId, CategoryId = accessoriesId, Price = 449.00M, Stock = 90, ImageUrl = "/images/products/hp-mug-gryffindor.jpg", CreatedAt = now, UpdatedAt = now },
                new Product { UniverseId = harryPotterId, CategoryId = collectiblesId, Price = 1899.00M, Stock = 20, ImageUrl = "/images/products/hp-wand-elder.jpg", CreatedAt = now, UpdatedAt = now },
                new Product { UniverseId = harryPotterId, CategoryId = figuresId, Price = 2199.00M, Stock = 25, ImageUrl = "/images/products/hp-figure-hedwig.jpg", CreatedAt = now, UpdatedAt = now },
                
                // The Witcher Products
                new Product { UniverseId = witcherId, CategoryId = clothingId, Price = 999.00M, Stock = 55, ImageUrl = "/images/products/tw-tshirt-wolf.jpg", CreatedAt = now, UpdatedAt = now },
                new Product { UniverseId = witcherId, CategoryId = figuresId, Price = 4299.00M, Stock = 8, ImageUrl = "/images/products/tw-figure-geralt.jpg", CreatedAt = now, UpdatedAt = now },
                new Product { UniverseId = witcherId, CategoryId = collectiblesId, Price = 2499.00M, Stock = 12, ImageUrl = "/images/products/tw-medallion-wolf.jpg", CreatedAt = now, UpdatedAt = now },
                new Product { UniverseId = witcherId, CategoryId = postersId, Price = 349.00M, Stock = 70, ImageUrl = "/images/products/tw-poster-wildHunt.jpg", CreatedAt = now, UpdatedAt = now },
                
                // Marvel Products
                new Product { UniverseId = marvelId, CategoryId = clothingId, Price = 1099.00M, Stock = 65, ImageUrl = "/images/products/mv-tshirt-avengers.jpg", CreatedAt = now, UpdatedAt = now },
                new Product { UniverseId = marvelId, CategoryId = clothingId, Price = 1699.00M, Stock = 35, ImageUrl = "/images/products/mv-hoodie-spiderman.jpg", CreatedAt = now, UpdatedAt = now },
                new Product { UniverseId = marvelId, CategoryId = figuresId, Price = 2799.00M, Stock = 18, ImageUrl = "/images/products/mv-figure-ironman.jpg", CreatedAt = now, UpdatedAt = now },
                new Product { UniverseId = marvelId, CategoryId = accessoriesId, Price = 599.00M, Stock = 75, ImageUrl = "/images/products/mv-cap-shield.jpg", CreatedAt = now, UpdatedAt = now },
                
                // DC Comics Products
                new Product { UniverseId = dcId, CategoryId = clothingId, Price = 949.00M, Stock = 50, ImageUrl = "/images/products/dc-tshirt-batman.jpg", CreatedAt = now, UpdatedAt = now },
                new Product { UniverseId = dcId, CategoryId = clothingId, Price = 1549.00M, Stock = 28, ImageUrl = "/images/products/dc-hoodie-joker.jpg", CreatedAt = now, UpdatedAt = now },
                new Product { UniverseId = dcId, CategoryId = figuresId, Price = 3199.00M, Stock = 12, ImageUrl = "/images/products/dc-figure-batman.jpg", CreatedAt = now, UpdatedAt = now },
                new Product { UniverseId = dcId, CategoryId = postersId, Price = 279.00M, Stock = 85, ImageUrl = "/images/products/dc-poster-darkKnight.jpg", CreatedAt = now, UpdatedAt = now }
            };
            context.Products.AddRange(products);
            await context.SaveChangesAsync();

            // ========== 7. PRODUCT TRANSLATIONS ==========
            var productTranslations = new ProductTranslation[]
            {
                // Star Wars - Vader T-Shirt (product[0])
                new ProductTranslation { ProductId = products[0].Id, LanguageCode = "uk", Name = "Футболка 'Дарт Вейдер'", ShortDescription = "Класична чорна футболка з Дартом Вейдером.", FullDescription = "Високоякісна бавовняна футболка з принтом Дарта Вейдера. 100% бавовна, комфортний крій." },
                new ProductTranslation { ProductId = products[0].Id, LanguageCode = "en", Name = "Darth Vader T-Shirt", ShortDescription = "Classic black t-shirt featuring Darth Vader.", FullDescription = "High-quality cotton t-shirt with Darth Vader print. 100% cotton, comfortable fit." },
                
                // Star Wars - Jedi Hoodie (product[1])
                new ProductTranslation { ProductId = products[1].Id, LanguageCode = "uk", Name = "Худі 'Орден Джедаїв'", ShortDescription = "Тепле худі з емблемою Ордену Джедаїв.", FullDescription = "Затишне худі з капюшоном та вишитою емблемою Джедаїв. Ідеальне для холодної погоди." },
                new ProductTranslation { ProductId = products[1].Id, LanguageCode = "en", Name = "Jedi Order Hoodie", ShortDescription = "Warm hoodie with Jedi Order emblem.", FullDescription = "Cozy hoodie with hood and embroidered Jedi emblem. Perfect for cold weather." },
                
                // Star Wars - Yoda Figure (product[2])
                new ProductTranslation { ProductId = products[2].Id, LanguageCode = "uk", Name = "Фігурка 'Магістр Йода'", ShortDescription = "Деталізована фігурка Магістра Йоди.", FullDescription = "Колекційна фігурка Магістра Йоди висотою 15 см. Ручна робота, лімітована серія." },
                new ProductTranslation { ProductId = products[2].Id, LanguageCode = "en", Name = "Master Yoda Figure", ShortDescription = "Detailed Master Yoda figure.", FullDescription = "Collectible Master Yoda figure, 15 cm tall. Handcrafted, limited edition." },
                
                // Star Wars - Mandalorian Mug (product[3])
                new ProductTranslation { ProductId = products[3].Id, LanguageCode = "uk", Name = "Чашка 'Мандалорець'", ShortDescription = "Керамічна чашка з Мандалорцем.", FullDescription = "Керамічна чашка 350 мл з принтом Мандалорця та Грогу. Можна мити в посудомийці." },
                new ProductTranslation { ProductId = products[3].Id, LanguageCode = "en", Name = "Mandalorian Mug", ShortDescription = "Ceramic mug with Mandalorian.", FullDescription = "350ml ceramic mug with Mandalorian and Grogu print. Dishwasher safe." },
                
                // Cyberpunk - Samurai Hoodie (product[4])
                new ProductTranslation { ProductId = products[4].Id, LanguageCode = "uk", Name = "Худі 'Самурай'", ShortDescription = "Тепле худі з логотипом гурту Samurai.", FullDescription = "Стильне чорне худі з легендарним логотипом Samurai. Флісова підкладка для комфорту." },
                new ProductTranslation { ProductId = products[4].Id, LanguageCode = "en", Name = "Samurai Hoodie", ShortDescription = "Warm hoodie with Samurai band logo.", FullDescription = "Stylish black hoodie with legendary Samurai logo. Fleece lining for comfort." },
                
                // Cyberpunk - Johnny T-Shirt (product[5])
                new ProductTranslation { ProductId = products[5].Id, LanguageCode = "uk", Name = "Футболка 'Джонні Сільверхенд'", ShortDescription = "Футболка з портретом Джонні.", FullDescription = "Футболка з яскравим принтом Джонні Сільверхенда. Зручний крій, якісна тканина." },
                new ProductTranslation { ProductId = products[5].Id, LanguageCode = "en", Name = "Johnny Silverhand T-Shirt", ShortDescription = "T-shirt with Johnny's portrait.", FullDescription = "T-shirt with vibrant Johnny Silverhand print. Comfortable fit, quality fabric." },
                
                // Cyberpunk - V Figure (product[6])
                new ProductTranslation { ProductId = products[6].Id, LanguageCode = "uk", Name = "Фігурка 'V'", ShortDescription = "Деталізована фігурка головного героя V.", FullDescription = "Колекційна фігурка V з гри Cyberpunk 2077. Висота 20 см, рухомі частини." },
                new ProductTranslation { ProductId = products[6].Id, LanguageCode = "en", Name = "V Figure", ShortDescription = "Detailed figure of protagonist V.", FullDescription = "Collectible V figure from Cyberpunk 2077. 20 cm tall, articulated parts." },
                
                // Cyberpunk - Night City Poster (product[7])
                new ProductTranslation { ProductId = products[7].Id, LanguageCode = "uk", Name = "Постер 'Найт-Сіті'", ShortDescription = "Панорама нічного Найт-Сіті.", FullDescription = "Глянцевий постер з панорамою Найт-Сіті вночі. Розмір 60x90 см." },
                new ProductTranslation { ProductId = products[7].Id, LanguageCode = "en", Name = "Night City Poster", ShortDescription = "Night City panorama at night.", FullDescription = "Glossy poster with Night City panorama at night. Size 60x90 cm." },
                
                // Harry Potter - Hogwarts Hoodie (product[8])
                new ProductTranslation { ProductId = products[8].Id, LanguageCode = "uk", Name = "Худі 'Гоґвортс'", ShortDescription = "Худі з гербом Гоґвортсу.", FullDescription = "Затишне худі з вишитим гербом школи чарів Гоґвортс. Ідеальне для справжніх фанатів." },
                new ProductTranslation { ProductId = products[8].Id, LanguageCode = "en", Name = "Hogwarts Hoodie", ShortDescription = "Hoodie with Hogwarts crest.", FullDescription = "Cozy hoodie with embroidered Hogwarts School of Witchcraft crest. Perfect for true fans." },
                
                // Harry Potter - Gryffindor Mug (product[9])
                new ProductTranslation { ProductId = products[9].Id, LanguageCode = "uk", Name = "Чашка 'Ґрифіндор'", ShortDescription = "Чашка з гербом Ґрифіндору.", FullDescription = "Керамічна чашка 400 мл з гербом та кольорами факультету Ґрифіндор." },
                new ProductTranslation { ProductId = products[9].Id, LanguageCode = "en", Name = "Gryffindor Mug", ShortDescription = "Mug with Gryffindor crest.", FullDescription = "400ml ceramic mug with Gryffindor house crest and colors." },
                
                // Harry Potter - Elder Wand (product[10])
                new ProductTranslation { ProductId = products[10].Id, LanguageCode = "uk", Name = "Бузинова Паличка", ShortDescription = "Репліка легендарної Бузинової Палички.", FullDescription = "Детальна репліка Бузинової Палички з дерева. Довжина 38 см, подарункова коробка." },
                new ProductTranslation { ProductId = products[10].Id, LanguageCode = "en", Name = "Elder Wand", ShortDescription = "Replica of the legendary Elder Wand.", FullDescription = "Detailed wooden replica of the Elder Wand. Length 38 cm, gift box included." },
                
                // Harry Potter - Hedwig Figure (product[11])
                new ProductTranslation { ProductId = products[11].Id, LanguageCode = "uk", Name = "Фігурка 'Гедвіґа'", ShortDescription = "Фігурка снігової сови Гедвіґи.", FullDescription = "Колекційна фігурка Гедвіґи з реалістичним оперенням. Висота 12 см." },
                new ProductTranslation { ProductId = products[11].Id, LanguageCode = "en", Name = "Hedwig Figure", ShortDescription = "Snowy owl Hedwig figure.", FullDescription = "Collectible Hedwig figure with realistic feathers. Height 12 cm." },
                
                // The Witcher - Wolf T-Shirt (product[12])
                new ProductTranslation { ProductId = products[12].Id, LanguageCode = "uk", Name = "Футболка 'Школа Вовка'", ShortDescription = "Футболка з медальйоном Школи Вовка.", FullDescription = "Стильна футболка з принтом медальйона Школи Вовка. 100% бавовна." },
                new ProductTranslation { ProductId = products[12].Id, LanguageCode = "en", Name = "Wolf School T-Shirt", ShortDescription = "T-shirt with Wolf School medallion.", FullDescription = "Stylish t-shirt with Wolf School medallion print. 100% cotton." },
                
                // The Witcher - Geralt Figure (product[13])
                new ProductTranslation { ProductId = products[13].Id, LanguageCode = "uk", Name = "Фігурка 'Ґеральт з Рівії'", ShortDescription = "Деталізована фігурка Ґеральта.", FullDescription = "Преміум фігурка Ґеральта з Рівії з двома мечами. Висота 25 см, ручний розпис." },
                new ProductTranslation { ProductId = products[13].Id, LanguageCode = "en", Name = "Geralt of Rivia Figure", ShortDescription = "Detailed Geralt figure.", FullDescription = "Premium Geralt of Rivia figure with two swords. 25 cm tall, hand-painted." },
                
                // The Witcher - Wolf Medallion (product[14])
                new ProductTranslation { ProductId = products[14].Id, LanguageCode = "uk", Name = "Медальйон Відьмака", ShortDescription = "Металевий медальйон Школи Вовка.", FullDescription = "Високоякісна репліка медальйона Відьмака з ланцюжком. Метал, ручна робота." },
                new ProductTranslation { ProductId = products[14].Id, LanguageCode = "en", Name = "Witcher Medallion", ShortDescription = "Metal Wolf School medallion.", FullDescription = "High-quality Witcher medallion replica with chain. Metal, handcrafted." },
                
                // The Witcher - Wild Hunt Poster (product[15])
                new ProductTranslation { ProductId = products[15].Id, LanguageCode = "uk", Name = "Постер 'Дикий Гін'", ShortDescription = "Епічний постер з Диким Гоном.", FullDescription = "Атмосферний постер з вершниками Дикого Гону. Розмір 50x70 см." },
                new ProductTranslation { ProductId = products[15].Id, LanguageCode = "en", Name = "Wild Hunt Poster", ShortDescription = "Epic poster featuring the Wild Hunt.", FullDescription = "Atmospheric poster with Wild Hunt riders. Size 50x70 cm." },
                
                // Marvel - Avengers T-Shirt (product[16])
                new ProductTranslation { ProductId = products[16].Id, LanguageCode = "uk", Name = "Футболка 'Месники'", ShortDescription = "Футболка з логотипом Месників.", FullDescription = "Класична футболка з культовим логотипом Месників. Зручний крій для повсякденного носіння." },
                new ProductTranslation { ProductId = products[16].Id, LanguageCode = "en", Name = "Avengers T-Shirt", ShortDescription = "T-shirt with Avengers logo.", FullDescription = "Classic t-shirt with iconic Avengers logo. Comfortable fit for everyday wear." },
                
                // Marvel - Spider-Man Hoodie (product[17])
                new ProductTranslation { ProductId = products[17].Id, LanguageCode = "uk", Name = "Худі 'Людина-Павук'", ShortDescription = "Худі з дизайном костюма Людини-Павука.", FullDescription = "Яскраве худі з елементами костюма Людини-Павука. Капюшон та кишені." },
                new ProductTranslation { ProductId = products[17].Id, LanguageCode = "en", Name = "Spider-Man Hoodie", ShortDescription = "Hoodie with Spider-Man suit design.", FullDescription = "Vibrant hoodie with Spider-Man suit elements. Hood and pockets included." },
                
                // Marvel - Iron Man Figure (product[18])
                new ProductTranslation { ProductId = products[18].Id, LanguageCode = "uk", Name = "Фігурка 'Залізна Людина'", ShortDescription = "Фігурка Тоні Старка в броні Mark 85.", FullDescription = "Деталізована фігурка Залізної Людини в броні Mark 85. LED-підсвітка, рухомі частини." },
                new ProductTranslation { ProductId = products[18].Id, LanguageCode = "en", Name = "Iron Man Figure", ShortDescription = "Tony Stark figure in Mark 85 armor.", FullDescription = "Detailed Iron Man figure in Mark 85 armor. LED lighting, articulated parts." },
                
                // Marvel - Captain America Shield (product[19])
                new ProductTranslation { ProductId = products[19].Id, LanguageCode = "uk", Name = "Щит Капітана Америки", ShortDescription = "Міні-репліка щита Капітана Америки.", FullDescription = "Металева міні-репліка легендарного щита. Діаметр 15 см, настільна підставка." },
                new ProductTranslation { ProductId = products[19].Id, LanguageCode = "en", Name = "Captain America Shield", ShortDescription = "Mini replica of Captain America's shield.", FullDescription = "Metal mini replica of the legendary shield. 15 cm diameter, desk stand included." },
                
                // DC - Batman T-Shirt (product[20])
                new ProductTranslation { ProductId = products[20].Id, LanguageCode = "uk", Name = "Футболка 'Бетмен'", ShortDescription = "Футболка з класичним логотипом Бетмена.", FullDescription = "Чорна футболка з жовтим логотипом Бетмена. Класичний дизайн для фанатів." },
                new ProductTranslation { ProductId = products[20].Id, LanguageCode = "en", Name = "Batman T-Shirt", ShortDescription = "T-shirt with classic Batman logo.", FullDescription = "Black t-shirt with yellow Batman logo. Classic design for fans." },
                
                // DC - Joker Hoodie (product[21])
                new ProductTranslation { ProductId = products[21].Id, LanguageCode = "uk", Name = "Худі 'Джокер'", ShortDescription = "Худі з культовим образом Джокера.", FullDescription = "Стильне фіолетове худі з принтом Джокера. Унікальний дизайн, флісова підкладка." },
                new ProductTranslation { ProductId = products[21].Id, LanguageCode = "en", Name = "Joker Hoodie", ShortDescription = "Hoodie with iconic Joker image.", FullDescription = "Stylish purple hoodie with Joker print. Unique design, fleece lining." },
                
                // DC - Batman Figure (product[22])
                new ProductTranslation { ProductId = products[22].Id, LanguageCode = "uk", Name = "Фігурка 'Бетмен'", ShortDescription = "Преміум фігурка Темного Лицаря.", FullDescription = "Колекційна фігурка Бетмена з плащем. Висота 22 см, рухомі частини, підставка." },
                new ProductTranslation { ProductId = products[22].Id, LanguageCode = "en", Name = "Batman Figure", ShortDescription = "Premium Dark Knight figure.", FullDescription = "Collectible Batman figure with cape. 22 cm tall, articulated parts, stand included." },
                
                // DC - Dark Knight Poster (product[23])
                new ProductTranslation { ProductId = products[23].Id, LanguageCode = "uk", Name = "Постер 'Темний Лицар'", ShortDescription = "Атмосферний постер з Бетменом.", FullDescription = "Постер з Бетменом на фоні нічного Ґотема. Розмір 50x70 см, матовий друк." },
                new ProductTranslation { ProductId = products[23].Id, LanguageCode = "en", Name = "Dark Knight Poster", ShortDescription = "Atmospheric poster with Batman.", FullDescription = "Poster with Batman against night Gotham. Size 50x70 cm, matte print." }
            };
            context.ProductTranslations.AddRange(productTranslations);
            await context.SaveChangesAsync();

            // ========== 8. PRODUCT SIZES (for clothing items) ==========
            var sizes = new[] { "XS", "S", "M", "L", "XL", "XXL" };
            var clothingProducts = products.Where(p => p.CategoryId == clothingId).ToList();
            var productSizes = new List<ProductSize>();

            foreach (var product in clothingProducts)
            {
                var stockPerSize = product.Stock / sizes.Length;
                foreach (var size in sizes)
                {
                    productSizes.Add(new ProductSize
                    {
                        ProductId = product.Id,
                        Size = size,
                        Stock = stockPerSize
                    });
                }
            }

            context.ProductSizes.AddRange(productSizes);
            await context.SaveChangesAsync();
        }
    }
}