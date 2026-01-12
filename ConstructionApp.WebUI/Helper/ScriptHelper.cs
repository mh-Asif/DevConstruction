namespace ConstructionApp.WebUI.Helper
{
    public static class ScriptHelper
    {
        public static List<string> GetScriptsForPage(string controller, string action)
        {
            var scripts = new List<string> { "~/assets/vendor/js/menu.js",
            "https://code.jquery.com/jquery-3.7.1.min.js",
            "~/assets/js/Common.js",
            "https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js",
            "~/assets/js/main.js"
            }; // Always include core

            if (controller == "Home" && action == "Dashboard")
            {
                scripts.Add("https://cdn.jsdelivr.net/npm/chart.js");
                scripts.Add("https://cdn.jsdelivr.net/npm/chart.js");
            }
            else if (controller == "Home" && action == "Contact")
            {
                scripts.Add("/js/contact-form.js");
            }

            return scripts;
        }
    }
}
