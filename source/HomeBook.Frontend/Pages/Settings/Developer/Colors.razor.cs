using Microsoft.AspNetCore.Components;

namespace HomeBook.Frontend.Pages.Settings.Developer;

public partial class Colors : ComponentBase
{
    private readonly List<string> _brandColors =
    [
        // modern colors
        "brand-github", "brand-docker", "brand-ubuntu",
    ];

    private readonly List<string> _availableColors =
    [
        // modern colors
        "amethyst", "cerulean", "crimson", "ruby",
        "emerald", "amber", "teal", "petrol", "azure", "lavender",


        // demo colors
        // warme natürliche Töne
        "sand", "beige", "linen", "amber1", "honey", "caramel", "clay", "terracotta", "copper", "bronze",

        // erdige / organische Töne
        "moss", "olive", "sage", "avocado", "forest", "basil", "fern", "khaki", "stone", "graphite",

        // frische und aquatische Töne
        "sea", "ocean", "aqua", "turquoise", "mint", "sky", "iceblue", "denim",

        // elegante dunkle und neutrale Töne
        "charcoal", "slate", "storm", "ash", "smoke", "shadow", "steel", "pebble", "fog", "cloud",

        // moderne Akzentfarben
        "plum", "rose", "coral", "peach", "apricot", "sunset", "wine", "mulberry", "magenta"
    ];
}
