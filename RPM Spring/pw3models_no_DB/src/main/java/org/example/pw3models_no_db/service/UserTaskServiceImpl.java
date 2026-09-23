package org.example.pw3models_no_db.service;

import org.example.pw3models_no_db.api.UserTaskApiClient;
import org.example.pw3models_no_db.model.UserTaskModel;
import org.springframework.stereotype.Service;

import java.util.List;

@Service
public class UserTaskServiceImpl implements UserTaskService {

    private final UserTaskApiClient userTaskApiClient;

    public UserTaskServiceImpl(UserTaskApiClient userTaskApiClient) {
        this.userTaskApiClient = userTaskApiClient;
    }

    @Override
    public List<UserTaskModel> findAll() {
        return userTaskApiClient.findAll();
    }

    @Override
    public UserTaskModel addUserTask(UserTaskModel userTask) {
        return userTaskApiClient.addUserTask(userTask);
    }

    @Override
    public UserTaskModel updateUserTask(UserTaskModel userTask) {
        userTaskApiClient.updateUserTask(userTask);
        return userTask;
    }

    @Override
    public void deleteUserTask(int id) {
        userTaskApiClient.deleteUserTask(id);
    }

    @Override
    public List<UserTaskModel> findByStatus(String status) {
        return userTaskApiClient.findByStatus(status);
    }

    @Override
    public List<UserTaskModel> findByUserId(int userId) {
        return userTaskApiClient.findByUserId(userId);
    }

    @Override
    public UserTaskModel findById(int id) {
        return userTaskApiClient.findById(id);
    }
}