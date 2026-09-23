package org.example.pw3models_no_db.service;

import org.example.pw3models_no_db.api.CategoryApiClient;
import org.example.pw3models_no_db.model.CategoryModel;
import org.springframework.stereotype.Service;

import java.util.List;

@Service
public class CategoryServiceImpl implements CategoryService {

    private final CategoryApiClient categoryApiClient;

    public CategoryServiceImpl(CategoryApiClient categoryApiClient) {
        this.categoryApiClient = categoryApiClient;
    }

    @Override
    public List<CategoryModel> findAll() {
        return categoryApiClient.findAll();
    }

    @Override
    public CategoryModel addCategory(CategoryModel category) {
        return categoryApiClient.addCategory(category);
    }

    @Override
    public CategoryModel updateCategory(CategoryModel category) {
        categoryApiClient.updateCategory(category);
        return category;
    }

    @Override
    public void deleteCategory(int id) {
        categoryApiClient.deleteCategory(id);
    }

    @Override
    public List<CategoryModel> findByCatName(String catName) {
        return categoryApiClient.findByCatName(catName);
    }

    @Override
    public CategoryModel findById(int id) {
        return categoryApiClient.findById(id);
    }
}