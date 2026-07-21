using MudBlazor;

namespace CarRental.Web;

/// <summary>
/// Single source of truth for the app's visual identity.
/// Every MudBlazor component that uses Color.Primary / Color.Secondary etc.
/// picks these up automatically — no !important overrides needed.
/// </summary>
public static class CarRentalTheme
{
    public static readonly MudTheme Theme = new()
    {
        PaletteLight = new PaletteLight
        {
            // Structure & identity — deep navy slate
            Primary = "#1E293B",
            PrimaryContrastText = "#FFFFFF",

            // Action / CTA — amber ("Reserve", prices)
            Secondary = "#F59E0B",
            SecondaryContrastText = "#221A05",

            Tertiary = "#0EA5E9",

            Background = "#F8FAFC",
            Surface = "#FFFFFF",

            AppbarBackground = "#1E293B",
            AppbarText = "#F8FAFC",

            TextPrimary = "#0F172A",
            TextSecondary = "#64748B",
            ActionDefault = "#64748B",

            LinesDefault = "#E2E8F0",
            TableLines = "#E2E8F0",
            Divider = "#E2E8F0",

            Success = "#16A34A",
            Error = "#DC2626",
            Warning = "#D97706",
            Info = "#0284C7",

            HoverOpacity = 0.06
        },

        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "10px"
        },

        // NOTE: property shapes below target MudBlazor v7/v8.
        // If you're on MudBlazor 6.x: use `Default = new Default { ... }`
        // and integer FontWeight values (600 instead of "600").
        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = ["Roboto", "Segoe UI", "Helvetica", "sans-serif"]
            },
            H4 = new H4Typography
            {
                FontWeight = "700",
                LetterSpacing = "-0.02em"
            },
            H5 = new H5Typography { FontWeight = "650" },
            H6 = new H6Typography { FontWeight = "600" },
            Subtitle1 = new Subtitle1Typography { FontWeight = "600" },
            Button = new ButtonTypography
            {
                FontWeight = "600",
                TextTransform = "none"
            }
        }
    };
}