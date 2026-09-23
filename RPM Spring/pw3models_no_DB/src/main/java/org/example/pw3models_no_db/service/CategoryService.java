package org.example.pw3models_no_db.service;

import org.example.pw3models_no_db.model.CategoryModel;

import java.util.List;

public interface CategoryService {
    List<CategoryModel> findAll();
    CategoryModel addCategory(CategoryModel category);
    CategoryModel updateCategory(CategoryModel category);
    void deleteCategory(int id);
    List<CategoryModel> findByCatName(String catName);
    CategoryModel findById(int id);
}