package org.example.pw3models_no_db.api;

import org.example.pw3models_no_db.model.ResponseModel;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.core.ParameterizedTypeReference;
import org.springframework.stereotype.Component;
import org.springframework.util.MultiValueMap;
import org.springframework.web.client.RestTemplate;

import java.util.List;

@Component
public class ResponseApiClient extends BaseApiClient {

    public ResponseApiClient(RestTemplate restTemplate,
                             @Value("${api.base.url}") String apiBaseUrl) {
        super(restTemplate, apiBaseUrl + "/api/response");
    }

    public List<ResponseModel> findAll() {
        return getList("", new ParameterizedTypeReference<List<ResponseModel>>() {});
    }

    public ResponseModel findById(int id) {
        return getOne("/" + id, ResponseModel.class);
    }

    public List<ResponseModel> findByStatus(String status) {
        return findAll().stream()
                .filter(r -> r.getStatus() != null &&
                        r.getStatus().toLowerCase().contains(status.toLowerCase()))
                .toList();
    }

    public List<ResponseModel> findByTaskId(int taskId) {
        return findAll().stream()
                .filter(r -> r.getTask() != null && r.getTask().getId() == taskId)
                .toList();
    }

    public ResponseModel addResponse(ResponseModel response) {
        MultiValueMap<String, String> params = form();
        add(params, "userId", response.getUser() != null ? response.getUser().getId() : null);
        add(params, "taskId", response.getTask() != null ? response.getTask().getId() : null);
        add(params, "message", response.getMessage());
        add(params, "status", response.getStatus());
        return postForm("", params, ResponseModel.class);
    }

    public void updateResponse(ResponseModel response) {
        MultiValueMap<String, String> params = form();
        add(params, "id", response.getId());
        add(params, "userId", response.getUser() != null ? response.getUser().getId() : null);
        add(params, "taskId", response.getTask() != null ? response.getTask().getId() : null);
        add(params, "message", response.getMessage());
        add(params, "status", response.getStatus());
        putForm("", params);
    }

    public void deleteResponse(int id) {
        delete("/" + id);
    }
}