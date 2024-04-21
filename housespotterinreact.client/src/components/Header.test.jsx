import React from "react";
import { render, screen, fireEvent } from "@testing-library/react";
import "@testing-library/jest-dom";
import Header from "./Header";
import { BrowserRouter } from "react-router-dom";

// Mocks for useAuth and useNotification hooks
jest.mock("../contexts/AuthContext", () => ({
  useAuth: jest.fn(),
}));

jest.mock("../contexts/NotificationContext", () => ({
  useNotification: jest.fn(),
}));

// Mock for useNavigate
jest.mock("react-router-dom", () => ({
  ...jest.requireActual("react-router-dom"), // Use actual for all non-hook parts
  useNavigate: jest.fn(), // Mock useNavigate
}));

describe("Header Component", () => {
  // Setup common mocks
  const mockLogout = jest.fn();
  const mockNotify = jest.fn();
  const mockNavigate = jest.fn();

  beforeEach(() => {
    // Reset mocks before each test
    jest.clearAllMocks();

    // Setup mock implementations
    require("../contexts/AuthContext").useAuth.mockReturnValue({
      user: null, // or { isAdmin: true } for logged in state
      logout: mockLogout,
    });

    require("../contexts/NotificationContext").useNotification.mockReturnValue({
      notify: mockNotify,
    });

    require("react-router-dom").useNavigate.mockReturnValue(mockNavigate);
  });
  it("displays the login button when user is not logged in", () => {
    require("../contexts/AuthContext").useAuth.mockReturnValue({ user: null });
    render(
      <BrowserRouter>
        <Header />
      </BrowserRouter>
    );
    expect(screen.getByText("Prisijungti")).toBeInTheDocument();
  });

  it("displays user-specific links when user is logged in", () => {
    require("../contexts/AuthContext").useAuth.mockReturnValue({
      user: { isAdmin: true },
    });
    render(
      <BrowserRouter>
        <Header />
      </BrowserRouter>
    );
    expect(screen.getByText("Mėgstamiausi")).toBeInTheDocument();
    expect(screen.getByText("Scraper")).toBeInTheDocument();
  });

  it("calls logout and redirects to home on logout button click", () => {
    const mockLogout = jest.fn();
    const mockNotify = jest.fn();
    const mockNavigate = jest.fn();

    // Provide mock implementations for useAuth and useNotification
    require("../contexts/AuthContext").useAuth.mockReturnValue({
      user: { isAdmin: true },
      logout: mockLogout,
    });

    require("../contexts/NotificationContext").useNotification.mockReturnValue({
      notify: mockNotify,
    });

    // Set mock implementation for useNavigate
    require("react-router-dom").useNavigate.mockReturnValue(mockNavigate);

    render(
      <BrowserRouter>
        <Header />
      </BrowserRouter>
    );

    fireEvent.click(screen.getByText("Atsijungti"));

    expect(mockLogout).toHaveBeenCalled();
    expect(mockNotify).toHaveBeenCalledWith(
      "Sėkmingai atsijungėte!",
      "success"
    );
    expect(mockNavigate).toHaveBeenCalledWith("/");
  });
});
