package org.example.pw3models_no_db.api;

import org.example.pw3models_no_db.model.CategoryModel;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.core.ParameterizedTypeReference;
import org.springframework.stereotype.Component;
import org.springframework.web.client.RestTemplate;

import java.util.List;

@Component
public class CategoryApiClient extends BaseApiClient {

    public CategoryApiClient(RestTemplate restTemplate,
                             @Value("${api.base.url}") String apiBaseUrl) {
        super(restTemplate, apiBaseUrl + "/api/category");
    }

    public List<CategoryModel> findAll() {
        return getList("", new ParameterizedTypeReference<List<CategoryModel>>() {});
    }

    public CategoryModel findById(int id) {
        return getOne("/" + id, CategoryModel.class);
    }

    public List<CategoryModel> findByCatName(String catName) {
        return findAll().stream()
                .filter(c -> c.getCatName() != null &&
                        c.getCatName().toLowerCase().contains(catName.toLowerCase()))
                .toList();
    }

    public CategoryModel addCategory(CategoryModel category) {
        return postJson("", category, CategoryModel.class);
    }

    public void updateCategory(CategoryModel category) {
        putJson("", category);
    }

    public void deleteCategory(int id) {
        delete("/" + id);
    }
}