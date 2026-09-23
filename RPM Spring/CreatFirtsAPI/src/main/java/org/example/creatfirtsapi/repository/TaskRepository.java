package org.example.creatfirtsapi.repository;


import org.example.creatfirtsapi.dto.TaskDto;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface TaskRepository extends JpaRepository<TaskDto, Integer> {
    List<TaskDto> findByTitleContainingIgnoreCase(String title);
    List<TaskDto> findByPriceGreaterThan(double price);
}
/*
@Repository
public class TaskRepository {

    private List<TaskModel> tasks = new ArrayList<>();
    private AtomicInteger idCounter = new AtomicInteger(1);

    public List<TaskModel> findAll() {
        return new ArrayList<>(tasks);
    }

    public TaskModel addTask(TaskModel task) {
        task.setId(idCounter.getAndIncrement());
        tasks.add(task);
        return task;
    }

    public TaskModel updateTask(TaskModel task) {
        for (int i = 0; i < tasks.size(); i++) {
            if (tasks.get(i).getId() == task.getId()) {
                tasks.set(i, task);
                return task;
            }
        }
        return null;
    }

    public void deleteTask(int id) {
        tasks.removeIf(task -> task.getId() == id);
    }

    public TaskModel findById(int id) {
        return tasks.stream()
                .filter(task -> task.getId() == id)
                .findFirst()
                .orElse(null);
    }
    public List<TaskModel> findByTitle(String title) {
        return tasks.stream()
                .filter(t -> t.getTitle() != null &&
                        t.getTitle().toLowerCase().contains(title.toLowerCase()))
                .toList();
    }

    public List<TaskModel> findByPriceGreaterThan(double minPrice) {
        return tasks.stream()
                .filter(t -> t.getPrice() >= minPrice)
                .toList();
    }
}*/
