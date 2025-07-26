class Exercise {
  final int id;
  final String name;
  final String description;
  final int sets;
  final int reps;
  final int? weight; // Optional weight - may be null for bodyweight exercises
  final String targetMuscleGroup;
  final String imageUrl;
  final String videoUrl;
  
  Exercise({
    required this.id,
    required this.name,
    required this.description,
    required this.sets,
    required this.reps,
    this.weight,
    required this.targetMuscleGroup,
    this.imageUrl = '',
    this.videoUrl = '',
  });

  factory Exercise.fromJson(Map<String, dynamic> json) {
    return Exercise(
      id: json['id'],
      name: json['name'],
      description: json['description'],
      sets: json['sets'],
      reps: json['reps'],
      weight: json['weight'],
      targetMuscleGroup: json['targetMuscleGroup'],
      imageUrl: json['imageUrl'] ?? '',
      videoUrl: json['videoUrl'] ?? '',
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'description': description,
      'sets': sets,
      'reps': reps,
      'weight': weight,
      'targetMuscleGroup': targetMuscleGroup,
      'imageUrl': imageUrl,
      'videoUrl': videoUrl,
    };
  }
}
