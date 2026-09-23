package org.example.pw3models_no_db.service;

import org.example.pw3models_no_db.model.ResponseModel;

import java.util.List;

public interface ResponseService {
    List<ResponseModel> findAll();
    ResponseModel addResponse(ResponseModel response);
    ResponseModel updateResponse(ResponseModel response);
    void deleteResponse(int id);
    List<ResponseModel> findByStatus(String status);
    List<ResponseModel> findByTaskId(int taskId);
    ResponseModel findById(int id);
}