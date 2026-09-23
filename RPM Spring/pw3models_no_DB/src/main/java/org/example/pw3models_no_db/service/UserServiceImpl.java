package org.example.pw3models_no_db.service;

import org.example.pw3models_no_db.api.UserApiClient;
import org.example.pw3models_no_db.model.UserModel;
import org.springframework.stereotype.Service;

import java.util.List;

@Service
public class UserServiceImpl implements UserService {

    private final UserApiClient userApiClient;

    public UserServiceImpl(UserApiClient userApiClient) {
        this.userApiClient = userApiClient;
    }

    @Override
    public List<UserModel> findAll() {
        return userApiClient.findAll();
    }

    @Override
    public UserModel addUser(UserModel user) {
        return userApiClient.addUser(user);
    }

    @Override
    public UserModel updateUser(UserModel user) {
        return userApiClient.updateUser(user);
    }

    @Override
    public void deleteUser(int id) {
        userApiClient.deleteUser(id);
    }

    @Override
    public List<UserModel> findByUsername(String username) {
        return userApiClient.findByUsername(username);
    }

    @Override
    public List<UserModel> findByEmail(String email) {
        return userApiClient.findAll().stream()
                .filter(u -> u.getEmail() != null &&
                        u.getEmail().toLowerCase().contains(email.toLowerCase()))
                .toList();
    }

    @Override
    public UserModel findById(int id) {
        return userApiClient.findById(id);
    }

    @Override
    public UserModel findOneByUsername(String username) {
        return userApiClient.findOneByUsername(username);
    }
}