using System.Reflection;

namespace HouseSpotter.Unit.Fixtures
{
    public class StaticFixtures
    {
        public string SkelbiuPositiveHtml;
        public string AruodasPositiveHtml;
        public string DomoPositiveHtml;

        public StaticFixtures()
        {
            var outputDirectory = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "../../../"));
            SkelbiuPositiveHtml = File.ReadAllText(Path.Combine(outputDirectory, "Utils", "Htmls", "SkelbiuPositiveHtml.txt"));
            AruodasPositiveHtml = File.ReadAllText(Path.Combine(outputDirectory, "Utils", "Htmls", "AruodasPositiveHtml.txt"));
            var filePath = Path.Combine(outputDirectory, "Utils", "Htmls", "SkelbiuPositiveHtml.txt");
            SkelbiuPositiveHtml = File.ReadAllText(filePath);
            DomoPositiveHtml = File.ReadAllText(Path.Combine(outputDirectory, "Utils", "Htmls", "DomoPositiveHtml.txt"));
        }
    }
}