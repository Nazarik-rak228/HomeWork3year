package org.example.pw3models_no_db.service;

import org.example.pw3models_no_db.model.UserTaskModel;

import java.util.List;

public interface UserTaskService {
    List<UserTaskModel> findAll();
    UserTaskModel addUserTask(UserTaskModel userTask);
    UserTaskModel updateUserTask(UserTaskModel userTask);
    void deleteUserTask(int id);
    List<UserTaskModel> findByStatus(String status);
    List<UserTaskModel> findByUserId(int userId);
    UserTaskModel findById(int id);
}