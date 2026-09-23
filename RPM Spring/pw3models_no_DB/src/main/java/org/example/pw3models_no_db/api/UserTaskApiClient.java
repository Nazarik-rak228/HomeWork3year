package org.example.pw3models_no_db.api;

import org.example.pw3models_no_db.model.UserTaskModel;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.core.ParameterizedTypeReference;
import org.springframework.stereotype.Component;
import org.springframework.util.MultiValueMap;
import org.springframework.web.client.RestTemplate;

import java.util.List;

@Component
public class UserTaskApiClient extends BaseApiClient {

    public UserTaskApiClient(RestTemplate restTemplate,
                             @Value("${api.base.url}") String apiBaseUrl) {
        super(restTemplate, apiBaseUrl + "/api/usertask");
    }

    public List<UserTaskModel> findAll() {
        return getList("", new ParameterizedTypeReference<List<UserTaskModel>>() {});
    }

    public UserTaskModel findById(int id) {
        return getOne("/" + id, UserTaskModel.class);
    }

    public List<UserTaskModel> findByStatus(String status) {
        return findAll().stream()
                .filter(u -> u.getStatus() != null &&
                        u.getStatus().toLowerCase().contains(status.toLowerCase()))
                .toList();
    }

    public List<UserTaskModel> findByUserId(int userId) {
        return findAll().stream()
                .filter(u -> u.getUser() != null && u.getUser().getId() == userId)
                .toList();
    }

    public UserTaskModel addUserTask(UserTaskModel userTask) {
        MultiValueMap<String, String> params = form();
        add(params, "userId", userTask.getUser() != null ? userTask.getUser().getId() : null);
        add(params, "taskId", userTask.getTask() != null ? userTask.getTask().getId() : null);
        add(params, "status", userTask.getStatus());
        return postForm("", params, UserTaskModel.class);
    }

    public void updateUserTask(UserTaskModel userTask) {
        MultiValueMap<String, String> params = form();
        add(params, "id", userTask.getId());
        add(params, "userId", userTask.getUser() != null ? userTask.getUser().getId() : null);
        add(params, "taskId", userTask.getTask() != null ? userTask.getTask().getId() : null);
        add(params, "status", userTask.getStatus());
        putForm("", params);
    }

    public void deleteUserTask(int id) {
        delete("/" + id);
    }
}