package org.example.creatfirtsapi.dto;

import jakarta.persistence.*;

@Entity
@Table(name = "responses")
public class ResponseDto {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private int id;

    @ManyToOne(fetch = FetchType.EAGER)
    @JoinColumn(name = "user_id", nullable = false)
    private UserDto user;

    @ManyToOne(fetch = FetchType.EAGER)
    @JoinColumn(name = "task_id", nullable = false)
    private TaskDto task;

    private String message;
    private String status;

    public ResponseDto(int id, UserDto user, TaskDto task, String message, String status) {
        this.id = id;
        this.user = user;
        this.task = task;
        this.message = message;
        this.status = status;
    }

    public ResponseDto() {
    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public UserDto getUser() {
        return user;
    }

    public void setUser(UserDto user) {
        this.user = user;
    }

    public TaskDto getTask() {
        return task;
    }

    public void setTask(TaskDto task) {
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
