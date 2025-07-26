import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:shared_preferences/shared_preferences.dart';
import 'package:phase_play/models/user.dart';

class AuthService extends ChangeNotifier {
  final String baseUrl = 'https://api.phaseplay.com';
  User? _currentUser;
  bool _isAuthenticated = false;
  bool _isLoading = false;
  String _error = '';

  User? get currentUser => _currentUser;
  bool get isAuthenticated => _isAuthenticated;
  bool get isLoading => _isLoading;
  String get error => _error;

  // Check if user is already logged in
  Future<void> checkAuthStatus() async {
    _isLoading = true;
    notifyListeners();

    try {
      final prefs = await SharedPreferences.getInstance();
      final userData = prefs.getString('user_data');
      final token = prefs.getString('auth_token');

      if (userData != null && token != null) {
        _currentUser = User.fromJson(json.decode(userData));
        _isAuthenticated = true;
      }
      
      _isLoading = false;
      notifyListeners();
    } catch (e) {
      _error = e.toString();
      _isLoading = false;
      notifyListeners();
    }
  }

  // Login function
  Future<bool> login(String username, String password) async {
    _isLoading = true;
    _error = '';
    notifyListeners();

    try {
      // In a real app, you'd make an HTTP request to your ASP.NET Core API:
      // final response = await http.post(
      //   Uri.parse('$baseUrl/api/auth/login'),
      //   headers: {'Content-Type': 'application/json'},
      //   body: json.encode({
      //     'username': username,
      //     'password': password,
      //   }),
      // );
      
      // Simulate network request
      await Future.delayed(const Duration(seconds: 1));
      
      // Mock login - in a real app you'd validate credentials on server
      if (username == 'user' && password == 'password') {
        // Mock user data
        final userData = {
          'id': 1,
          'username': username,
          'email': 'user@example.com',
          'firstName': 'John',
          'lastName': 'Doe',
          'profileImageUrl': '',
          'workoutIds': [1, 2, 3],
          'preferences': {'darkMode': true, 'notifications': true},
        };
        
        _currentUser = User.fromJson(userData);
        _isAuthenticated = true;
        
        // Save to SharedPreferences
        final prefs = await SharedPreferences.getInstance();
        await prefs.setString('user_data', json.encode(userData));
        await prefs.setString('auth_token', 'mock_token_12345');
        
        _isLoading = false;
        notifyListeners();
        return true;
      } else {
        _error = 'Invalid username or password';
        _isLoading = false;
        notifyListeners();
        return false;
      }
    } catch (e) {
      _error = e.toString();
      _isLoading = false;
      notifyListeners();
      return false;
    }
  }

  // Register function
  Future<bool> register(String username, String email, String password) async {
    _isLoading = true;
    _error = '';
    notifyListeners();

    try {
      // In a real app:
      // final response = await http.post(
      //   Uri.parse('$baseUrl/api/auth/register'),
      //   headers: {'Content-Type': 'application/json'},
      //   body: json.encode({
      //     'username': username,
      //     'email': email,
      //     'password': password,
      //   }),
      // );
      
      await Future.delayed(const Duration(seconds: 1));
      
      // Mock registration - in a real app this would be handled server-side
      final userData = {
        'id': 1,
        'username': username,
        'email': email,
        'firstName': '',
        'lastName': '',
        'profileImageUrl': '',
        'workoutIds': [],
        'preferences': {'darkMode': false, 'notifications': true},
      };
      
      _currentUser = User.fromJson(userData);
      _isAuthenticated = true;
      
      // Save to SharedPreferences
      final prefs = await SharedPreferences.getInstance();
      await prefs.setString('user_data', json.encode(userData));
      await prefs.setString('auth_token', 'mock_token_12345');
      
      _isLoading = false;
      notifyListeners();
      return true;
    } catch (e) {
      _error = e.toString();
      _isLoading = false;
      notifyListeners();
      return false;
    }
  }

  // Logout function
  Future<void> logout() async {
    _isLoading = true;
    notifyListeners();

    try {
      final prefs = await SharedPreferences.getInstance();
      await prefs.remove('user_data');
      await prefs.remove('auth_token');
      
      _currentUser = null;
      _isAuthenticated = false;
      _isLoading = false;
      notifyListeners();
    } catch (e) {
      _error = e.toString();
      _isLoading = false;
      notifyListeners();
    }
  }
}
