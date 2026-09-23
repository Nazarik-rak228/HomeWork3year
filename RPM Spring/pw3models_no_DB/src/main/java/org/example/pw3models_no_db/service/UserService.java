package org.example.pw3models_no_db.service;

import org.example.pw3models_no_db.model.UserModel;

import java.util.List;

public interface UserService {
    List<UserModel> findAll();
    UserModel addUser(UserModel user);
    UserModel updateUser(UserModel user);
    void deleteUser(int id);
    List<UserModel> findByUsername(String username);
    List<UserModel> findByEmail(String email);
    UserModel findById(int id);
    UserModel findOneByUsername(String username);
}