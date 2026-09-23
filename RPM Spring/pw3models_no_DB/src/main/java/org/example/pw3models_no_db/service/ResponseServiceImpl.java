package org.example.pw3models_no_db.service;

import org.example.pw3models_no_db.api.ResponseApiClient;
import org.example.pw3models_no_db.model.ResponseModel;
import org.springframework.stereotype.Service;

import java.util.List;

@Service
public class ResponseServiceImpl implements ResponseService {

    private final ResponseApiClient responseApiClient;

    public ResponseServiceImpl(ResponseApiClient responseApiClient) {
        this.responseApiClient = responseApiClient;
    }

    @Override
    public List<ResponseModel> findAll() {
        return responseApiClient.findAll();
    }

    @Override
    public ResponseModel addResponse(ResponseModel response) {
        return responseApiClient.addResponse(response);
    }

    @Override
    public ResponseModel updateResponse(ResponseModel response) {
        responseApiClient.updateResponse(response);
        return response;
    }

    @Override
    public void deleteResponse(int id) {
        responseApiClient.deleteResponse(id);
    }

    @Override
    public List<ResponseModel> findByStatus(String status) {
        return responseApiClient.findByStatus(status);
    }

    @Override
    public List<ResponseModel> findByTaskId(int taskId) {
        return responseApiClient.findByTaskId(taskId);
    }

    @Override
    public ResponseModel findById(int id) {
        return responseApiClient.findById(id);
    }
}