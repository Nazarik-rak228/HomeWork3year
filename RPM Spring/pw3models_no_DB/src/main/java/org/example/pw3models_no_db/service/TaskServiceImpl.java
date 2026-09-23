package org.example.pw3models_no_db.service;

import org.example.pw3models_no_db.api.TaskApiClient;
import org.example.pw3models_no_db.model.TaskModel;
import org.springframework.stereotype.Service;

import java.util.List;

@Service
public class TaskServiceImpl implements TaskService {

    private final TaskApiClient taskApiClient;

    public TaskServiceImpl(TaskApiClient taskApiClient) {
        this.taskApiClient = taskApiClient;
    }

    @Override
    public List<TaskModel> findAll() {
        return taskApiClient.findAll();
    }

    @Override
    public TaskModel addTask(TaskModel task) {
        return taskApiClient.addTask(task);
    }

    @Override
    public TaskModel updateTask(TaskModel task) {
        taskApiClient.updateTask(task);
        return task;
    }

    @Override
    public void deleteTask(int id) {
        taskApiClient.deleteTask(id);
    }

    @Override
    public List<TaskModel> findByTitle(String title) {
        return taskApiClient.findByTitle(title);
    }

    @Override
    public List<TaskModel> findByPriceGreaterThan(double minPrice) {
        return taskApiClient.findAll().stream()
                .filter(t -> t.getPrice() >= minPrice)
                .toList();
    }

    @Override
    public TaskModel findById(int id) {
        return taskApiClient.findById(id);
    }
}