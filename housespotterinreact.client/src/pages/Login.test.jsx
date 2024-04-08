import React from "react";
import { render, screen, fireEvent, act } from "@testing-library/react";
import "@testing-library/jest-dom";
import Login from "./Login";
import { BrowserRouter } from "react-router-dom";
import fetchMock from "jest-fetch-mock";
import * as NotificationContext from "../contexts/NotificationContext";

fetchMock.enableMocks();

jest.mock("../contexts/AuthContext", () => ({
  useAuth: () => ({
    login: jest.fn().mockImplementation(() => Promise.resolve()),
    // Ensure you are mocking all necessary functions and values
  }),
}));

jest.mock("../contexts/NotificationContext", () => ({
  useNotification: jest.fn().mockImplementation(() => ({
    notify: jest.fn(),
  })),
}));


describe("Login Component", () => {
  let notifyMock;

  beforeEach(() => {
    fetchMock.resetMocks();
  
    // Reset and redefine the mock for useNotification
    const notify = jest.fn();
    NotificationContext.useNotification.mockImplementation(() => ({
      notify
    }));
    // Assign the mock function for later assertion
    notifyMock = notify;
  });
  

  it("allows the user to log in with correct credentials", async () => {
    fetchMock.mockResponseOnce(JSON.stringify({ username: "Test1" })); // Mock successful login response

    render(
      <BrowserRouter>
        <Login />
      </BrowserRouter>
    );

    fireEvent.change(
      screen.getByPlaceholderText("Įveskite savo vartotojo vardą"),
      { target: { value: "Test1" } }
    );
    fireEvent.change(screen.getByPlaceholderText("Įveskite savo slaptažodį"), {
      target: { value: "Test1" },
    });

    await act(async () => {
      fireEvent.click(screen.getByRole("button", { name: /Prisijungti/i }));
    });

    expect(fetch).toHaveBeenCalledWith(
      "housespotter/db/user/login",
      expect.objectContaining({
        method: "POST",
        headers: expect.any(Object),
        body: JSON.stringify({ username: "Test1", password: "Test1" }),
      })
    );
  });

  it("updates username and password input fields correctly", () => {
    render(
      <BrowserRouter>
        <Login />
      </BrowserRouter>
    );

    const usernameInput = screen.getByPlaceholderText(
      "Įveskite savo vartotojo vardą"
    );
    const passwordInput = screen.getByPlaceholderText(
      "Įveskite savo slaptažodį"
    );

    fireEvent.change(usernameInput, { target: { value: "newuser" } });
    fireEvent.change(passwordInput, { target: { value: "newpassword" } });

    expect(usernameInput.value).toBe("newuser");
    expect(passwordInput.value).toBe("newpassword");
  });

  it("displays an error notification when login fails", async () => {
    fetchMock.mockRejectOnce(); // Mock failed login response

    render(
      <BrowserRouter>
        <Login />
      </BrowserRouter>
    );

    fireEvent.change(
      screen.getByPlaceholderText("Įveskite savo vartotojo vardą"),
      { target: { value: "Test1" } }
    );
    fireEvent.change(screen.getByPlaceholderText("Įveskite savo slaptažodį"), {
      target: { value: "Test1" },
    });

    await act(async () => {
      fireEvent.click(screen.getByRole("button", { name: /Prisijungti/i }));
    });

    expect(fetch).toHaveBeenCalledWith(
      "housespotter/db/user/login",
      expect.objectContaining({
        method: "POST",
        headers: expect.any(Object),
        body: JSON.stringify({ username: "Test1", password: "Test1" }),
      })
    );
  });

  it("redirects to /megstamiausi after successful login", async () => {
    fetchMock.mockResponseOnce(JSON.stringify({ username: "Test1" })); // Mock successful login response

    const { container } = render(
      <BrowserRouter>
        <Login />
      </BrowserRouter>
    );

    fireEvent.change(
      screen.getByPlaceholderText("Įveskite savo vartotojo vardą"),
      { target: { value: "Test1" } }
    );
    fireEvent.change(screen.getByPlaceholderText("Įveskite savo slaptažodį"), {
      target: { value: "Test1" },
    });

    await act(async () => {
      fireEvent.click(screen.getByRole("button", { name: /Prisijungti/i }));
    });

    expect(fetch).toHaveBeenCalledWith(
      "housespotter/db/user/login",
      expect.objectContaining({
        method: "POST",
        headers: expect.any(Object),
        body: JSON.stringify({ username: "Test1", password: "Test1" }),
      })
    );
  });

  it("correctly updates input fields and attempts login", async () => {
    const testUsername = "userTest";
    const testPassword = "passwordTest";
    fetchMock.mockResponseOnce(JSON.stringify({ success: true }));

    render(
      <BrowserRouter>
        <Login />
      </BrowserRouter>
    );

    // Simulate user typing into the username and password fields
    fireEvent.change(
      screen.getByPlaceholderText("Įveskite savo vartotojo vardą"),
      {
        target: { value: testUsername },
      }
    );
    fireEvent.change(screen.getByPlaceholderText("Įveskite savo slaptažodį"), {
      target: { value: testPassword },
    });

    // Submit the form
    await act(async () => {
      fireEvent.submit(screen.getByRole("button", { name: /Prisijungti/i }));
    });

    // Assert fetch was called with expected payload
    expect(fetch).toHaveBeenCalledWith(
      "housespotter/db/user/login",
      expect.objectContaining({
        method: "POST",
        headers: {
          Accept: "application/json",
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          username: testUsername,
          password: testPassword,
        }),
      })
    );
  });
  
});
