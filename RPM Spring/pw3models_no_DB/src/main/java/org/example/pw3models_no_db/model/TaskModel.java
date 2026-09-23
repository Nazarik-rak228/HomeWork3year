package org.example.pw3models_no_db.model;

import jakarta.validation.constraints.DecimalMin;
import jakarta.validation.constraints.Future;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;

import java.time.LocalDate;

public class TaskModel {

    private int id;

    @NotBlank(message = "Название задания не может быть пустым")
    private String title;

    @NotBlank(message = "Описание задания не может быть пустым")
    private String description;

    @NotNull(message = "Срок выполнения обязателен")
    @Future(message = "Срок выполнения должен быть в будущем")
    private LocalDate deadline;

    @NotNull(message = "Цена обязательна")
    @DecimalMin(value = "500.0", message = "Минимальная цена задания — 500₽")
    private Double price;

    @NotNull(message = "Автор задания обязателен")
    private UserModel user;

    @NotNull(message = "Категория обязательна")
    private CategoryModel category;

    // Статус задания: Открыто / В работе / Завершено
    private String status;

    public TaskModel(int id, String title, String description, LocalDate deadline, double price, UserModel user, CategoryModel category) {
        this.id = id;
        this.title = title;
        this.description = description;
        this.deadline = deadline;
        this.price = price;
        this.user = user;
        this.category = category;
    }

    public TaskModel() {

    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public LocalDate getDeadline() {
        return deadline;
    }

    public void setDeadline(LocalDate deadline) {
        this.deadline = deadline;
    }

    public double getPrice() {
        return price;
    }

    public void setPrice(double price) {
        this.price = price;
    }

    public UserModel getUser() {
        return user;
    }

    public void setUser(UserModel user) {
        this.user = user;
    }

    public CategoryModel getCategory() {
        return category;
    }

    public void setCategory(CategoryModel category) {
        this.category = category;
    }

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }
}