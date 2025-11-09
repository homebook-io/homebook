using HomeBook.Frontend.Module.Kitchen.Contracts;
using HomeBook.Frontend.Module.Kitchen.Models;

namespace HomeBook.Frontend.Module.Kitchen.Services;

/// <inheritdoc/>
public class MealService : IMealService
{
    /// <inheritdoc/>
    public async Task<IEnumerable<Meal>> GetMealsAsync(string filter,
        CancellationToken cancellationToken = default)
    {
        // wait for 5 seconds with cancellation support
        await Task.Delay(5000, cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        var random = new Random();
        var meals = new List<Meal>();

        meals.AddRange([
            new Meal($"Spaghetti Bolognese {random.Next(1, 100)}",
                "Spaghetti, Hackfleisch, Tomatensauce, Zwiebeln, Knoblauch",
                TimeSpan.FromMinutes(45),
                random.Next(600, 900)),
            new Meal($"Caesar Salad {random.Next(1, 100)}",
                "Römersalat, Hähnchenbrust, Croutons, Caesar-Dressing, Parmesan",
                TimeSpan.FromMinutes(20),
                random.Next(600, 900)),
            new Meal($"Vegetarische Pizza {random.Next(1, 100)}",
                "Pizzateig, Tomatensauce, Mozzarella, Paprika, Zucchini, Oliven",
                TimeSpan.FromMinutes(30),
                random.Next(600, 900)),
            new Meal($"Linsensuppe {random.Next(1, 100)}",
                "Linsen, Karotten, Sellerie, Zwiebeln, Gemüsebrühe",
                TimeSpan.FromMinutes(40),
                random.Next(600, 900)),
            new Meal($"Grilled Cheese Sandwich {random.Next(1, 100)}",
                "Brot, Cheddar-Käse, Butter",
                TimeSpan.FromMinutes(10),
                random.Next(600, 900)),
            new Meal($"Chicken Curry {random.Next(1, 100)}",
                "Hähnchenbrust, Curry-Gewürz, Kokosmilch, Reis",
                TimeSpan.FromMinutes(50),
                random.Next(600, 900)),
            new Meal($"Beef Tacos {random.Next(1, 100)}",
                "Taco-Schalen, Hackfleisch, Salat, Tomaten, Käse, Salsa",
                TimeSpan.FromMinutes(25),
                random.Next(600, 900)),
            new Meal($"Pancakes {random.Next(1, 100)}",
                "Mehl, Eier, Milch, Zucker, Butter, Ahornsirup",
                TimeSpan.FromMinutes(15),
                random.Next(600, 900)),
            new Meal($"Sushi Rolls {random.Next(1, 100)}",
                "Sushi-Reis, Nori-Blätter, Lachs, Gurke, Avocado, Sojasauce",
                TimeSpan.FromMinutes(60),
                random.Next(600, 900)),
            new Meal($"Quinoa Bowl {random.Next(1, 100)}",
                "Quinoa, Kichererbsen, Avocado, Tomaten, Gurken, Zitronendressing",
                TimeSpan.FromMinutes(35),
                random.Next(600, 900))
        ]);

        return meals
            .Where(m => m.Name.Contains(filter, StringComparison.OrdinalIgnoreCase));
    }
}
