package org.example.pw3models_no_db.api;

import org.example.pw3models_no_db.model.TaskModel;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.core.ParameterizedTypeReference;
import org.springframework.stereotype.Component;
import org.springframework.util.MultiValueMap;
import org.springframework.web.client.RestTemplate;

import java.util.List;

@Component
public class TaskApiClient extends BaseApiClient {

    public TaskApiClient(RestTemplate restTemplate,
                         @Value("${api.base.url}") String apiBaseUrl) {
        super(restTemplate, apiBaseUrl + "/api/task");
    }

    public List<TaskModel> findAll() {
        return getList("", new ParameterizedTypeReference<List<TaskModel>>() {});
    }

    public TaskModel findById(int id) {
        return getOne("/" + id, TaskModel.class);
    }

    public List<TaskModel> findByTitle(String title) {
        return findAll().stream()
                .filter(t -> t.getTitle() != null &&
                        t.getTitle().toLowerCase().contains(title.toLowerCase()))
                .toList();
    }

    public TaskModel addTask(TaskModel task) {
        MultiValueMap<String, String> params = form();
        add(params, "title", task.getTitle());
        add(params, "description", task.getDescription());
        add(params, "deadline", task.getDeadline());
        add(params, "price", task.getPrice());
        add(params, "userId", task.getUser() != null ? task.getUser().getId() : null);
        add(params, "categoryId", task.getCategory() != null ? task.getCategory().getId() : null);
        add(params, "status", task.getStatus());
        return postForm("", params, TaskModel.class);
    }

    public void updateTask(TaskModel task) {
        MultiValueMap<String, String> params = form();
        add(params, "id", task.getId());
        add(params, "title", task.getTitle());
        add(params, "description", task.getDescription());
        add(params, "deadline", task.getDeadline());
        add(params, "price", task.getPrice());
        add(params, "userId", task.getUser() != null ? task.getUser().getId() : null);
        add(params, "categoryId", task.getCategory() != null ? task.getCategory().getId() : null);
        add(params, "status", task.getStatus());
        putForm("", params);
    }

    public void deleteTask(int id) {
        delete("/" + id);
    }
}