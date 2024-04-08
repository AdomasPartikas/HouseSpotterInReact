using System.Reflection;

namespace HouseSpotter.Unit.Fixtures
{
    public class StaticFixtures
    {
        public string SkelbiuPositiveHtml;

        public StaticFixtures()
        {
            var outputDirectory = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "../../../"));
            var filePath = Path.Combine(outputDirectory, "Utils", "Htmls", "SkelbiuPositiveHtml.txt");
            SkelbiuPositiveHtml = File.ReadAllText(filePath);
        }
    }
}