import 'package:flutter/material.dart';
import 'package:phase_play/screens/home_screen.dart';
import 'package:phase_play/screens/workout_list_screen.dart';
import 'package:phase_play/screens/exercise_screen.dart';
import 'package:phase_play/screens/profile_screen.dart';
import 'package:provider/provider.dart';
import 'package:phase_play/services/workout_service.dart';

void main() {
  runApp(
    MultiProvider(
      providers: [
        ChangeNotifierProvider(create: (_) => WorkoutService()),
      ],
      child: const MyApp(),
    ),
  );
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'PhasePlay',
      theme: ThemeData(
        primarySwatch: Colors.blue,
        visualDensity: VisualDensity.adaptivePlatformDensity,
        useMaterial3: true,
      ),
      initialRoute: '/',
      routes: {
        '/': (context) => const HomeScreen(),
        '/workouts': (context) => const WorkoutListScreen(),
        '/exercises': (context) => const ExerciseScreen(),
        '/profile': (context) => const ProfileScreen(),
      },
    );
  }
}
