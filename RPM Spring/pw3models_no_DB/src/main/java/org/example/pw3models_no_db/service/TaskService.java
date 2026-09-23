package org.example.pw3models_no_db.service;
import org.example.pw3models_no_db.model.TaskModel;

import java.util.List;

public interface TaskService {
    List<TaskModel> findAll();
    TaskModel addTask(TaskModel task);
    TaskModel updateTask(TaskModel task);
    void deleteTask(int id);
    List<TaskModel> findByTitle(String title);
    List<TaskModel> findByPriceGreaterThan(double minPrice);
    TaskModel findById(int id);
}