# PhasePlay Flutter App

A Flutter application for workout and fitness tracking, converted from an ASP.NET MVC web application.

## Features

- **User Authentication**: Login and registration with secure authentication
- **Workout Management**: Create, view, and manage workouts
- **Exercise Library**: Browse and search through exercise database
- **Phase-based Training**: Organize workouts into training phases
- **Progress Tracking**: Monitor your fitness journey with stats and metrics

## Project Structure

```
phase_play/
├── assets/                  # Images, fonts, and other static files
├── lib/
│   ├── main.dart            # Application entry point
│   ├── models/              # Data models
│   │   ├── exercise.dart    # Exercise model
│   │   ├── user.dart        # User model
│   │   └── workout.dart     # Workout model
│   ├── screens/             # UI screens
│   │   ├── exercise_screen.dart    # Exercise browsing screen
│   │   ├── home_screen.dart        # Main dashboard
│   │   ├── profile_screen.dart     # User profile
│   │   └── workout_list_screen.dart # Workout management
│   ├── services/            # Business logic and API communication
│   │   ├── auth_service.dart # Authentication service
│   │   └── workout_service.dart # Workout management service
│   └── widgets/             # Reusable UI components
└── pubspec.yaml             # Project dependencies
```

## Getting Started

### Prerequisites

- Flutter SDK (latest version)
- Android Studio or VS Code with Flutter extensions
- An emulator or physical device for testing

### Installation

1. Clone the repository
2. Install dependencies:
   ```
   flutter pub get
   ```
3. Run the app:
   ```
   flutter run
   ```

## Authentication

For demo purposes, the app uses the following credentials:
- Username: `user`
- Password: `password`

## Backend Integration

This app is designed to work with an ASP.NET Core backend API. For demonstration purposes, it currently uses mock data, but can be easily connected to your existing ASP.NET Core API by updating the service classes.

## Converting from ASP.NET MVC

This Flutter app was converted from an ASP.NET MVC web application. The conversion process involved:

1. Recreating the data models in Dart
2. Implementing equivalent business logic in Flutter services
3. Designing a mobile-first UI with Flutter widgets
4. Setting up navigation and state management

## Dependencies

- flutter: The core Flutter SDK
- http: For API communication
- provider: For state management
- shared_preferences: For local storage
- intl: For date formatting and localization

## Future Enhancements

- Workout timer functionality
- Exercise video demonstrations
- Social sharing features
- Offline support
- Advanced analytics and progress charts
