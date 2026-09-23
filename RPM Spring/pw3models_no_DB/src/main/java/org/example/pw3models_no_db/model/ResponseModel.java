package org.example.pw3models_no_db.model;

import jakarta.validation.constraints.NotBlank;

public class ResponseModel {

    private int id;

    private UserModel user;

    private TaskModel task;

    @NotBlank(message = "Текст отклика не может быть пустым")
    private String message;

    private String status;

    public ResponseModel(int id, UserModel user, TaskModel task, String message, String status) {
        this.id = id;
        this.user = user;
        this.task = task;
        this.message = message;
        this.status = status;
    }

    public ResponseModel() {

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

    public String getMessage() {
        return message;
    }

    public void setMessage(String message) {
        this.message = message;
    }

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }
}