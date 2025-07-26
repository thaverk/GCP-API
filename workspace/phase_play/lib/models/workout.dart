class Workout {
  final int id;
  final String name;
  final String description;
  final List<Exercise> exercises;
  final String phase;
  final String imageUrl;
  final DateTime createdDate;

  Workout({
    required this.id,
    required this.name,
    required this.description,
    required this.exercises,
    required this.phase,
    this.imageUrl = '',
    required this.createdDate,
  });

  factory Workout.fromJson(Map<String, dynamic> json) {
    return Workout(
      id: json['id'],
      name: json['name'],
      description: json['description'],
      phase: json['phase'],
      imageUrl: json['imageUrl'] ?? '',
      createdDate: DateTime.parse(json['createdDate']),
      exercises: (json['exercises'] as List)
          .map((e) => Exercise.fromJson(e))
          .toList(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'description': description,
      'phase': phase,
      'imageUrl': imageUrl,
      'createdDate': createdDate.toIso8601String(),
      'exercises': exercises.map((e) => e.toJson()).toList(),
    };
  }
}
