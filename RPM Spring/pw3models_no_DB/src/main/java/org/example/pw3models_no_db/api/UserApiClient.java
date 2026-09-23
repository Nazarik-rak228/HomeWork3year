package org.example.pw3models_no_db.api;

import org.example.pw3models_no_db.model.UserModel;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.core.ParameterizedTypeReference;
import org.springframework.stereotype.Component;
import org.springframework.web.client.RestTemplate;

import java.util.List;

@Component
public class UserApiClient extends BaseApiClient {

    public UserApiClient(RestTemplate restTemplate,
                         @Value("${api.base.url}") String apiBaseUrl) {
        super(restTemplate, apiBaseUrl + "/api/user");
    }

    public List<UserModel> findAll() {
        return getList("", new ParameterizedTypeReference<List<UserModel>>() {});
    }

    public UserModel findById(int id) {
        return getOne("/" + id, UserModel.class);
    }

    public List<UserModel> findByUsername(String username) {
        return findAll().stream()
                .filter(u -> u.getUsername() != null &&
                        u.getUsername().toLowerCase().contains(username.toLowerCase()))
                .toList();
    }

    public UserModel findOneByUsername(String username) {
        return findAll().stream()
                .filter(u -> u.getUsername() != null &&
                        u.getUsername().equalsIgnoreCase(username))
                .findFirst()
                .orElse(null);
    }

    public UserModel addUser(UserModel user) {
        return postJson("", user, UserModel.class);
    }

    public UserModel updateUser(UserModel user) {
        putJson("", user);
        return user;
    }

    public void deleteUser(int id) {
        delete("/" + id);
    }
}