import React from "react";
import { render, screen, fireEvent } from "@testing-library/react";
import "@testing-library/jest-dom";
import Notification from "./Notification";
import * as NotificationContext from "../contexts/NotificationContext";

jest.mock("../contexts/NotificationContext", () => ({
  useNotification: jest.fn(),
}));

describe("Notification Component", () => {
  it("renders a success notification", () => {
    require("../contexts/NotificationContext").useNotification.mockReturnValue({
      notification: { message: "Success", type: "success" },
    });
    render(<Notification />);
    expect(screen.getByText("Success")).toBeInTheDocument();
    // Adjusted to check the presence of class in the correct element
    expect(screen.getByText("Success").closest("div.notification")).toHaveClass(
      "notification"
    );
  });

  it("renders an error notification", () => {
    require("../contexts/NotificationContext").useNotification.mockReturnValue({
      notification: { message: "Error", type: "error" },
    });
    render(<Notification />);
    expect(screen.getByText("Error")).toBeInTheDocument();
    // Adjusted to check the presence of class in the correct element
    expect(screen.getByText("Error").closest("div.notification")).toHaveClass(
      "notification",
      "error"
    );
  });
  it("clears the notification on close button click", () => {
    const mockClearNotification = jest.fn();
    require("../contexts/NotificationContext").useNotification.mockReturnValue({
      notification: { message: "Success", type: "success" },
      clearNotification: mockClearNotification,
    });

    render(<Notification />);
    expect(screen.getByText("Success")).toBeInTheDocument();

    // Make sure your Notification component's close button has a `data-testid="close-button"`
    fireEvent.click(screen.getByTestId("close-button"));

    // Assert that the clearNotification method was called
    expect(mockClearNotification).toHaveBeenCalled();
  });
  it("does not render notification if message is empty", () => {
    // Mock the useNotification hook right inside your test
    jest.spyOn(NotificationContext, "useNotification").mockReturnValue({
      notification: {},
      clearNotification: jest.fn(),
    });

    const { container } = render(<Notification />);
    expect(container).toBeEmptyDOMElement();
  });
});
