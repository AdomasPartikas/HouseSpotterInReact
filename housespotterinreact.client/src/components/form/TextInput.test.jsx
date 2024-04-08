import React from 'react';
import { render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import TextInput from './TextInput';
import fetchMock from 'jest-fetch-mock';

fetchMock.enableMocks();

describe('TextInput Component', () => {
  it('renders a single input', () => {
    render(
      <TextInput 
        label="Username" 
        name="username" 
        placeholder="Enter your username" 
        onChange={() => {}} 
      />
    );
    expect(screen.getByPlaceholderText('Enter your username')).toBeInTheDocument();
  });

  it('renders a double input', () => {
    render(
      <TextInput 
        label="Range" 
        nameFrom="start" 
        nameTo="end" 
        placeholderFrom="Start" 
        placeholderTo="End" 
        onChange={() => {}} 
      />
    );
    expect(screen.getByPlaceholderText('Start')).toBeInTheDocument();
    expect(screen.getByPlaceholderText('End')).toBeInTheDocument();
  });
});

