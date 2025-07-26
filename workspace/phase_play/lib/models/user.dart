class User {
  final int id;
  final String username;
  final String email;
  final String firstName;
  final String lastName;
  final String profileImageUrl;
  final List<int> workoutIds;
  final Map<String, dynamic> preferences;
  
  User({
    required this.id,
    required this.username,
    required this.email,
    required this.firstName,
    required this.lastName,
    this.profileImageUrl = '',
    required this.workoutIds,
    required this.preferences,
  });

  factory User.fromJson(Map<String, dynamic> json) {
    return User(
      id: json['id'],
      username: json['username'],
      email: json['email'],
      firstName: json['firstName'],
      lastName: json['lastName'],
      profileImageUrl: json['profileImageUrl'] ?? '',
      workoutIds: List<int>.from(json['workoutIds'] ?? []),
      preferences: json['preferences'] ?? {},
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'username': username,
      'email': email,
      'firstName': firstName,
      'lastName': lastName,
      'profileImageUrl': profileImageUrl,
      'workoutIds': workoutIds,
      'preferences': preferences,
    };
  }
}
