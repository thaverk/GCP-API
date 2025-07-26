<!-- Use this file to provide workspace-specific custom instructions to Copilot. For more details, visit https://code.visualstudio.com/docs/copilot/copilot-customization#_use-a-githubcopilotinstructionsmd-file -->

# PhasePlay Flutter App

This workspace contains a Flutter application for workout and fitness tracking, converted from an ASP.NET MVC web application.

## Project Context

- This is a mobile fitness application built with Flutter
- The app allows users to track workouts, exercises, and fitness progress
- It uses Provider for state management
- Navigation is handled through Flutter's routing system
- Mock data is used for demonstration purposes, but designed to work with a REST API

## Code Conventions

- Use camelCase for variables and methods
- Use PascalCase for classes and types
- Group related functionality in separate files
- Use named parameters for constructors and methods with multiple parameters
- Prefer const constructors when possible
- Use async/await for asynchronous operations
- Use the Provider pattern for state management

## Architecture

- models/ - Data structures and entities
- screens/ - UI pages and navigation
- services/ - Business logic and API communication
- widgets/ - Reusable UI components

## Flutter-Specific Guidelines

- Use StatelessWidget for presentation-only components
- Use StatefulWidget when the component needs to manage its own state
- Prefer composition over inheritance
- Keep build methods small and focused
- Extract complex UI into separate widget methods or classes
- Use MediaQuery to make layouts responsive
- Apply the material design guidelines
