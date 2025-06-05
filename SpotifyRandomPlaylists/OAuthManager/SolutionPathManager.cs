namespace OAuthManager;

public static class SolutionPathManager
{
    public static string GetSolutionPath()
    {
        string currentDirectory = AppContext.BaseDirectory;
        // On monte dans l'arborescence jusqu'à trouver un dossier qui contient un fichier .sln
        // Ou un marqueur connu de la racine de la solution (ex: un dossier "SharedData")
        while (!string.IsNullOrEmpty(currentDirectory))
        {
            if (Directory.GetFiles(currentDirectory, "*.sln").Any() ||
                Directory.Exists(Path.Combine(currentDirectory, "SharedData")))
            {
                return currentDirectory;
            }
            DirectoryInfo parent = Directory.GetParent(currentDirectory);
            if (parent == null) break; // Sortir si plus de parent
            currentDirectory = parent.FullName;
        }
        // Fallback si la racine de la solution n'est pas trouvée (moins robuste)
        return Path.Combine(AppContext.BaseDirectory, "../../../../");
    }
}