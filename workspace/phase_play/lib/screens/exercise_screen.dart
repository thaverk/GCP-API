import 'package:flutter/material.dart';
import 'package:phase_play/models/exercise.dart';

class ExerciseScreen extends StatefulWidget {
  const ExerciseScreen({super.key});

  @override
  State<ExerciseScreen> createState() => _ExerciseScreenState();
}

class _ExerciseScreenState extends State<ExerciseScreen> {
  // Mock exercises data - in a real app, this would come from a service
  final List<Exercise> _exercises = [
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
    Exercise(
      id: 5,
      name: 'Pull-ups',
      description: 'Bodyweight pull-ups for back and biceps',
      sets: 3,
      reps: 8,
      targetMuscleGroup: 'Back',
    ),
    Exercise(
      id: 6,
      name: 'Lunges',
      description: 'Walking lunges for leg development',
      sets: 3,
      reps: 10,
      targetMuscleGroup: 'Legs',
    ),
    Exercise(
      id: 7,
      name: 'Shoulder Press',
      description: 'Overhead press for shoulder development',
      sets: 4,
      reps: 8,
      weight: 95,
      targetMuscleGroup: 'Shoulders',
    ),
    Exercise(
      id: 8,
      name: 'Bicep Curls',
      description: 'Dumbbell curls for bicep development',
      sets: 3,
      reps: 12,
      weight: 25,
      targetMuscleGroup: 'Arms',
    ),
  ];

  String _searchQuery = '';
  String _selectedMuscleGroup = 'All';
  final List<String> _muscleGroups = [
    'All',
    'Chest',
    'Back',
    'Legs',
    'Shoulders',
    'Arms',
    'Core',
  ];

  List<Exercise> get _filteredExercises {
    return _exercises.where((exercise) {
      final matchesSearch = exercise.name
          .toLowerCase()
          .contains(_searchQuery.toLowerCase());
      final matchesMuscleGroup = _selectedMuscleGroup == 'All' ||
          exercise.targetMuscleGroup == _selectedMuscleGroup;
      return matchesSearch && matchesMuscleGroup;
    }).toList();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Exercises'),
      ),
      body: Column(
        children: [
          Padding(
            padding: const EdgeInsets.all(16.0),
            child: TextField(
              decoration: InputDecoration(
                hintText: 'Search exercises...',
                prefixIcon: const Icon(Icons.search),
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(12),
                ),
                contentPadding: const EdgeInsets.symmetric(vertical: 12.0),
              ),
              onChanged: (value) {
                setState(() {
                  _searchQuery = value;
                });
              },
            ),
          ),
          SizedBox(
            height: 50,
            child: ListView.builder(
              scrollDirection: Axis.horizontal,
              itemCount: _muscleGroups.length,
              itemBuilder: (context, index) {
                final muscleGroup = _muscleGroups[index];
                return Padding(
                  padding: EdgeInsets.only(
                    left: index == 0 ? 16.0 : 8.0,
                    right: index == _muscleGroups.length - 1 ? 16.0 : 0.0,
                  ),
                  child: ChoiceChip(
                    label: Text(muscleGroup),
                    selected: _selectedMuscleGroup == muscleGroup,
                    onSelected: (selected) {
                      if (selected) {
                        setState(() {
                          _selectedMuscleGroup = muscleGroup;
                        });
                      }
                    },
                  ),
                );
              },
            ),
          ),
          Expanded(
            child: _filteredExercises.isEmpty
                ? const Center(
                    child: Text('No exercises found'),
                  )
                : ListView.builder(
                    itemCount: _filteredExercises.length,
                    itemBuilder: (context, index) {
                      final exercise = _filteredExercises[index];
                      return Card(
                        margin: const EdgeInsets.symmetric(
                          horizontal: 16,
                          vertical: 8,
                        ),
                        child: InkWell(
                          onTap: () {
                            _showExerciseDetails(context, exercise);
                          },
                          child: Padding(
                            padding: const EdgeInsets.all(16.0),
                            child: Row(
                              children: [
                                Container(
                                  width: 60,
                                  height: 60,
                                  decoration: BoxDecoration(
                                    color: Colors.blueGrey,
                                    borderRadius: BorderRadius.circular(8),
                                  ),
                                  child: const Center(
                                    child: Icon(
                                      Icons.sports_gymnastics,
                                      color: Colors.white,
                                      size: 32,
                                    ),
                                  ),
                                ),
                                const SizedBox(width: 16),
                                Expanded(
                                  child: Column(
                                    crossAxisAlignment: CrossAxisAlignment.start,
                                    children: [
                                      Text(
                                        exercise.name,
                                        style: const TextStyle(
                                          fontSize: 18,
                                          fontWeight: FontWeight.bold,
                                        ),
                                      ),
                                      const SizedBox(height: 4),
                                      Text(
                                        exercise.targetMuscleGroup,
                                        style: TextStyle(
                                          color: Colors.grey[600],
                                        ),
                                      ),
                                      const SizedBox(height: 4),
                                      Text(
                                        'Sets: ${exercise.sets} | Reps: ${exercise.reps}',
                                        style: TextStyle(
                                          color: Colors.grey[600],
                                        ),
                                      ),
                                    ],
                                  ),
                                ),
                                Icon(
                                  Icons.arrow_forward_ios,
                                  color: Colors.grey[400],
                                  size: 16,
                                ),
                              ],
                            ),
                          ),
                        ),
                      );
                    },
                  ),
          ),
        ],
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: () {
          // TODO: Implement add exercise functionality
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(
              content: Text('Add Exercise feature coming soon!'),
            ),
          );
        },
        child: const Icon(Icons.add),
      ),
    );
  }

  void _showExerciseDetails(BuildContext context, Exercise exercise) {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(16)),
      ),
      builder: (context) {
        return Padding(
          padding: const EdgeInsets.all(16.0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisSize: MainAxisSize.min,
            children: [
              Center(
                child: Container(
                  width: 40,
                  height: 5,
                  decoration: BoxDecoration(
                    color: Colors.grey[300],
                    borderRadius: BorderRadius.circular(10),
                  ),
                ),
              ),
              const SizedBox(height: 16),
              Row(
                children: [
                  Container(
                    width: 80,
                    height: 80,
                    decoration: BoxDecoration(
                      color: Colors.blueGrey,
                      borderRadius: BorderRadius.circular(8),
                    ),
                    child: const Center(
                      child: Icon(
                        Icons.sports_gymnastics,
                        color: Colors.white,
                        size: 40,
                      ),
                    ),
                  ),
                  const SizedBox(width: 16),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          exercise.name,
                          style: const TextStyle(
                            fontSize: 24,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        const SizedBox(height: 4),
                        Text(
                          exercise.targetMuscleGroup,
                          style: TextStyle(
                            color: Colors.grey[600],
                            fontSize: 16,
                          ),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 24),
              const Text(
                'Description',
                style: TextStyle(
                  fontSize: 18,
                  fontWeight: FontWeight.bold,
                ),
              ),
              const SizedBox(height: 8),
              Text(
                exercise.description,
                style: const TextStyle(fontSize: 16),
              ),
              const SizedBox(height: 24),
              const Text(
                'Exercise Details',
                style: TextStyle(
                  fontSize: 18,
                  fontWeight: FontWeight.bold,
                ),
              ),
              const SizedBox(height: 16),
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceAround,
                children: [
                  _buildExerciseDetail('Sets', exercise.sets.toString()),
                  _buildExerciseDetail('Reps', exercise.reps.toString()),
                  _buildExerciseDetail(
                      'Weight',
                      exercise.weight != null
                          ? '${exercise.weight} lbs'
                          : 'Bodyweight'),
                ],
              ),
              const SizedBox(height: 32),
              SizedBox(
                width: double.infinity,
                child: ElevatedButton(
                  style: ElevatedButton.styleFrom(
                    padding: const EdgeInsets.symmetric(vertical: 16.0),
                  ),
                  onPressed: () {
                    // TODO: Implement add to workout functionality
                    Navigator.pop(context);
                    ScaffoldMessenger.of(context).showSnackBar(
                      const SnackBar(
                        content: Text('Add to Workout feature coming soon!'),
                      ),
                    );
                  },
                  child: const Text(
                    'Add to Workout',
                    style: TextStyle(fontSize: 16),
                  ),
                ),
              ),
              const SizedBox(height: 16),
            ],
          ),
        );
      },
    );
  }

  Widget _buildExerciseDetail(String label, String value) {
    return Column(
      children: [
        Text(
          value,
          style: const TextStyle(
            fontWeight: FontWeight.bold,
            fontSize: 16,
          ),
        ),
        Text(
          label,
          style: TextStyle(
            color: Colors.grey[600],
          ),
        ),
      ],
    );
  }
}
