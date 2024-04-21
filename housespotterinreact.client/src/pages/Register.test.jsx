import React from "react";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import "@testing-library/jest-dom";
import Register from "./Register"; // Adjust the import path as necessary
import { BrowserRouter } from "react-router-dom";
import fetchMock from "jest-fetch-mock";

fetchMock.enableMocks();

// Mocking useNavigate
jest.mock("react-router-dom", () => ({
  ...jest.requireActual("react-router-dom"), // preserve non-hook exports
  useNavigate: () => jest.fn(), // mock navigate function
}));

// Mocking both AuthContext and NotificationContext
jest.mock("../contexts/AuthContext", () => ({
  useAuth: () => ({
    user: {}, // Provide a default mock user object or the required properties
    logout: jest.fn(),
    // Add any other function or value that's used within your components
  }),
}));

jest.mock("../contexts/NotificationContext", () => ({
  useNotification: () => ({
    notify: jest.fn(),
  }),
}));

describe("Register Component", () => {
  beforeEach(() => {
    fetchMock.resetMocks();
  });

  it("allows the user to register with valid information", async () => {
    fetchMock.mockResponseOnce(JSON.stringify({ success: true }));

    render(
      <BrowserRouter>
        <Register />
      </BrowserRouter>
    );

    // Fill out the form
    fireEvent.change(
      screen.getByPlaceholderText("Įveskite savo vartotojo vardą"),
      { target: { value: "NewUser" } }
    );
    fireEvent.change(screen.getByPlaceholderText("Įveskite savo el. paštą"), {
      target: { value: "user@example.com" },
    });
    fireEvent.change(screen.getByPlaceholderText("Įveskite savo tel. nr."), {
      target: { value: "123456789" },
    });
    fireEvent.change(screen.getByPlaceholderText("Įveskite savo slaptažodį"), {
      target: { value: "password" },
    });

    fireEvent.click(screen.getByRole("button", { name: /Registruotis/i }));

    await waitFor(() => {
      expect(fetchMock).toHaveBeenCalledWith(
        "housespotter/db/user/register",
        expect.objectContaining({
          method: "POST",
          headers: expect.any(Object),
          body: JSON.stringify({
            email: "user@example.com",
            phoneNumber: "123456789",
            username: "NewUser",
            password: "password",
            isAdmin: false,
          }),
        })
      );
    });
  });
});
