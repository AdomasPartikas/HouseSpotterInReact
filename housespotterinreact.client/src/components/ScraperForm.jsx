import React from "react";
import { useNotification } from "../contexts/NotificationContext";

const ScraperForm = ({ title, allUrl, recentUrl, enrichUrl }) => {
  const { notify } = useNotification();

  const handleRequest = async (url) => {
    try {
      const response = await fetch(url, {
        method: "POST",
        headers: {
          Accept: "application/json",
          "Content-Type": "application/json",
        },
      });

      if (response.ok) {
        const data = await response.json();
        console.log(data);
        if (data.scrapeStatus == "Success") {
          notify(`Operacija pavyko: ${data.message}`, "success");
        } else {
          notify(`Operacija nepavyko: ${data.message}`, "error");
        }
      } else {
        notify(`Operacija nepavyko.`, "error");
      }
    } catch (error) {
      notify(`Operacija nepavyko.`, "error");
    }
  };

  return (
    <div className="scraper-form">
      <h2>{title}</h2>
      <button type="button" onClick={() => handleRequest(allUrl)}>
        Find all housing
      </button>
      <button type="button" onClick={() => handleRequest(recentUrl)}>
        Find recent housing
      </button>
      <button type="button" onClick={() => handleRequest(enrichUrl)}>
        Enrich housing
      </button>
    </div>
  );
};

export default ScraperForm;
