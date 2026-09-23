package org.example.pw3models_no_db.service;

import org.example.pw3models_no_db.api.UserApiClient;
import org.example.pw3models_no_db.model.UserModel;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.security.core.userdetails.UserDetailsService;
import org.springframework.security.core.userdetails.UsernameNotFoundException;
import org.springframework.stereotype.Service;

@Service
public class CustomUserDetailsService implements UserDetailsService {

    private final UserApiClient userApiClient;

    public CustomUserDetailsService(UserApiClient userApiClient) {
        this.userApiClient = userApiClient;
    }

    @Override
    public UserDetails loadUserByUsername(String username) throws UsernameNotFoundException {
        System.out.println("=== LOAD USER BY USERNAME (via API) ===");
        System.out.println("Попытка входа для: " + username);

        UserModel user = userApiClient.findOneByUsername(username);

        if (user == null) {
            System.out.println("Пользователь НЕ НАЙДЕН: " + username);
            throw new UsernameNotFoundException("User not found: " + username);
        }

        System.out.println("Пользователь найден: " + user.getUsername()
                + ", роль: " + (user.getRole() != null ? user.getRole().getRolName() : "null"));

        return new UserPrincipal(user);
    }
}