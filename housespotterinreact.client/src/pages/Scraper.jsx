import Header from "../components/Header";
import ScraperForm from "../components/ScraperForm";

function Scraper() {
  return (
    <div className="scraper">
      <Header />
      <div className="layout">
        <h1>Scraper</h1>
        <ScraperForm
          title={"Domoplius"}
          allUrl={"/housespotter/scrapers/domo/findhousing/all"}
          recentUrl={"/housespotter/scrapers/domo/findhousing/recent"}
          enrichUrl={"/housespotter/scrapers/domo/enrichhousing"}
        />
        <ScraperForm
          title={"Aruodas"}
          allUrl={"/housespotter/scrapers/aruodas/findhousing/all"}
          recentUrl={"/housespotter/scrapers/aruodas/findhousing/recent"}
          enrichUrl={"/housespotter/scrapers/aruodas/enrichhousing"}
        />
        <ScraperForm
          title={"Skelbiu"}
          allUrl={"/housespotter/scrapers/skelbiu/findhousing/all"}
          recentUrl={"/housespotter/scrapers/skelbiu/findhousing/recent"}
          enrichUrl={"/housespotter/scrapers/skelbiu/enrichhousing"}
        />
      </div>
    </div>
  );
}

export default Scraper;
