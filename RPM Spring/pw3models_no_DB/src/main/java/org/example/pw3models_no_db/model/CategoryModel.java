package org.example.pw3models_no_db.model;

import jakarta.validation.constraints.NotBlank;

public class CategoryModel {

    private int id;

    @NotBlank(message = "Название категории не может быть пустым")
    private String catName;

    public CategoryModel(int id, String catName) {
        this.id = id;
        this.catName = catName;
    }

    public CategoryModel() {

    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getCatName() {
        return catName;
    }

    public void setCatName(String catName) {
        this.catName = catName;
    }
}