package org.example.pw3models_no_db.model;

import jakarta.validation.constraints.NotBlank;

public class UserTaskModel {

    private int id;

    private UserModel user;

    private TaskModel task;

    @NotBlank(message = "Статус не может быть пустым")
    private String status;

    public UserTaskModel(int id, UserModel user, TaskModel task, String status) {
        this.id = id;
        this.user = user;
        this.task = task;
        this.status = status;
    }

    public UserTaskModel() {

    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public UserModel getUser() {
        return user;
    }

    public void setUser(UserModel user) {
        this.user = user;
    }

    public TaskModel getTask() {
        return task;
    }

    public void setTask(TaskModel task) {
        this.task = task;
    }

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }
}