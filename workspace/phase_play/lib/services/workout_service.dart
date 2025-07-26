import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:phase_play/models/workout.dart';
import 'package:phase_play/models/exercise.dart';

class WorkoutService extends ChangeNotifier {
  final String baseUrl = 'https://api.phaseplay.com';
  List<Workout> _workouts = [];
  bool _isLoading = false;
  String _error = '';

  List<Workout> get workouts => _workouts;
  bool get isLoading => _isLoading;
  String get error => _error;

  // In a real app, you'd connect to your ASP.NET Core API
  // For now, we'll simulate with mock data
  Future<void> fetchWorkouts() async {
    _isLoading = true;
    _error = '';
    notifyListeners();

    try {
      // Simulate network request to ASP.NET Core backend
      await Future.delayed(const Duration(seconds: 1));
      
      // Mock data - in a real app, you'd use:
      // final response = await http.get(Uri.parse('$baseUrl/api/workouts'));
      // if (response.statusCode == 200) {
      //   final data = json.decode(response.body) as List;
      //   _workouts = data.map((json) => Workout.fromJson(json)).toList();
      // } else {
      //   _error = 'Failed to load workouts';
      // }
      
      _workouts = _getMockWorkouts();
      _isLoading = false;
      notifyListeners();
    } catch (e) {
      _error = e.toString();
      _isLoading = false;
      notifyListeners();
    }
  }

  Future<Workout?> getWorkoutById(int id) async {
    try {
      // In a real app: final response = await http.get(Uri.parse('$baseUrl/api/workouts/$id'));
      await Future.delayed(const Duration(milliseconds: 500));
      return _workouts.firstWhere((workout) => workout.id == id);
    } catch (e) {
      _error = e.toString();
      notifyListeners();
      return null;
    }
  }

  Future<void> createWorkout(Workout workout) async {
    _isLoading = true;
    notifyListeners();

    try {
      // In a real app:
      // final response = await http.post(
      //   Uri.parse('$baseUrl/api/workouts'),
      //   headers: {'Content-Type': 'application/json'},
      //   body: json.encode(workout.toJson()),
      // );
      
      await Future.delayed(const Duration(seconds: 1));
      _workouts.add(workout);
      _isLoading = false;
      notifyListeners();
    } catch (e) {
      _error = e.toString();
      _isLoading = false;
      notifyListeners();
    }
  }

  // Mock data for demonstration
  List<Workout> _getMockWorkouts() {
    return [
      Workout(
        id: 1,
        name: 'Beginner Strength',
        description: 'A beginner friendly strength training workout',
        phase: 'Phase 1',
        createdDate: DateTime.now().subtract(const Duration(days: 10)),
        exercises: [
          Exercise(
            id: 1,
            name: 'Push-ups',
            description: 'Standard push-ups for chest and triceps',
            sets: 3,
            reps: 10,
            targetMuscleGroup: 'Chest',
          ),
          Exercise(
            id: 2,
            name: 'Squats',
            description: 'Bodyweight squats for legs',
            sets: 3,
            reps: 15,
            targetMuscleGroup: 'Legs',
          ),
        ],
      ),
      Workout(
        id: 2,
        name: 'Intermediate Hypertrophy',
        description: 'Build muscle with this intermediate level workout',
        phase: 'Phase 2',
        createdDate: DateTime.now().subtract(const Duration(days: 5)),
        exercises: [
          Exercise(
            id: 3,
            name: 'Bench Press',
            description: 'Barbell bench press for chest development',
            sets: 4,
            reps: 8,
            weight: 135,
            targetMuscleGroup: 'Chest',
          ),
          Exercise(
            id: 4,
            name: 'Deadlift',
            description: 'Conventional deadlift for back and posterior chain',
            sets: 3,
            reps: 6,
            weight: 225,
            targetMuscleGroup: 'Back',
          ),
        ],
      ),
      Workout(
        id: 3,
        name: 'Advanced Power',
        description: 'Advanced power workout for experienced lifters',
        phase: 'Phase 3',
        createdDate: DateTime.now().subtract(const Duration(days: 2)),
        exercises: [
          Exercise(
            id: 5,
            name: 'Power Clean',
            description: 'Olympic lift for explosive power',
            sets: 5,
            reps: 3,
            weight: 185,
            targetMuscleGroup: 'Full Body',
          ),
          Exercise(
            id: 6,
            name: 'Front Squat',
            description: 'Front loaded squat for quad development',
            sets: 4,
            reps: 5,
            weight: 205,
            targetMuscleGroup: 'Legs',
          ),
        ],
      ),
    ];
  }
}
